namespace FW.Core
{
    /// <summary>Fixed numbers shared by the whole game. See Docs/03_TechnicalDesign/00_Architecture.md.</summary>
    public static class SimConstants
    {
        public const int TicksPerSecond = 20;
        public const float TickDt = 1f / TicksPerSecond;

        /// <summary>Movement grid cell size in metres (02_Movement §1).</summary>
        public const float MoveCellSize = 4f;

        /// <summary>Vision grid cell size in metres (05_Vision §1).</summary>
        public const float VisionCellSize = 8f;

        /// <summary>SpatialHash bucket size in metres (00_Architecture §3).</summary>
        public const float SpatialCellSize = 16f;
    }
}
