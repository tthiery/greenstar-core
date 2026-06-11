# Discoverable Trait

##  How it works

Each actor which is visible to all or a subset of all players has a `Discoverable` trait attached. The trait persist the player, the level of discovery and the turn in which the discovery was made.

The discovery trait does not holds only the moments when the player made the discover, not the actual data. This has to be done as part of the traits. The trait contains a property to return the current list of all relevant turns to help other traits archiving snapshots of their data.



Fog of War

- Fog of War View
  - Restricted View on the world
  - Known to the player (Level = PropertyAware) is
    - Every actor associated or friendly to a player reports itself
    - Every actor associated or friendly to a player reports within a scan range
      - Planets report their orbits
      - Ships report in their scan range
      - SpySatellite report stuff in their scan range
  - Lifetime of fog is one turn.
    - In the turn everything reported is on level PropertyAware
    - After the turn, everything on PropertyAware but no longer reported is downgraded to Known, fixed elements to LocationAware
  - Traits are flagged as Known / LocationAware / PropertyAware.
    - Planet properties like temperature or resources might be misreported by the trait to deceive the player when in mode Known/LocationAware but not in PropertyAware

- Technically
  - Fog of War is implemented in `PlayerActorView` by filtering out everything not at least Known
    - ✅ Used for rendering
    - ✅ Used for property viewing
    - ✅ Not used for algorithms in the core
  - `EnvironmentAwarenessTranscript` runs over all `Associatable`, which then call the trait `EnvironmentAware` which performs the scanning. Each identified object is called on its `Discoverable.Discover(PropertyAware, CurrentTurn)` function. The run is very early to ensure avoiding friendly fire.
    - `Planet` with an population scan everything in their orbit as PropertyAware
    - `Ship` scan everything in their hosting location
    - `SpySatellite` scans the larger range.
  - Traits implement past knowledge and report their turn-based state. They discard old entries no longer in the discovery list
    - `Resourceful`
    - `Populatable`
    - `Associatable`
  - A helper function on the trait lookup `TraitByPlayerKnowledge` supports the UI and game logic to be deceived and filter them based on discover.


## Contributions to domain model

### Command

### Exposed Properties

### Utilized Properties

## See Also
