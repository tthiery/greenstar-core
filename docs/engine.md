

## UI Parts

- GamePanel: Contains all the navigatable and modifyable data. Relies on app services and views
- Map: Renders data based on views



## Engine Core

- App Services
  - Expose useful services for the user interface (Command Service, search service, setup, turn services). remote enabled

- TurnManager
  - Hosts the core stores (actor context, player context, ...)
  - Maintains the turn loop
  - Expose Views (actor view, player view, ...) to the world

- Core Engine (in memory, no restriction, access to full actor context, player context, ...)
  - Commands: Only way of users to modify state. Executed by a Transcript.
  - Transcripts: Either Setup, Command or Turn Transcripts. Modify the state.
  - Context: The context in which a transcript runs