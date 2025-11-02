using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

// ───────────────────────────── FELDER & INSPECTOR ─────────────────────────────
public class DetailWindowManager : MonoBehaviour
{
    public static DetailWindowManager Instance { get; private set; }

    private Vector3 startPos;
    private Vector3 targetPos;
    private const int moveSpeed = 1000;
    public Transform ReferenceHex;

    [Header("Socket UI")]
    public Sprite socketSprite;
    public float spriteScale = 0.07f;
    public float hoverOffset = 20f;

    public List<GameObject> DetailWindows = new();
    public GameObject SettlDetails;
    
    public Vector3 iterationOffset;
    public GameObject VictoryDetails;
    public List<TextMeshProUGUI> VictoryTextFields;
    public List<Image> VictoryBars;

    public float offsetX;
    public float offsetY;
    public Vector3 offsetVector = new(0, -20, -15);
    public Vector3 localScaleVector = new(0.52f, 0.52f, 0.52f);
    public Vector3 localRotationVector = new(0, 90, 0);
    public float scaleMultiplier;

    [Header("Detailswindow Elemente")]
    public GameObject resMoni;

    [Header("UI References")]
    [SerializeField] private Button upgradeButtonPrefab;
    [SerializeField] private RectTransform buttonsParent;

    [Header("Layout")]
    [SerializeField] private float horizontalSpacing = 50f;

    private readonly List<Button> spawnedUpgradeButtons = new();

    [Header("RoadDetails UI")]
    [SerializeField] private TextMeshProUGUI roadTitleText; // ← Im Inspector zuweisen!
    [SerializeField] private Image roadCoverImage;              // Cover-Image

    [Header("ResourceDetails UI")]
    [SerializeField] private GameObject resourceDetailsWindow; // Das UI-Panel
    [SerializeField] private TextMeshProUGUI resourceTitleText;    // Titel-Feld
    [SerializeField] private Image resourceCoverImage;              // Cover-Image
    [SerializeField] private GameObject resourceDetails;

    // ───────────────────────────── UNITY LIFECYCLE ─────────────────────────────
    private void Awake()
    {
        startPos = transform.localPosition;
        targetPos = startPos + new Vector3(-375, 0, 0);

        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    // ───────────────────────────── DETAIL WINDOW SETUP ─────────────────────────────
    public void SetUpDetailWindow(string winType, Catch caCa)
    {
        ReferenceHex = caCa.CurrentHex;
        var hex = caCa.CurrentHex != null ? caCa.CurrentHex.GetComponent<Hex>() : null;

        switch (winType)
        {
            case "SettlementDetails":
                ShowSettlementDetails(caCa.CurrentHex);
                break;
            case "CardDetails":
                SelectUI(winType);
                UpdateDetailsCard(caCa.CurrentCard, caCa.CurrentHex);
                StartCoroutine(ShowWindow());
                break;
            case "HexDetails":
                if (hex.occupantType == OccupType.Settlement)
                {
                    SelectUI("SettlementDetails");
                    UpdateSockets(hex.transform);
                    UpdateDetailsHex(hex.transform);
                    SetUpSettlementDetails(hex.transform);
                    StartCoroutine(ShowWindow());
                }
                else if (hex.occupantType == OccupType.Road)
                {
                    SelectUI("RoadDetails");
                    UpdateDetailsRoad(hex);
                    StartCoroutine(ShowWindow());
                }
                else if (hex.occupantType == OccupType.Resource)
                {
                    SelectUI("ResourceDetails");
                    UpdateDetailsResourceTile(hex);
                    SetUpResourceDetails(hex.transform);
                    StartCoroutine(ShowWindow());
                }
                else
                {
                    SelectUI(winType);
                    UpdateSockets(caCa.CurrentHex);
                    UpdateDetailsHex(caCa.CurrentHex);
                    StartCoroutine(ShowWindow());
                }
                break;
            case "RoadDetails":
                SelectUI("RoadDetails");
                UpdateDetailsRoad(hex);
                StartCoroutine(ShowWindow());
                break;
            case "SocketDetails":
                SelectUI(winType);
                UpdateSockets(caCa.CurrentHex);
                UpdateDetailsHex(caCa.CurrentHex);
                StartCoroutine(ShowWindow());
                break;
            default:
                Debug.LogWarning("Unbekannter winType: " + winType);
                break;
        }
    }

    private void ShowSettlementDetails(Transform hexTrans)
    {
        SelectUI("SettlementDetails");
        UpdateSockets(hexTrans);
        UpdateDetailsHex(hexTrans);
        SetUpSettlementDetails(hexTrans);
        StartCoroutine(ShowWindow());
    }

    // ───────────────────────────── SETTLEMENT DETAILS & VICTORY ─────────────────────────────
    private void SetUpSettlementDetails(Transform hexTrans)
    {
        VictoryDetails.SetActive(false);
        Hex hex = hexTrans.GetComponent<Hex>();
        if (hex == null) return;

        foreach (Transform child in SettlDetails.transform)
            Destroy(child.gameObject);

        DisableLayoutGroups(SettlDetails);

        Vector2 start = new(-80f, 5f);
        Vector2 step = new(0f, -50f);
        const float zDepth = -0.6f;
        const float width = 260f;
        const float height = 50f;

        int index = 0;
        foreach (var hexe in hex.resTilesOnSettlement)
        {
            GameObject uiGO = Instantiate(resMoni, SettlDetails.transform);
            var rt = uiGO.GetComponent<RectTransform>();

            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            rt.anchoredPosition = start + step * index;
            var lp = rt.localPosition;
            lp.z = zDepth;
            rt.localPosition = lp;

            uiGO.GetComponent<ResMoni>().SetUpResMoni(hexe);
            index++;
        }
    }

    private void DisableLayoutGroups(GameObject go)
    {
        if (go.TryGetComponent(out VerticalLayoutGroup vlg)) vlg.enabled = false;
        if (go.TryGetComponent(out HorizontalLayoutGroup hlg)) hlg.enabled = false;
        if (go.TryGetComponent(out GridLayoutGroup glg)) glg.enabled = false;
        if (go.TryGetComponent(out ContentSizeFitter csf)) csf.enabled = false;
    }

    private void SetUpSettlementVictoryPoints(Transform hexTrans)
    {
        VictoryVector victoryVector = new VictoryVector(1f, 2f, 5f, 4f, 0f, 2f);

        float topAtZero = 143f;
        float bottomAtZero = -46.83f;
        float pxPerUnit = 15f;

        for (int i = 0; i < 6; i++)
        {
            float v = victoryVector[i];
            VictoryTextFields[i].text = v.ToString("0.##");

            RectTransform rt = VictoryBars[i].rectTransform;
            var offMin = rt.offsetMin;
            offMin.y = bottomAtZero;
            rt.offsetMin = offMin;

            var offMax = rt.offsetMax;
            offMax.y = (topAtZero - pxPerUnit * v) * -1;
            rt.offsetMax = offMax;
        }
    }

    public void SwitchToVictoryPoints()
    {
        foreach (Transform child in SettlDetails.transform)
            Destroy(child.gameObject);
        VictoryDetails.SetActive(true);
        SetUpSettlementVictoryPoints(ReferenceHex);
    }

    public void SwitchToResources()
    {
        SetUpSettlementDetails(ReferenceHex);
    }

    // ───────────────────────────── HEX & ROAD UI ACTIONS ─────────────────────────────
    public void HexUIFoundVillage()
    {
        StructureManager.Instance.FoundSettlement(ReferenceHex);
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText();
        ShowSettlementDetails(ReferenceHex);    
        GameUtils.Instance.ResetToDefault();
    }

    public void HexUIBuildRoad()
    {
        StructureManager.Instance.CreateRoadOnHex(ReferenceHex);
        SelectUI("RoadDetails");
        UpdateDetailsRoad(ReferenceHex.GetComponent<Hex>());
        StartCoroutine(ShowWindow());
        GameUtils.Instance.ResetToDefault();
    }

    public void RoadUIUpGradeRoad(Road road)
    {
        StructureManager.Instance.UpgradeRoad(ReferenceHex.GetComponent<Hex>(), RoadType.Base);
        UpdateDetailsRoad(ReferenceHex.GetComponent<Hex>());
        Debug.Log("Base111");
    }

    public void RoadUIUpgradeAvenue()      => UpgradeRoadAndUpdate(RoadType.Avenue, "Avenue222");
    public void RoadUIUpgradeInn()         => UpgradeRoadAndUpdate(RoadType.Inn, "Inn333");
    public void RoadUIUpgradeIntersection()=> UpgradeRoadAndUpdate(RoadType.Intersection, "Intersection444");
    public void RoadUIUpgradeReinforced()  => UpgradeRoadAndUpdate(RoadType.Reinforced, "Reinforced555");

    private void UpgradeRoadAndUpdate(RoadType type, string debugMsg)
    {
        StructureManager.Instance.UpgradeRoad(ReferenceHex.GetComponent<Hex>(), type);
        UpdateDetailsRoad(ReferenceHex.GetComponent<Hex>());
        Debug.Log(debugMsg);
    }

    // ───────────────────────────── UI ANIMATION & SELECTION ─────────────────────────────
    private IEnumerator ShowWindow()
    {
        while (Vector3.Distance(transform.localPosition, targetPos) > .0001f)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPos, step);
            yield return null;
        }
        transform.localPosition = targetPos;
    }

    private void SelectUI(string winType)
    {
        foreach (GameObject window in DetailWindows)
            if (window != null)
                window.SetActive(window.name == winType);
    }

    public void Hide()
    {
        transform.localPosition = startPos;
    }

    // ───────────────────────────── SOCKETS & SLOTS ─────────────────────────────
    public void UpdateSockets(Transform hexTrans)
    {
        MyEventHandler.Instance.chosenSocketUI = null;
        ReferenceHex = hexTrans;
        ResetAllSockets();

        Transform socketHolderTransform = hexTrans?.gameObject?.transform.Find("SocketHolder");
        if (socketHolderTransform == null) return;

        var rarityColors = new Dictionary<Rarity, string>
        {
            { Rarity.Common,    "#BABABA" },
            { Rarity.Uncommon,  "#25AE37" },
            { Rarity.Rare,      "#0003FF" },
            { Rarity.Epic,      "#7A00FF" },
            { Rarity.Legendary, "#FFC700" }
        };

        var counters = new Dictionary<Rarity, int>
        {
            { Rarity.Common,    1 },
            { Rarity.Uncommon,  1 },
            { Rarity.Rare,      1 },
            { Rarity.Epic,      1 },
            { Rarity.Legendary, 1 }
        };

        foreach (Transform t in socketHolderTransform)
        {
            Socket socket = t.GetComponent<Socket>();
            if (socket == null || socket.socketType == Rarity.None) continue;

            Rarity r = socket.socketType;
            int index = counters[r];
            string childName = $"{r}{index}";

            Transform slot = FindDeepChild(transform, childName);

            if (slot != null && slot.TryGetComponent(out Image img))
            {
                if (rarityColors.TryGetValue(r, out string hexCode) &&
                    ColorUtility.TryParseHtmlString(hexCode, out Color c))
                {
                    img.color = c;
                    slot.GetComponent<UISocket>().referenceSocket = t.gameObject;
                }
            }
            counters[r] = index + 1;

            if (socket.isBlocked)
            {
                Vector3 worldPos = slot.position + slot.TransformVector(offsetVector);
                GameObject copy = Instantiate(socket.structure, worldPos, slot.rotation);
                copy.transform.SetParent(slot, worldPositionStays: false);
                copy.transform.localPosition = offsetVector;
                copy.transform.localScale = localScaleVector;
                copy.transform.localRotation = Quaternion.Euler(localRotationVector);
                copy.name = $"{socket.structure.name}_Copy";
            }
        }
    }

    private void ResetAllSockets()
    {
        Rarity[] rarities = { Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic, Rarity.Legendary };
        List<string> childNames = new();
        foreach (var r in rarities)
            for (int i = 1; i <= 3; i++)
                childNames.Add($"{r}{i}");

        const string resetHex = "#383838";
        if (!ColorUtility.TryParseHtmlString(resetHex, out Color resetColor))
        {
            Debug.LogError($"Ungültiger Hex-Code: {resetHex}");
            resetColor = Color.white;
        }

        foreach (string childName in childNames)
        {
            Transform slot = FindDeepChild(transform, childName);
            if (slot == null)
            {
                Debug.LogWarning($"Slot \"{childName}\" nicht gefunden.");
                continue;
            }

            foreach (var child in slot)
                Destroy(((Transform)child).gameObject);

            if (slot.TryGetComponent<Image>(out Image img))
                img.color = new Color(resetColor.r, resetColor.g, resetColor.b, 27f / 255f);
            else
                Debug.LogWarning($"Image-Komponente nicht gefunden in \"{childName}\".");
        }
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }

    // ───────────────────────────── DETAIL UPDATES ─────────────────────────────
    public void UpdateDetailsHex(Transform hex)
    {
        //DetailWindowScript.Instance.DisplayClickedHex(hex);
    }

    private void UpdateDetailsCard(GameObject cardGO, Transform hex)
    {
        //DetailWindowScript.Instance.ClickedOnCard(cardGO.transform, hex);
    }

    private void UpdateDetailsRoad(Hex hex)
    {
        if (hex == null || hex.structure == null) return;

        Road hexRoad = hex.structure.GetComponent<Road>();
        if (hexRoad == null || hexRoad.upgradeList == null || hexRoad.upgradeList.Count == 0)
            return;

        // RoadType im Titel anzeigen
        if (roadTitleText != null)
            roadTitleText.text = hexRoad.type.ToString();

        if (roadCoverImage != null)
            roadCoverImage.sprite = hexRoad.uiSprite;

        ClearUpgradeButtons();

        int count = hexRoad.upgradeList.Count;
        Vector3 center = new(-102f, 314f, -0.5f);
        float totalWidth = (count - 1) * horizontalSpacing;
        float startX = center.x - totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            Road roadUpgrade = hexRoad.upgradeList[i];
            Button btn = Instantiate(upgradeButtonPrefab, buttonsParent);
            spawnedUpgradeButtons.Add(btn);

            var img = btn.GetComponent<Image>();
            if (img != null && roadUpgrade != null && roadUpgrade.icon != null)
            {
                img.sprite = roadUpgrade.icon;
                img.preserveAspect = true;
            }
            else
            {
                var childImg = btn.GetComponentInChildren<Image>();
                if (childImg != null && roadUpgrade != null && roadUpgrade.icon != null)
                {
                    childImg.sprite = roadUpgrade.icon;
                    childImg.preserveAspect = true;
                }
            }

            RectTransform rt = btn.GetComponent<RectTransform>();
            float x = startX + i * horizontalSpacing;
            rt.anchoredPosition = new Vector2(x, center.y);
            Vector3 lp = rt.localPosition;
            lp.z = center.z;
            rt.localPosition = lp;

            Road capturedUpgrade = roadUpgrade;
            btn.onClick.RemoveAllListeners();
            switch (capturedUpgrade.type)
            {
                case RoadType.Base:
                    btn.onClick.AddListener(() => this.RoadUIUpGradeRoad(capturedUpgrade));
                    break;
                case RoadType.Avenue:
                    btn.onClick.AddListener(this.RoadUIUpgradeAvenue);
                    break;
                case RoadType.Inn:
                    btn.onClick.AddListener(this.RoadUIUpgradeInn);
                    break;
                case RoadType.Intersection:
                    btn.onClick.AddListener(this.RoadUIUpgradeIntersection);
                    break;
                case RoadType.Reinforced:
                    btn.onClick.AddListener(this.RoadUIUpgradeReinforced);
                    break;
                default:
                    btn.onClick.AddListener(() => this.RoadUIUpGradeRoad(capturedUpgrade));
                    break;
            }
        }
    }

    private void ClearUpgradeButtons()
    {
        foreach (var b in spawnedUpgradeButtons)
            if (b) Destroy(b.gameObject);
        spawnedUpgradeButtons.Clear();
    }

    private void UpdateDetailsResourceTile(Hex hex)
    {
        if (hex == null || hex.structure == null) return;
        var resTile = hex.structure.GetComponent<Resource>();
        if (resTile == null) return;

        if (resourceTitleText != null)
            resourceTitleText.text = resTile.resourceType.ToString();

        if (resourceCoverImage != null)
            resourceCoverImage.sprite = resTile.uiSprite;
    }

    private void SetUpResourceDetails(Transform hexTrans)
    {
        GameObject hex = hexTrans.gameObject;

        foreach (Transform child in resourceDetails.transform)
            Destroy(child.gameObject);

        // UI-Element erzeugen
        GameObject uiGO = Instantiate(resMoni, resourceDetails.transform);
        var rt = uiGO.GetComponent<RectTransform>();

        // Layout/Positionierung
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 260f);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50f);
        rt.anchoredPosition = new Vector2(-80f, 5f);
        var lp = rt.localPosition;
        lp.z = -0.6f;
        rt.localPosition = lp;

        
        if (hex != null)
            uiGO.GetComponent<ResMoni>().SetUpResMoni(hex);
    }
}