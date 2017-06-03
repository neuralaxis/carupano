# CARUPANO
A library for writing event-sourced CQRS bounded contexts with a minimal amount of intrusion on the core domain models.
 
 ## Goals
  - To facilitate the rapid development of expressive, scalable, robust software
  - To reduce the amount of infrastructure code needed to achieve an Information System (IS) goal
  - To reduce the number of design decisions necessary to provide functionality
  
## Design Goals
 - Model DDD concepts themselves
 - Do not require implementation domain assets to inherit, extend or implement any Carupano libraries.
 - Provide extensibility points
 
 ## Concepts
 
 - [Models](#models)
 - [Aggregates](#aggregates)
 - [Factories](#factories)
 - [Commands](#commands)
 - [Command Bus](#command-bus)
 - [Events](#events)
 - [Event Store](#event-store)
 - [Projections](#projections)
 - [Queries](#queries)
 - [Services](#services)
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
                .WithId(c => c.Localizer)
                .CreatedBy<CreateFlightReservation>()
                .Executes<CancelFlightReservation>(c => c.Localizer);
            });
            builder.Projection<ReservationList>(cfg =>
            {
                cfg
                    .WithState(c => c.LastEventId)
                    .SubscribesTo<FlightReservationCreated>()
                    .SubscribesTo<FlightReservationCancelled>();
            });
            Model = builder.Build();
 ```       
 
#### Aggregates
Aggregates in Carupano are classic DDD aggregates. They possess a unique identity attribute, handle [commands](#commands), and mutate their state through changesets represented as [events](#events).

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
Commands are POCOs that are sent to aggregates or stand alone command handlers through a [bus](#bus). They are usually named after an imperative verb on an aggregate, such as:

*CancelFlightReservation*,*ChangeCustomerEmailAddress*, *CancelDelivery*,*ExecuteOrder*, etc.

#### Command Bus
A bus is a simple interface that allows us to send [commands](#commands) across the system. Example implementations are an InProcess, AzureServiceBus, RabbitMQ, etc.

#### Factories
Factories are methods in an aggregate, or a standalone class, that create [aggregates](#aggregates) either from a [commmand](#command) or in some cases, as a reaction to an [event](#events) from another part of the system. Factory commands are generally named Create*Aggregate*, such as:

*CreateFlightReservation*,*CreateCustomer*,*CreateOrder*,*ExecuteOrder*, etc.

#### Events
Events are POCOs that represent "something" important that happened in our domain model. They are generally emitted as a result of executing a [command](#commands) but that is not required. They are also used to maintain the state of an aggregate via an event stream. They are generally named in the past tense, and **always** represent something that has happened already. For example:

*FlightReservationCreated*,*CustomerEmailAddressChanged*,*DeliveryCancelled*,*OrderExecuted*, etc.

#### Event Stores
An event store is a place to persist events, and from which to receive notification of new events having occurred.

#### Projections
Projections are classes that subscribe to event streams of interest in order to build a new model from an accumulation of disparate events over time. They usually represent a view on a user interface that is important to the domain, and facilitate [queries](#queries), for example a *FlightReservationList*, *CustomerList*, *CancelledDeliveries*,*NewOrders*, etc.

A valuable characteristic of projections is that they can be created at any time, from events that ocurred in the past and are persisted in our [event store](#event-stores)

```cs
    public class ReservationList
    {
        List<ReservationListItem> _reservations = new List<ReservationListItem>();
        public void On(FlightReservationCreated created)
        {
            _reservations.Add(new ReservationListItem { Localizer = created.Localizer, FlightId = created.Localizer });
        }
        public void On(FlightReservationCancelled cancelled)
        {
            var resv = _reservations.Single(c => c.Localizer == cancelled.Localizer);
            _reservations.Remove(resv);
        }

        public IEnumerable<ReservationListItem> Query(SearchReservationsByFlight query)
        {
            return _reservations.Skip(query.Page * query.PageSize).Take(query.PageSize).Where(c => c.FlightId == query.FlightId);
        }
    }

```
#### Queries
Queries are special [commands](#commands) that imply a question that demands an immediate answer. Typically handled by a projection, but not a requirement. 

### Services
Services represent functionality that doesn't fit neatly into the concept of an aggregate. You can register services into Carupano, for injection into your aggregates, projections, or command handlers.

#### REPL interface
Carupano provides a command-line REPL interface to perform tasks such as:

 - Resetting a projection
 - Sending a command
 - Running a query
 - Viewing the event history for an aggregate
 - Deleting an aggregate
 - Rolling back an aggregate to a version
