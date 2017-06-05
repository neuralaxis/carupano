using Carupano.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Carupano.SqlServer
{
    public class SqlServerEventStore : IEventStore, IDisposable
    {
        SqlConnection _conn;
        public SqlServerEventStore(string connectionString)
        {
            _conn = new SqlConnection(connectionString);
        }

        public IEnumerable Load(string aggregate, string id)
        {
            var list = new List<object>();
            _conn.Open();
            try
            {
                using (var cmd = _conn.CreateCommand())
                {
                    cmd.CommandText = "select id,event from events where aggregate=@aggregate and aggregateid=@id order by createdonutc asc";
                    cmd.Parameters.AddWithValue("aggregate", aggregate);
                    cmd.Parameters.AddWithValue("id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var data = JsonConvert.DeserializeObject(reader.GetString(1));
                            list.Add(data);
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
                                cmd.CommandText = "insert into events (aggregate,aggregateid,event) values (@aggregate,@aggregateid,@event); select @@SCOPE_IDENTITY";
                                cmd.Parameters.AddWithValue("aggregate", aggregate);
                                cmd.Parameters.AddWithValue("aggregateid", id);
                                cmd.Parameters.AddWithValue("event", JsonConvert.SerializeObject(evt));
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
    }
}
