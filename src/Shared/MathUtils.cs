using Godot;
using System;

namespace RoboGattai.Shared;

public static class MathUtils
{
    private static readonly RandomNumberGenerator RNG = new();
    
    static MathUtils()
    {
        RNG.Randomize();
    }
    
    public static int RollD6(int count = 1)
    {
        int total = 0;
        for (int i = 0; i < count; i++)
        {
            total += RNG.RandiRange(1, 6);
        }
        return total;
    }
    
    public static int Roll2D6()
    {
        return RollD6(2);
    }
    
    public static bool PercentRoll(float percentage)
    {
        return RNG.Randf() < percentage;
    }
    
    public static int RandomRange(int min, int max)
    {
        return RNG.RandiRange(min, max);
    }
    
    public static float RandomRangeFloat(float min, float max)
    {
        return RNG.RandfRange(min, max);
    }
    
    public static T RandomElement<T>(System.Collections.Generic.List<T> list)
    {
        if (list.Count == 0) return default;
        return list[RNG.RandiRange(0, list.Count - 1)];
    }
    
    public static int Clamp(int value, int min, int max)
    {
        return Mathf.Clamp(value, min, max);
    }
    
    public static int CalculateAccuracyRoll(int gunnery, int systemsBonus, int rangePenalty)
    {
        return Roll2D6() + gunnery + systemsBonus - rangePenalty;
    }
    
    public static int CalculateEvasionValue(int speed, int piloting, int systemsBonus)
    {
        return speed + piloting + systemsBonus;
    }
    
    public static bool IsHit(int accuracyRoll, int evasionValue)
    {
        return accuracyRoll >= evasionValue;
    }
}