using Godot;
using Godot.Collections;
using RoboGattai.Shared;
using RoboGattai.Shared.Data;

namespace RoboGattai.Cards.Hand;

public partial class CardHand : Node
{
    [Export] public NodePath DeckPath { get; set; }
    
    private CardDeck _deck;
    private Array<CardData> _currentHand = new();
    
    public Array<CardData> CurrentHand => _currentHand;
    public int HandSize => _currentHand.Count;
    
    public override void _Ready()
    {
        _deck = GetNode<CardDeck>(DeckPath);
    }
    
    public void DrawStartingHand()
    {
        DrawCards(CONSTANTS.STARTING_HAND_SIZE);
    }
    
    public void DrawCards(int count)
    {
        int spaceAvailable = CONSTANTS.MAX_HAND_SIZE - _currentHand.Count;
        int actualDraw = Mathf.Min(count, spaceAvailable);
        
        var drawn = _deck.DrawCards(actualDraw);
        foreach (var card in drawn)
        {
            _currentHand.Add(card);
        }
    }
    
    public bool PlayCard(CardData card, int targetSlot = -1)
    {
        if (!_currentHand.Contains(card)) return false;
        if (!card.CanPlay(null)) return false;
        
        _currentHand.Remove(card);
        card.OnPlay(null);
        
        EventBus.Instance.EmitSignal(EventBus.SignalName.CardPlayed, card.CardId, targetSlot);
        
        if (card.CardType != CARD_TYPE.COMPONENT_WEAPON && 
            card.CardType != CARD_TYPE.COMPONENT_ARMOR &&
            card.CardType != CARD_TYPE.COMPONENT_SYSTEM &&
            card.CardType != CARD_TYPE.COMPONENT_POWER)
        {
            _deck.Discard(card);
        }
        
        return true;
    }
    
    public void DiscardCard(CardData card)
    {
        if (!_currentHand.Remove(card)) return;
        _deck.Discard(card);
    }
    
    public void DiscardHand()
    {
        foreach (var card in _currentHand)
        {
            _deck.Discard(card);
        }
        _currentHand.Clear();
    }
    
    public void ExhaustCard(CardData card)
    {
        if (!_currentHand.Remove(card)) return;
        _deck.Exhaust(card);
    }
    
    public bool HasCard(string cardId)
    {
        foreach (var card in _currentHand)
        {
            if (card.CardId == cardId) return true;
        }
        return false;
    }
    
    public Array<CardData> GetCardsByType(CARD_TYPE type)
    {
        Array<CardData> result = new();
        foreach (var card in _currentHand)
        {
            if (card.CardType == type) result.Add(card);
        }
        return result;
    }
    
    public void RemoveCardFromHand(CardData card)
    {
        _currentHand.Remove(card);
    }
}