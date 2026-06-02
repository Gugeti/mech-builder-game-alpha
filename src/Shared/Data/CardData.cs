using Godot;
using System;

namespace RoboGattai.Shared.Data;

[GlobalClass]
public partial class CardData : Resource
{
    [Export] public string CardId { get; set; }
    [Export] public string DisplayName { get; set; }
    [Export] public string Description { get; set; }
    [Export] public CARD_TYPE CardType { get; set; }
    [Export] public int EnergyCost { get; set; }
    [Export] public int Tier { get; set; } = 1;
    [Export] public Texture2D Icon { get; set; }
    [Export] public bool IsStarter { get; set; } = false;
    
    public virtual bool CanPlay(GameState state) => true;
    public virtual void OnPlay(GameState state) { }
    public virtual void OnDraw(GameState state) { }
    public virtual void OnDiscard(GameState state) { }
}

public enum CARD_TYPE
{
    FRAME,
    COMPONENT_WEAPON,
    COMPONENT_ARMOR,
    COMPONENT_SYSTEM,
    COMPONENT_POWER,
    PILOT,
    TACTICS,
    CORE
}