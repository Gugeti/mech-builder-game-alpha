using Godot;
using Godot.Collections;
using RoboGattai.Shared;
using RoboGattai.Shared.Data;

namespace RoboGattai.Cards.Hand;

public partial class CardDeck : Node
{
    private Array<CardData> _drawPile = new();
    private Array<CardData> _discardPile = new();
    private Array<CardData> _exhaustPile = new();
    
    public int DrawPileCount => _drawPile.Count;
    public int DiscardPileCount => _discardPile.Count;
    
    public void Initialize(Array<CardData> startingDeck)
    {
        _drawPile.Clear();
        _discardPile.Clear();
        _exhaustPile.Clear();
        
        foreach (var card in startingDeck)
        {
            _drawPile.Add(card);
        }
        
        Shuffle();
    }
    
    public void Shuffle()
    {
        for (int i = _drawPile.Count - 1; i > 0; i--)
        {
            int j = MathUtils.RandomRange(0, i + 1);
            (_drawPile[i], _drawPile[j]) = (_drawPile[j], _drawPile[i]);
        }
        EventBus.Instance.EmitSignal(EventBus.SignalName.HandShuffled);
    }
    
    public CardData DrawCard()
    {
        if (_drawPile.Count == 0)
        {
            if (_discardPile.Count == 0) return null;
            ReshuffleDiscard();
        }
        
        int index = _drawPile.Count - 1;
        CardData card = _drawPile[index];
        _drawPile.RemoveAt(index);
        
        card.OnDraw(null);
        EventBus.Instance.EmitSignal(EventBus.SignalName.CardDrawn, card.CardId, (int)card.CardType);
        
        return card;
    }
    
    public Array<CardData> DrawCards(int count)
    {
        Array<CardData> drawn = new();
        for (int i = 0; i < count; i++)
        {
            var card = DrawCard();
            if (card == null) break;
            drawn.Add(card);
        }
        return drawn;
    }
    
    public void Discard(CardData card)
    {
        _discardPile.Add(card);
        card.OnDiscard(null);
        EventBus.Instance.EmitSignal(EventBus.SignalName.CardDiscarded, card.CardId);
    }
    
    public void DiscardMultiple(Array<CardData> cards)
    {
        foreach (var card in cards)
        {
            Discard(card);
        }
    }
    
    public void Exhaust(CardData card)
    {
        _exhaustPile.Add(card);
    }
    
    public void AddToDiscard(CardData card)
    {
        _discardPile.Add(card);
    }
    
    public void AddToDrawPile(CardData card, bool shuffleIn = false)
    {
        if (shuffleIn)
        {
            int index = MathUtils.RandomRange(0, _drawPile.Count + 1);
            _drawPile.Insert(index, card);
        }
        else
        {
            _drawPile.Insert(0, card);
        }
    }
    
    public Array<CardData> GetDiscardPile()
    {
        return new Array<CardData>(_discardPile);
    }
    
    public void ReshuffleDiscard()
    {
        foreach (var card in _discardPile)
        {
            _drawPile.Add(card);
        }
        _discardPile.Clear();
        Shuffle();
    }
    
    public void ScrapeFromDiscard(ComponentData component)
    {
        if (_discardPile.Remove(component))
        {
            _drawPile.Add(component);
        }
    }
}