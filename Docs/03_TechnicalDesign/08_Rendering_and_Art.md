# 08 · Rendering and art pipeline

Target: the **locked look**, in-engine low-poly 3D at real scale with a pixel filter (`Art/Concepts/pixel/11_style_lock.jpg`).

---

## 1. URP settings

- **Unity 6 LTS + URP.** Forward renderer, MSAA off (the pixel filter replaces it), HDR off, post-processing off at launch.
- **One directional light** (sun): pitch 50°, yaw −135° (light from the top-left of the screen). Shadows are **hard**, with 1 cascade, a 2048 map (1024 on low phones), and a distance fitted to the camera view (see §4).
- **Ambient:** flat colour, slightly warm, at 35% brightness. No real-time GI and no reflection probes.
- **SRP Batcher on, GPU instancing on** for all unit materials.

## 2. The pixel filter (how it works)

```
World camera ──► RenderTexture (screen ÷ 4, FilterMode.Point, no MSAA)
                      │
                      ▼
Display: full-screen RawImage on a Screen-Space Canvas, scaled up ×4 (nearest)
                      │
UI camera / Overlay Canvas (FULL resolution): HUD, text, health bars, selection rings, icons
```
1. `PixelFilterController` creates the RenderTexture at `ceil(Screen.width / 4) × ceil(Screen.height / 4)`, and rebuilds it when the screen size changes.
2. The world camera's `targetTexture` = that RT. The display canvas shows it with `RawImage`.
3. **Pixel snapping, to stop the shimmering:** each frame, move the camera to the nearest low-res pixel in its own right/up axes:
   `worldPerPixel = 2 × orthoSize / rtHeight`; snap `pos` along `camera.right` and `camera.up` to multiples of `worldPerPixel`.
   Then shift the RawImage's UV rect by the leftover fraction, so panning still looks smooth.
4. **Health bars, rings and icons draw at full resolution** on the UI layer (they stay crisp and readable). The world, fog and effects are pixelated.
5. A setting called **Pixel size** (3 / 4 / 5) defaults to 4. Lower-end phones get a bonus: rendering 1/16 of the pixels is very fast.

## 3. Models (made in Blender)

| Rule | Value |
|---|---|
| Units | **1 Blender unit = 1 m = 1 Unity unit.** Apply scale before export. Forward = −Z, up = Y (use the FBX export preset in `Art/Tools/`). |
| Style | Low-poly, big flat shapes, bevels only on large edges. **No detail smaller than 0.3 m** (it vanishes under the filter). |
| Triangle budget | Infantry 300 · small vehicle 800 · tank 1,500 · mech 2,000 · ship 2,500–6,000 · building 2,000–6,000 · experimental 8,000–15,000 |
| Textures | **One shared palette texture** (256×256 colour swatches). The UVs of each face point at a swatch. Every model uses the same material, so everything batches and the colours stay consistent. |
| Team colour | One swatch row is "team colour". The shader replaces it with `_TeamColor` (MaterialPropertyBlock per unit). |
| Moving parts | Separate child objects named `Turret`, `Barrel`, `Rotor`, `Wheel_*`, `Leg_*`, `Muzzle_*` (empty). The view script finds them by name. |
| Damage states | Clean / damaged (darker swatch plus a smoke emitter) / wreck (separate simple mesh, burnt palette) |

**Placeholder first:** every unit starts as **grey boxes at the right size** (`Art/Models/_Placeholder`). Real models replace them only after the unit is fun in play.

**Solo-dev tip:** buying one consistent low-poly military or modern pack from the Unity Asset Store, then re-colouring it to our palette, can save months. Check the licence allows commercial use and modification. Mix packs only if they share the same style.

## 4. Shadows with an orthographic camera

- Shadow distance = the camera's visible area plus a 20 m margin. It's recalculated when zooming. Above zoom 180 (strategic), **turn unit shadows off** and keep terrain shadows only.
- Depth bias is tuned once per map size. Test with tanks on slopes to avoid shadow acne.

## 5. Terrain

- **Mesh terrain per map**, not the Unity Terrain component (that's heavy on mobile). Generate it from the map grid: a height per vertex at 4 m spacing, plus vertex colours or a splat texture for grass, dirt, road and sand.
- Roads, fields and water edges are decal-style meshes on top.
- **Water:** a simple plane shader with a colour gradient by depth (from the grid), a foam strip at the shore, and a slow normal scroll. Ship wakes are particle trails.
- **Props:** trees, rocks, fences and houses are placed with an **instanced prop painter** (an editor tool) and drawn with `Graphics.RenderMeshInstanced` per prop type. They have no colliders and no GameObjects at runtime.

## 6. Unit views

- `UnitView` (a pooled prefab instance per live unit) reads the sim unit every frame and sets: position (interpolated), hull rotation, turret and barrel rotation, rotor spin, track UV scroll and wheel spin.
- **Mechs and infantry:** use simple Animator clips (idle, walk, fire, die). The Animator culls when off-screen. At more than 300 infantry on screen, switch to baked vertex animation (only if the profiler says so).
- **Culling:** views outside the camera view are disabled (positions still update cheaply). At strategic zoom, views are hidden and icons are drawn instead.

## 7. Effects (pooled Particle Systems)

| Effect | Look rule |
|---|---|
| Muzzle flash | 2–3 frames, small and yellow-white |
| Tracer | A thin line that fades over 0.1 s |
| Explosion (small / large) | A short orange flash, then **grey-brown smoke** that rises and lingers for 3–8 s |
| Burning wreck | A small fire with a tall grey smoke column (like the style lock) |
| Dust | Brown puffs behind moving vehicles on dirt |
| Wake | White foam trail |
| Shield | A faint blue hex bubble that ripples when hit |
| Craters | Decals that stay 120 s, max 200 at once |

- **No bloom and no screen shake** above 2 px. There's an effect quality setting: Low halves the particle counts.
- **Hard caps:** 150 active particle systems and 200 smoke columns. The oldest are recycled first.

## 8. Art production order (per asset)

1. Grey-box at the right size, then **check it in game** next to the style lock and the scale chart.
2. Model the final low-poly version and UV it to the palette.
3. Export the FBX, then make the prefab `PF_Unit_X` with `UnitView` and the child names set.
4. Run the **in-game check**: at zoom 60, does it read clearly? Is the team colour visible? Is it consistent with its neighbours?
5. Only then mark it "Final" in `Docs/04_Production/Asset_List.md`.
