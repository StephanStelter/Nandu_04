using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// ───────────────────────────── FELDER & INSPECTOR ─────────────────────────────
public class CardProperties : MonoBehaviour
{
    [SerializeField] private int cardID;

    private bool inHand;
    public bool isSelected = false;
    private bool inFinalPosition;
    private bool inStartPosition;
    public int handPosition;
    private bool cardInSlot = false;
    private bool cardLvlStart = false;
    public int playedCard = 0;

    public Coroutine moveRoutine;

    [Header("Verwendung von XP")]
    public int xpEarningsModificator;
    public int xpEarningsCosts;

    [Header("Allgemeine Kartenklassifizierung")]
    [SerializeField] public Rarity rarity;
    [SerializeField] public CardType Kartentyp;
    [SerializeField] public BuildingType Gebaeudetyp;
    [SerializeField] private int KartenLevelMultiplikator;

    [Header("Angaben für Action Card")]
    [SerializeField] private string alternativerKartenname;
    [SerializeField] private int actionRadius;
    [SerializeField] private ActionChangeArea actionChangeArea;

    [Header("Besondere Action Card")]
    public bool isGeologist = false;
    public bool isReroll = false;
    public bool isRerollAllRecources = false;
    public ResourceType resourceNameToReroll;

    [Header("Sonstiges")]
    [SerializeField] public int lvlZahlInt;

    [Header("Erträge der Karte")]
    public List<Resources> resourceEarningList = new();
    [Header("Kosten für die Karte")]
    [SerializeField] private int costMultiplierNormal;
    [SerializeField] private int costMultiplierLvlCardOnCard;
    [SerializeField] private int costMultiplierLvlCardNormal;
    public List<ResourceCosts> resourceCostsListBasic = new();
    public List<ResourceCosts> resourceCostsList = new();
    public List<ResourceCosts> resourceCostsListLvlCardOnCard = new();
    public List<ResourceCosts> resourceCostsListLvlCardNormal = new();

    int newPlayedCard;
    public GameObject model = null;

    public ResList cost = new(0, 0, 10, 0, 10, 0, 0, 0);

    // ───────────────────────────── NESTED KLASSE ─────────────────────────────
    [Serializable]
    public class Resources
    {
        public ResourceType resourceName;
        public int resourceAmount;
        public bool isBlocked;
        public bool isVillage;
        public Player.PlayerSelecting playerName;
    }

    [Serializable]
    public class ResourceCosts
    {
        public ResourceType resourceName;
        public int resourceAmount;
    }

    // ───────────────────────────── UNITY LIFECYCLE ─────────────────────────────
    private void Awake()
    {
        SetBasicCostResources();
        inFinalPosition = false;
        inStartPosition = false;
        SetStartPosition();
    }

    public TextMeshProUGUI costsInUI, lvlZahl, buildingText, earnText, earnHeaderText;

    private void Start()
    {
        if (earnHeaderText != null)
            earnHeaderText.text = "Verbesserung: " + actionChangeArea;
    }

    // ───────────────────────────── ANZEIGE-METHODEN ─────────────────────────────
    public void DisplayResourceCosts()
    {
        if (costsInUI == null) return;
        string displayText = "";
        foreach (ResourceCosts cost in resourceCostsList)
            displayText += $"{cost.resourceName}: <color=red>-{cost.resourceAmount}</color>\n";
        costsInUI.text = displayText;
    }

    public void DisplayCardEarnings()
    {
        if (earnText == null) return;
        string displayText = "";
        foreach (Resources cost in resourceEarningList)
            displayText += $"{cost.resourceName}: {cost.resourceAmount}\n";
        earnText.text = displayText;
    }

    public void DisplayActionCardName()
    {
        if (buildingText == null) return;
        if (Kartentyp == CardType.Building)
            buildingText.text = Gebaeudetyp.ToString();
        else if (Kartentyp == CardType.Action)
            buildingText.text = alternativerKartenname;
        else
            buildingText.text = "NA";
    }

    public void DestroyCard()
    {
        Destroy(gameObject);
    }

    // ───────────────────────────── POSITION & STATUS ─────────────────────────────
    private void SetStartPosition()
    {
        if (transform.localPosition != Vector3.zero)
            transform.localPosition = Vector3.zero;
        if (transform.localRotation != Quaternion.identity)
            transform.localRotation = Quaternion.identity;
    }

    public bool GetInHand() => inHand;
    public void SetInHand(bool _inHand) => inHand = _inHand;
    public int GetHandPosition() => handPosition;
    public void SetHandPosition(int _handPosition) => handPosition = _handPosition;
    public bool GetInFinalPosition() => inFinalPosition;
    public void SetInFinalPosition(bool _inFinalPosition) => inFinalPosition = _inFinalPosition;
    public bool GetInStartPosition() => inStartPosition;
    public void SetInStartPosition(bool _inStartPosition) => inStartPosition = _inStartPosition;
    public int GetCardID() => cardID;
    public void SetCardID(int _cardID) => cardID = _cardID;
    public int GetLvlZahl() => lvlZahlInt;
    public void SetLvlZahl()
    {
        lvlZahlInt++;
        if (lvlZahl != null)
            lvlZahl.text = lvlZahlInt.ToString();
    }
    public bool GetCardInSlot() => cardInSlot;
    public void SetCardInSlot(bool _cardInSlot) => cardInSlot = true;
    public string GetActionChangeArea() => actionChangeArea.ToString();
    public void SetCardLvlStart() => cardLvlStart = true;
    public bool GetCardLvlStart() => cardLvlStart;
    public int GetId() => cardID;

    // ───────────────────────────── KARTEN-LOGIK ─────────────────────────────
    public int levelFunction(int res)
    {
        int cardCount = 3;
        int potenz = 9;
        int result = (int)((1.0 / potenz) * Math.Pow(cardCount, 2) + 1);
        return res * result;
    }

    public string GetCardType() => Kartentyp.ToString();
    public string GetCardRarity() => rarity.ToString();
    public string GetBuildingType() => Gebaeudetyp.ToString();

    public List<Resources> GetResourceEarningList() => resourceEarningList;

    public void ChangeResourceEarningList(List<Resources> resourceEarningListWithChanges)
    {
        foreach (Resources resources in resourceEarningListWithChanges)
        {
            int index = resourceEarningList.FindIndex(item => item.resourceName == resources.resourceName);
            if (index != -1)
                resourceEarningList[index].resourceAmount += resources.resourceAmount;
        }
        DisplayCardEarnings();
    }

    public void CardLvln(CardProperties _cardPropertiesCardToWorkWith)
    {
        int cardLvlCardToWorkWith = _cardPropertiesCardToWorkWith.GetLvlZahl();
        foreach (Resources resources in _cardPropertiesCardToWorkWith.resourceEarningList)
        {
            int index = resourceEarningList.FindIndex(item => item.resourceName == resources.resourceName);
            if (index != -1)
                resourceEarningList[index].resourceAmount *= (1 + cardLvlCardToWorkWith);
        }
        SetLvlZahl();
        DisplayCardEarnings();
    }

    public void CardLvlPreview(CardProperties _cardPropertiesCardToWorkWith)
    {
        if (earnText == null) return;
        int cardLvlCardToWorkWith = _cardPropertiesCardToWorkWith.GetLvlZahl();
        string displayText = "";
        foreach (Resources earnings in resourceEarningList)
        {
            int index = resourceEarningList.FindIndex(item => item.resourceName == earnings.resourceName);
            if (index != -1)
            {
                int diff = (resourceEarningList[index].resourceAmount * (1 + cardLvlCardToWorkWith)) - earnings.resourceAmount;
                displayText += $"{earnings.resourceName}: {earnings.resourceAmount}<color=green> +{diff}</color>\n";
            }
        }
        earnText.text = displayText;
    }

    public void CardIncreasingEarningsPreview(CardProperties _cardPropertiesCardToWorkWith)
    {
        if (earnText == null) return;
        int cardLvlCardToWorkWith = _cardPropertiesCardToWorkWith.GetLvlZahl();
        string displayText = "";
        foreach (Resources earnings in resourceEarningList)
        {
            int index = resourceEarningList.FindIndex(item => item.resourceName == earnings.resourceName);
            if (index != -1)
            {
                int diff = xpEarningsModificator * (1 + cardLvlCardToWorkWith);
                displayText += $"{earnings.resourceName}: {earnings.resourceAmount}<color=green> +{diff}</color>\n";
            }
        }
        earnText.text = displayText;
    }

    public void CardIncreasingEarnings(CardProperties _cardPropertiesCardToWorkWith)
    {
        int cardLvlCardToWorkWith = _cardPropertiesCardToWorkWith.GetLvlZahl();
        foreach (Resources earnings in resourceEarningList)
        {
            int index = resourceEarningList.FindIndex(item => item.resourceName == earnings.resourceName);
            if (index != -1)
                resourceEarningList[index].resourceAmount += xpEarningsModificator * (1 + cardLvlCardToWorkWith);
        }
        DisplayCardEarnings();
    }

    // ───────────────────────────── KOSTEN-BERECHNUNG ─────────────────────────────
    public void SetBasicCostResources()
    {
        resourceCostsList.Clear();
        resourceCostsListLvlCardOnCard.Clear();
        resourceCostsListLvlCardNormal.Clear();

        foreach (ResourceCosts resource in resourceCostsListBasic)
        {
            resourceCostsList.Add(new ResourceCosts
            {
                resourceName = resource.resourceName,
                resourceAmount = resource.resourceAmount
            });
            resourceCostsListLvlCardOnCard.Add(new ResourceCosts
            {
                resourceName = resource.resourceName,
                resourceAmount = resource.resourceAmount
            });
            resourceCostsListLvlCardNormal.Add(new ResourceCosts
            {
                resourceName = resource.resourceName,
                resourceAmount = resource.resourceAmount
            });
        }
        NewCostsForCard();
    }

    private void NewCostsForCard()
    {
        try
        {
            NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();
            if (activePlayer != null)
                newPlayedCard = activePlayer.GetNewMultiplierForCardCosts(gameObject);

            if (newPlayedCard > 0)
            {
                foreach (ResourceCosts resourceCosts in resourceCostsList)
                    resourceCosts.resourceAmount *= lvlZahlInt * (newPlayedCard + 1) * costMultiplierNormal;
                foreach (ResourceCosts resourceCostsLvlCardOnCard in resourceCostsListLvlCardOnCard)
                    resourceCostsLvlCardOnCard.resourceAmount *= lvlZahlInt * (newPlayedCard + 1) * costMultiplierLvlCardOnCard;
                foreach (ResourceCosts resourceCostsLvlCardNormal in resourceCostsListLvlCardNormal)
                    resourceCostsLvlCardNormal.resourceAmount *= lvlZahlInt * (newPlayedCard + 1) * costMultiplierLvlCardNormal;
            }
            DisplayResourceCosts();
            DisplayCardEarnings();
            DisplayActionCardName();
            if (lvlZahl != null)
                lvlZahl.text = lvlZahlInt.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    // ───────────────────────────── ANIMATION ─────────────────────────────
    public void MoveAlongPath(List<Vector3> pathPoints, float speed = 800f, float rotation = 0f, string transformMode = "local")
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveToPoints(pathPoints, speed, rotation, transformMode));
    }

    private IEnumerator MoveToPoints(List<Vector3> targets, float speed, float rotation, string transformMode)
    {
        Transform tf = transformMode == "global"
            ? this.transform
            : GetComponentInChildren<CardAnimationController>().transform;

        int rotationSpeed = 700;
        foreach (Vector3 target in targets)
        {
            bool moving = true;
            bool rotating = true;
            float rotated = 0f;

            while (moving || rotating)
            {
                if (moving)
                {
                    float distance = Vector3.Distance(tf.localPosition, target);
                    if (distance > 0.01f)
                        tf.localPosition = Vector3.MoveTowards(tf.localPosition, target, speed * Time.deltaTime);
                    else
                    {
                        tf.localPosition = target;
                        moving = false;
                    }
                }
                if (rotating)
                {
                    float step = Mathf.Min(rotationSpeed * Time.deltaTime, rotation - rotated);
                    if (rotated < rotation)
                    {
                        tf.Rotate(Vector3.up, step);
                        rotated += step;
                    }
                    if (rotated >= rotation)
                    {
                        tf.localEulerAngles = Vector3.zero;
                        rotating = false;
                    }
                }
                yield return null;
            }
        }
        moveRoutine = null;
    }
}
