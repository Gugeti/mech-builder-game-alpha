using Godot;
using System.Collections.Generic;
using RoboGattai.Shared;

namespace RoboGattai.Map.Generation;

public partial class MapGenerator : Node
{
    [Export] public int Acts { get; set; } = 3;
    [Export] public int MinNodesPerAct { get; set; } = 8;
    [Export] public int MaxNodesPerAct { get; set; } = 12;
    
    private List<MapNode> _currentMap;
    private int _currentAct;
    
    public List<MapNode> GenerateMap(int act)
    {
        _currentAct = act;
        _currentMap = new List<MapNode>();
        
        int nodeCount = MathUtils.RandomRange(MinNodesPerAct, MaxNodesPerAct + 1);
        
        // Generate start node
        var startNode = new MapNode
        {
            NodeId = "START",
            Type = NODE_TYPE.START,
            Position = new Vector2I(0, 0),
            Connections = new List<string>()
        };
        _currentMap.Add(startNode);
        
        // Generate intermediate nodes
        for (int i = 1; i < nodeCount - 1; i++)
        {
            var node = GenerateNode(i, act);
            _currentMap.Add(node);
        }
        
        // Generate boss node
        var bossNode = new MapNode
        {
            NodeId = $"BOSS_ACT{act}",
            Type = NODE_TYPE.BOSS,
            Position = new Vector2I(nodeCount - 1, 0),
            EnemyId = GetBossForAct(act)
        };
        _currentMap.Add(bossNode);
        
        // Create connections (ensure path exists)
        ConnectNodes();
        
        return _currentMap;
    }
    
    private MapNode GenerateNode(int index, int act)
    {
        NODE_TYPE type = RollNodeType(index, act);
        
        return new MapNode
        {
            NodeId = $"NODE_{act}_{index}",
            Type = type,
            Position = new Vector2I(index, MathUtils.RandomRange(-2, 3)),
            EnemyId = type == NODE_TYPE.COMBAT ? GetEnemyForAct(act) : null,
            EliteEnemyId = type == NODE_TYPE.ELITE ? GetEliteForAct(act) : null
        };
    }
    
    private NODE_TYPE RollNodeType(int index, int act)
    {
        // First few nodes are always combat
        if (index <= 2) return NODE_TYPE.COMBAT;
        
        float roll = MathUtils.RandomRangeFloat(0, 1);
        
        // Adjust probabilities based on act progression
        float combatChance = 0.40f;
        float eliteChance = CONSTANTS.ELITE_CHANCE;
        float shopChance = CONSTANTS.SHOP_CHANCE;
        float restChance = CONSTANTS.REST_CHANCE;
        float treasureChance = CONSTANTS.TREASURE_CHANCE;
        float eventChance = CONSTANTS.EVENT_CHANCE;
        
        if (roll < combatChance) return NODE_TYPE.COMBAT;
        if (roll < combatChance + eliteChance) return NODE_TYPE.ELITE;
        if (roll < combatChance + eliteChance + shopChance) return NODE_TYPE.SHOP;
        if (roll < combatChance + eliteChance + shopChance + restChance) return NODE_TYPE.REST;
        if (roll < combatChance + eliteChance + shopChance + restChance + treasureChance) return NODE_TYPE.TREASURE;
        
        return NODE_TYPE.EVENT;
    }
    
    private void ConnectNodes()
    {
        // Ensure every node has a path forward
        // Simplified: linear with some branches
        
        for (int i = 0; i < _currentMap.Count - 1; i++)
        {
            var current = _currentMap[i];
            var next = _currentMap[i + 1];
            
            current.Connections.Add(next.NodeId);
            
            // Add some random cross-connections
            if (MathUtils.PercentRoll(0.3f) && i < _currentMap.Count - 2)
            {
                var skip = _currentMap[i + 2];
                current.Connections.Add(skip.NodeId);
            }
        }
    }
    
    private string GetEnemyForAct(int act)
    {
        // Return random enemy ID appropriate for act
        return $"ENEMY_ACT{act}_{MathUtils.RandomRange(1, 4)}";
    }
    
    private string GetEliteForAct(int act)
    {
        return $"ELITE_ACT{act}_{MathUtils.RandomRange(1, 3)}";
    }
    
    private string GetBossForAct(int act)
    {
        return $"BOSS_ACT{act}";
    }
}

public class MapNode
{
    public string NodeId { get; set; }
    public NODE_TYPE Type { get; set; }
    public Vector2I Position { get; set; }
    public List<string> Connections { get; set; } = new();
    public bool Visited { get; set; } = false;
    public bool Available { get; set; } = false;
    
    // Content
    public string EnemyId { get; set; }
    public string EliteEnemyId { get; set; }
    public string EventId { get; set; }
}

public enum NODE_TYPE
{
    START,
    COMBAT,
    ELITE,
    SHOP,
    REST,
    TREASURE,
    EVENT,
    BOSS
}