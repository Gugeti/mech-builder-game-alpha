using Godot;
using Godot.Collections;
using System.Linq;
using RoboGattai.Shared;
using RoboGattai.Shared.Data;
using RoboGattai.Cards.Hand;
using RoboGattai.Cards.Energy;

namespace RoboGattai.MechBuilding.Assembly;

public partial class MechAssembly : Node
{
    [Export] public NodePath EnergyManagerPath { get; set; }
    
    private EnergyManager _energyManager;
    private MechData _activeMech;
    private Array<FrameData> _reserveFrames = new();
    private PilotData _assignedPilot;
    private int _bpSpentThisPhase;
    
    public MechData ActiveMech => _activeMech;
    public bool HasActiveMech => _activeMech != null && !_activeMech.IsDisabled;
    
    public override void _Ready()
    {
        if (!EnergyManagerPath.IsEmpty)
        {
            _energyManager = GetNode<EnergyManager>(EnergyManagerPath);
        }
    }
    
    public void StartAssemblyPhase(PilotData pilot, int availableBp)
    {
        _assignedPilot = pilot;
        _bpSpentThisPhase = 0;
        
        if (_activeMech != null)
        {
            _activeMech.DissipateHeat(0);
            _activeMech.IsOverheated = false;
        }
        
        EventBus.Instance.EmitSignal(EventBus.SignalName.BuildPhaseStarted, availableBp);
    }
    
    public bool DeployFrame(FrameData frame)
    {
        if (_activeMech != null) return false;
        if (_energyManager != null && !_energyManager.SpendEnergy(2)) return false;
        
        _activeMech = new MechData();
        _activeMech.Initialize(frame, _assignedPilot);
        
        _bpSpentThisPhase += 2;
        EventBus.Instance.EmitSignal(EventBus.SignalName.FrameDeployed, frame.CardId);
        return true;
    }
    
    public bool AttachComponent(ComponentData component, string hardpointId)
    {
        if (_activeMech == null) return false;
        if (_energyManager != null && !_energyManager.SpendEnergy(component.EnergyCost)) return false;
        
        if (!ValidateHardpointAvailability(hardpointId)) return false;
        
        int projectedWeight = _activeMech.CurrentWeight + component.Weight;
        
        _activeMech.AttachedComponents[hardpointId] = component;
        
        switch (component.Category)
        {
            case COMPONENT_CATEGORY.WEAPON:
                _activeMech.Weapons.Add(component);
                break;
            case COMPONENT_CATEGORY.ARMOR:
                _activeMech.Armor.Add(component);
                break;
            case COMPONENT_CATEGORY.SYSTEM:
                _activeMech.Systems.Add(component);
                break;
            case COMPONENT_CATEGORY.POWER:
                _activeMech.PowerCore = component;
                break;
        }
        
        _activeMech.RecalculateWeight();
        _bpSpentThisPhase += component.EnergyCost;
        
        EventBus.Instance.EmitSignal(EventBus.SignalName.ComponentAttached, component.CardId, hardpointId);
        return true;
    }
    
    public bool InstallPilot(PilotData pilot)
    {
        if (_activeMech == null) return false;
        if (_activeMech.Pilot != null) return false;
        if (_energyManager != null && !_energyManager.SpendEnergy(1)) return false;
        
        _activeMech.Pilot = pilot;
        _assignedPilot = pilot;
        _bpSpentThisPhase += 1;
        return true;
    }
    
    public bool SwapFrame(FrameData newFrame)
    {
        if (_activeMech == null) return false;
        if (!_reserveFrames.Contains(newFrame)) return false;
        if (_energyManager != null && !_energyManager.SpendEnergy(1)) return false;
        
        var oldComponents = new Dictionary<string, ComponentData>(_activeMech.AttachedComponents);
        
        _activeMech.Initialize(newFrame, _assignedPilot);
        
        foreach (var kvp in oldComponents)
        {
            if (ValidateHardpointCompatibility(kvp.Value, kvp.Key))
            {
                _activeMech.AttachedComponents[kvp.Key] = kvp.Value;
            }
        }
        
        _activeMech.RecalculateWeight();
        _bpSpentThisPhase += 1;
        return true;
    }
    
    public bool SalvageComponent(ComponentData component, CardDeck deck)
    {
        if (_energyManager != null && !_energyManager.SpendEnergy(1)) return false;
        
        string key = null;
        foreach (var kvp in _activeMech.AttachedComponents)
        {
            if (kvp.Value == component)
            {
                key = kvp.Key;
                break;
            }
        }
        
        if (key != null)
        {
            _activeMech.AttachedComponents.Remove(key);
            _activeMech.Weapons.Remove(component);
            _activeMech.Armor.Remove(component);
            _activeMech.Systems.Remove(component);
        }
        
        _activeMech.RecalculateWeight();
        deck.ScrapeFromDiscard(component);
        _bpSpentThisPhase += 1;
        return true;
    }
    
    public void Overclock(CardHand hand, CardDeck deck)
    {
        hand.DrawCards(2);
    }
    
    public void FinalizeAssembly()
    {
        if (_energyManager != null)
        {
            int unspent = _energyManager.CurrentEnergy;
            if (unspent > 0)
            {
                _energyManager.ConvertToEmergencyShields();
                if (_activeMech != null)
                {
                    _activeMech.EmergencyShields = Mathf.Min(unspent, CONSTANTS.MAX_EMERGENCY_SHIELDS);
                }
            }
        }
    }
    
    public void OnMechDestroyed()
    {
        _activeMech = null;
    }
    
    private bool ValidateHardpointAvailability(string hardpointId)
    {
        return !_activeMech.AttachedComponents.ContainsKey(hardpointId) || 
               _activeMech.AttachedComponents[hardpointId] == null;
    }
    
    private bool ValidateHardpointCompatibility(ComponentData component, string hardpointId)
    {
        return true;
    }
}