# Discoverable Trait

##  How it works

Each actor which should is visible to all or a subset of all players has a `DiscoverableTrait` attached. The trait persist the player, the level of discovery and the turn in which the discovery was made.

The discovery trait does not holds only the moments when the player made the discover, not the actual data. This has to be done as part of the traits. The trait contains a property to return the current list of all relevant turns to help other traits archiving snapshots of their data.

## Contributions to domain model

### Command

### Exposed Properties

### Utilized Properties

## See Also
