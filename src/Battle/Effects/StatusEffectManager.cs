using Godot;
using System.Collections.Generic;
using RoboGattai.Shared.Data;

namespace RoboGattai.Battle.Effects;

public partial class StatusEffectManager : Node
{
    private Dictionary<MechData, List<StatusEffect>> _activeEffects = new();
    
    public void ApplyEffect(MechData target, StatusEffect effect)
    {
        if (!_activeEffects.ContainsKey(target))
            _activeEffects[target] = new List<StatusEffect>();
        
        // Check for stacking or refreshing
        var existing = _activeEffects[target].Find(e => e.EffectId == effect.EffectId);
        if (existing != null)
        {
            existing.Duration = System.Math.Max(existing.Duration, effect.Duration);
            existing.Stacks += effect.Stacks;
        }
        else
        {
            _activeEffects[target].Add(effect);
        }
        
        effect.OnApply(target);
    }
    
    public void ProcessTurnStart(MechData mech)
    {
        if (!_activeEffects.ContainsKey(mech)) return;
        
        foreach (var effect in _activeEffects[mech].ToArray())
        {
            effect.OnTurnStart(mech);
            effect.Duration--;
            
            if (effect.Duration <= 0)
            {
                effect.OnRemove(mech);
                _activeEffects[mech].Remove(effect);
            }
        }
    }
    
    public void ProcessTurnEnd(MechData mech)
    {
        if (!_activeEffects.ContainsKey(mech)) return;
        
        foreach (var effect in _activeEffects[mech])
        {
            effect.OnTurnEnd(mech);
        }
    }
    
    public void ClearEffects(MechData mech)
    {
        if (_activeEffects.ContainsKey(mech))
        {
            foreach (var effect in _activeEffects[mech])
            {
                effect.OnRemove(mech);
            }
            _activeEffects[mech].Clear();
        }
    }
}

public abstract class StatusEffect
{
    public string EffectId { get; set; }
    public string DisplayName { get; set; }
    public int Duration { get; set; }
    public int Stacks { get; set; } = 1;
    public STATUS_TYPE Type { get; set; }
    
    public virtual void OnApply(MechData target) { }
    public virtual void OnTurnStart(MechData target) { }
    public virtual void OnTurnEnd(MechData target) { }
    public virtual void OnRemove(MechData target) { }
}

public enum STATUS_TYPE
{
    BUFF,
    DEBUFF,
    NEUTRAL
}