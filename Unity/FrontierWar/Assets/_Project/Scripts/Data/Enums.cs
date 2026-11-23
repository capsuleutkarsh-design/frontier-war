namespace FW.Data
{
    /// <summary>Armour classes. Damage multipliers per class live in WeaponDef (01_Data §2).</summary>
    public enum ArmourClass { Infantry, Light, Armoured, Air, Naval, Submerged, Structure, Experimental }

    /// <summary>Who can go where. Terrain costs per class live in the move-cost table (02_Movement §2).</summary>
    public enum MoveClass { Wheeled, Tracked, Legged, Infantry, Hover, Naval, Submerged, Colossus, Air, Static }

    /// <summary>Unit groups for UI tabs, AI and stats.</summary>
    public enum UnitCategory { Builder, Land, Mech, Infantry, Air, Naval, Experimental }

    /// <summary>Map terrain types (02_Movement §1).</summary>
    public enum Terrain : byte { Grass, Dirt, Road, Forest, Rough, Cliff, ShallowWater, DeepWater, Lava, Bridge }
}
