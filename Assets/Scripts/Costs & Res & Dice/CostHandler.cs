using System.Collections.Generic;
using UnityEngine;
using static CardProperties;

public class CostHandler : MonoBehaviour
{
    public static CostHandler Instance { get; private set; }

    public List<Socket> socketListMouseOver = new();
    public List<string> problemCosts = new();

    [SerializeField] private GameObject myEventHandlerGameObject;
    private MyEventHandler myEventHandlerScript;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        myEventHandlerScript = myEventHandlerGameObject.GetComponent<MyEventHandler>();
        if (myEventHandlerScript == null) Debug.Log("myEventHandlerScript == null");
    }

    // ───────────────────────────── KOSTEN BEZAHLEN ─────────────────────────────
    public void PayCostsForCard(GameObject card)
    {
        var cardProps = card.GetComponent<CardProperties>();
        if (cardProps == null) return;
        foreach (var resource in cardProps.resourceCostsList)
            Buy(resource.resourceName, resource.resourceAmount);
    }

    public void PayCostsForCardLvlNormal(GameObject card)
    {
        var cardProps = card.GetComponent<CardProperties>();
        if (cardProps == null) return;
        foreach (var resource in cardProps.resourceCostsListLvlCardNormal)
            Buy(resource.resourceName, resource.resourceAmount);
    }

    public void PayCostsForIncreasingEarnings(GameObject card)
    {
        var cardProps = card.GetComponent<CardProperties>();
        if (cardProps == null) return;
        Buy(ResourceType.XP, cardProps.xpEarningsCosts);
    }

    public void PayCostsForSocketCardsLvl(GameObject socketCardGameObject)
    {
        var cardProps = socketCardGameObject.GetComponent<CardProperties>();
        if (cardProps == null) return;
        foreach (var resource in cardProps.resourceCostsListLvlCardOnCard)
            Buy(resource.resourceName, resource.resourceAmount);
    }

    private static void Buy(ResourceType resourceType, int amount)
    {
        bool bought = UIResourceSummary.Instance.BuyWith(resourceType, amount);
        if (!bought)
            Debug.Log("Nicht genug Resourcen: " + resourceType);
    }

    // ───────────────────────────── KOSTENPRÜFUNG ─────────────────────────────
    public bool CanAfford(CardProperties cardProps)
    {
        if (cardProps == null) return false;
        foreach (var resource in cardProps.resourceCostsList)
            if (!UIResourceSummary.Instance.GetCostCheck(resource.resourceName, resource.resourceAmount))
                return false;
        return true;
    }

    public bool FirstCheckOfCostsResourceListNormal()
    {
        var cardProps = GetCurrentCardPropsAndResetProblems();
        foreach (var resource in cardProps.resourceCostsList)
            CheckResourceCost(resource);
        return problemCosts.Count == 0;
    }

    public bool FirstCheckOfCostsResourceListLvlCardNormal(GameObject card)
    {
        var cardProps = card.GetComponent<CardProperties>();
        if (cardProps == null) return false;
        problemCosts.Clear();
        foreach (var resource in cardProps.resourceCostsListLvlCardNormal)
            CheckResourceCost(resource);
        return problemCosts.Count == 0;
    }

    public bool FirstCheckOfCostsResourceListLvlCardOnCard()
    {
        var cardProps = GetCurrentCardPropsAndResetProblems();
        foreach (var resource in cardProps.resourceCostsListLvlCardOnCard)
            CheckResourceCost(resource);
        return problemCosts.Count == 0;
    }

    public bool FirstCheckXPCostsOnIncreasingEarnings(GameObject card)
    {
        var cardProps = card.GetComponent<CardProperties>();
        if (cardProps == null) return false;
        return UIResourceSummary.Instance.GetCostCheck(ResourceType.XP, cardProps.xpEarningsCosts);
    }

    // ───────────────────────────── HILFSMETHODEN ─────────────────────────────
    private CardProperties GetCurrentCardPropsAndResetProblems()
    {
        var cardProps = myEventHandlerScript.chosenCard.GetComponent<CardProperties>();
        if (cardProps == null) Debug.Log("cardPropertiesCurrentCard == null");
        cardProps.SetBasicCostResources();
        problemCosts.Clear();
        return cardProps;
    }

    private void CheckResourceCost(ResourceCosts resource)
    {
        if (!UIResourceSummary.Instance.GetCostCheck(resource.resourceName, resource.resourceAmount))
            problemCosts.Add(resource.resourceName.ToString());
    }
}