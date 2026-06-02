using Godot;
using Godot.Collections;

namespace RoboGattai.Shared.Data;

// Enums must be defined OUTSIDE the class
public enum ENEMY_TYPE
{
    NORMAL,
    ELITE,
    BOSS
}

public enum AI_BEHAVIOR
{
    AGGRESSIVE,
    DEFENSIVE,
    TACTICAL,
    BERSERKER,
    SUPPORT,
    ADAPTIVE
}

[GlobalClass]
public partial class EnemyData : Resource
{
    [Export] public string EnemyId { get; set; }
    [Export] public string DisplayName { get; set; }
    [Export] public ENEMY_TYPE EnemyType { get; set; }
    [Export] public int Act { get; set; } = 1;
    
    // Stats
    [Export] public int MaxHp { get; set; }
    [Export] public int Speed { get; set; }
    [Export] public int Armor { get; set; }
    [Export] public int BaseDamage { get; set; }
    [Export] public int HeatGeneration { get; set; }
    
    // AI Behavior
    [Export] public AI_BEHAVIOR Behavior { get; set; }
    
    // Intents array - you were missing this
    [Export] public Array<EnemyIntent> Intents { get; set; } = new();
    
    // Use Godot Array instead of List
    [Export] public Array<ComponentData> StartingComponents { get; set; } = new();
    
    // Rewards
    [Export] public int GoldReward { get; set; }
    [Export] public int CardRewardCount { get; set; } = 1;
    [Export] public float RuneRewardChance { get; set; } = 0.3f;
    
    // Boss specific
    [Export] public int PhaseThreshold { get; set; } = 0;
    [Export] public Array<EnemyIntent> PhaseTwoIntents { get; set; } = new();
}