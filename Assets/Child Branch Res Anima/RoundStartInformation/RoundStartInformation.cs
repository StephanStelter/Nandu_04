
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundStartInformation : MonoBehaviour

{
    //UNITY IST KACKE TESTESTEST
    public static RoundStartInformation Instance { get; set; }


    [System.Serializable]
    public class ResourcesPerRound
    {
        public ResourceType resourceType;
        public int amount;
        public Sprite sprite;
        public RessourceField ressourceField;
    }

    public List<ResourcesPerRound> resourceListPerRoundMainPLayer = new List<ResourcesPerRound>() { };

    [System.Serializable]
    public class ResourcesPerSign
    {
        public Sign sign;
        public int woodAmount;
        public int stoneAmount;
        public int foodAmount;
        public int ironAmount;
        public int woolAmount;
        public int goldAmount;
    }

    public List<ResourcesPerSign> resourceListPerSign = new List<ResourcesPerSign>() { };



    private NewPlayer mainPlayer;
    public GameObject uiGameobject;
    public GameObject blockScreen;
    public TextMeshProUGUI roundText;
    public GameObject[] signResourcesArray; // Array für die Sign Ressourcen GameObjects
    public GameObject summaryGameobject;
    public GameObject detailedGameobject;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) return;
        // Reset
        ResetList();
        SetTextForUI();
        CloseUI();
    }

    public void GetResourceSummaryList(List<ResourceSummary> resourceSummaries)
    {
        foreach (ResourceSummary resourceSummary in resourceSummaries)
        {
            if (resourceSummary.ResourceName != ResourceType.XP)
            {
                // Prüfen, ob dieser ResourceType schon in der Liste existiert
                var existing = resourceListPerRoundMainPLayer.Find(r => r.resourceType == resourceSummary.ResourceName);
                if (existing != null)
                {
                    // Amount erhöhen
                    existing.amount += resourceSummary.Amount;
                }
                else { Debug.Log("resourceListPerRoundMainPLayer != null" + resourceSummary.ResourceName); }
            }
        }
        // Sign Ressourcen hinzufügen
        SetSignResources(resourceSummaries);
        // Sign Ressourcen Text setzen
        SetSignRecoucesText();
    }

    public void SetTextForUI()
    {
        roundText.text = "Runde: " + TurnManager.Instance.rundenZähler;

        foreach (ResourcesPerRound resourcesPerRound in resourceListPerRoundMainPLayer)
        {
            resourcesPerRound.ressourceField.image.sprite = resourcesPerRound.sprite;
            resourcesPerRound.ressourceField.text.text = UIResourceSummary.Instance.ZahlenFormatierung(resourcesPerRound.amount);
        }
    }


    public void OpenUI()
    {
        // Set
        SetTextForUI();
        // Open UI
        ChangeToSummaryOverview();
        //uiGameobject.SetActive(true);
        blockScreen.SetActive(true);
        MyEventHandler.Instance.gameMode = GameMode.SkillMenu;
    }

    public void CloseUI()
    {
        // Close UI
        CloseUIObjects();
        //uiGameobject.SetActive(false);
        blockScreen.SetActive(false);
        MyEventHandler.Instance.gameMode = GameMode.Default;
        // Reset
        ResetList();
        SetTextForUI();
    }

    public void ChangeToDetailedOverview()
    {
        detailedGameobject.SetActive(true);
        summaryGameobject.SetActive(false);
    }

    public void ChangeToSummaryOverview()
    {
        detailedGameobject.SetActive(false);
        summaryGameobject.SetActive(true);
    }

    private void CloseUIObjects()
    {
        detailedGameobject.SetActive(false);
        summaryGameobject.SetActive(false);
    }

    private void ResetList()
    {
        foreach (ResourcesPerRound resourcesPerRound in resourceListPerRoundMainPLayer)
        {
            resourcesPerRound.amount = 0;
        }

        foreach (ResourcesPerSign resourcesPerSign in resourceListPerSign)
        {
            resourcesPerSign.woodAmount = 0;
            resourcesPerSign.stoneAmount = 0;
            resourcesPerSign.foodAmount = 0;
            resourcesPerSign.ironAmount = 0;
            resourcesPerSign.woolAmount = 0;
            resourcesPerSign.goldAmount = 0;
        }
    }

    private void SetSignResources(List<ResourceSummary> resourceSummaries)
    {
        Sign currentSign = Dice.Instance.GetSignOfDice();

        ResourcesPerSign resourcesPerSign = resourceListPerSign.Find(r => r.sign == currentSign);
        if (resourcesPerSign != null)
        {
            foreach (ResourceSummary resourceSummary in resourceSummaries)
            {
                if (resourceSummary.ResourceName != ResourceType.XP)
                {


                    if (resourceSummary.ResourceName == ResourceType.Wood)
                    {
                        resourcesPerSign.woodAmount += resourceSummary.Amount;
                    }
                    else if (resourceSummary.ResourceName == ResourceType.Stone)
                    {
                        resourcesPerSign.stoneAmount += resourceSummary.Amount;
                    }
                    else if (resourceSummary.ResourceName == ResourceType.Food)
                    {
                        resourcesPerSign.foodAmount += resourceSummary.Amount;
                    }
                    else if (resourceSummary.ResourceName == ResourceType.Iron)
                    {
                        resourcesPerSign.ironAmount += resourceSummary.Amount;
                    }
                    else if (resourceSummary.ResourceName == ResourceType.Wool)
                    {
                        resourcesPerSign.woolAmount += resourceSummary.Amount;
                    }
                    else if (resourceSummary.ResourceName == ResourceType.Gold)
                    {
                        resourcesPerSign.goldAmount += resourceSummary.Amount;
                    }

                }
            }
        }
    }

    private void SetSignRecoucesText()
    {
        if (signResourcesArray.Length > 0)
        {
            int counter = 0;

            foreach (ResourcesPerSign resourcesPerSign in resourceListPerSign)
            {
                SignResourceTextHandler signResourceTextHandler = signResourcesArray[counter].GetComponent<SignResourceTextHandler>();

                signResourceTextHandler.woodResourceText.text = UIResourceSummary.Instance.ZahlenFormatierung(resourcesPerSign.woodAmount);
                signResourceTextHandler.stoneResourceText.text = UIResourceSummary.Instance.ZahlenFormatierung(resourcesPerSign.stoneAmount);
                signResourceTextHandler.foodResourceText.text = UIResourceSummary.Instance.ZahlenFormatierung(resourcesPerSign.foodAmount);
                signResourceTextHandler.ironResourceText.text = UIResourceSummary.Instance.ZahlenFormatierung(resourcesPerSign.ironAmount);
                signResourceTextHandler.woolResourceText.text = UIResourceSummary.Instance.ZahlenFormatierung(resourcesPerSign.woolAmount);
                signResourceTextHandler.goldResourceText.text = UIResourceSummary.Instance.ZahlenFormatierung(resourcesPerSign.goldAmount);

                counter++;
            }
        }
    }
}
