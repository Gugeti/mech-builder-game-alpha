using Godot;

namespace RoboGattai.Autoload;

// GameManager is a singleton (autoload) responsible for controlling the overall game flow
// and managing transitions between different game states (menu, map, battle, etc.)
public partial class GameManager : Node
{
    // Singleton instance of the GameManager for global access
    public static GameManager Instance { get; private set; }
    
    // Current state of the game (menu, map, battle, etc.)
    public GAME_STATE CurrentState { get; private set; } = GAME_STATE.MENU;
    
    // Unique identifier for the current run (used for save/track purposes)
    public string CurrentRunId { get; private set; }
    
    // Unique identifier for the current battle (used for tracking battles)
    public string CurrentBattleId { get; private set; }
    
    // Called when the node enters the scene tree
    public override void _Ready()
    {
        // Ensure there is only one instance of GameManager
        if (Instance != null)
        {
            QueueFree(); // Remove duplicate
            return;
        }
        
        Instance = this;
        
        // Ensure _Process always runs regardless of scene pause state
        ProcessMode = ProcessModeEnum.Always;
    }
    
    // Starts a new run for the player with a specific pilot
    public void StartNewRun(string pilotId)
    {
        // Generate a unique run ID
        CurrentRunId = System.Guid.NewGuid().ToString();
        
        // Transition the game to the MAP state
        CurrentState = GAME_STATE.MAP;
        
        // Emit signals to notify other systems that a run has started and state changed
        EventBus.Instance.EmitSignal(EventBus.SignalName.RunStarted, CurrentRunId);
        EventBus.Instance.EmitSignal(EventBus.SignalName.StateChanged, (int)CurrentState);
    }
    
    // Initiates a battle against a specified enemy
    public void EnterBattle(string enemyId)
    {
        // Generate a unique battle ID
        CurrentBattleId = System.Guid.NewGuid().ToString();
        
        // Set the game state to BATTLE_ASSEMBLY, where the player might configure their units/cards
        CurrentState = GAME_STATE.BATTLE_ASSEMBLY;
        
        // Emit signals to notify other systems that a battle has started and state changed
        EventBus.Instance.EmitSignal(EventBus.SignalName.BattleStarted, CurrentBattleId);
        EventBus.Instance.EmitSignal(EventBus.SignalName.StateChanged, (int)CurrentState);
    }
    
    // Ends the current battle and transitions to the appropriate state
    public void EndBattle(bool playerWon)
    {
        // If player won, go to MAP_REWARD; if lost, go to GAME_OVER
        CurrentState = playerWon ? GAME_STATE.MAP_REWARD : GAME_STATE.GAME_OVER;
        
        // Clear the current battle ID
        CurrentBattleId = null;
        
        // Notify other systems that the game state has changed
        EventBus.Instance.EmitSignal(EventBus.SignalName.StateChanged, (int)CurrentState);
    }
    
    // Allows transitioning to any arbitrary game state
    public void TransitionToState(GAME_STATE newState)
    {
        CurrentState = newState;
        
        // Emit a state change signal for other systems to respond
        EventBus.Instance.EmitSignal(EventBus.SignalName.StateChanged, (int)newState);
    }
}

// Enum representing all possible game states
// Used to control the game flow in a readable way
public enum GAME_STATE
{
    MENU,               // Main menu
    MAP,                // Map/exploration phase
    MAP_REWARD,         // Reward phase after winning a battle
    BATTLE_ASSEMBLY,    // Battle preparation phase
    BATTLE_COMBAT,      // Actual combat phase
    SHOP,               // Shop interaction
    REST,               // Rest area (heal/upgrade)
    TREASURE,           // Treasure/event encounter
    EVENT,              // Random event
    GAME_OVER,          // Player has lost
    VICTORY             // Player has completed the run successfully
}