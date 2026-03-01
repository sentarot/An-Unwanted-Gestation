# Language & Engine Selection

## Recommendation: C# with Unity

**C#** (targeting the **Unity** engine) is the strongest fit for this project. Below is the analysis.

---

## Why C# + Unity

### 1. Vivid Visuals — The Primary Requirement

The design docs call for real-time character deformation, particle effects, layered sprite manipulation, and a clinical dashboard UI. Unity handles all of these natively:

| Visual Requirement (from GDD) | Unity Feature |
|---|---|
| Abdominal rig scaling & posture changes | Bone-based 2D (Sprite Rigging) or 3D skeletal animation |
| Sweat particles, glowing veins, skin movement | Particle System + Shader Graph (URP/HDRP) |
| Layered clothing degradation | Sprite layering with dynamic swap or material property changes |
| Multi-panel dashboard (HUD, skill tree, schedule, event log) | Unity UI Toolkit / uGUI with anchored layout groups |
| Per-tick visual state updates | Coroutine-driven or event-driven rendering pipeline |

Unity's **Shader Graph** makes it straightforward to build the "clinical, voyeuristic" aesthetic described in the Core Loop doc — glow effects, color grading, procedural animation — without writing raw GLSL.

### 2. Simulation Architecture

The game's core loop is a **tick-based simulation** with an AI state machine, stat tracking, and event generation. C# excels here:

- **State machines** — C# enums, interfaces, and the `switch` expression make Host AI states (Active / Isolated / Bedridden) clean to implement
- **Event system** — C# delegates and events map directly to the design's event log (mechanical changes -> narrative descriptions)
- **Data modeling** — Classes/structs for Host Archetypes, Gestation Classes, Skill Nodes, and the Biomass economy are natural in C#'s type system
- **Scriptable Objects** — Unity's `ScriptableObject` is ideal for authoring the 5 archetypes, 3 gestation classes, and 30+ skill tree nodes as data assets without hardcoding

### 3. Ecosystem & Practical Advantages

- **Largest indie game ecosystem** — extensive asset store for UI kits, 2D animation tools, shader packs
- **Cross-platform** — desktop (Windows/Mac/Linux), WebGL (browser), and mobile from one codebase
- **Community** — the most documentation, tutorials, and hiring pool of any game engine
- **Mature tooling** — Visual Studio / Rider integration, profiler, frame debugger, animation timeline editor

### 4. Genre Precedent

The majority of ambitious indie simulation games with real-time visual feedback (Plague Inc., RimWorld, Dwarf Fortress Premium, Slay the Spire) are built in Unity/C#. The niche adult game space also heavily favors Unity for projects that go beyond visual-novel mechanics.

---

## Alternatives Considered

### Godot (GDScript / C#)

| Pros | Cons |
|---|---|
| Free and fully open-source | Smaller shader/VFX ecosystem |
| Lightweight, fast iteration | 3D pipeline less mature than Unity |
| GDScript is beginner-friendly | Fewer community assets for complex UI |
| C# support available (Godot 4+) | Particle system less feature-rich |

**Verdict:** Strong runner-up. Best choice if open-source licensing or engine cost is a hard constraint. The visual ceiling is lower for the "vivid visuals" requirement, but adequate for a 2D-only approach.

### Unreal Engine (C++ / Blueprints)

| Pros | Cons |
|---|---|
| Best-in-class 3D rendering | Massive overkill for a 2D/2.5D sim |
| Powerful material editor | C++ iteration speed is slow |
| Nanite/Lumen for photorealism | Steep learning curve |
| | Heavy engine — long compile times |

**Verdict:** Only justified if the project commits to full 3D with photorealistic rendering. The design docs describe a dashboard sim, not a 3D exploration game — Unreal's strengths are wasted here.

### Ren'Py (Python)

| Pros | Cons |
|---|---|
| Dominant in adult VN space | No real-time simulation loop |
| Simple to prototype | No particle system or shader pipeline |
| Built-in dialogue/choice system | Cannot handle the dashboard UI described |
| | Stat-heavy sim mechanics fight the engine |

**Verdict:** Wrong tool for this job. The design goes far beyond visual-novel territory — tick-based simulation, AI state machines, skill trees, and real-time visual deformation are all outside Ren'Py's wheelhouse.

### Pygame / Raw Python

| Pros | Cons |
|---|---|
| Maximum flexibility | No built-in rendering pipeline |
| Python is accessible | Performance ceiling for particle effects |
| | Must build every system from scratch |
| | No shader support without OpenGL bindings |

**Verdict:** Would require rebuilding what Unity provides out of the box. Not practical for the visual fidelity required.

### Rust (Bevy)

| Pros | Cons |
|---|---|
| Memory safety, high performance | Ecosystem is immature (pre-1.0) |
| ECS architecture fits sim games | Steep learning curve |
| | Minimal UI framework |
| | Few community resources for this genre |

**Verdict:** Promising long-term but too early-stage. The UI and visual tooling needed for this project doesn't exist yet in the Bevy ecosystem.

---

## Summary

| Criterion | Unity (C#) | Godot | Unreal | Ren'Py | Pygame | Bevy |
|---|---|---|---|---|---|---|
| Vivid 2D/3D visuals | **A+** | B+ | A+ | D | C | C+ |
| Dashboard / complex UI | **A** | B | B | D | C | D |
| Tick-based simulation | **A** | A | A | C | A | A |
| AI state machines | **A** | A | A | C | A | A |
| Skill tree / data modeling | **A** | A | B+ | C | B | B |
| Iteration speed | **A** | A+ | C | A+ | B | C |
| Community & ecosystem | **A+** | B+ | A | B (niche) | C | D |
| Cost | **Free*** | Free | Free** | Free | Free | Free |

\* Unity Free tier covers studios under $200K revenue. \*\* Unreal takes 5% royalty after $1M.

**Final answer: C# with Unity** delivers the best balance of visual power, simulation architecture, and practical ecosystem support for this project's specific requirements.
