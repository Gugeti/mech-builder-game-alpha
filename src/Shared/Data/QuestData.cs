using Godot;

namespace RoboGattai.Player.Progression;

[GlobalClass]
public partial class QuestData : Resource
{
    [Export] public string QuestId { get; set; }
    [Export] public string Title { get; set; }
    [Export] public string Description { get; set; }
    [Export] public QUEST_TYPE Type { get; set; }
    [Export] public int TargetAmount { get; set; }
    [Export] public int CurrentProgress { get; set; }
    
    [Export] public int GoldReward { get; set; }
    
    public void UpdateProgress(int amount)
    {
        CurrentProgress += amount;
    }
}

public enum QUEST_TYPE
{
    KILL_ENEMIES,
    KILL_ELITES,
    KILL_BOSSES,
    COLLECT_CARDS,
    COLLECT_RUNES,
    REACH_ACT,
    NO_DAMAGE_BATTLE,
    OVERHEAT_KILLS
}