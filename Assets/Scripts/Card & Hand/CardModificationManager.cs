using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static CardProperties;

public class CardModificationManager : MonoBehaviour
{
    public static CardModificationManager Instance { get; set; }


    [SerializeField] private GameObject cardModificationGameObject;
    [SerializeField] private GameObject cardPlacePointGameObject;

    private CardInModificationGameobject cardInModificationGameobjectScript;

    private GameObject currentCardFromList; // echte Karte aus Kartensockel
    public GameObject socketCardClone; // Kopie der Karte (in UI)

    public Button cardLvlBtn;
    public Button cardEarningsBtn;
    public List<ResourceCosts> resourceCostsListLvlCardNormal = new List<ResourceCosts>(); // Zwischenspeicher Kosten für Karten aufleveln per Button

    public bool tooltipControllerCardLvlBtnOnEnter = false;
    public bool toolTipControllerIncreaseEarningsBtnOnEnter = false;

    public float moveSpeed = 3f;
    public float rotateSpeed = 2f;
    public Vector3 offset = new Vector3(0, -1000, 0); // Höhe und Entfernung relativ zum Ziel
    public float height = -1000f;




    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    // Start is called before the first frame update
    void Start()
    {
        cardInModificationGameobjectScript = cardModificationGameObject.GetComponentInChildren<CardInModificationGameobject>();
        if (cardInModificationGameobjectScript == null) { Debug.Log("cardInModificationGameobjectScript == null"); }

        cardModificationGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            //Debug.Log("V");

            //cardModificationGameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("B");

            EndCardModification();
        }

        // Buttons
        BtnActionCardLvl();
        BtnActionCardEarnings();
    }



    public void StartCardOverlayForModification(GameObject currentCardGameobject)
    {
        // UI anzeigen
        cardModificationGameObject.SetActive(true);

        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();
        if (activePlayer != null)
            currentCardFromList = activePlayer.GetCardFromList(currentCardGameobject);

        if (currentCardFromList == null) { Debug.Log("currentCardFromList == null"); }

        // alte Karte löschen 
        DestroyChilds();

        // neue Karte klonen und richtig setzen
        socketCardClone = Instantiate(currentCardFromList);
        socketCardClone.transform.SetParent(cardPlacePointGameObject.transform, false);
        socketCardClone.transform.localScale = Vector3.one;

        // Karte auf Position 0 setzen
        CardAnimationController cardAnimationControlleScript = socketCardClone.GetComponentInChildren<CardAnimationController>();
        cardAnimationControlleScript.gameObject.transform.localPosition = Vector3.zero;
        if (cardAnimationControlleScript.gameObject.transform.localPosition != new Vector3(0, 0, 0))
            cardAnimationControlleScript.gameObject.transform.localPosition = Vector3.zero;

        // Animationen verhindern (Collider deaktivieren)
        DeactivateAllCollider();

        // Kamera setzen
        CameraMovement.Instance.FokusPosition(currentCardFromList);
    }

    private void DeactivateAllCollider()
    {
        BoxCollider[] colliderToDeactivateArray = socketCardClone.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider boxCollider in colliderToDeactivateArray)
        {
            boxCollider.enabled = false;
        }
    }

    private void DestroyChilds()
    {
        foreach (var child in cardPlacePointGameObject.transform)
        {
            Destroy(((Transform)child).gameObject);
        }
    }

    public void EndCardModification()
    {
        cardModificationGameObject.SetActive(false);

        CameraMovement.Instance.SetLastRotation();
        CameraMovement.Instance.startFocus = false;
    }

    public void BtnActionCardLvl()
    {
        if (cardModificationGameObject.activeSelf)
        {
            if (CostHandler.Instance.FirstCheckOfCostsResourceListLvlCardNormal(socketCardClone)) // NEU
                cardLvlBtn.interactable = true;
            else
                cardLvlBtn.interactable = false;
        }
    }

    public void CardLvln()
    {
        // Kosten abrechnen
        //CostHandler.Instance.payCosts(currentCardFromList.GetComponent<CardProperties>(), currentCardFromList.GetComponent<CardProperties>().resourceCostsListLvlCardNormal);
        // gesockelte Karte wir aufgewertet
        CardProperties cardPropertiesCurrentCardFromList = currentCardFromList.gameObject.GetComponent<CardProperties>();
        if (cardPropertiesCurrentCardFromList == null) { Debug.Log("cardPropertiesCurrentCardFromList == null"); }
        cardPropertiesCurrentCardFromList.CardLvln(cardPropertiesCurrentCardFromList);
        // Werte auf Hexfeld aktualisieren
        Hex resourcesOnHex = currentCardFromList.gameObject.GetComponentInParent<Hex>();
        if (resourcesOnHex == null) { Debug.Log("resourcesOnHex == null"); }
        GameUtils.Instance.UpdateResourceOnHex(resourcesOnHex.transform);
        // UI Summary aktualisieren
        UIRefresh();
    }

    public void IncreaseEarnings()
    {
        // Kosten abrechnen
        CostHandler.Instance.PayCostsForIncreasingEarnings(currentCardFromList); // NEU
        // gesockelte Karte wird aufgewertet
        CardProperties cardPropertiesCurrentCardFromList = currentCardFromList.GetComponent<CardProperties>();
        if (cardPropertiesCurrentCardFromList == null) { Debug.Log("cardPropertiesCurrentCardFromList == null"); }
        cardPropertiesCurrentCardFromList.CardIncreasingEarnings(cardPropertiesCurrentCardFromList);
        // Werte auf Hexfeld aktualisieren
        Hex resourcesOnHex = currentCardFromList.GetComponentInParent<Hex>();
        if (resourcesOnHex == null) { Debug.Log("resourcesOnHex == null"); }
        GameUtils.Instance.UpdateResourceOnHex(resourcesOnHex.transform);
        // UI Summary aktualisieren
        UIRefresh();
    }

    public void BtnActionCardEarnings()
    {
        if (cardModificationGameObject.activeSelf)
        {
            if (CostHandler.Instance.FirstCheckXPCostsOnIncreasingEarnings(socketCardClone))
                cardEarningsBtn.interactable = true;
            else
                cardEarningsBtn.interactable = false;
        }
    }

    private void UIRefresh()
    {
        //Overlay mit Karte aktualisieren
        StartCardOverlayForModification(currentCardFromList);
        // UI Summary aktualisieren
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText();
        // Hexfeld aktualisieren
        Hex resourcesOnHex = currentCardFromList.gameObject.GetComponentInParent<Hex>();
        if (resourcesOnHex == null) { Debug.Log("resourcesOnHex == null"); }
        // Detailfenster aktualisieren
        DetailWindowManager.Instance.UpdateSockets(resourcesOnHex.gameObject.transform);
        // UI Currenthex im Infoscreen anzeigen
        DetailWindowManager.Instance.UpdateDetailsHex(resourcesOnHex.gameObject.transform);
    }
}
