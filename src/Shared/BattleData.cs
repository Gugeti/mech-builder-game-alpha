using RoboGattai.Shared.Data;

namespace RoboGattai.Shared;

public class BattleData
{
    public MechData PlayerMech { get; set; }
    public PilotData PlayerPilot { get; set; }
    public CoreData PlayerCore { get; set; }
    public EnemyData Enemy { get; set; }
    public int EnemyCoreIntegrity { get; set; }
    
    public BattleData(MechData playerMech, EnemyData enemy)
    {
        PlayerMech = playerMech;
        Enemy = enemy;
        EnemyCoreIntegrity = 20; // Base, can vary by enemy
    }
}