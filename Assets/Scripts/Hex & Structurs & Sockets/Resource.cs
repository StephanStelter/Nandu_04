using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Resource : Structure
{
    public ResourceType resourceType;
    public Rarity productionRarity;
    public Sign sign;
    public int capacity;
    public int amount;
    public int blocked;
    public Hex hexReference;
    public int production;

    public void SetProductionRarity()
    {
        ResourceType type = this.resourceType;
        float rawValue = GetValueForResource(type);

        switch (rawValue)
        {
            case 0:
                productionRarity = Rarity.Common;
                break;
            case 1:
                productionRarity = Rarity.Uncommon;
                break;
            case 2:
                productionRarity = Rarity.Rare;
                break;
            case 3:
                productionRarity = Rarity.Epic;
                break;
            case 4:
                productionRarity = Rarity.Legendary;
                break;
        }
    }
    public ResList produce()
    {
        ResList productVect = new ResList();

        ResourceType type = this.resourceType;
        float rawValue = GetValueForResource(type);
        int producedAmount = GetProducedAmount(rawValue);

        // Setze den entsprechenden Wert im ResourceVector
        switch (type)
        {
            case ResourceType.Food:
                productVect.Grain = producedAmount;
                break;
            case ResourceType.Gold:
                productVect.Gold = producedAmount;
                break;
            case ResourceType.Wood:
                productVect.Wood = producedAmount;
                break;
            case ResourceType.Iron:
                productVect.Iron = producedAmount;
                break;
            case ResourceType.Stone:
                productVect.Stone = producedAmount;
                break;
            case ResourceType.Wool:
                productVect.Wool = producedAmount;
                break;
        }

        // XP und Money bleiben 0
        return productVect;
    }
    float GetValueForResource(ResourceType type)
    {
        return type switch
        {
            ResourceType.Food => hexReference.actValues.Grain,
            ResourceType.Gold => hexReference.actValues.Gold,
            ResourceType.Wool => hexReference.actValues.Wool,
            ResourceType.Wood => hexReference.actValues.Wood,
            ResourceType.Iron => hexReference.actValues.Iron,
            ResourceType.Stone => hexReference.actValues.Stone,
            _ => 0f
        };
    }
    int GetProducedAmount(float value)
    {
        int v = Mathf.RoundToInt(value); // Nur Ganzzahlen 0–4

        float r = UnityEngine.Random.value; // 0.0 - 1.0

        return v switch
        {
            0 => r < 0.8f ? 0 : 1,                   // 80% = 0, 20% = 1
            1 => r < 0.3f ? 0 : 1,                   // 30% = 0, 70% = 1
            2 => 1,                                  // immer 1
            3 => r < 0.2f ? 2 : 1,                   // 20% = 2, 80% = 1
            4 => r < 0.1f ? 3 : (r < 0.6f ? 2 : 1),  // 10% = 3, 50% = 2, 40% = 1
            _ => 0
        };
    }
    public int GetCurrentProducedAmount()
    {
        ResourceType type = this.resourceType;
        float rawValue = GetValueForResource(type);
        int producedAmount = GetProducedAmount(rawValue);

        return producedAmount;
    }
}
