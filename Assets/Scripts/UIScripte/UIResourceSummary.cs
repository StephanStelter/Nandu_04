using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIResourceSummary : MonoBehaviour
{
    public static UIResourceSummary Instance { get; private set; }

    [Header("UI-Textfeld für Mainplayer")]
    public SpritesEnty resourceSpriteEntyScript;
    [SerializeField] private List<GameObject> resourceListUI;
    private int uiSpriteCounter = 0;

    private List<ResourceSummary> resourceListPerRound;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateMainPlayerSummaryText();
    }

    // ------------------------
    // Round-Start Aktualisierung
    // ------------------------
    public void StartRoundSummary()
    {
        foreach (NewPlayer player in TurnManager.Instance.Players)
        {
            SummaryResource(player); //Liste zum hinzufügen von Resourcen erstellen
            XPPerResourcesPerRound(player); //XP pro Resource
            if (!player.isMainPlayer)
            {
                AddResourcesToSummary(player); //Resourcen hinzufügen
            }
            AddToTotalSummaryList(player);//Resourcen der Gesamtliste hinzufügen

            // Main Player
            if (player.isMainPlayer)
            {
                // Resourcen für Rundenzusammenfassung speichern und rechnen  --> später löschen
                RoundStartInformation.Instance.GetResourceSummaryList(resourceListPerRound);
                // Resourcen Animation, später noch rechnen
                ResPerRoundHandler.Instance.GetResourceSummaryPerRound(resourceListPerRound);
            }
        }
    }

    // ------------------------
    // Zusammenfassungsanzeige
    // ------------------------
    public void UpdateMainPlayerSummaryText()
    {
        NewPlayer mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) return;

        SummaryResource(mainPlayer);

        uiSpriteCounter = 0;

        foreach (ResourceSummary resource in mainPlayer.SummaryResourcesList)
        {
            if (resource.ResourceName != ResourceType.XP) // keine Anzeige für XP mehr
            {
                GameObject uiSpriteText = resourceListUI[uiSpriteCounter];
                if (uiSpriteText == null) { Debug.Log("uiSpriteText == null"); }

                Image image = uiSpriteText.transform.GetComponentInChildren<Image>();
                if (image == null) { Debug.Log("image == null"); }

                image.sprite = resourceSpriteEntyScript.GetResourceSprite(resource.ResourceName);

                TextMeshProUGUI uiText = uiSpriteText.transform.GetComponentInChildren<TextMeshProUGUI>();
                if (uiText == null) { Debug.Log("uiText == null"); }

                int amount = resource.Amount;
                int amountPerRound = 0;

                ResourceSummary resourceInListPerRound = resourceListPerRound.Find(r => r.ResourceName == resource.ResourceName);
                if (resourceInListPerRound != null)
                    amountPerRound = resourceInListPerRound.Amount;

                //uiText.text = ($"{ZahlenFormatierung(amount)} (+{ZahlenFormatierung(amountPerRound)})");
                uiText.text = ($"{ZahlenFormatierung(amount)}");

                uiSpriteCounter++;
            }
        }
    }

    public void UpdateMainPlayerSummaryTextWithAnimationRound(ResourceType resourceType, int amount)
    {
        NewPlayer mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) return;

        uiSpriteCounter = 0;

        foreach (ResourceSummary resource in mainPlayer.SummaryResourcesList)
        {
            if (resource.ResourceName != ResourceType.XP) // keine Anzeige für XP mehr
            {
                if (resource.ResourceName == resourceType)
                {
                    resource.Amount += amount;

                    GameObject uiSpriteText = resourceListUI[uiSpriteCounter];
                    if (uiSpriteText == null) { Debug.Log("uiSpriteText == null"); }

                    TextMeshProUGUI uiText = uiSpriteText.transform.GetComponentInChildren<TextMeshProUGUI>();
                    if (uiText == null) { Debug.Log("uiText == null"); }

                    int amountPerRound = 0;

                    ResourceSummary resourceInListPerRound = resourceListPerRound.Find(r => r.ResourceName == resource.ResourceName);
                    if (resourceInListPerRound != null)
                        amountPerRound = resourceInListPerRound.Amount;

                    uiText.text = ($"{ZahlenFormatierung(resource.Amount)}");
                }

                uiSpriteCounter++;
            }
        }
    }

    public string ZahlenFormatierung(int resourceAmount)
    {
        if (resourceAmount >= 1_000_000_000)
            return (resourceAmount / 1_000_000_000.0).ToString("0.00") + "B";
        else if (resourceAmount >= 1_000_000)
            return (resourceAmount / 1_000_000.0).ToString("0.00") + "M";
        else if (resourceAmount >= 1_000)
            return (resourceAmount / 1_000.0).ToString("0.00") + "K";
        else
            return resourceAmount.ToString();
    }

    private void XPPerResourcesPerRound(NewPlayer player)
    {
        int counter = 0;

        foreach (ResourceSummary resourceSummary in resourceListPerRound)
        {
            counter += resourceSummary.Amount;
        }

        ResourceSummary resourceInListPerRound = resourceListPerRound.Find(r => r.ResourceName == ResourceType.XP);

        if (resourceInListPerRound != null)
        {
            // Resource existiert bereits → Menge erhöhen
            resourceInListPerRound.Amount += counter;
        }
        else
        {
            resourceListPerRound.Add(new ResourceSummary
            {
                ResourceName = ResourceType.XP,
                Amount = counter
            });
        }

    }

    private void SummaryResource(NewPlayer player)
    {
        //Liste reseten
        resourceListPerRound = new List<ResourceSummary>();

        foreach (GameObject gameObject in player.PlayerHexFieldSummaryList)
        {
            Hex hex = gameObject.GetComponent<Hex>();
            if (hex == null)
            {
                Debug.Log(hex.ToString());
                continue;
            }


            if (hex.diceNumber != Dice.Instance.GetSpriteNumberRound())
            {
                Debug.Log(hex.diceNumber + " != " + Dice.Instance.GetSpriteNumberRound());
                continue;
            }
            Debug.Log(hex.diceNumber + " == " + Dice.Instance.GetSpriteNumberRound());

            // Resourcen für aktuelles Würfelzeichen
            var resourceList = hex.resourcesList_MEPR;
            if (resourceList == null || resourceList.Count == 0) continue;

            foreach (Res resource in resourceList)
            {
                ResourceSummary resourceInListPerRound = resourceListPerRound.Find(r => r.ResourceName == resource.resourceName);

                if (resourceInListPerRound != null)
                {
                    if (!player.isMainPlayer)
                    {
                        // Resource existiert bereits → Menge erhöhen
                        resourceInListPerRound.Amount += resource.resourceAmount;
                    }
                }
                else
                {
                    resourceListPerRound.Add(new ResourceSummary
                    {
                        ResourceName = resource.resourceName,
                        Amount = resource.resourceAmount
                    });
                }

            }
        }
    }

    private void AddToTotalSummaryList(NewPlayer player)
    {
        foreach (ResourceSummary resourceInListPerRound in resourceListPerRound)
        {
            ResourceSummaryTotalCounter resourceTotalCounter = player.ResourceSummaryTotalCounterList.Find(r => r.ResourceName == resourceInListPerRound.ResourceName);

            if (resourceTotalCounter != null)
            {
                resourceTotalCounter.Amount += resourceInListPerRound.Amount;
            }
            else
            {
                player.ResourceSummaryTotalCounterList.Add(new ResourceSummaryTotalCounter
                {
                    ResourceName = resourceInListPerRound.ResourceName,
                    Amount = resourceInListPerRound.Amount
                });
            }
        }
    }

    private void AddResourcesToSummary(NewPlayer player)
    {
        foreach (ResourceSummary resource in player.SummaryResourcesList)
        {
            ResourceSummary resourceInListPerRound = resourceListPerRound.Find(r => r.ResourceName == resource.ResourceName);

            if (resourceInListPerRound != null)
            {
                resource.Amount += resourceInListPerRound.Amount;
            }
        }

    }
    // ------------------------
    // BuyWith-Methoden (Abzug + UI-Update)
    // ------------------------

    public bool BuyWith(ResourceType type, int amount)
    {
        bool success = false;

        NewPlayer player = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (player == null) { Debug.Log("player == null"); }

        foreach (ResourceSummary resource in player.SummaryResourcesList)
        {
            if (resource.ResourceName == type)
            {
                if (resource.Amount <= amount)
                {
                    resource.Amount -= amount;
                    success = true;
                }
                else
                    success = false;
            }
        }

        if (success && player.isMainPlayer)
        {
            UpdateMainPlayerSummaryText();
        }

        return success;
    }

    public bool GetCostCheck(ResourceType type, int amount)
    {
        bool success = false;

        NewPlayer player = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (player == null) { Debug.Log("player == null"); }

        foreach (ResourceSummary resource in player.SummaryResourcesList)
        {
            if (resource.ResourceName == type)
            {
                if (resource.Amount <= amount)
                {
                    success = true;
                }
                else
                    success = false;
            }
        }

        return success;
    }

    public void SetXP(int newXP)
    {
        NewPlayer activPlayer = TurnManager.Instance.GetCurrentPlayer();
        if (activPlayer != null)
        {
            activPlayer.AddResourceAmount(ResourceType.XP, newXP);

            // Main Player
            if (activPlayer.isMainPlayer)
            {
                //xpAmountInt += newXP;

                foreach (ResourceSummary resource in activPlayer.SummaryResourcesList)
                {
                    if (resource.ResourceName == ResourceType.XP)
                    {
                        //xpAmountText.text = resource.Amount.ToString();
                        break;
                    }
                }
            }
            if (activPlayer.isMainPlayer)
                UpdateMainPlayerSummaryText();
        }
    }
}
