# Engine Comparison

The goal of this project is to implement the same feature set in Unity,
Godot, and LibGDX and compare the development experience.

Development time is approximate active development time.

The first implementation may include additional time spent designing the
feature, learning the problem domain, and defining requirements that can
later be reused by the other implementations.

---

## Generation

### Recursive Backtracker

**Requirements**
- Generate a perfect maze using the Recursive Backtracker algorithm.
- Keep the outer border closed.
- Support deterministic generation using a seed.
- Support rectangular maps.

#### Unity

**Time:** ~1h · **Difficulty:** Medium

**Notes**
- The basic algorithm and seeded generation were straightforward to implement.
- Coordinate handling required care because the dungeon grid uses `[x, y]` indexing.
- Rectangular and even-sized maps exposed assumptions that were less obvious with square odd-sized maps.
- An iteration limit was added while debugging an infinite loop.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

### Randomized Prim

**Requirements**
- Generate a perfect maze using the Randomized Prim algorithm.
- Keep the outer border closed.
- Support deterministic generation using a seed.
- Support rectangular maps.

#### Unity

**Time:** ~1h · **Difficulty:** Medium

**Notes**
- Reusing the grid representation and neighbor-handling infrastructure from Recursive Backtracker made the implementation relatively straightforward.
- Preventing duplicate cells from being added to the frontier required explicit handling.
- Rectangular and even-sized maps helped expose assumptions that were not visible with square odd-sized maps.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

### Randomized Kruskal

**Requirements**
- Generate a perfect maze using the Randomized Kruskal algorithm.
- Keep the outer border closed.
- Support deterministic generation using a seed.
- Support rectangular maps.

#### Unity

**Time:** ~1.5h · **Difficulty:** Medium

**Notes**
- Randomized Kruskal required adding a disjoint-set data structure, unlike the previous generators.
- The disjoint set uses path compression and union by size.
- Connections are shuffled using the generator's seeded random source so generation remains deterministic.
- Existing maze-generation infrastructure could still be reused for map initialization, passage carving, bounds checking, and generation-step reporting.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

## Generation History & Playback

### Record generation history

**Requirements**
- Optionally record intermediate dungeon states during generation.
- Preserve the sequence of generation steps without affecting the final generated map.
- Allow generation to run without recording history when playback is not needed.
- Keep history recording independent from visualization.

#### Unity

**Time:** ~3h · **Difficulty:** Medium

**Notes**
- Generation steps are stored as immutable snapshots, allowing the same read-only map representation to represent both final and historical states.
- Recording is kept outside individual generation algorithms: generators report completed steps without knowing whether those steps are being stored.
- History recording subscribes to generation-step events only for the duration of generation and unsubscribes afterward, including when generation fails.
- When recording is disabled, intermediate snapshots are not created, avoiding the main memory and copying cost of the feature.
- History contains generation data only; current-step and playback state remain UI concerns.
- Full snapshots make playback and random access simple, at the cost of considerably more memory than storing only per-step changes.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

### Step-by-step playback

**Requirements**
- Allow recorded generation history to be inspected step by step.
- Allow the user to move forward and backward through recorded steps.
- Allow the user to jump directly to a specific step.
- Allow recorded steps to be played automatically.
- Allow playback to be paused and resumed.
- Allow playback speed to be changed.
- Disable playback controls when no generation history is available.

#### Unity

**Time:** ~2.5h · **Difficulty:** Medium

**Notes**
- Automatic playback reuses the same step-selection path as manual navigation and slider changes rather than maintaining a separate visualization flow.
- UI step numbers are displayed as 1-based values while stored history remains 0-based.
- Playback controls derive their state from the current history and step, preventing navigation beyond the first or last recorded state.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

## Visualization

### Dungeon rendering

...

### Fit dungeon to preview

...

## UI

### Configuration UI

...

### Responsive layout

...

### Theme system

...

## Input

### Mouse and touch

...

### Gamepad navigation

...

## Gameplay scene handoff

**Requirements**
- Allow a generated dungeon to be passed from the Dungeon Lab to a selected gameplay demo.
- Prevent gameplay from starting before a dungeon has been generated.
- Pass the same generated dungeon map to the gameplay scene.
- Do not retain transferred dungeon data after it has been consumed by the receiving scene.
- Allow gameplay scenes to be run independently during development.

#### Unity

**Time:** ~2h · **Difficulty:** Medium

**Notes**
- Cross-scene dungeon data is initially stored in a static runtime session and treated as a one-time payload: the Dungeon Lab stores the map before loading a gameplay scene, and the receiving scene consumes and removes it.
- Consuming the payload prevents an old generated map from surviving a return to the Dungeon Lab without its corresponding generation history and UI state.
- Static session state persists outside individual test lifecycles, so tests that use it explicitly clear the session before and after each test to prevent state leaking between tests.
- Requiring a map from the Dungeon Lab made isolated gameplay-scene development cumbersome, so gameplay scenes use a reproducible debug dungeon when started directly in the Unity Editor.
- The debug fallback uses the normal generation pipeline with fixed parameters and is shared between gameplay demos rather than belonging specifically to Flood Escape.
- Missing session data still produces a warning in the Editor, preventing the debug fallback from silently hiding a broken handoff. In builds, missing dungeon data remains an error and no debug map is generated.
- Scene handoff is tested without actually loading the target scene: the Dungeon Lab verifies the requested scene and transferred map, while the gameplay controller separately verifies that the same map instance is consumed.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

## Gameplay

### Gameplay map rendering

**Requirements**
- Render the generated dungeon as a grid-based gameplay map.
- Visually distinguish walkable and non-walkable cells.
- Support rectangular maps of different sizes.
- Keep the complete gameplay map visible within the camera view.

#### Unity

**Time:** ~1h · **Difficulty:** Easy

**Notes**
- The gameplay map is rendered using a Tilemap, with dungeon `[x, y]` coordinates mapping directly to Tilemap cell coordinates.
- Map rendering is separated from the gameplay controller; the renderer only translates dungeon cells into tiles.
- Tile references use the common tile base type, leaving the renderer compatible with different tile implementations, including animated tiles.
- The existing camera-fitting component could be reused from the Dungeon Lab. Configurable world-space padding was added to keep the map away from the screen edges.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

### Flood Escape

...

## Testing

### Automated tests

**Requirements**
- Automatically verify core dungeon generation behavior.
- Test map, snapshot, history, and generation behavior independently from the graphical application where possible.
- Verify shared maze-generation invariants and deterministic generation.
- Verify engine-dependent behavior through Play Mode tests where required.
- Verify interactive UI behavior, including generation controls and history playback.

#### Unity

**Time:** ~7h · **Difficulty:** Medium

**Notes**
- Pure generation and data structures can be tested in Edit Mode, while components that depend on Unity lifecycle behavior require Play Mode tests.
- Shared generator tests verify the same invariants across maze algorithms without duplicating the test logic for each implementation.
- UI testing required a reusable fixture that constructs the required controls and dependencies programmatically.
- Engine-dependent collaborators are replaced with lightweight test implementations where their actual rendering or camera behavior is outside the test's responsibility.
- Setting up test assemblies and their dependencies required noticeable initial configuration, but extending the test suite became straightforward once the structure was in place.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

## Platform

### Android build

...