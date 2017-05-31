# CARUPANO
A library for writing event-sourced CQRS bounded contexts with a minimal amount of intrusion on the core domain models.
 
 ## Concepts
 
 - [Models](#models)
 - [Aggregates](#aggregates)
 - [Factories](#factories)
 - [Commands](#commands)
 - [Projections](TODO)
 - [Jobs](TODO)
 
### [Models]
 The models express the configuration of your bounded context. 
 
#### Example
```cs
            var builder = new Model.BoundedContextModelBuilder();
            builder.Aggregate<FlightReservation>(cfg =>
            {
                cfg
                .HasId(c => c.Id)
                .CreatedBy<CreateFlightReservation>()
                .Executes<CancelFlightReservation>(c => c.Id);
            });
            Model = builder.Build();
 ```       
 
