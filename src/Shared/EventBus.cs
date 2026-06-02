using Godot;

namespace RoboGattai.Shared
{
    // EventBus is a global singleton used to communicate between different systems of the game
    // without creating direct dependencies (loosely-coupled architecture).
    // Other scripts can connect to signals to react to game events.
    public partial class EventBus : Node
    {
        // Singleton instance for global access
        public static EventBus Instance { get; private set; }

        // ================= Signals =================
        // Signals are Godot's way to send events; other nodes can connect to them

        [Signal] public delegate void CardDrawnEventHandler(string cardId, int cardType);
        // Emitted when a card is drawn from the deck
        // cardId: unique identifier for the card
        // cardType: type/category of the card

        [Signal] public delegate void CardPlayedEventHandler(string cardId, int slotIndex);
        // Emitted when a card is played
        // slotIndex: position in the player's hand or play area

        [Signal] public delegate void CardDiscardedEventHandler(string cardId);
        // Emitted when a card is discarded from the hand

        [Signal] public delegate void HandShuffledEventHandler();
        // Emitted when the hand/deck is shuffled

        [Signal] public delegate void EnergyChangedEventHandler(int current, int maximum);
        // Emitted when the player's energy changes (for playing cards)

        [Signal] public delegate void BuildPhaseStartedEventHandler(int bpAmount);
        // Emitted when a build phase starts (for mech assembly or upgrades)
        // bpAmount: number of build points available

        [Signal] public delegate void FrameDeployedEventHandler(string frameId);
        // Emitted when a mech frame is deployed

        [Signal] public delegate void ComponentAttachedEventHandler(string componentId, string hardpoint);
        // Emitted when a component is attached to a mech frame
        // hardpoint: location on the mech

        [Signal] public delegate void MechDestroyedEventHandler(string mechId);
        // Emitted when a mech is destroyed

        [Signal] public delegate void HeatThresholdExceededEventHandler(string mechId);
        // Emitted when a mech overheats

        [Signal] public delegate void TurnStartedEventHandler(bool isPlayerTurn);
        // Emitted at the start of a turn
        // isPlayerTurn: true if it's the player's turn

        [Signal] public delegate void DamageDealtEventHandler(string targetId, int damage, int damageType);
        // Emitted when damage is dealt
        // targetId: who received damage
        // damageType: physical, energy, etc.

        [Signal] public delegate void RangeChangedEventHandler(int newRange);
        // Emitted when the range of a mech or attack changes

        [Signal] public delegate void ManeuverSelectedEventHandler(int maneuverType);
        // Emitted when a maneuver is selected (combat action type)

        [Signal] public delegate void RuneAcquiredEventHandler(string runeId);
        // Emitted when the player acquires a rune (permanent bonus/effect)

        [Signal] public delegate void CurseAppliedEventHandler(string curseId);
        // Emitted when a curse is applied to the player or mech

        [Signal] public delegate void CardAddedToDeckEventHandler(string cardId);
        // Emitted when a card is added to the player's deck

        [Signal] public delegate void CardRemovedFromDeckEventHandler(string cardId);
        // Emitted when a card is removed from the deck

        [Signal] public delegate void StateChangedEventHandler(int newState);
        // Emitted whenever the global game state changes
        // Used by GameManager to notify UI, battle systems, etc.

        [Signal] public delegate void RunStartedEventHandler(string runId);
        // Emitted when a new run starts

        [Signal] public delegate void BattleStartedEventHandler(string battleId);
        // Emitted when a new battle starts

        // ============== Godot Lifecycle ==============

        public override void _Ready()
        {
            // Ensure singleton pattern: only one EventBus exists
            if (Instance != null)
            {
                QueueFree(); // Remove duplicate
                return;
            }

            Instance = this;

            // Make sure this node processes signals/events even if scenes are paused
            ProcessMode = ProcessModeEnum.Always;
        }
    }
}