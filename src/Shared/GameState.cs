using System.Collections.Generic;
using RoboGattai.Battle.Core;
using RoboGattai.Shared.Data;

namespace RoboGattai.Shared;

public class GameState
{
    // Battle state
    public MechData ActivePlayerMech { get; set; }
    public MechData ActiveEnemyMech { get; set; }
    public int CurrentRange { get; set; }
    public int CurrentRound { get; set; }
    public TURN_PHASE CurrentPhase { get; set; }
    
    // Resources
    public int AvailableEnergy { get; set; }
    public int PlayerCoreIntegrity { get; set; }
    public int EnemyCoreIntegrity { get; set; }
    
    // Modifiers
    public List<RuneData> ActiveRunes { get; set; }
    public List<CurseData> ActiveCurses { get; set; }
    
    // Validation helpers
    public bool IsPlayerTurn { get; set; }
    public bool CanPlayCards { get; set; }
    public bool InAssemblyPhase { get; set; }
}