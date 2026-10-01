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

---

### Randomized Prim

...

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

### Automated tests for dungeon generation

**Requirements**
- Automatically verify core dungeon generation behavior.
- Test core map, snapshot, history, and generation behavior independently
  from the graphical application where possible.
- Verify maze generation invariants and deterministic generation.

#### Unity

**Time:** ~2h · **Difficulty:** Medium

**Notes**
- Uses Unity Test Framework with NUnit and Edit Mode tests for non-graphical code.
- Production code and tests are separated into dedicated assemblies.
- NUnit assertions and parameterized test cases were straightforward to use.
- Pure C# code is straightforward to test in Edit Mode.
- Test Runner provides useful failure diagnostics.
- Setting up assembly boundaries and dependencies took more time than writing
  the initial tests themselves.
- Shared contract tests for the different maze generators are planned but not
  yet implemented.

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