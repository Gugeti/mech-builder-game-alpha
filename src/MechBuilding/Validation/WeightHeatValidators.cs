using RoboGattai.Shared.Data;

namespace RoboGattai.MechBuilding.Validation;

public static class WeightHeatValidator
{
    public static int CalculateSpeedPenalty(int currentWeight, int maxWeight, int baseSpeed)
    {
        int excess = currentWeight - maxWeight;
        if (excess <= 0) return 0;
        
        // 1 speed per excess weight unit
        int penalty = excess;
        return System.Math.Max(0, baseSpeed - penalty);
    }
    
    public static bool WillOverheat(int currentHeat, int heatToAdd, int threshold)
    {
        return (currentHeat + heatToAdd) > threshold;
    }
    
    public static int CalculateOverheatDuration(int excessHeat, int threshold)
    {
        // More excess heat = longer shutdown
        float ratio = (float)excessHeat / threshold;
        return 1 + (int)(ratio * 2);
    }
    
    public static int CalculateHeatDissipation(MechData mech)
    {
        int dissipation = 0;
        
        // Base dissipation
        dissipation += 1;
        
        // System bonuses
        foreach (var sys in mech.Systems)
        {
            dissipation += sys.HeatDissipation;
        }
        
        // Power core
        if (mech.PowerCore != null)
        {
            // Check for cooling systems in power core
        }
        
        return dissipation;
    }
}