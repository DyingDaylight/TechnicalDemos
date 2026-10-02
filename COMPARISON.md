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
- Generation logic is independent from visualization.
- The basic algorithm and seeded generation were straightforward to implement.
- Coordinate handling required care because the dungeon grid uses `[x, y]` indexing (column, row).
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
- Generation logic is independent from visualization.
- The algorithm was implemented using a frontier of unvisited cells adjacent to the generated maze.
- Reusing the same grid representation and neighbor-handling approach as Recursive Backtracker made the implementation relatively straightforward.
- Preventing duplicate cells from being added to the frontier required explicit handling.
- Rectangular and even-sized maps were useful for verifying that the algorithm did not rely on square or odd-sized dimensions.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

### Randomized Kruskal

...

## Generation History & Playback

### Record generation history

...

### Step-by-step playback

...

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

## Testing

### Automated tests for dungeon generation and UI

**Requirements**
- Automatically verify core dungeon generation behavior.
- Test core map, snapshot, history, and generation behavior independently from the graphical application where possible.
- Verify shared maze generation invariants and deterministic generation.
- Test Unity lifecycle-dependent UI behavior in Play Mode.

#### Unity

**Time:** ~4h · **Difficulty:** Medium

**Notes**
- Unity Test Framework supports both fast tests for non-graphical logic and Play Mode tests for behavior that depends on the engine lifecycle.
- Testing pure C# code was straightforward; engine-dependent behavior required a separate Play Mode test setup.
- Shared tests made it possible to verify the same behavioral requirements across different maze algorithms without duplicating test logic.
- Setting up test assemblies and their dependencies required noticeable initial configuration, but adding further tests became straightforward once the structure was in place.

#### Godot

*Not implemented.*

#### LibGDX

*Not implemented.*

### UI tests

...

## Platform

### Android build

...

## Gameplay

### Flood Escape

...