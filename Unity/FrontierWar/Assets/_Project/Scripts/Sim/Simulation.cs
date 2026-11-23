using System.Collections.Generic;

namespace FW.Sim
{
    /// <summary>
    /// All game data. Plain C#, no UnityEngine objects (00_Architecture §1).
    /// M0: only the tick counter. Units, teams, map are added in M1.
    /// </summary>
    public sealed class GameState
    {
        public long Tick;
    }

    /// <summary>One job per system. Systems run in a fixed order every tick (00_Architecture §2).</summary>
    public interface ISimSystem
    {
        void Tick(GameState state);
    }

    /// <summary>Owns the GameState and runs every system once per tick.</summary>
    public sealed class Simulation
    {
        public GameState State { get; } = new GameState();

        private readonly List<ISimSystem> _systems = new List<ISimSystem>();

        /// <summary>Add systems in the order from 00_Architecture §2. Never reorder without a reason.</summary>
        public void AddSystem(ISimSystem system) => _systems.Add(system);

        public void Tick()
        {
            for (int i = 0; i < _systems.Count; i++) _systems[i].Tick(State);
            State.Tick++;
        }
    }
}
