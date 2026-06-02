using Godot;
using Godot.Collections;

namespace RoboGattai.Shared.Data;

[GlobalClass]
public partial class FrameData : CardData
{
    [Export] public int BaseHp { get; set; }
    [Export] public int BaseSpeed { get; set; }
    [Export] public int BaseWeightCapacity { get; set; }
    [Export] public int BaseHeatThreshold { get; set; }
    [Export] public int BaseArmorFront { get; set; }
    [Export] public int BaseArmorSide { get; set; }
    [Export] public int BaseArmorRear { get; set; }
    
    [Export] public int WeaponHardpoints { get; set; }
    [Export] public int SystemHardpoints { get; set; }
    [Export] public int ArmorHardpoints { get; set; }
    
    // Use Godot Array instead of List
    [Export] public Array<string> WeaponHardpointSizes { get; set; } = new();
    [Export] public Array<string> SystemHardpointSizes { get; set; } = new();
    
    public FrameData()
    {
        CardType = CARD_TYPE.FRAME;
    }
}