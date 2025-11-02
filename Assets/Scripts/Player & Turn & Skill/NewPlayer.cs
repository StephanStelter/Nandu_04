using System.Collections.Generic;
using UnityEngine;
using static CardProperties;
using static GameHexFieldManager;

[System.Serializable]
public class ResourceSummary
{
    public ResourceType ResourceName;
    public int Amount;
}

[System.Serializable]
public class ResourceSummaryTotalCounter
{
    public ResourceType ResourceName;
    public int Amount;
}

[System.Serializable]
public class CardCounter
{
    public GameObject gameObject;
    public int value;

    public CardCounter(GameObject obj, int initialValue)
    {
        gameObject = obj;
        value = initialValue;
    }
}

public class NewPlayer : MonoBehaviour
{
    public bool IsActivePlayerVisible;  // sichtbar im Inspector
    public bool isMainPlayer;
    public bool isPlayer;
    public bool isNPC;
    public int amountOfCardsInHand;
    public int playerLevel = 1;
    public int skillPoints = 120;

    ResList totalResources = new ResList(0, 0, 0, 0, 0, 0, 0, 0); // Initialisierung mit 0
    ResList resourcesPerTurn = new ResList(0, 0, 0, 0, 0, 0, 0, 0); // Initialisierung mit 0

    public List<ResourceSummary> SummaryResourcesList = new();
    public List<GameObject> PlayerHexFieldSummaryList = new();
    public List<CardCounter> PlayerCardCounterList = new();
    public List<ResourceSummaryTotalCounter> ResourceSummaryTotalCounterList = new();
    [Header("HandKarten")]
    public List<Vector3> cardPositions = new List<Vector3>();
    public List<CardProperties> cardsInHand = new List<CardProperties>();
    public List<GameObject> instantiatedGameobjects = new List<GameObject>();

    private void Start()
    {
        SetSummaryResourcesList();
        SetResourceSummaryTotalCounterList();
    }

    private void Update()
    {
        // Hier ständig aktualisieren, damit der Inspector up-to-date ist
        IsActivePlayerVisible = IsActivePlayer;
    }


    public bool IsActivePlayer
    {
        get
        {
            if (TurnManager.Instance == null) return false;

            NewPlayer current = TurnManager.Instance.GetCurrentPlayer();
            return current != null && current == this;
        }
    }


    public void BeginTurn()
    {
        Debug.Log($"{name} beginnt seinen Zug.");
        // z. B. Resourcen generieren, UI aktivieren
    }

    public void EndTurn()
    {
        Debug.Log($"{name} beendet seinen Zug.");
        TurnManager.Instance.NextTurn();
        TurnManager.Instance.GetCurrentPlayer().BeginTurn();
    }

    private void SetSummaryResourcesList()
    {
        SummaryResourcesList.Clear();

        SummaryResourcesList.Add(new ResourceSummary { ResourceName = ResourceType.Wood, Amount = 0 });
        SummaryResourcesList.Add(new ResourceSummary { ResourceName = ResourceType.Stone, Amount = 0 });
        SummaryResourcesList.Add(new ResourceSummary { ResourceName = ResourceType.Food, Amount = 0 });
        SummaryResourcesList.Add(new ResourceSummary { ResourceName = ResourceType.Iron, Amount = 0 });
        SummaryResourcesList.Add(new ResourceSummary { ResourceName = ResourceType.Wool, Amount = 0 });
        SummaryResourcesList.Add(new ResourceSummary { ResourceName = ResourceType.Gold, Amount = 0 });
        SummaryResourcesList.Add(new ResourceSummary { ResourceName = ResourceType.XP, Amount = 0 });
    }

    private void SetResourceSummaryTotalCounterList()
    {
        ResourceSummaryTotalCounterList.Clear();

        ResourceSummaryTotalCounterList.Add(new ResourceSummaryTotalCounter { ResourceName = ResourceType.Wood, Amount = 0 });
        ResourceSummaryTotalCounterList.Add(new ResourceSummaryTotalCounter { ResourceName = ResourceType.Stone, Amount = 0 });
        ResourceSummaryTotalCounterList.Add(new ResourceSummaryTotalCounter { ResourceName = ResourceType.Food, Amount = 0 });
        ResourceSummaryTotalCounterList.Add(new ResourceSummaryTotalCounter { ResourceName = ResourceType.Wool, Amount = 0 });
        ResourceSummaryTotalCounterList.Add(new ResourceSummaryTotalCounter { ResourceName = ResourceType.Gold, Amount = 0 });
        ResourceSummaryTotalCounterList.Add(new ResourceSummaryTotalCounter { ResourceName = ResourceType.Iron, Amount = 0 });
        ResourceSummaryTotalCounterList.Add(new ResourceSummaryTotalCounter { ResourceName = ResourceType.XP, Amount = 0 });
    }

    public void AddResourceAmount(ResourceType resourceName, int amount)
    {
        ResourceSummary resource = SummaryResourcesList.Find(r => r.ResourceName == resourceName);
        if (resource != null)
            resource.Amount += amount;
        else
            SummaryResourcesList.Add(new ResourceSummary() { ResourceName = resourceName, Amount = amount });
    }

    public void ReduceResourceAmount(ResourceType resourceName, int amount)
    {
        ResourceSummary resource = SummaryResourcesList.Find(r => r.ResourceName == resourceName);
        if (resource != null && resource.Amount <= amount)
            resource.Amount -= amount;
        else
            Debug.Log("Nicht genügend Resourcen vorhanden!");
    }

    public bool CheckPlayerHexFieldSummaryList(GameObject hexfieldGameobject)
    {
        if (PlayerHexFieldSummaryList.Contains(hexfieldGameobject))
            return true; // Hexfeld geh�rt Player One
        else return false; // Hexfeld geh�rt NICHT Player One
    }

    public GameObject GetCardFromList(GameObject currentCardGameobject)
    {
        CardProperties cardPropertiesCurrentCard = currentCardGameobject.GetComponent<CardProperties>();
        if (cardPropertiesCurrentCard == null) { Debug.Log("cardPropertiesCurrentCard == null"); }

        foreach (GameObject cardObjectInList in PlayerHexFieldSummaryList)
        {
            CardProperties[] cardPropsList = cardObjectInList.GetComponentsInChildren<CardProperties>();

            foreach (CardProperties cardProp in cardPropsList)
            {
                if (cardPropertiesCurrentCard.GetId() == cardProp.GetId())
                {
                    //Debug.Log("cardProp gefunden");
                    return cardProp.gameObject;
                }
                //else { Debug.Log("cardProp NICHT gefunden"); }
            }
        }
        return currentCardGameobject;
    }

    public void AddCardToPlayedList(CardProperties cardProps)
    {
        bool found = false;

        foreach (var cardObjectInList in PlayerCardCounterList)
        {
            CardProperties cardPropsList = cardObjectInList.gameObject.GetComponent<CardProperties>();
            if (cardPropsList != null)
            {
                if (cardPropsList.rarity == cardProps.rarity &&
                    cardPropsList.Kartentyp == cardProps.Kartentyp &&
                    cardPropsList.Gebaeudetyp == cardProps.Gebaeudetyp)
                {
                    // Karte existiert schon � Wert erh�hen
                    IncreaseValue(cardPropsList.gameObject);
                    found = true;
                    break; // nicht weiter pr�fen
                }
            }
        }

        if (!found)
        {
            // Karte existiert noch nicht � neu hinzuf�gen
            PlayerCardCounterList.Add(new CardCounter(cardProps.gameObject, 1));
            //Debug.Log($"Added new card {cardProps.gameObject.name} with initial value 1");
        }
    }

    private void IncreaseValue(GameObject obj, int increaseValue = 1)
    {
        foreach (var entry in PlayerCardCounterList)
        {
            if (entry.gameObject == obj)
            {
                entry.value++;
                //Debug.Log($"Updated {obj.name} to value {entry.value}");
                return;
            }
        }
    }

    public int GetNewMultiplierForCardCosts(GameObject cardObjectsCostsToCheck)
    {
        CardProperties cardPropertiesCostsToCheck = cardObjectsCostsToCheck.GetComponent<CardProperties>();

        List<ResourceCosts> tempList = new List<ResourceCosts>();

        int tempAmount = 0;
        if (PlayerCardCounterList.Count > 0)
        {

            foreach (var cardObjectInList in PlayerCardCounterList)
            {
                CardProperties cardPropertiesInList = cardObjectInList.gameObject.GetComponent<CardProperties>();
                if (cardPropertiesInList != null)
                {
                    //gleiche Karten finden
                    if (cardPropertiesInList.rarity == cardPropertiesCostsToCheck.rarity &&
                        cardPropertiesInList.Kartentyp == cardPropertiesCostsToCheck.Kartentyp &&
                        cardPropertiesInList.Gebaeudetyp == cardPropertiesCostsToCheck.Gebaeudetyp)
                    {
                        tempAmount = cardObjectInList.value;
                        //Debug.Log("cardObjectInList.value: " + cardObjectInList.value);
                    }
                }
            }
        }
        else tempAmount = 0;

        return tempAmount;
    }

    public void ClearCardPosition()
    {
        cardPositions.Clear();
    }

    public void AddCardPosition(Vector3 cardPosition)
    {
        cardPositions.Add(cardPosition);
    }

    public void RemoveCardPosition(int handPosition)
    {
        cardPositions.RemoveAt(handPosition);
    }

    public Vector3 GetCardPosition(int cardPosition)
    {
        return cardPositions[cardPosition];
    }

    public void ClearCardsInHand()
    {
        cardsInHand.Clear();
    }

    public void SetAmountCardsInHand(int amount)
    {
        cardsInHand = new List<CardProperties>(amount);
    }

    public void RemoveCardInHand(int handPosition)
    {
        cardsInHand.RemoveAt(handPosition);
    }

    public void ClearInstatiatedGameobjects()
    {
        instantiatedGameobjects.Clear();
    }

    public void RemoveInstatiatedGameobjects(int handPosition)
    {
        instantiatedGameobjects.RemoveAt(handPosition);
    }

    public void RemoveCardAt(int index)
    {
        if (index < cardsInHand.Count)
            cardsInHand.RemoveAt(index);

        if (index < cardPositions.Count)
            cardPositions.RemoveAt(index);

        if (index < instantiatedGameobjects.Count)
            instantiatedGameobjects.RemoveAt(index);
    }

}
