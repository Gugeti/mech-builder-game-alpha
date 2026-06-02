using Godot;
using System.Collections.Generic;
using RoboGattai.Shared.Data;

namespace RoboGattai.MechBuilding.Validation;

public static class HardpointValidator
{
    public static readonly Dictionary<string, string> HARDPOINT_CATEGORIES = new()
    {
        { "ARM_LEFT", "WEAPON" },
        { "ARM_RIGHT", "WEAPON" },
        { "SHOULDER_LEFT", "WEAPON" },
        { "SHOULDER_RIGHT", "WEAPON" },
        { "BACK", "WEAPON" },
        { "HEAD", "SYSTEM" },
        { "CHEST", "SYSTEM" },
        { "LEG_LEFT", "SYSTEM" },
        { "LEG_RIGHT", "SYSTEM" },
        { "ARMOR_1", "ARMOR" },
        { "ARMOR_2", "ARMOR" },
        { "ARMOR_3", "ARMOR" },
        { "POWER", "POWER" }
    };
    
    public static bool IsValidHardpointForComponent(string hardpoint, ComponentData component)
    {
        if (!HARDPOINT_CATEGORIES.TryGetValue(hardpoint, out string category))
            return false;
        
        return category switch
        {
            "WEAPON" => component.Category == COMPONENT_CATEGORY.WEAPON,
            "SYSTEM" => component.Category == COMPONENT_CATEGORY.SYSTEM,
            "ARMOR" => component.Category == COMPONENT_CATEGORY.ARMOR,
            "POWER" => component.Category == COMPONENT_CATEGORY.POWER,
            _ => false
        };
    }
    
    public static List<string> GetAvailableHardpoints(FrameData frame, ComponentData component)
    {
        List<string> available = new();
        
        // Check each hardpoint type based on frame stats
        if (component.Category == COMPONENT_CATEGORY.WEAPON)
        {
            for (int i = 0; i < frame.WeaponHardpoints; i++)
            {
                available.Add($"WEAPON_{i}");
            }
        }
        else if (component.Category == COMPONENT_CATEGORY.SYSTEM)
        {
            for (int i = 0; i < frame.SystemHardpoints; i++)
            {
                available.Add($"SYSTEM_{i}");
            }
        }
        else if (component.Category == COMPONENT_CATEGORY.ARMOR)
        {
            for (int i = 0; i < frame.ArmorHardpoints; i++)
            {
                available.Add($"ARMOR_{i}");
            }
        }
        
        return available;
    }
    
    public static bool IsSizeCompatible(string hardpointSize, string componentSize)
    {
        // Light fits in Light, Medium, Heavy
        // Medium fits in Medium, Heavy  
        // Heavy only fits in Heavy
        
        if (hardpointSize == "HEAVY") return true;
        if (hardpointSize == "MEDIUM" && componentSize != "HEAVY") return true;
        if (hardpointSize == "LIGHT" && componentSize == "LIGHT") return true;
        
        return false;
    }
}