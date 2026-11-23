# Rules so this game does not get stuck

Solo games usually die for the same few reasons. Each rule below fixes one of them. Read this page every Monday.

| Why games get stuck | Rule |
|---|---|
| The code becomes a tangle, and every change breaks something | **Sim and View are separate** (`00_Architecture`). Logic lives in Systems, never in MonoBehaviours. |
| Feature creep: always adding, never finishing | **The GDD is frozen.** New ideas go to `Ideas_Later.md`, not into the game, until 1.0. |
| Making art before the game is fun | **Grey boxes until the M3 Fun Gate.** No final art before M6. |
| Tested only in the editor, and it's slow on the phone | **Weekly build on the Target Phone.** It's the only judge. |
| Big tasks that never feel done | **Every task is 1 day or less.** Bigger ones are split. Write them in `Task_Board.md`. |
| Losing work, being afraid to change things | **Git, with a commit every day** ("what I did today"). Branch for experiments. |
| Re-thinking decisions again and again | **Decision_Log.md:** write a decision once, with the reason. Don't reopen it without new information. |
| Motivation drops in the middle | **Always keep a playable build.** Record a 30-second clip every Friday and post it (devlog). |
| Rewriting systems "properly" | **Make it work, make it right, make it fast**, in that order. Only optimise what the profiler shows. |
| The scope is too big for one person | **The cut list** (below) is decided now, not when you're tired. |

## Daily routine (suggested)
1. Open `Task_Board.md` and pick **one** task.
2. Do it, and test it on the phone or in the Sandbox.
3. Commit, then write one line in `Devlog.md`.

## The cut list (cut in this order if you're behind schedule)
1. Challenges 30 → 10
2. Skirmish maps 24 → 12
3. Units 47 → 36 (keep every counter role, and cut look-alikes first: Heavy Interceptor, AA Cruiser, Hunter Sub, Shield Truck, Mortar Mech, Missile Airship, Jammer Drone, Repair Truck, Scorcher, Hive Mothership, Leviathan)
4. Campaign 50 → 30 missions (3 chapters at launch, the rest as free updates)
5. Weather and night → off at launch
6. Support powers → keep Recon Sweep and Air Strike only

**Never cut:** the movement quality, the AI quality, save/load, or the performance target.

## Definition of Done (for any feature)
- [ ] It works on the Target Phone
- [ ] Its tests in the design doc pass
- [ ] No errors in the console, and 0 GC alloc per frame in battle
- [ ] The design doc is updated if anything changed
- [ ] Committed to git
