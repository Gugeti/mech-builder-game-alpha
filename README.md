RoboGattaiRoguelike/
├── Autoload/              # Singletons (GameManager, EventBus)
├── Shared/                # Shared methods, utilities, constants
├── Battle/
│   ├── Core/             # Battle initialization, turn manager
│   ├── Actions/          # Attack, defend, maneuver actions
│   └── Effects/          # Damage calculation, status effects
├── MechBuilding/
│   ├── Assembly/         # Frame deployment, component attachment
│   ├── Validation/       # Hardpoint checks, weight/heat limits
│   └── Management/       # Active mech state, reserve frames
├── Cards/
│   ├── Hand/             # Draw, play, discard logic
│   ├── Energy/             # BP/Energy management
│   └── Types/              # Frame, Component, Tactics implementations
├── Player/
│   ├── Pilots/           # Character classes, abilities
│   ├── Progression/      # Runes, curses, deck building
│   └── Economy/          # Credits, shop interactions
├── Enemies/
│   ├── Base/             # Enemy base class, AI
│   ├── Types/            # Normal, Elite, Boss implementations
│   └── Patterns/         # Attack patterns, behaviors
├── Map/
│   ├── Generation/       # Node generation, path creation
│   └── Nodes/            # Combat, rest, shop, treasure, event
└── UI/                   # (GDScript/C# hybrid - UI logic)


Flow Loops:

[Player starts game / Main Menu]
           |
           v
    GameManager.StartNewRun(pilotId)
           |
           v
   [GAME_STATE.MAP] ---------------------+
           |                             |
           v                             |
    Player encounters enemy              |
           |                             |
           v                             |
    GameManager.EnterBattle(enemyId)     |
           |                             |
           v                             |
[GAME_STATE.BATTLE_ASSEMBLY]             |
           |                             |
           v                             |
   BattleManager.InitializeBattle(BattleData)
           |
           v
   BattleManager.StartAssemblyPhase()
           |
           v
   Player configures mech (MechAssembly)
           |
           v
   BattleManager.EndAssemblyPhase()
           |
           v
[GAME_STATE.BATTLE_COMBAT]
           |
           v
   BattleManager.StartCombatPhase()
           |
           v
   BattleManager.DetermineInitiative()
           |
           v
   BattleManager.StartCombatRound()
           |
           +---------------------------+
           |                           |
           v                           v
  Player's Turn?                  Enemy's Turn?
      |                                |
      v                                v
  Player selects maneuver           AI selects maneuver
      |                                |
      +----------+---------------------+
                 v
        BattleManager.ResolveManeuver(playerManeuver, enemyManeuver)
                 |
                 v
          Update Range -> EventBus.EmitSignal(RangeChanged)
                 |
                 v
        BattleManager.ProcessCombatActions(facing)
                 |
       +-----------------------+
       |                       |
       v                       v
  Player attacks?           Enemy attacks?
       |                       |
       v                       v
BattleManager.ResolvePlayerAttack    BattleManager.ResolveEnemyAttack
       |                       |
       +-----------+-----------+
                   v
          EventBus.EmitSignal(DamageDealt)
                   |
                   v
        BattleManager.EndCombatRound()
                   |
           CheckWinCondition()
                   |
        +----------+----------+
        |                     |
        v                     v
 Battle continues        BattleManager.EndBattle()
        |                     |
        v                     v
  Next Combat Round     GameManager.EndBattle(playerWon)
        |                     |
        v                     v
      Loop                [GAME_STATE.MAP_REWARD or GAME_OVER]