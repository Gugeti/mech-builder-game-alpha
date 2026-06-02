using Godot;
using RoboGattai.Player.Progression;

namespace RoboGattai.Map.Nodes;

public partial class RestManager : Node
{
    public enum REST_OPTION
    {
        HEAL,
        UPGRADE_CARD,
        REMOVE_CARD,
        REPAIR_CORE,
        MEDITATE
    }
    
    public void ExecuteRestOption(REST_OPTION option, RunData run)
    {
        // implementation
    }
}