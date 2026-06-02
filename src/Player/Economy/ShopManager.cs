using Godot;
using Godot.Collections;
using System.Collections.Generic;
using RoboGattai.Player.Progression;
using RoboGattai.Shared;
using RoboGattai.Shared.Data;

namespace RoboGattai.Player.Economy;

public class ShopItem
{
    public string ItemId { get; set; }
    public int Price { get; set; }
    public SHOP_ITEM_TYPE Type { get; set; }
    public bool Sold { get; set; }
}

public enum SHOP_ITEM_TYPE
{
    CARD,
    RUNE,
    REPAIR,
    REMOVE_CARD
}

public partial class ShopManager : Node
{
    [Export] public Array<CardData> CardPool { get; set; } = new();
    [Export] public Array<RuneData> RunePool { get; set; } = new();
    
    private List<ShopItem> _currentStock = new();
    private float _priceMultiplier = 1.0f;
    
    public void GenerateShopStock(int act, Array<RuneData> playerRunes)
    {
        _currentStock.Clear();
        
        float discount = 0;
        if (playerRunes != null)
        {
            foreach (var rune in playerRunes)
            {
                discount += rune.ShopDiscount;
            }
        }
        _priceMultiplier = 1.0f - discount;
        
        int cardCount = MathUtils.RandomRange(3, 6);
        for (int i = 0; i < cardCount; i++)
        {
            if (CardPool.Count == 0) break;
            var card = CardPool[MathUtils.RandomRange(0, CardPool.Count)];
            _currentStock.Add(new ShopItem
            {
                ItemId = card.CardId,
                Price = CalculateCardPrice(card, act),
                Type = SHOP_ITEM_TYPE.CARD
            });
        }
        
        int runeCount = MathUtils.RandomRange(2, 4);
        for (int i = 0; i < runeCount; i++)
        {
            if (RunePool.Count == 0) break;
            var rune = RunePool[MathUtils.RandomRange(0, RunePool.Count)];
            _currentStock.Add(new ShopItem
            {
                ItemId = rune.RuneId,
                Price = CalculateRunePrice(rune, act),
                Type = SHOP_ITEM_TYPE.RUNE
            });
        }
        
        _currentStock.Add(new ShopItem
        {
            ItemId = "REPAIR",
            Price = 20 * act,
            Type = SHOP_ITEM_TYPE.REPAIR
        });
        
        _currentStock.Add(new ShopItem
        {
            ItemId = "REMOVE_CARD",
            Price = 75 + (25 * act),
            Type = SHOP_ITEM_TYPE.REMOVE_CARD
        });
    }
    
    public bool PurchaseItem(ShopItem item, RunData playerRun)
    {
        if (playerRun.PlayerGold < item.Price) return false;
        
        playerRun.PlayerGold -= item.Price;
        
        switch (item.Type)
        {
            case SHOP_ITEM_TYPE.CARD:
                // Find card in pool and add
                foreach (var card in CardPool)
                {
                    if (card.CardId == item.ItemId)
                    {
                        playerRun.AddCardToDeck(card);
                        break;
                    }
                }
                break;
            case SHOP_ITEM_TYPE.RUNE:
                foreach (var rune in RunePool)
                {
                    if (rune.RuneId == item.ItemId)
                    {
                        playerRun.AcquireRune(rune);
                        break;
                    }
                }
                break;
            case SHOP_ITEM_TYPE.REPAIR:
                playerRun.PlayerCore.Repair(10);
                break;
            case SHOP_ITEM_TYPE.REMOVE_CARD:
                // Open card removal UI
                break;
        }
        
        _currentStock.Remove(item);
        return true;
    }
    
    private int CalculateCardPrice(CardData card, int act)
    {
        int basePrice = card.Tier * 20;
        return (int)(basePrice * _priceMultiplier * (1 + (act * 0.2f)));
    }
    
    private int CalculateRunePrice(RuneData rune, int act)
    {
        int basePrice = rune.Tier * 50;
        return (int)(basePrice * _priceMultiplier * (1 + (act * 0.2f)));
    }
}