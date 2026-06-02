namespace RoboGattai.Shared;

public static class CONSTANTS
{
    // Deck/Hand
    public const int MAX_HAND_SIZE = 10;
    public const int STARTING_HAND_SIZE = 5;
    public const int MIN_DECK_SIZE = 20;
    public const int MAX_DECK_SIZE = 50;
    public const int MAX_CARD_COPIES = 3;
    
    // Mech/Assembly
    public const int MAX_ACTIVE_FRAMES = 1;
    public const int MAX_RESERVE_FRAMES = 3;
    public const int BASE_BUILD_POINTS = 3;
    public const int MAX_EMERGENCY_SHIELDS = 3;
    public const int OVERHEAT_PENALTY_SKIPS = 1;
    
    // Combat
    public const int STARTING_RANGE = 2; // Mid range
    public const int MIN_RANGE = 0; // Close
    public const int MAX_RANGE = 6; // Far
    public const int BASE_CORE_INTEGRITY = 20;
    
    // Map
    public const int MAP_ACTS = 3;
    public const int NODES_PER_ACT_MIN = 8;
    public const int NODES_PER_ACT_MAX = 12;
    public const float ELITE_CHANCE = 0.25f;
    public const float SHOP_CHANCE = 0.20f;
    public const float REST_CHANCE = 0.15f;
    public const float TREASURE_CHANCE = 0.10f;
    public const float EVENT_CHANCE = 0.10f;
    
    // Rewards
    public const int BASE_CARD_REWARD_OPTIONS = 3;
    public const int BASE_RUNE_REWARD_OPTIONS = 2;
    public const int GOLD_PER_NORMAL_ENCOUNTER = 15;
    public const int GOLD_PER_ELITE_ENCOUNTER = 40;
    public const int GOLD_PER_BOSS_ENCOUNTER = 100;
}