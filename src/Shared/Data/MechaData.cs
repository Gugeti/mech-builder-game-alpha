using Godot;
using System.Collections.Generic;
using System.Linq;

namespace RoboGattai.Shared.Data;

public class MechData
{
    public FrameData Frame { get; set; }
    public PilotData Pilot { get; set; }
    
    // Current state
    public int CurrentHp { get; set; }
    public int CurrentHeat { get; set; }
    public int CurrentWeight { get; set; }
    public int CurrentSpeed { get; set; }
    
    // Components attached
    public Dictionary<string, ComponentData> AttachedComponents { get; set; } = new();
    public List<ComponentData> Weapons { get; set; } = new();
    public List<ComponentData> Systems { get; set; } = new();
    public List<ComponentData> Armor { get; set; } = new();
    public ComponentData PowerCore { get; set; }
    
    // Combat state
    public bool IsDisabled { get; set; } = false;
    public bool IsOverheated { get; set; } = false;
    public int OverheatSkipsRemaining { get; set; } = 0;
    public int EmergencyShields { get; set; } = 0;
    
    // Calculated stats
    public int MaxHp => Frame.BaseHp + Armor.Sum(a => a?.HpBonus ?? 0);
    public int HeatThreshold => Frame.BaseHeatThreshold + (Pilot?.BaseGuts ?? 0) + GetHeatThresholdBonus();
    public int TotalArmorFront => Frame.BaseArmorFront + GetArmorBonus("FRONT");
    public int TotalArmorSide => Frame.BaseArmorSide + GetArmorBonus("SIDE");
    public int TotalArmorRear => Frame.BaseArmorRear + GetArmorBonus("REAR");
    public int EvasionValue => CurrentSpeed + (Pilot?.BasePiloting ?? 0) + GetEvasionBonus();
    public int AccuracyBonus => (Pilot?.BaseGunnery ?? 0) + GetAccuracyBonus();
    
    public void Initialize(FrameData frame, PilotData pilot)
    {
        Frame = frame;
        Pilot = pilot;
        CurrentHp = frame.BaseHp;
        CurrentHeat = 0;
        CurrentSpeed = frame.BaseSpeed;
        RecalculateWeight();
    }
    
    public void RecalculateWeight()
    {
        CurrentWeight = 0;
        foreach (var comp in AttachedComponents.Values)
        {
            if (comp != null) CurrentWeight += comp.Weight;
        }
        
        // Speed penalty for overweight
        int excessWeight = CurrentWeight - Frame.BaseWeightCapacity;
        if (excessWeight > 0)
        {
            CurrentSpeed = Frame.BaseSpeed - excessWeight;
        }
    }
    
    public void AddHeat(int amount)
    {
        CurrentHeat += amount;
        if (CurrentHeat > HeatThreshold)
        {
            IsOverheated = true;
            OverheatSkipsRemaining = CONSTANTS.OVERHEAT_PENALTY_SKIPS;
            EventBus.Instance.EmitSignal(EventBus.SignalName.MechDestroyed);
        }
    }
    
    public void DissipateHeat(int amount)
    {
        int dissipation = GetHeatDissipation();
        CurrentHeat = Mathf.Max(0, CurrentHeat - (amount + dissipation));
        if (CurrentHeat <= HeatThreshold * 0.5f)
        {
            IsOverheated = false;
        }
    }
    
    public int TakeDamage(int damage, string facing)
    {
        int armor = facing switch
        {
            "FRONT" => TotalArmorFront,
            "SIDE" => TotalArmorSide,
            "REAR" => TotalArmorRear,
            _ => TotalArmorFront
        };
        
        int actualDamage = Mathf.Max(1, damage - armor);
        
        if (EmergencyShields > 0)
        {
            int shieldAbsorb = Mathf.Min(EmergencyShields, actualDamage);
            EmergencyShields -= shieldAbsorb;
            actualDamage -= shieldAbsorb;
        }
        
        CurrentHp -= actualDamage;
        
        if (CurrentHp <= 0)
        {
            IsDisabled = true;
            EventBus.Instance.EmitSignal(EventBus.SignalName.MechDestroyed);
        }
        
        return actualDamage;
    }
    
    private int GetHeatThresholdBonus()
    {
        int bonus = 0;
        foreach (var sys in Systems)
        {
            // Check for heat threshold bonuses in systems
        }
        return bonus;
    }
    
    private int GetHeatDissipation()
    {
        int dissipation = 0;
        foreach (var sys in Systems)
        {
            dissipation += sys.HeatDissipation;
        }
        return dissipation;
    }
    
    private int GetArmorBonus(string facing)
    {
        int bonus = 0;
        foreach (var armor in Armor)
        {
            bonus += armor.ArmorBonus;
        }
        return bonus;
    }
    
    private int GetEvasionBonus()
    {
        int bonus = 0;
        foreach (var sys in Systems)
        {
            bonus += sys.EvasionBonus;
        }
        return bonus;
    }
    
    private int GetAccuracyBonus()
    {
        int bonus = 0;
        foreach (var sys in Systems)
        {
            bonus += sys.AccuracyBonus;
        }
        return bonus;
    }
}