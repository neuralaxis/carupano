using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
namespace Carupano.SqlServer.Tests
{
    using Runtime;
    public class SqlServerEventStoreTests
    {
        SqlServerEventStore Store;
        ISerialization Serializer;
        public SqlServerEventStoreTests()
        {
            Serializer = new JsonSerialization();
            Store = new SqlServerEventStore("data source=.;initial catalog=sqlservereventstoretests;trusted_connection=true", Serializer);
            Store.Clear();
        }
        [Fact]
        public void when_empty_returns_no_events()
        {
            Assert.Empty(Store.Load(0));
        }
        [Fact]
        public void when_saving_returns_persisted_events()
        {
            var evts = Store.Save("customer", "1", new object[]
            {
                new CustomerCreated(),
                new CustomerDeleted()
            });
            Assert.NotNull(evts);
            Assert.NotEmpty(evts);
            Assert.Equal(2, evts.Count());
        }

        [Fact]
        public void can_load_after_saving_events()
        {
            Store.Save("customer", "1", new object[]
            {
                new CustomerCreated(),
                new CustomerDeleted()
            });
            var evts = Store.Load("customer", "1");
            Assert.NotNull(evts);
            Assert.NotEmpty(evts);
            var firstEvt = evts.ElementAt(0);
            var secEvt = evts.ElementAt(1);
            Assert.NotNull(firstEvt);
            Assert.NotNull(secEvt);
            Assert.IsAssignableFrom<CustomerCreated>(firstEvt.Event);
            Assert.Equal(1, firstEvt.SequenceNo);
            Assert.IsAssignableFrom<CustomerDeleted>(secEvt.Event);
            Assert.Equal(2, secEvt.SequenceNo);
        }
        class CustomerCreated
        {

        }
        class CustomerDeleted
        {

        }
    }
}
