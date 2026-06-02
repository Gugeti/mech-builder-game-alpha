using Godot;
using System;
using RoboGattai.Shared;
using RoboGattai.Shared.Data;
using RoboGattai.MechBuilding.Assembly;
using RoboGattai.Autoload;
using RoboGattai.Battle.AI;

namespace RoboGattai.Battle.Core;

// BattleManager handles all battle logic, from assembly phases to combat rounds and attacks
public partial class BattleManager : Node
{
    // Exposed paths to connect in the editor
    [Export] public NodePath PlayerAssemblyPath { get; set; }
    [Export] public NodePath EnemyControllerPath { get; set; }

    // References to key nodes in the scene
    private MechAssembly _playerAssembly;
    private EnemyController _enemyController;

    // Singleton instance for global access (optional if needed by UI/other systems)
    public static BattleManager Instance { get; private set; }   

    // Current battle data, phase, round, and range
    public BattleData CurrentBattle { get; private set; }
    public TURN_PHASE CurrentPhase { get; private set; }
    public int CurrentRound { get; private set; }
    public int CurrentRange { get; private set; }

    // Internal flags
    private bool _isPlayerTurn;
    private bool _battleActive;

    // ================= Godot Lifecycle =================
    public override void _Ready()
    {
        // Retrieve node references if paths are set
        if (!PlayerAssemblyPath.IsEmpty)
            _playerAssembly = GetNode<MechAssembly>(PlayerAssemblyPath);
        if (!EnemyControllerPath.IsEmpty)
            _enemyController = GetNode<EnemyController>(EnemyControllerPath);
    }

    // ================= Battle Initialization =================
    public void InitializeBattle(BattleData battleData)
    {
        // Set up the battle
        CurrentBattle = battleData;
        CurrentRound = 1;
        CurrentRange = CONSTANTS.STARTING_RANGE;
        _battleActive = true;

        _enemyController?.Initialize(battleData.Enemy);

        // Begin the assembly phase where player configures mechs
        StartAssemblyPhase();
    }

    public void StartAssemblyPhase()
    {
        CurrentPhase = TURN_PHASE.ASSEMBLY;

        // Update GameManager state for UI and flow
        GameManager.Instance.TransitionToState(GAME_STATE.BATTLE_ASSEMBLY);

        // Calculate build points and start player assembly
        int availableBp = CalculateBuildPoints();
        _playerAssembly?.StartAssemblyPhase(CurrentBattle.PlayerPilot, availableBp);
    }

    public void EndAssemblyPhase()
    {
        // Finalize player setup and move to combat
        _playerAssembly?.FinalizeAssembly();
        StartCombatPhase();
    }

    // ================= Combat Phase =================
    public void StartCombatPhase()
    {
        CurrentPhase = TURN_PHASE.COMBAT_MANEUVER;
        GameManager.Instance.TransitionToState(GAME_STATE.BATTLE_COMBAT);

        DetermineInitiative(); // Decide who goes first
        StartCombatRound();
    }

    public void StartCombatRound()
    {
        CurrentPhase = TURN_PHASE.COMBAT_MANEUVER;

        // Notify listeners that a new turn started
        EventBus.Instance.EmitSignal(EventBus.SignalName.TurnStarted, _isPlayerTurn);
    }

    public void ResolveManeuver(MANEUVER_TYPE playerManeuver, MANEUVER_TYPE enemyManeuver)
    {
        // Update distance between mechs based on maneuvers
        int newRange = CalculateNewRange(CurrentRange, playerManeuver, enemyManeuver);
        CurrentRange = MathUtils.Clamp(newRange, CONSTANTS.MIN_RANGE, CONSTANTS.MAX_RANGE);
        EventBus.Instance.EmitSignal(EventBus.SignalName.RangeChanged, CurrentRange);

        CurrentPhase = TURN_PHASE.COMBAT_ACTION;

        string facing = DetermineFacing(playerManeuver, enemyManeuver);
        ProcessCombatActions(facing);
    }

    public void ProcessCombatActions(string facing)
    {
        if (_isPlayerTurn)
        {
            // Wait for player input to perform action
        }
        else
        {
            // AI takes its turn
            _enemyController?.TakeTurn(CurrentRange, facing);
        }
    }

    public void ResolvePlayerAttack(ComponentData weapon, string facing)
    {
        var enemyMech = _enemyController?.GetCurrentMech();
        if (enemyMech == null) return;

        if (!IsWeaponInRange(weapon, CurrentRange)) return;

        int accuracy = CalculatePlayerAccuracy(weapon);
        int evasion = enemyMech.EvasionValue;

        bool hit = MathUtils.IsHit(accuracy, evasion);

        if (hit)
        {
            int damage = CalculateDamage(weapon, facing, enemyMech);
            enemyMech.TakeDamage(damage, facing);

            EventBus.Instance.EmitSignal(EventBus.SignalName.DamageDealt, "enemy", damage, (int)DAMAGE_TYPE.KINETIC);

            // Apply heat generation and component checks
            _playerAssembly?.ActiveMech?.AddHeat(weapon.HeatGeneration);
            CheckComponentDamage(enemyMech);
        }
    }

    public void ResolveEnemyAttack(EnemyIntent intent)
    {
        var playerMech = _playerAssembly?.ActiveMech;
        if (playerMech == null) return;

        int accuracy = intent.Damage + (_enemyController?.GetAccuracyBonus() ?? 0);
        int evasion = playerMech.EvasionValue;

        bool hit = MathUtils.IsHit(accuracy, evasion);

        if (hit)
        {
            string facing = "FRONT";
            int damage = intent.Damage - playerMech.TotalArmorFront;
            playerMech.TakeDamage(damage, facing);

            EventBus.Instance.EmitSignal(EventBus.SignalName.DamageDealt, "player", damage, (int)DAMAGE_TYPE.KINETIC);
            playerMech.AddHeat(intent.HeatGain);
        }
    }

    public void EndCombatRound()
    {
        // Dissipate heat at the end of round
        _playerAssembly?.ActiveMech?.DissipateHeat(0);
        _enemyController?.DissipateHeat();

        CurrentRound++;

        // Check for win/loss conditions
        if (CheckWinCondition())
        {
            EndBattle();
            return;
        }

        // Start next round
        StartCombatRound();
    }

    public void EndBattle()
    {
        _battleActive = false;

        // Determine winner and notify GameManager
        bool playerWon = _playerAssembly?.ActiveMech != null && !_playerAssembly.ActiveMech.IsDisabled;
        GameManager.Instance.EndBattle(playerWon);
    }

    // ================= Helper Methods =================
    private int CalculateBuildPoints()
    {
        int bp = CONSTANTS.BASE_BUILD_POINTS;
        bp += CurrentBattle.PlayerCore?.GenerationRating ?? 0;
        bp += CurrentBattle.PlayerPilot?.BaseTactics ?? 0;
        return bp;
    }

    private void DetermineInitiative()
    {
        int playerSpeed = _playerAssembly?.ActiveMech?.CurrentSpeed ?? 0;
        int enemySpeed = _enemyController?.GetCurrentMech()?.CurrentSpeed ?? 0;

        if (playerSpeed > enemySpeed)
            _isPlayerTurn = true;
        else if (enemySpeed > playerSpeed)
            _isPlayerTurn = false;
        else
            _isPlayerTurn = MathUtils.PercentRoll(0.5f); // Random tie-breaker
    }

    private int CalculateNewRange(int current, MANEUVER_TYPE player, MANEUVER_TYPE enemy)
    {
        // Simple distance logic based on maneuvers
        if (player == MANEUVER_TYPE.ADVANCE && enemy == MANEUVER_TYPE.ADVANCE) return current - 2;
        if (player == MANEUVER_TYPE.RETREAT && enemy == MANEUVER_TYPE.RETREAT) return current + 2;
        if ((player == MANEUVER_TYPE.ADVANCE && enemy == MANEUVER_TYPE.HOLD) ||
            (player == MANEUVER_TYPE.HOLD && enemy == MANEUVER_TYPE.ADVANCE)) return current - 1;
        if ((player == MANEUVER_TYPE.RETREAT && enemy == MANEUVER_TYPE.HOLD) ||
            (player == MANEUVER_TYPE.HOLD && enemy == MANEUVER_TYPE.RETREAT)) return current + 1;

        return current;
    }

    private string DetermineFacing(MANEUVER_TYPE player, MANEUVER_TYPE enemy)
    {
        if (player == MANEUVER_TYPE.ADVANCE && CurrentRange <= (int)RANGE_BAND.CLOSE) return "FRONT";
        if (enemy == MANEUVER_TYPE.RETREAT) return "REAR";
        return "FRONT";
    }

    private int CalculatePlayerAccuracy(ComponentData weapon)
    {
        var mech = _playerAssembly?.ActiveMech;
        if (mech == null) return 0;

        int gunnery = mech.Pilot?.BaseGunnery ?? 0;
        int systems = mech.AccuracyBonus;
        int rangePenalty = GetRangePenalty(weapon, CurrentRange);

        return MathUtils.CalculateAccuracyRoll(gunnery, systems, rangePenalty);
    }

    private int GetRangePenalty(ComponentData weapon, int range)
    {
        int optimal = (int)weapon.Range;
        int diff = Mathf.Abs(range - optimal);
        return diff * 2;
    }

    private bool IsWeaponInRange(ComponentData weapon, int range)
    {
        return range >= (int)weapon.Range && range <= (int)weapon.Range + 2;
    }

    private int CalculateDamage(ComponentData weapon, string facing, MechData target)
    {
        int baseDamage = weapon.Damage;
        int armor = facing switch
        {
            "FRONT" => target.TotalArmorFront,
            "SIDE" => target.TotalArmorSide,
            "REAR" => target.TotalArmorRear,
            _ => target.TotalArmorFront
        };

        return Mathf.Max(1, baseDamage - armor);
    }

    private void CheckComponentDamage(MechData target)
    {
        if (target.CurrentHp <= target.MaxHp / 2)
        {
            // Placeholder: logic for component damage when mech is below half HP
        }
    }

    private bool CheckWinCondition()
    {
        bool playerAlive = _playerAssembly?.ActiveMech != null && !_playerAssembly.ActiveMech.IsDisabled;
        bool enemyAlive = _enemyController?.IsAlive() ?? false;

        if (!playerAlive || !enemyAlive) return true;
        if (CurrentBattle.PlayerCore?.CurrentIntegrity <= 0) return true;
        if (CurrentBattle.EnemyCoreIntegrity <= 0) return true;

        return false;
    }
}

// ================= Enums =================
public enum TURN_PHASE
{
    ASSEMBLY,          // Player configures mech
    COMBAT_MANEUVER,    // Maneuver phase: advance/hold/retreat
    COMBAT_ACTION,      // Resolve attacks
    COMBAT_RESOLVE      // Apply damage, effects, check win conditions
}

public enum MANEUVER_TYPE
{
    ADVANCE,
    HOLD,
    RETREAT
}

public enum DAMAGE_TYPE
{
    KINETIC,
    ENERGY,
    EXPLOSIVE,
    HEAT
}