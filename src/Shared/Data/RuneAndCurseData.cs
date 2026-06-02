using Godot;

namespace RoboGattai.Shared.Data;

[GlobalClass]
public partial class RuneData : Resource
{
    [Export] public string RuneId { get; set; }
    [Export] public string DisplayName { get; set; }
    [Export] public string Description { get; set; }
    [Export] public RUNE_TYPE Type { get; set; }
    [Export] public int Tier { get; set; } = 1;
    [Export] public Texture2D Icon { get; set; }
    
    // Effects
    [Export] public int MaxHpBonus { get; set; }
    [Export] public int SpeedBonus { get; set; }
    [Export] public int DamageBonus { get; set; }
    [Export] public int HeatThresholdBonus { get; set; }
    [Export] public int StartEnergyBonus { get; set; }
    [Export] public int CardDrawBonus { get; set; }
    [Export] public float ShopDiscount { get; set; }
    [Export] public float HealingBonus { get; set; }
    
    // Special flags
    [Export] public bool CanSalvageTwice { get; set; }
    [Export] public bool OverclockNoDiscard { get; set; }
    [Export] public bool StartWithEmergencyShield { get; set; }
}

public enum RUNE_TYPE
{
    COMBAT,     // Battle bonuses
    ECONOMY,    // Shop/gold benefits
    UTILITY,    // Quality of life
    SYNERGY     // Specific build enablers
}

[GlobalClass]
public partial class CurseData : Resource
{
    [Export] public string CurseId { get; set; }
    [Export] public string DisplayName { get; set; }
    [Export] public string Description { get; set; }
    [Export] public CURSE_TYPE Type { get; set; }
    
    // Penalties
    [Export] public int MaxHpPenalty { get; set; }
    [Export] public int EnergyPenalty { get; set; }
    [Export] public int CardDrawPenalty { get; set; }
    [Export] public float DamageTakenIncrease { get; set; }
    [Export] public float HealingReduction { get; set; }
    [Export] public int ShopPriceIncrease { get; set; }
    
    // Special
    [Export] public bool StartWithHeat { get; set; }
    [Export] public bool RandomComponentDamage { get; set; }
    [Export] public bool EliteOnlyBattles { get; set; }
}

public enum CURSE_TYPE
{
    MINOR,      // Small inconvenience
    MAJOR,      // Significant challenge
    CRITICAL    // Run-threatening
}