using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace CarupanoAir.Passenger.Projections
{
    using Events;
    using ReadModels;
    using Queries;
    public class PassengerList
    {
        IMongoCollection<Passenger> Passengers;

        public PassengerList(string connectionString)
        {
            var url = new MongoUrl(connectionString);
            var mongo = new MongoClient(url);
            var db = mongo.GetDatabase(url.DatabaseName);
            Passengers = db.GetCollection<Passenger>("passengers");
        }

        public void On(PassengerCreated evt)
        {
            Passengers.InsertOne(new Passenger
            {
                Id = evt.PassengerId,
                Name = evt.Name,
                Email = evt.Email
            });
        }
        public Passenger Query(FindPassengerByEmail query)
        {
            return Passengers.Find(Builders<Passenger>.Filter.Eq(c => c.Email, query.Email)).SingleOrDefault();
        }
        
    }
}
