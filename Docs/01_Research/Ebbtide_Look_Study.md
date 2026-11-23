# Ebbtide: look and feel study

**Why we study it:** for the **look only**. Gameplay comes from Rusted Warfare (see `RustedWarfare_Study.md`).
**What it is:** a zombie-survival RTS by ProjectAlive (PC, coming soon on Steam). Its store text says the player directs forces on a "real-time operations map", then zooms "into the 3D pixel-art battlefield".
**Source:** the Steam store page screenshots (looked at 23 Nov 2026). We never copy its art. We copy the *rules* that make it look like a real game.

---

## 1. What makes it look like a real game

| # | Rule we saw | Why it works | Our rule |
|---|---|---|---|
| 1 | **It is real 3D**, with a fixed camera and a pixel filter on top | Shapes and shadows are always correct, and the pixels make it crisp | Low-poly 3D, rendered small and scaled up with no smoothing |
| 2 | **True real-world scale** | Soldiers are tiny next to cars, cars are small next to houses, roads are wide | Follow the scale table (section 3) |
| 3 | **Simple, clean models** | Big flat shapes read well at a small size, and there is no noisy detail | Few polygons, flat colour textures, no fine detail |
| 4 | **One hard sun shadow** | It makes the scene look solid and "real" | One directional light, hard shadows, soft ambient light |
| 5 | **Calm colours** | Grey roads, muted greens, brown roofs. Team colour is small. | Lower the saturation, and add a team stripe only |
| 6 | **Orthographic, about 45–55° down** | No perspective distortion, so it reads like a map | Orthographic camera, fixed yaw, zoom only |
| 7 | **Dense, believable world** | Houses, fences, cars, palm trees and street lines | Props everywhere, but they stay low-poly |
| 8 | **UI is thin and dark** | Small dark panels around the edges keep the battlefield free | Dark, semi-transparent panels in the corners only |
| 9 | **Effects are grounded** | Dust and grey smoke, small fires, tracer lines | No bloom, no huge glowing explosions |
| 10 | **Two zoom worlds** | An operations map (like a satellite view) plus the battlefield | We use **strategic zoom with icons** instead (simpler for a solo developer) |

## 2. Screens we saw, described in words

- **City street fight:** an orthographic camera over a suburb grid. Houses with real roof shapes, parked cars, crosswalks and huge zombie crowds as tiny figures. A unit bar runs along the bottom, a support-power list sits bottom-left, and a minimap sits bottom-right.
- **Base building (FOB):** walls made from modular blocks, howitzers, sandbag rings and floodlights. A **build bar** at the bottom has categories (Defence, Supply, Lighting…) and a ghost preview with a green/red placement check.
- **Operations map:** a dark satellite-style map with district names, a red "infected" overlay, unit icons and a resource bar at the top.
- **Night scenes:** an orange, low-light grade with lights from buildings. The pixel filter still keeps things readable.

## 3. Our scale table (locked)

| Object | Real size | Default zoom (orthographic size 60, 1080p) | After the pixel filter (÷4) |
|---|---|---|---|
| Soldier | 1.8 m | ~16 px | ~4 big pixels |
| Jeep / buggy | 4.5 m | ~40 px | ~10 |
| Main battle tank | 9.5 m | ~85 px | ~21 |
| Light mech | 7 m tall | ~63 px | ~16 |
| House | 12 m | ~108 px | ~27 |
| Factory / Command Core | 40–60 m | ~360–540 px | ~90–135 |
| Experimental walker | 30 m tall | ~270 px | ~68 |
| Carrier | 300 m | ~2,700 px (bigger than the screen) | – |

The rule: at orthographic size 60 the screen shows about 120 m of height, so 1 m = 9 screen pixels on a 1080p screen. Zooming out to 180 (strategic) makes everything 3× smaller.

**1 Unity unit = 1 metre.** Every model is made at real size, and nothing is scaled by hand in the scene.

## 4. How we make this look in Unity

The full technical steps are in `03_TechnicalDesign/08_Rendering_and_Art.md`. In short:
1. URP, one directional light, hard shadows.
2. An orthographic camera at a 50° pitch and 45° yaw, which never rotates.
3. The world camera renders into a **RenderTexture at 1/4 screen size** with **Point (no filter)** sampling, then a full-screen image shows it scaled up. That's the pixel filter.
4. The UI is drawn **at full resolution on top**, so text stays sharp.
5. Selection rings, health bars and icons are drawn after the filter, at full resolution.

## 5. What we will NOT take

- Zombies, the setting, its names, its UI layout pixel for pixel, or its art.
- Its two-layer operations map. That's a big feature, and strategic zoom covers the need.

## 6. Style lock

- Image: `Art/Concepts/pixel/11_style_lock.jpg`, with `Art/Concepts/clean/11_style_lock.png` as the version without the filter.
- Every new model, prop or effect is checked next to this image **in the game** before it is approved.
