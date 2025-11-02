using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SpritesEnty;
using static SkilltreeAndBonusesHandler;


public class SkilltreeAndBonusesHandler : MonoBehaviour
{
    public static SkilltreeAndBonusesHandler Instance { get; private set; }
    #region Panel Controller
    [SerializeField] private GameObject skilltreeObject;

    [Header("Panels")]
    public CanvasGroup victoryPanel;
    public CanvasGroup skilltreePanel;
    public CanvasGroup challengesPanel;
    public CanvasGroup cardsPanel;

    [Header("Buttons")]
    public Button victoryBtn;
    public Button mainBtn;
    public Button resourcesBtn;
    public Button cardsBtn;



    private bool skilltreeToogle = false;

    public ScrollSkills scrollSkillsChallenges;

    public List<Button> panelButtonsClicked;
    public string panelChildImageNameClicked = "ClickedBtnImage"; // Name des Child-Images im Button
    public string panelChildImageNameUnclicked = "UnclickedBtnImage"; // Name des Child-Images im Button

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }


    private void PanelButtonStart()
    {
        foreach (Button btn in panelButtonsClicked)
        {
            // Initial: Alle "Clicked"-Images ausblenden, "Unclicked"-Images einblenden
            Transform clickedImage = btn.transform.Find(panelChildImageNameClicked);
            Transform unclickedImage = btn.transform.Find(panelChildImageNameUnclicked);

            if (clickedImage != null)
                clickedImage.gameObject.SetActive(false);

            if (unclickedImage != null)
                unclickedImage.gameObject.SetActive(true);

            // Button-Listener hinzufügen
            btn.onClick.AddListener(() => OnPanelButtonClicked(btn));
        }
    }

    private void OnPanelButtonClicked(Button clickedButton)
    {
        foreach (Button btn in panelButtonsClicked)
        {
            bool isClicked = (btn == clickedButton);

            Transform clickedImage = btn.transform.Find(panelChildImageNameClicked);
            Transform unclickedImage = btn.transform.Find(panelChildImageNameUnclicked);

            if (clickedImage != null)
                clickedImage.gameObject.SetActive(isClicked);

            if (unclickedImage != null)
                unclickedImage.gameObject.SetActive(!isClicked);
        }
    }


    private void ShowPanel(CanvasGroup panelToShow)
    {
        Debug.Log("Switching Panel: " + panelToShow.name);

        CanvasGroup[] allPanels = { victoryPanel, skilltreePanel, challengesPanel, cardsPanel };

        foreach (CanvasGroup panel in allPanels)
        {
            bool isActive = (panel == panelToShow);

            panel.alpha = isActive ? 1f : 0f;
            panel.interactable = isActive;
            panel.blocksRaycasts = isActive;
        }
    }

    public void OpenSkilltree()
    {
        MyEventHandler.Instance.gameMode = GameMode.SkillMenu;
        skilltreeToogle = !skilltreeToogle;

        if (skilltreeToogle)
        {
            skilltreeObject.SetActive(true);

            // Button Events zuweisen
            victoryBtn.onClick.AddListener(() => ShowPanel(victoryPanel));
            mainBtn.onClick.AddListener(() => ShowPanel(skilltreePanel));
            resourcesBtn.onClick.AddListener(() => ShowPanel(challengesPanel));
            cardsBtn.onClick.AddListener(() => ShowPanel(cardsPanel));

            PanelButtonStart();

            // Initial: Übersicht Victory Points sichtbar
            ShowPanel(victoryPanel);
            OnPanelButtonClicked(panelButtonsClicked[0]);

            GetChallengeArray();
            UpdateChallengeObjects();
            ShowFirstChallenge();
            scrollSkillsChallenges.Initialize();

            ButtonStart();
            SetVictoryPoints();

            // Startzustand: Panel geschlossen
            victoryPointPanel.fillAmount = 0f;
            victoryPointShortcut.SetActive(false);

            SkillTreeManager skillTreeManager = GetComponent<SkillTreeManager>();
            if (skillTreeManager == null) { Debug.Log("(skillTreeManager == null"); };
            skillTreeManager.SetSkilltree();
        }
        else
        {
            CloseSkilltree();
        }
    }

    public void CloseSkilltree()
    {
        MyEventHandler.Instance.gameMode = GameMode.Default;
        skilltreeToogle = false;

        skilltreeObject.SetActive(false);
    }
    #endregion


    #region Challenges
    // Challenges ------------------------------------------------------------------------------------------------------
    [Header("Challenges")]
    public RectTransform challengeObjectParent;
    private ChallengeObject[] challangeObjectArray;
    private ChallengeObject currentChallengeObject;
    public Button skilltreeButton;
    public PlayerLevelManager playerLevelManager;

    public void UpdateChallengeObjects()
    {
        skilltreeButton.image.color = Color.white;

        GetChallengeArray();

        foreach (ChallengeObject challengeObject in challangeObjectArray)
        {
            challengeObject.UpdatUI();

            CheckChallenges(challengeObject);
        }
    }

    private void ShowFirstChallenge()
    {
        foreach (ChallengeObject challengeObject in challangeObjectArray)
        {
            if (challengeObject.resourceForSkill == ResourceType.Wood)
            {
                challengeObject.ClickUiButton();
            }
        }
    }

    public void GetChallengeObject(GameObject challengeGameObject)
    {
        ChallengeObject challengeObject = challengeGameObject.GetComponent<ChallengeObject>();

        currentChallengeObject = challengeObject;

        Debug.Log("currentChallengeObject" + currentChallengeObject.resourceForSkill);
    }

    public void RedeemChallengeAwardButton()
    {
        currentChallengeObject.ClickRedeemButton();
        UpdateChallengeObjects();
        playerLevelManager.SetLevelStats();
    }

    private void CheckChallenges(ChallengeObject challengeObject)
    {
        if (challengeObject.challengeCompleted)
            skilltreeButton.image.color = Color.green;
    }

    private void GetChallengeArray()
    {
        challangeObjectArray = challengeObjectParent.GetComponentsInChildren<ChallengeObject>();
        if (challangeObjectArray.Length == 0) { Debug.Log("(challangeObjectArray.Length == 0"); };
    }
    #endregion


    #region Victory Points
    // Victory Points ------------------------------------------------------------------------------------------------------
    [Header("Victory Points")]
    public GameObject playerListVP;
    private UIVictoryPointsPlayer[] uIVictoryPointsPlayerArray;

    public GameObject victoryPointShortcut;
    public Image victoryPointPanel;
    private bool victoryPointShortcutToogle = false;
    public float fillSpeed = 1f;
    private Coroutine currentRoutine;

    public SpritesEnty resourceSpriteEnty;

    public TextMeshProUGUI populationText;
    public TextMeshProUGUI religionText;
    public TextMeshProUGUI militaryText;
    public TextMeshProUGUI craftsmenext;
    public TextMeshProUGUI tradingText;
    public TextMeshProUGUI politicsText;
    public TextMeshProUGUI VPsummaryText;

    public Image grainImage;
    public Image populationImage;
    public Image stoneImage;
    public Image religionImage;
    public Image metalImage;
    public Image militaryImage;
    public Image woodImage;
    public Image craftsmenImage;
    public Image goldImage;
    public Image tradingImage;
    public Image woolImage;
    public Image politicsImage;
    public Image vpImage;

    public int resourceAmountToGetAVictoryPoint;

    // Höchstwerte VP
    [System.Serializable]
    public class HighestVictoryPoints
    {
        public int craftsmenVPSummaryHS;
        public int religionVPSummaryHS;
        public int populationVPSummaryHS;
        public int militaryVPSummaryHS;
        public int politicsVPSummaryHS;
        public int tradingVPSummaryHS;
        public int totalVPSummaryHS;
    }

    public List<HighestVictoryPoints> highestVictoryPointsList = new List<HighestVictoryPoints>();

    // VP Sammelliste
    [System.Serializable]
    public class VictoryPointsSummary
    {
        public string playerName;
        public int craftsmenVPSummary;
        public int religionVPSummary;
        public int populationVPSummary;
        public int militaryVPSummary;
        public int politicsVPSummary;
        public int tradingVPSummary;
        public int totalVPSummary;
    }

    public List<VictoryPointsSummary> victoryPointsSummaryList = new List<VictoryPointsSummary>();



    public void SetVictoryPoints()
    {
        // Basic
        victoryPointsSummaryList = new List<VictoryPointsSummary>();
        GetPlayerListVPArray();

        // Calculation
        foreach (NewPlayer player in TurnManager.Instance.Players)
        {
            CalculateVictoryPoints(player);
        }

        // Highest VP
        GetHighestVP();

        // Main Player Short UI
        SetMainPlayerShortText();

        // Sorting List
        SortHighestVictoryPointsList("totalVPSummary");
        OnButtonClicked(VPButtons[0]);
    }

    private void UISortingOrder()
    {
        // Summery UI
        for (int i = 0; i < uIVictoryPointsPlayerArray.Length; i++)
        {
            UIVictoryPointsPlayer uIVictoryPointsPlayer = uIVictoryPointsPlayerArray[i];
            uIVictoryPointsPlayer.SetText(victoryPointsSummaryList, i, highestVictoryPointsList);

            if (i >= victoryPointsSummaryList.Count - 1)
            {
                break;
            }
        }
    }

    private void GetPlayerListVPArray()
    {
        uIVictoryPointsPlayerArray = playerListVP.GetComponentsInChildren<UIVictoryPointsPlayer>();
        if (uIVictoryPointsPlayerArray.Length == 0) { Debug.Log("(uIVictoryPointsPlayerArray.Length == 0"); return; };
    }

    private void CalculateVictoryPoints(NewPlayer player)
    {
        // Reset
        int populationVP = 0;
        int religionVP = 0;
        int militaryVP = 0;
        int craftsmenVP = 0;
        int tradingVP = 0;
        int politicsVP = 0;
        int summeVP = 0;

        // Calculation
        foreach (ResourceSummaryTotalCounter resourceSummaryTotalCounter in player.ResourceSummaryTotalCounterList)
        {
            if (resourceSummaryTotalCounter.ResourceName == ResourceType.Food)
                populationVP = resourceSummaryTotalCounter.Amount / resourceAmountToGetAVictoryPoint;

            if (resourceSummaryTotalCounter.ResourceName == ResourceType.Stone)
                religionVP = resourceSummaryTotalCounter.Amount / resourceAmountToGetAVictoryPoint;

            if (resourceSummaryTotalCounter.ResourceName == ResourceType.Iron)
                militaryVP = resourceSummaryTotalCounter.Amount / resourceAmountToGetAVictoryPoint;

            if (resourceSummaryTotalCounter.ResourceName == ResourceType.Wood)
                craftsmenVP = resourceSummaryTotalCounter.Amount / resourceAmountToGetAVictoryPoint;

            if (resourceSummaryTotalCounter.ResourceName == ResourceType.Gold)
                tradingVP = resourceSummaryTotalCounter.Amount / resourceAmountToGetAVictoryPoint;

            if (resourceSummaryTotalCounter.ResourceName == ResourceType.Wool)
                politicsVP = resourceSummaryTotalCounter.Amount / resourceAmountToGetAVictoryPoint;
        }

        summeVP = populationVP + religionVP + militaryVP + craftsmenVP + tradingVP + politicsVP;

        // Add to list
        victoryPointsSummaryList.Add(new VictoryPointsSummary
        {
            playerName = player.name,
            craftsmenVPSummary = craftsmenVP,
            religionVPSummary = religionVP,
            populationVPSummary = populationVP,
            militaryVPSummary = militaryVP,
            politicsVPSummary = politicsVP,
            tradingVPSummary = tradingVP,
            totalVPSummary = summeVP,
        });
    }

    private void GetHighestVP()
    {
        // Sicherstellen, dass ein Eintrag existiert
        if (highestVictoryPointsList == null || highestVictoryPointsList.Count == 0)
        {
            highestVictoryPointsList = new List<HighestVictoryPoints>
        {
            new HighestVictoryPoints()
        };
        }

        var highscore = highestVictoryPointsList[0];

        // Erstmal alles auf 0 setzen
        highscore.craftsmenVPSummaryHS = 0;
        highscore.religionVPSummaryHS = 0;
        highscore.populationVPSummaryHS = 0;
        highscore.militaryVPSummaryHS = 0;
        highscore.politicsVPSummaryHS = 0;
        highscore.tradingVPSummaryHS = 0;
        highscore.totalVPSummaryHS = 0;

        // Max-Werte je Kategorie ermitteln
        int maxCraftsmen = victoryPointsSummaryList.Max(p => p.craftsmenVPSummary);
        int maxReligion = victoryPointsSummaryList.Max(p => p.religionVPSummary);
        int maxPopulation = victoryPointsSummaryList.Max(p => p.populationVPSummary);
        int maxMilitary = victoryPointsSummaryList.Max(p => p.militaryVPSummary);
        int maxPolitics = victoryPointsSummaryList.Max(p => p.politicsVPSummary);
        int maxTrading = victoryPointsSummaryList.Max(p => p.tradingVPSummary);
        int maxTotal = victoryPointsSummaryList.Max(p => p.totalVPSummary);

        // Anzahl der Vorkommen des Max-Werts zählen
        int craftsmenCount = victoryPointsSummaryList.Count(p => p.craftsmenVPSummary == maxCraftsmen);
        int religionCount = victoryPointsSummaryList.Count(p => p.religionVPSummary == maxReligion);
        int populationCount = victoryPointsSummaryList.Count(p => p.populationVPSummary == maxPopulation);
        int militaryCount = victoryPointsSummaryList.Count(p => p.militaryVPSummary == maxMilitary);
        int politicsCount = victoryPointsSummaryList.Count(p => p.politicsVPSummary == maxPolitics);
        int tradingCount = victoryPointsSummaryList.Count(p => p.tradingVPSummary == maxTrading);
        int totalCount = victoryPointsSummaryList.Count(p => p.totalVPSummary == maxTotal);

        // Nur eindeutige Max-Werte setzen
        foreach (var player in victoryPointsSummaryList)
        {
            if (player.craftsmenVPSummary == maxCraftsmen && craftsmenCount == 1)
                highscore.craftsmenVPSummaryHS = maxCraftsmen;

            if (player.religionVPSummary == maxReligion && religionCount == 1)
                highscore.religionVPSummaryHS = maxReligion;

            if (player.populationVPSummary == maxPopulation && populationCount == 1)
                highscore.populationVPSummaryHS = maxPopulation;

            if (player.militaryVPSummary == maxMilitary && militaryCount == 1)
                highscore.militaryVPSummaryHS = maxMilitary;

            if (player.politicsVPSummary == maxPolitics && politicsCount == 1)
                highscore.politicsVPSummaryHS = maxPolitics;

            if (player.tradingVPSummary == maxTrading && tradingCount == 1)
                highscore.tradingVPSummaryHS = maxTrading;

            if (player.totalVPSummary == maxTotal && totalCount == 1)
                highscore.totalVPSummaryHS = maxTotal;
        }

    }




    // Button Short
    public void SetVictoryPointsMainPlayer()
    {
        victoryPointShortcutToogle = !victoryPointShortcutToogle;

        if (victoryPointShortcutToogle)
        {
            SetVictoryPoints();
            OpenPanel();
        }
        else
            ClosePanel();
    }

    public void OpenPanel()
    {
        victoryPointShortcut.SetActive(true);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FillPanel(1f));
    }

    public void ClosePanel()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(FillPanel(0f));
    }

    private void SetMainPlayerShortText()
    {
        NewPlayer mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) { Debug.Log("mainPlayer == null"); }

        foreach (VictoryPointsSummary victoryPointsSummary in victoryPointsSummaryList)
        {
            if (victoryPointsSummary.playerName == mainPlayer.name)
            {
                populationImage.sprite = resourceSpriteEnty.GetVictorySprite(VictoryNames.Bevölkerung);
                grainImage.sprite = resourceSpriteEnty.GetResourceSprite(ResourceType.Food);
                int populationValue = highestVictoryPointsList[0].populationVPSummaryHS;
                if (populationValue > 0 && populationValue == victoryPointsSummary.populationVPSummary)
                    populationText.text = "<color=green>" + victoryPointsSummary.populationVPSummary.ToString() + "</color>";
                else
                    populationText.text = victoryPointsSummary.populationVPSummary.ToString();


                religionImage.sprite = resourceSpriteEnty.GetVictorySprite(VictoryNames.Religion);
                stoneImage.sprite = resourceSpriteEnty.GetResourceSprite(ResourceType.Stone);
                int religionValue = highestVictoryPointsList[0].religionVPSummaryHS;
                if (religionValue > 0 && religionValue == victoryPointsSummary.religionVPSummary)
                    religionText.text = "<color=green>" + victoryPointsSummary.religionVPSummary.ToString() + "</color>";
                else
                    religionText.text = victoryPointsSummary.religionVPSummary.ToString();

                militaryImage.sprite = resourceSpriteEnty.GetVictorySprite(VictoryNames.Militär);
                metalImage.sprite = resourceSpriteEnty.GetResourceSprite(ResourceType.Iron);
                int militaryValue = highestVictoryPointsList[0].militaryVPSummaryHS;
                if (religionValue > 0 && religionValue == victoryPointsSummary.militaryVPSummary)
                    militaryText.text = "<color=green>" + victoryPointsSummary.militaryVPSummary.ToString() + "</color>";
                else
                    militaryText.text = victoryPointsSummary.militaryVPSummary.ToString();

                craftsmenImage.sprite = resourceSpriteEnty.GetVictorySprite(VictoryNames.Handwerk);
                woodImage.sprite = resourceSpriteEnty.GetResourceSprite(ResourceType.Wood);
                int craftsmenValue = highestVictoryPointsList[0].craftsmenVPSummaryHS;
                if (craftsmenValue > 0 && craftsmenValue == victoryPointsSummary.craftsmenVPSummary)
                    craftsmenext.text = "<color=green>" + victoryPointsSummary.craftsmenVPSummary.ToString() + "</color>";
                else
                    craftsmenext.text = victoryPointsSummary.craftsmenVPSummary.ToString();

                tradingImage.sprite = resourceSpriteEnty.GetVictorySprite(VictoryNames.Handel);
                goldImage.sprite = resourceSpriteEnty.GetResourceSprite(ResourceType.Gold);
                int tradingValue = highestVictoryPointsList[0].tradingVPSummaryHS;
                if (tradingValue > 0 && tradingValue == victoryPointsSummary.tradingVPSummary)
                    tradingText.text = "<color=green>" + victoryPointsSummary.tradingVPSummary.ToString() + "</color>";
                else
                    tradingText.text = victoryPointsSummary.tradingVPSummary.ToString();

                politicsImage.sprite = resourceSpriteEnty.GetVictorySprite(VictoryNames.Politik);
                woolImage.sprite = resourceSpriteEnty.GetResourceSprite(ResourceType.Wool);
                int politicsValue = highestVictoryPointsList[0].politicsVPSummaryHS;
                if (politicsValue > 0 && politicsValue == victoryPointsSummary.politicsVPSummary)
                    politicsText.text = "<color=green>" + victoryPointsSummary.politicsVPSummary.ToString() + "</color>";
                else
                    politicsText.text = victoryPointsSummary.politicsVPSummary.ToString();

                int totalValue = highestVictoryPointsList[0].totalVPSummaryHS;
                if (totalValue > 0 && totalValue == victoryPointsSummary.totalVPSummary)
                    VPsummaryText.text = "<color=green>" + victoryPointsSummary.totalVPSummary.ToString() + "</color>";
                else
                    VPsummaryText.text = victoryPointsSummary.totalVPSummary.ToString();
            }
        }
    }

    public void SortByCraftsmenBtn()
    {
        SortHighestVictoryPointsList("craftsmenVPSummary");
    }

    public void SortByReligionBtn()
    {
        SortHighestVictoryPointsList("religionVPSummary");
    }

    public void SortByPopulationBtn()
    {
        SortHighestVictoryPointsList("populationVPSummary");
    }

    public void SortByMilitaryBtn()
    {
        SortHighestVictoryPointsList("militaryVPSummary");
    }

    public void SortByPoliticsBtn()
    {
        SortHighestVictoryPointsList("politicsVPSummary");
    }

    public void SortByTradingBtn()
    {
        SortHighestVictoryPointsList("tradingVPSummary");
    }

    public void SortByTotalVPBtn()
    {
        SortHighestVictoryPointsList("totalVPSummary");
    }

    private void SortHighestVictoryPointsList(string sortByProperty)
    {
        var type = typeof(VictoryPointsSummary);
        var property = type.GetProperty(sortByProperty);
        var field = type.GetField(sortByProperty);

        if (property != null)
        {
            victoryPointsSummaryList = victoryPointsSummaryList
                .OrderByDescending(p => (int)property.GetValue(p))
                .ToList();
        }
        else if (field != null)
        {
            victoryPointsSummaryList = victoryPointsSummaryList
                .OrderByDescending(p => (int)field.GetValue(p))
                .ToList();
        }
        else
        {
            Debug.LogError($"Property oder Field '{sortByProperty}' existiert nicht in VictoryPointsSummary.");
        }

        UISortingOrder(); // Update UI
    }

    private IEnumerator FillPanel(float target)
    {
        float start = victoryPointPanel.fillAmount;
        float duration = Mathf.Abs(target - start) / fillSpeed; // z. B. 1.0 Sekunden
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            victoryPointPanel.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }

        victoryPointPanel.fillAmount = target;

        if (target == 0f)
            victoryPointShortcut.SetActive(false);
    }

    // Buttons---------------------------------------------------------------------
    public List<Button> VPButtons; // Alle Buttons im Inspector zuweisen

    public string childImageName = "SortingImage"; // Name des Child-Images im Button

    private void ButtonStart()
    {
        // Erst alle Auswahl-Images ausblenden
        foreach (Button btn in VPButtons)
        {
            Transform child = btn.transform.Find(childImageName);
            if (child != null)
            {
                child.gameObject.SetActive(false);
            }

            btn.onClick.AddListener(() => OnButtonClicked(btn));
        }
    }

    void OnButtonClicked(Button clickedButton)
    {
        foreach (Button btn in VPButtons)
        {
            Transform child = btn.transform.Find(childImageName);
            if (child != null)
            {
                child.gameObject.SetActive(btn == clickedButton);
            }
        }
    }
    #endregion
}
