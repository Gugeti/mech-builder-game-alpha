using Godot;

namespace RoboGattai.Shared.Data;

[GlobalClass]
public partial class ComponentData : CardData
{
    [Export] public COMPONENT_CATEGORY Category { get; set; }
    [Export] public string HardpointType { get; set; }
    [Export] public string Size { get; set; } // Light, Medium, Heavy
    
    // Weapon stats
    [Export] public int Damage { get; set; }
    [Export] public RANGE_BAND Range { get; set; }
    [Export] public int HeatGeneration { get; set; }
    [Export] public int AmmoCapacity { get; set; }
    
    // Armor stats
    [Export] public int ArmorBonus { get; set; }
    [Export] public int HpBonus { get; set; }
    
    // System stats
    [Export] public int SpeedBonus { get; set; }
    [Export] public int AccuracyBonus { get; set; }
    [Export] public int EvasionBonus { get; set; }
    [Export] public int HeatDissipation { get; set; }
    
    // General
    [Export] public int Weight { get; set; }
    [Export] public int EnergyRequirement { get; set; }
    
    public ComponentData()
    {
        CardType = CARD_TYPE.COMPONENT_WEAPON;
    }
}

public enum COMPONENT_CATEGORY
{
    WEAPON,
    ARMOR,
    SYSTEM,
    POWER
}

public enum RANGE_BAND
{
    CLOSE = 0, // 0-1
    MID = 2,   // 2-3
    FAR = 4    // 4-6
}