using System.Collections.Generic;
using RoboGattai.Shared;
using RoboGattai.Shared.Data;

namespace RoboGattai.Player.Progression;

public class RunData
{
    public PilotData SelectedPilot { get; set; }
    public MechData CurrentMech { get; set; }
    public CoreData PlayerCore { get; set; }
    
    public int CurrentAct { get; set; } = 1;
    public int CurrentNodeIndex { get; set; } = 0;
    public int PlayerGold { get; set; } = 100;
    
    public List<CardData> MasterDeck { get; set; } = new();
    public List<RuneData> ActiveRunes { get; set; } = new();
    public List<CurseData> ActiveCurses { get; set; } = new();
    
    public int BattlesWon { get; set; } = 0;
    public int ElitesDefeated { get; set; } = 0;
    public int BossesDefeated { get; set; } = 0;
    
    public List<QuestData> ActiveQuests { get; set; } = new();
    public List<QuestData> CompletedQuests { get; set; } = new();
    
    public RunData(PilotData pilot)
    {
        SelectedPilot = pilot;
        MasterDeck = new List<CardData>(pilot.StartingDeck);
        PlayerCore = new CoreData { MaxIntegrity = CONSTANTS.BASE_CORE_INTEGRITY, CurrentIntegrity = CONSTANTS.BASE_CORE_INTEGRITY, GenerationRating = 0 };
    }
    
    public void RecordVictory(EnemyData enemy)
    {
        BattlesWon++;
        PlayerGold += enemy.GoldReward;
        
        if (enemy.EnemyType == ENEMY_TYPE.ELITE) ElitesDefeated++;
        if (enemy.EnemyType == ENEMY_TYPE.BOSS) BossesDefeated++;
    }
    
    public void AddCardToDeck(CardData card)
    {
        MasterDeck.Add(card);
    }
    
    public void RemoveCardFromDeck(CardData card)
    {
        MasterDeck.Remove(card);
    }
    
    public void AcquireRune(RuneData rune)
    {
        ActiveRunes.Add(rune);
    }
    
    public void ApplyCurse(CurseData curse)
    {
        ActiveCurses.Add(curse);
    }
    
    public int GetMaxHpBonus()
    {
        int bonus = 0;
        foreach (var rune in ActiveRunes) bonus += rune.MaxHpBonus;
        foreach (var curse in ActiveCurses) bonus -= curse.MaxHpPenalty;
        return bonus;
    }
    
    public int GetEnergyBonus()
    {
        int bonus = 0;
        foreach (var rune in ActiveRunes) bonus += rune.StartEnergyBonus;
        foreach (var curse in ActiveCurses) bonus -= curse.EnergyPenalty;
        return bonus;
    }
}