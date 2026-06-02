using System.Collections.Generic;

namespace RoboGattai.Shared;

public static class ValidationUtils
{
    public static bool IsValidHandSize(int currentSize, int cardsToAdd)
    {
        return currentSize + cardsToAdd <= CONSTANTS.MAX_HAND_SIZE;
    }
    
    public static bool IsValidDeckSize(int currentSize)
    {
        return currentSize >= CONSTANTS.MIN_DECK_SIZE && currentSize <= CONSTANTS.MAX_DECK_SIZE;
    }
    
    public static bool IsValidCardCopyCount(Dictionary<string, int> cardCounts, string cardId)
    {
        if (!cardCounts.ContainsKey(cardId)) return true;
        return cardCounts[cardId] < CONSTANTS.MAX_CARD_COPIES;
    }
    
    public static bool IsValidBuildPoints(int currentBp, int cost)
    {
        return currentBp >= cost;
    }
    
    public static bool IsValidHeatLevel(int currentHeat, int threshold)
    {
        return currentHeat <= threshold;
    }
    
    public static bool IsValidWeight(int currentWeight, int maxWeight)
    {
        return currentWeight <= maxWeight;
    }
}