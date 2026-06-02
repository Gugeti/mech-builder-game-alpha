using RoboGattai.Shared.Data;
using RoboGattai.Battle.Core;

namespace RoboGattai.Battle.Effects;

public static class DamageCalculator
{
    public static int CalculateFinalDamage(
        int baseDamage,
        DAMAGE_TYPE damageType,
        string facing,
        MechData attacker,
        MechData defender,
        int range,
        bool isCritical = false)
    {
        float multiplier = 1.0f;
        
        // Type effectiveness
        multiplier *= GetTypeMultiplier(damageType, defender);
        
        // Critical hit
        if (isCritical) multiplier *= 1.5f;
        
        // Range effectiveness
        multiplier *= GetRangeMultiplier(range, attacker);
        
        // Facing bonus
        if (facing == "REAR") multiplier *= 1.2f;
        
        // Pilot abilities
        multiplier *= GetPilotDamageMultiplier(attacker.Pilot);
        
        // Runes
        multiplier *= GetRuneMultiplier(attacker);
        
        int finalDamage = (int)(baseDamage * multiplier);
        
        // Apply armor
        int armor = GetFacingArmor(facing, defender);
        finalDamage = System.Math.Max(1, finalDamage - armor);
        
        return finalDamage;
    }
    
    public static bool RollForCritical(int gunnerySkill, int luckBonus = 0)
    {
        int critThreshold = 12; // Natural 12 on 2d6
        // TODO: Implement critical hit chance based on skills
        return false;
    }
    
    private static float GetTypeMultiplier(DAMAGE_TYPE type, MechData defender)
    {
        // Energy vs high armor, Kinetic vs low armor, etc.
        return 1.0f;
    }
    
    private static float GetRangeMultiplier(int range, MechData attacker)
    {
        // Optimal range bonuses
        return 1.0f;
    }
    
    private static float GetPilotDamageMultiplier(PilotData pilot)
    {
        if (pilot == null) return 1.0f;
        // Guts-based damage bonuses for certain paths
        return 1.0f;
    }
    
    private static float GetRuneMultiplier(MechData mech)
    {
        // Check attached runes
        return 1.0f;
    }
    
    private static int GetFacingArmor(string facing, MechData mech)
    {
        return facing switch
        {
            "FRONT" => mech.TotalArmorFront,
            "SIDE" => mech.TotalArmorSide,
            "REAR" => mech.TotalArmorRear,
            _ => mech.TotalArmorFront
        };
    }
}