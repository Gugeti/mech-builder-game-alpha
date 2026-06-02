using Godot;

namespace RoboGattai.Shared.Data;

[GlobalClass]
public partial class EnemyIntent : Resource
{
    [Export] public string IntentName { get; set; }
    [Export] public INTENT_TYPE Type { get; set; }
    [Export] public int Damage { get; set; }
    [Export] public int Block { get; set; }
    [Export] public int HeatGain { get; set; }
    [Export] public int Heal { get; set; }
    [Export] public string SpecialEffect { get; set; }
    [Export] public int Cooldown { get; set; } = 0;
    [Export] public float Probability { get; set; } = 1.0f;
}

public enum INTENT_TYPE
{
    ATTACK,
    DEFEND,
    HEAT_MANAGE,
    SPECIAL,
    BUFF,
    DEBUFF
}