using Carupano.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Carupano.SqlServer
{
    public class SqlServerEventStore : IEventStore, IDisposable
    {
        SqlConnection _conn;
        ISerialization _serialization;
        Encoding _encoding = Encoding.UTF8;
        public SqlServerEventStore(string connectionString, ISerialization serialization)
        {
            _conn = new SqlConnection(connectionString);
            _serialization = serialization;
        }

        public IEnumerable Load(string aggregate, string id)
        {
            var list = new List<object>();
            _conn.Open();
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = "select seqnum,eventtype,event from events where aggregate=@aggregate and aggregateid=@id order by createdonutc asc";
                    cmd.Parameters.AddWithValue("aggregate", aggregate);
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var seq = reader.GetInt64(0);
                            var type = reader.GetString(1);
                            var data = reader.GetString(2);
                            var evt = _serialization.Deserialize(Type.GetType(type), _encoding.GetBytes(data));
                            list.Add(evt);
                        }
                    }
                }
            }
            finally
            {
                _conn.Close();

            }
            return list;
        }

        public IEnumerable<PersistedEvent> Save(string aggregate, string id, IEnumerable events)
        {
            _conn.Open();
            var committed = new List<PersistedEvent>();
            try
            {
                using (var trans = _conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var evt in events)
                        {
                            using (var cmd = _conn.CreateCommand())
                            {
                                cmd.Transaction = trans;
                                cmd.CommandText = "insert into events (aggregate,aggregateid,event,eventtype) values (@aggregate,@aggregateid,@event,@eventtype); select @@SCOPE_IDENTITY";
                                cmd.Parameters.AddWithValue("aggregate", aggregate);
                                cmd.Parameters.AddWithValue("aggregateid", id);
                                cmd.Parameters.AddWithValue("eventtype", evt.GetType().Name);
                                cmd.Parameters.AddWithValue("event", _encoding.GetString(_serialization.Serialize(evt)));
                                long seq = Convert.ToInt64(cmd.ExecuteScalar());
                                committed.Add(new PersistedEvent(evt, seq));
                            }
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        _conn.Close();
                        throw ex;
                    }
                }
            }
            finally
            {
                _conn.Close();
            }
            return committed;
        }

        public void Dispose()
        {
            _conn.Dispose();
        }

        public IEnumerable<PersistedEvent> Load(long seqNum)
        {
            _conn.Open();
            try
            {
                var list = new List<PersistedEvent>();
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = "select seqnum, eventtype,event from events order by createdonutc asc;";
                    using (var reader = cmd.ExecuteReader())
                    {
                        var seq = reader.GetInt64(0);
                        var type = reader.GetString(1);
                        var data = reader.GetString(2);
                        var evt = _serialization.Deserialize(Type.GetType(type), _encoding.GetBytes(data));
                        yield return new PersistedEvent(evt, seq);
                    }
                }
            }
            finally
            {
                _conn.Close();
            }
        }
    }
}
