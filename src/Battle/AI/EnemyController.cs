using Godot;
using System.Collections.Generic;
using System.Linq;
using RoboGattai.Shared;
using RoboGattai.Shared.Data;
using RoboGattai.Battle.Core;

namespace RoboGattai.Battle.AI;

public partial class EnemyController : Node
{
    private EnemyData _enemyData;
    private MechData _currentMech;
    private int _currentIntentIndex;
    private List<EnemyIntent> _availableIntents;
    private int _turnsSinceLastSpecial;
    
    public void Initialize(EnemyData data)
    {
        _enemyData = data;
        _availableIntents = new List<EnemyIntent>(data.Intents);
        _currentIntentIndex = 0;
        _turnsSinceLastSpecial = 0;
        
        // Build enemy mech
        BuildEnemyMech();
    }
    
    private void BuildEnemyMech()
    {
        _currentMech = new MechData();
        
        // Create a basic frame for enemy
        var frame = new FrameData
        {
            BaseHp = _enemyData.MaxHp,
            BaseSpeed = _enemyData.Speed,
            BaseArmorFront = _enemyData.Armor,
            BaseHeatThreshold = 10
        };
        
        _currentMech.Initialize(frame, null);
        
        // Attach starting components
        foreach (var comp in _enemyData.StartingComponents)
        {
            _currentMech.AttachedComponents[$"SLOT_{_currentMech.AttachedComponents.Count}"] = comp;
            if (comp.Category == COMPONENT_CATEGORY.WEAPON)
                _currentMech.Weapons.Add(comp);
        }
    }
    
    public MANEUVER_TYPE ChooseManeuver(int currentRange, MechData playerMech)
    {
        // AI behavior based on enemy type and current situation
        return _enemyData.Behavior switch
        {
            AI_BEHAVIOR.AGGRESSIVE => ChooseAggressiveManeuver(currentRange),
            AI_BEHAVIOR.DEFENSIVE => ChooseDefensiveManeuver(currentRange, playerMech),
            AI_BEHAVIOR.TACTICAL => ChooseTacticalManeuver(currentRange, playerMech),
            AI_BEHAVIOR.BERSERKER => ChooseBerserkerManeuver(currentRange),
            _ => MANEUVER_TYPE.HOLD
        };
    }
    
    public void TakeTurn(int currentRange, string facing)
    {
        var intent = SelectIntent();
        
        if (intent.Type == INTENT_TYPE.ATTACK)
        {
            // Execute attack
            BattleManager.Instance.ResolveEnemyAttack(intent);
        }
        else if (intent.Type == INTENT_TYPE.DEFEND)
        {
            // Gain block/armor
        }
        else if (intent.Type == INTENT_TYPE.HEAT_MANAGE)
        {
            _currentMech.DissipateHeat(intent.HeatGain);
        }
        else if (intent.Type == INTENT_TYPE.SPECIAL)
        {
            ExecuteSpecialIntent(intent);
        }
        
        _turnsSinceLastSpecial++;
    }
    
    public MechData GetCurrentMech() => _currentMech;
    
    public bool IsAlive() => _currentMech != null && !_currentMech.IsDisabled;
    
    public void DissipateHeat()
    {
        _currentMech?.DissipateHeat(0);
    }
    
    public int GetAccuracyBonus()
    {
        return 0; // Base accuracy
    }
    
    private EnemyIntent SelectIntent()
    {
        // Weighted random based on probabilities and cooldowns
        var available = _availableIntents.Where(i => i.Cooldown == 0 || _turnsSinceLastSpecial >= i.Cooldown).ToList();
        
        float totalWeight = available.Sum(i => i.Probability);
        float roll = MathUtils.RandomRangeFloat(0, totalWeight);
        
        float cumulative = 0;
        foreach (var intent in available)
        {
            cumulative += intent.Probability;
            if (roll <= cumulative)
                return intent;
        }
        
        return available.LastOrDefault();
    }
    
    private void ExecuteSpecialIntent(EnemyIntent intent)
    {
        _turnsSinceLastSpecial = 0;
        // Parse special effect string and execute
    }
    
    private MANEUVER_TYPE ChooseAggressiveManeuver(int range)
    {
        // Advance to get to optimal weapon range
        if (range > (int)RANGE_BAND.CLOSE)
            return MANEUVER_TYPE.ADVANCE;
        return MANEUVER_TYPE.HOLD;
    }
    
    private MANEUVER_TYPE ChooseDefensiveManeuver(int range, MechData playerMech)
    {
        // Retreat if taking damage, hold if safe
        if (_currentMech.CurrentHp < _currentMech.MaxHp * 0.5f)
            return MANEUVER_TYPE.RETREAT;
        return MANEUVER_TYPE.HOLD;
    }
    
    private MANEUVER_TYPE ChooseTacticalManeuver(int range, MechData playerMech)
    {
        // Try to maintain optimal range for weapons
        bool hasCloseWeapons = _currentMech.Weapons.Any(w => w.Range == RANGE_BAND.CLOSE);
        bool hasFarWeapons = _currentMech.Weapons.Any(w => w.Range == RANGE_BAND.FAR);
        
        if (hasCloseWeapons && range > (int)RANGE_BAND.CLOSE)
            return MANEUVER_TYPE.ADVANCE;
        if (hasFarWeapons && range < (int)RANGE_BAND.FAR)
            return MANEUVER_TYPE.RETREAT;
        
        return MANEUVER_TYPE.HOLD;
    }
    
    private MANEUVER_TYPE ChooseBerserkerManeuver(int range)
    {
        // Always advance to close range
        return MANEUVER_TYPE.ADVANCE;
    }
}