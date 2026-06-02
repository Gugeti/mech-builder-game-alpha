namespace RoboGattai.Shared.Data;

public class CoreData
{
    public int MaxIntegrity { get; set; } = 20;
    public int CurrentIntegrity { get; set; } = 20;
    public int GenerationRating { get; set; } = 0;
    public CORE_TYPE Type { get; set; } = CORE_TYPE.INDUSTRIAL;
    
    public void TakeDamage(int damage)
    {
        CurrentIntegrity -= damage;
    }
    
    public void Repair(int amount)
    {
        CurrentIntegrity = System.Math.Min(MaxIntegrity, CurrentIntegrity + amount);
    }
    
    public void UpgradeIntegrity(int amount)
    {
        MaxIntegrity += amount;
        CurrentIntegrity += amount;
    }
}

public enum CORE_TYPE
{
    INDUSTRIAL, // +5 HP, salvage bonus
    MILITARY,   // +1 damage, +1 generation
    PROTOTYPE,  // +2 generation, overclock bonus
    HIVE        // Can field 2 mechs, -1 generation
}