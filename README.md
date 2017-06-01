# CARUPANO
A library for writing event-sourced CQRS bounded contexts with a minimal amount of intrusion on the core domain models.
 
 ## Concepts
 
 - [Models](#models)
 - [Aggregates](#aggregates)
 - [Factories](#factories)
 - [Commands](#commands)
 - [Events](#commands)
 - [Projections](#projections)
 - [Queries](#queries)
 - [Jobs](#jobs)
 - [REPL](#repl)
 
### Models
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
 
#### Aggregates
Aggregates in Carupano are classic DDD aggregates. They possess a unique identity attribute, handle [commands](#commands] and maintain their state through state changes. They are built not on relational models, but on event streams.

- Aggregates do not need to inherit from any base class, and can have dependencies in its constructor.
- There must be a [factory](#factories) defined for each aggregate.
- Aggregates must have a string ID
- Command handlers are methods with one complex type argument (the "command" itself)
- Return values from command handlers are considered events. It can be one event or a list of events.
- When building an aggregate from the event store, if a method "Apply" exists in the aggregate for the event type it will be used to replay the state.

#### Example:
```cs
    public class FlightReservation
    {
        bool _cancelled;
        public string Id { get; private set; }
        public FlightReservationCreated Create(CreateFlightReservation cmd)
        {
            var evt = new FlightReservationCreated
            {
                Id = Guid.NewGuid().ToString(),
                PassengerId = cmd.PassengerId,
                Localizer = Guid.NewGuid().ToString().Substring(5)
            };
            Apply(evt);
            return evt;
        }
        public FlightReservationCancelled Cancel(CancelFlightReservation cmd)
        {
            if (_cancelled) throw new InvalidOperationException("Reservation already cancelled");
            var evt = new FlightReservationCancelled
            {
                Id = cmd.Id
            };
            Apply(evt);
            return evt;
        }
        void Apply(FlightReservationCancelled evt)
        {
            _cancelled = true;
        }
        void Apply(FlightReservationCreated created)
        {
            Id = created.Id;
        }
    }
   ```
   
#### Commands
Commands are POCOs that are sent to aggregates or stand alone command handlers through a [bus](#bus).

#### Factories
Factories are methods in an aggregate, or a standalone class, that create [aggregates](#aggregates) from a [commmand](#command).

#### Events
Events are POCOs that represent "something" important that happened in our domain model. They are generally emitted as a result of executing a [command](#commands) but that is not required. They are also used to maintain the state of an aggregate via an event stream.

#### Projections
Projections are classes that subscribe to event streams of interest in order to build a new model from the accumulation of those events.

#### Queries
Queries are special [commands](#commands) that imply a question that demands an immediate answer. Typically handled by a projection, but not a requirement.

#### Bus
A bus is a simple interface that allows us to send messages across the system.

#### REPL interface
Carupano provides a command-line REPL interface to perform tasks such as:

 - Resetting a projection
 - Sending a command
 - Running a query
 - Viewing the event history for an aggregate
 - Deleting an aggregate
 - Rolling back an aggregate to a version
