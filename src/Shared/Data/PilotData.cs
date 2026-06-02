using Godot;
using Godot.Collections;

namespace RoboGattai.Shared.Data;

public enum MECH_PATH
{
    BALANCED,
    ASSAULT,    // Heavy weapons, armor
    SKIRMISHER, // Speed, close range
    SNIPER,     // Long range, precision
    SUPPORT,    // Systems, heat management
    BERSERKER   // High risk/reward overheating
}

[GlobalClass]
public partial class PilotData : Resource
{
    [Export] public string PilotId { get; set; }
    [Export] public string DisplayName { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture2D Portrait { get; set; }
    
    // Stats
    [Export] public int BaseGunnery { get; set; }
    [Export] public int BasePiloting { get; set; }
    [Export] public int BaseGuts { get; set; }
    [Export] public int BaseTactics { get; set; }
    
    // Use Godot Array instead of List
    [Export] public Array<CardData> StartingDeck { get; set; } = new();
    
    // Special ability
    [Export] public string AbilityName { get; set; }
    [Export] public string AbilityDescription { get; set; }
    
    // Path selection for this run
    public MECH_PATH SelectedPath { get; set; } = MECH_PATH.BALANCED;
}