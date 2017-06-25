using Carupano.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Reflection;

namespace Carupano.SqlServer
{
    using Persistence;
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

        public IEnumerable<PersistedEvent> Load(string aggregate, string id)
        {
            var list = new List<PersistedEvent>();
            _conn.Open();
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = "select seqNum,eventtype,event from events where aggregate=@aggregate and aggregateid=@id order by createdonutc asc";
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
                            list.Add(new PersistedEvent(evt, seq));
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
                                cmd.CommandText = "insert into events (aggregate,aggregateid,event,eventtype,createdonutc) values (@aggregate,@aggregateid,@event,@eventtype,@createdonutc); select @@IDENTITY";
                                cmd.Parameters.AddWithValue("aggregate", aggregate);
                                cmd.Parameters.AddWithValue("aggregateid", id);
                                cmd.Parameters.AddWithValue("eventtype", evt.GetType().Name);
                                cmd.Parameters.AddWithValue("event", _encoding.GetString(_serialization.Serialize(evt)));
                                cmd.Parameters.AddWithValue("createdonutc", DateTime.UtcNow);
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
        
        public void Clear()
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = "delete from events;DBCC CHECKIDENT ('events', RESEED, 0)";
                _conn.Open();
                cmd.ExecuteNonQuery();
                _conn.Close();
            }
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
                        while(reader.Read()) { 
                            var seq = reader.GetInt64(0);
                            var type = reader.GetString(1);
                            var data = reader.GetString(2);
                            var evt = _serialization.Deserialize(Type.GetType(type), _encoding.GetBytes(data));
                            yield return new PersistedEvent(evt, seq);
                        }
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
