using Godot;
using Godot.Collections;
using RoboGattai.Shared;
using RoboGattai.Shared.Data;

namespace RoboGattai.Cards.Energy;

public partial class EnergyManager : Node
{
    public int CurrentEnergy { get; private set; }
    public int MaxEnergy { get; private set; }
    public int BaseEnergy { get; private set; }
    
    private int _bonusEnergy;
    private int _tempEnergy;
    
    public void Initialize(PilotData pilot, CoreData core, Array<RuneData> runes)
    {
        BaseEnergy = CONSTANTS.BASE_BUILD_POINTS;
        
        int generationBonus = core?.GenerationRating ?? 0;
        
        int runeBonus = 0;
        if (runes != null)
        {
            foreach (var rune in runes)
            {
                runeBonus += rune.StartEnergyBonus;
            }
        }
        
        int tacticsBonus = pilot?.BaseTactics ?? 0;
        
        MaxEnergy = BaseEnergy + generationBonus + runeBonus + tacticsBonus;
        _bonusEnergy = runeBonus + tacticsBonus;
        
        Refill();
    }
    
    public void Refill()
    {
        CurrentEnergy = MaxEnergy;
        _tempEnergy = 0;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnergyChanged, CurrentEnergy, MaxEnergy);
    }
    
    public void StartAssemblyPhase()
    {
        Refill();
        EventBus.Instance.EmitSignal(EventBus.SignalName.BuildPhaseStarted, CurrentEnergy);
    }
    
    public bool SpendEnergy(int amount)
    {
        if (CurrentEnergy < amount) return false;
        
        CurrentEnergy -= amount;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnergyChanged, CurrentEnergy, MaxEnergy);
        return true;
    }
    
    public void GainTempEnergy(int amount)
    {
        _tempEnergy += amount;
        CurrentEnergy += amount;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnergyChanged, CurrentEnergy, MaxEnergy);
    }
    
    public void ConvertToEmergencyShields()
    {
        int shields = Mathf.Min(CurrentEnergy, CONSTANTS.MAX_EMERGENCY_SHIELDS);
        CurrentEnergy = 0;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnergyChanged, CurrentEnergy, MaxEnergy);
    }
    
    public void AddPermanentEnergy(int amount)
    {
        MaxEnergy += amount;
        CurrentEnergy += amount;
        EventBus.Instance.EmitSignal(EventBus.SignalName.EnergyChanged, CurrentEnergy, MaxEnergy);
    }
    
    public void ResetToBase()
    {
        MaxEnergy = BaseEnergy + _bonusEnergy;
        Refill();
    }
}