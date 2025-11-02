using System.Collections.Generic;
using UnityEngine;

// ───────────────────────────── SINGLETON & INITIALISIERUNG ─────────────────────────────
public class GameHexFieldManager : MonoBehaviour
{
    [System.Serializable]
    public class CardCounterEntry
    {
        public GameObject gameObject;
        public int value;
        public CardCounterEntry(GameObject obj, int initialValue)
        {
            gameObject = obj;
            value = initialValue;
        }
    }

    public static GameHexFieldManager Instance;

    public List<GameObject> PlayerOneSummaryList = new();
    public List<CardCounterEntry> PlayerOneCardCounterList = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        PlayerOneSummaryList = new List<GameObject>();
    }

// ───────────────────────────── SPIELER-LISTEN & KARTENZÄHLER ─────────────────────────────
    public bool CheckPartofPlayerOneSummaryList(GameObject hexfieldGameobject)
        => PlayerOneSummaryList.Contains(hexfieldGameobject);

    public List<CardCounterEntry> GetPlayerOneCardCounterList()
        => PlayerOneCardCounterList;

    private void IncreaseValue(GameObject obj, int increaseValue = 1)
    {
        var entry = PlayerOneCardCounterList.Find(e => e.gameObject == obj);
        if (entry != null) entry.value += increaseValue;
    }

    public void AddCardToPlayedList(CardProperties cardProps)
    {
        foreach (var entry in PlayerOneCardCounterList)
        {
            var props = entry.gameObject.GetComponent<CardProperties>();
            if (props != null &&
                props.rarity == cardProps.rarity &&
                props.Kartentyp == cardProps.Kartentyp &&
                props.Gebaeudetyp == cardProps.Gebaeudetyp)
            {
                IncreaseValue(props.gameObject);
                return;
            }
        }
        PlayerOneCardCounterList.Add(new CardCounterEntry(cardProps.gameObject, 1));
    }

    public int GetNewMultiplierForCardCosts(GameObject cardObjectsCostsToCheck)
    {
        var cardProps = cardObjectsCostsToCheck.GetComponent<CardProperties>();
        if (cardProps == null) return 0;

        foreach (var entry in PlayerOneCardCounterList)
        {
            var props = entry.gameObject.GetComponent<CardProperties>();
            if (props != null &&
                props.rarity == cardProps.rarity &&
                props.Kartentyp == cardProps.Kartentyp &&
                props.Gebaeudetyp == cardProps.Gebaeudetyp)
            {
                return entry.value;
            }
        }
        return 0;
    }

// ───────────────────────────── KARTEN-SUCHE ─────────────────────────────
    public GameObject GetCardFromList(GameObject currentCardGameobject)
    {
        var cardPropsCurrent = currentCardGameobject.GetComponent<CardProperties>();
        if (cardPropsCurrent == null) { Debug.Log("cardPropertiesCurrentCard == null"); return currentCardGameobject; }

        foreach (var cardObjectInList in PlayerOneSummaryList)
        {
            foreach (var cardProp in cardObjectInList.GetComponentsInChildren<CardProperties>())
            {
                if (cardPropsCurrent.GetId() == cardProp.GetId())
                    return cardProp.gameObject;
            }
        }
        return currentCardGameobject;
    }
}


