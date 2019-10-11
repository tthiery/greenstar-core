# GreenStar

A classic 4X game (explore, expand, exploit, exterminate) in space based with a portion of humor.

## greenstar-core

***Note**: This is a repository for the developers of the game. The game is hosted in a different location.*

The repository `greenstar-core` hosts the core logic of the game. This includes the following .NET assemblies

- `GreenStar.Core`: Turn Engine, Player Abstractions, Cartography, Persistence Abstraction.
- `GreenStar.Stellar`: Stellar objects without the impact of life (e.g. planets, stars).
- `GreenStar.Ships`: Scouts, Colonization Ships, Battleships, ...

### Game Elements

See [docs](docs) sub folder for details.

### Engineering

The following core attributes help to understand the code:

- The game is based on typed actors (e.g. Battleship) which have a set of traits (e.g. discoverable, vector flight capable, destructable).
- The actors are persisted storing a set of properties on request.
- The turn engine executes a set of scripts for each turn.
- Traits can participate on each turn.
- Player commands participate on each turn.