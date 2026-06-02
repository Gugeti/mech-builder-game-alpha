namespace RoboGattai.Shared;

public static class StringUtils
{
    public static string FormatHardpointId(string frameId, string hardpointType, int index)
    {
        return $"{frameId}_{hardpointType}_{index}";
    }
    
    public static string FormatCardId(string name, int tier)
    {
        string safeName = name.Replace(" ", "_").ToUpper();
        return $"{safeName}_T{tier}";
    }
    
    public static string FormatDamageText(int damage, bool isCrit, bool isBlocked)
    {
        if (isBlocked) return $"{damage} (Blocked)";
        if (isCrit) return $"{damage} CRIT!";
        return damage.ToString();
    }
    
    public static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }
    
    public static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToUpperInvariant(input[0]) + input.Substring(1);
    }
}