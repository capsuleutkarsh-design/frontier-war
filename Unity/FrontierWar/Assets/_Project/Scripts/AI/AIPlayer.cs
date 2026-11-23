using FW.Sim;

namespace FW.AI
{
    /// <summary>
    /// One AI opponent. It only reads GameState and issues Commands, exactly like the player (06_AI §1).
    /// M0: empty. AI v0 (fixed build order) arrives in milestone M2.
    /// </summary>
    public sealed class AIPlayer
    {
        public int Team { get; }

        public AIPlayer(int team) { Team = team; }

        /// <summary>Called every 10 ticks (0.5 s).</summary>
        public void Think(GameState state) { }
    }
}
