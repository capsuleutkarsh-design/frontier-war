# Art tools

## Blender FBX export (for every model)
- Units: 1 Blender unit = 1 m. **Apply scale** (Ctrl+A > Scale) before export.
- Export > FBX: Selected objects only, Apply Scalings: **FBX Units Scale**, Forward: **-Z**, Up: **Y**, Apply Transform on.
- Child objects named `Turret`, `Barrel`, `Rotor`, `Wheel_*`, `Leg_*`, `Muzzle_*` (see `Docs/03_TechnicalDesign/08_Rendering_and_Art.md`).
- One shared palette texture: `Art/Tools/palette_256.png` (made in M6).
