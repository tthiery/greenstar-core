# VectorFlightCapable Trait

##  How it works

Actors with VectorFlightCapable trait are updated their LocatableTrait position when for each turn (if in a flight).

The source and the target of a vector flight need to be an actor with a Locatable trait which has a position.

A turnscript VectorFlightTranscript iterates all actors with this trait, and invokes the UpdatePosition method of the trait.

The Fuel is reduced on each turn by the same value as the speed capability is set on the flight.

## Contributions to domain model

### Command

- StartFlight (to)
- StopFlight

### Exposed Properties

- Fuel: A property assigned to the trait directly. Fuel is something specific to a vector ship (at least for now).

### Utilized Properties

- Technology Capability Speed (a technology capability of the ship)

## See Also

TODO: RefillShipTranscript also here