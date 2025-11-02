using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ───────────────────────────── SINGLETON & INITIALISIERUNG ─────────────────────────────
public class CostContentCheck : MonoBehaviour
{
    public static CostContentCheck Instance { get; private set; }

    private Catch caCa;
    public GameObject myEventHandlerGameObject;
    private MyEventHandler handler;

    public Vector3 standardPosition = new Vector3(-920, -300, 0);
    public Vector3 cardOverlayPosition = new Vector3(-920, -300, 0);

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        handler = myEventHandlerGameObject.GetComponent<MyEventHandler>();
        if (handler == null) { Debug.Log("(myEventHandlerScript == null"); }
        transform.localPosition = standardPosition;
    }

// ───────────────────────────── MAUS-INTERAKTION & KOSTEN-UI ─────────────────────────────
    public void MouseWithCardOverObject(Transform activeHex)
    {
        caCa = new Catch(activeHex);

        // 1. Über UI und ein Socket ist ausgewählt
        if (GameUtils.Instance.CheckUI() && caCa.CurrentSocket != null)
        {
            HandleSocketOverUI();
            return;
        }

        // 2. Alles auf HEX, NICHT auf UI
        if (CheckForDisplayOff())
            return;

        // 2a. Maus zeigt auf Hexfeld (kein Socket)
        if (caCa.CurrentSocket == null)
        {
            HandleHexField();
            return;
        }

        // 2b. Maus zeigt auf freien Socket
        var socketComp = caCa.CurrentSocket.GetComponent<Socket>();
        if (socketComp != null && !socketComp.isBlocked)
        {
            HandleFreeSocket();
            return;
        }

        // 2c. Maus zeigt auf belegten Socket (Karte)
        if (socketComp != null && socketComp.isBlocked)
        {
            CheckCardOnCardLvl(caCa.CurrentCard);
            handler.cardReleaseIntoSocket = false;
        }
    }

    public void MouseWithCardOverObjectExit()
    {
        CardActionDetailsOnScreen.Instance.NoCostText();
        caCa = null;
    }

// ───────────────────────────── KOSTEN- UND SOCKET-PRÜFUNGEN ─────────────────────────────
    private void CheckCardOnCardLvl(GameObject currentCardGameobject)
    {
        var cardInHand = handler.chosenCard?.GetComponent<CardProperties>();
        var cardInSocket = currentCardGameobject?.GetComponent<CardProperties>();

        if (cardInHand == null || cardInSocket == null)
        {
            Debug.Log("CardProperties nicht gefunden!");
            return;
        }

        if (cardInHand.GetCardRarity() == cardInSocket.GetCardRarity() &&
            cardInHand.GetBuildingType() == cardInSocket.GetBuildingType())
        {
            cardInSocket.SetBasicCostResources();

            if (CostHandler.Instance.FirstCheckOfCostsResourceListLvlCardOnCard())
            {
                handler.cardReleaseCardOnCardLvl = true;
                CardActionDetailsOnScreen.Instance.ShowAddCardLvl(currentCardGameobject);
            }
            else
            {
                handler.cardReleaseCardOnCardLvl = false;
                CardActionDetailsOnScreen.Instance.ShowAddCardLvlCostsToHeigh(currentCardGameobject);
            }
        }
        else
        {
            handler.cardReleaseCardOnCardLvl = false;
            CardActionDetailsOnScreen.Instance.ShowNoAddCardLvl();
        }
    }

    private bool CheckForDisplayOff()
    {
        NewPlayer activPlayer = TurnManager.Instance.GetCurrentPlayer();
        if (CheckHexfieldWithoutSockets(caCa.CurrentHex.gameObject) && activPlayer != null &&
            !activPlayer.CheckPlayerHexFieldSummaryList(caCa.CurrentHex.gameObject))
        {
            MouseWithCardOverObjectExit();
            return true;
        }
        return false;
    }

    private bool CheckOccupationType()
    {
        Hex res = caCa.CurrentHex.gameObject.GetComponent<Hex>();
        return res != null && res.occupantType == OccupType.Settlement;
    }

    private bool CheckHexfieldResource()
    {
        var hexGO = caCa.CurrentHex?.gameObject;
        var player = TurnManager.Instance.GetCurrentPlayer();
        if (player == null || !player.PlayerHexFieldSummaryList.Contains(hexGO))
        {
            Debug.Log("Falscher Spieler");
            return false;
        }

        var res = hexGO.GetComponent<Hex>();
        if (res == null || res.occupantType != OccupType.Settlement)
        {
            Debug.Log("kein Resourcenfeld");
            return false;
        }

        var cardProps = handler.chosenCard?.GetComponent<CardProperties>();
        CostHandler.Instance.socketListMouseOver.Clear();
        var slots = GetCardSlotsOnHexfield(hexGO);
        CostHandler.Instance.socketListMouseOver = slots;

        if (slots.Count == 0)
        {
            Debug.Log("kein freier kompatibler Sockel");
            return false;
        }
        return true;
    }

    private List<Socket> GetCardSlotsOnHexfield(GameObject hexGO)
    {
        CostHandler.Instance.socketListMouseOver.Clear();
        Socket freeSocket = GameUtils.Instance.FindFreeFittingSocket(hexGO.transform, handler.chosenCard.gameObject);
        if (freeSocket != null)
        {
            CostHandler.Instance.socketListMouseOver.Add(freeSocket);
            return new List<Socket> { freeSocket };
        }
        Debug.Log("Kein freier kompatibler Sockel auf Hexfield");
        return new List<Socket>();
    }

    private bool CheckSocket()
    {
        CostHandler.Instance.socketListMouseOver.Clear();
        Socket socket = caCa.CurrentSocket.GetComponentInChildren<Socket>();
        if (CheckCardPlacepoint(socket) != null)
        {
            CostHandler.Instance.socketListMouseOver.Add(socket);
            return true;
        }
        return false;
    }

    private bool CheckHexfieldWithoutSockets(GameObject hexGO)
    {
        return !hexGO
            .GetComponentsInChildren<Socket>()
            .Any(p => p.isActiveAndEnabled);
    }

    private Socket CheckCardPlacepoint(Socket socket)
    {
        var cardProps = handler.chosenCard?.GetComponent<CardProperties>();
        if (socket != null && cardProps != null)
            if (cardProps.rarity == socket.socketType && !socket.isBlocked)
                return socket;
        return null;
    }

// ───────────────────────────── HILFSMETHODEN ─────────────────────────────

private void HandleSocketOverUI()
{
    if (CheckSocket() && CostHandler.Instance.FirstCheckOfCostsResourceListNormal()) // NEU
    {
        handler.cardReleaseIntoSocket = true;
        handler.cardReleaseCardOnCardLvl = false;
        CardActionDetailsOnScreen.Instance.ShowAddCardInSocket(handler.chosenCard.gameObject);
    }
    else
    {
        handler.cardReleaseIntoSocket = false;
        handler.cardReleaseCardOnCardLvl = false;
        CardActionDetailsOnScreen.Instance.ShowNotSuitableSocket();
    }
}

private void HandleHexField()
{
    bool hasResource = CheckHexfieldResource();
    bool hasCosts = CostHandler.Instance.FirstCheckOfCostsResourceListNormal(); // NEU

    if (hasResource && hasCosts)
    {
        handler.cardReleaseIntoSocket = true;
        handler.cardReleaseCardOnCardLvl = false;
        CardActionDetailsOnScreen.Instance.ShowAddCardInSocketHex(handler.chosenCard.gameObject);
    }
    else if (!CheckOccupationType())
    {
        MouseWithCardOverObjectExit();
        handler.cardReleaseIntoSocket = false;
        handler.cardReleaseCardOnCardLvl = false;
    }
    else if (!hasResource)
    {
        handler.cardReleaseIntoSocket = false;
        handler.cardReleaseCardOnCardLvl = false;
        CardActionDetailsOnScreen.Instance.ShowNoEmptySocket();
    }
    else if (!hasCosts)
    {
        handler.cardReleaseIntoSocket = false;
        handler.cardReleaseCardOnCardLvl = false;
        CardActionDetailsOnScreen.Instance.ShowNoAddCardCostsToHeight(handler.chosenCard.gameObject);
    }
}

private void HandleFreeSocket()
{
    if (CheckSocket() && CostHandler.Instance.FirstCheckOfCostsResourceListNormal()) // NEU
    {
        handler.cardReleaseIntoSocket = true;
        handler.cardReleaseCardOnCardLvl = false;
        CardActionDetailsOnScreen.Instance.ShowAddCardInSocket(handler.chosenCard.gameObject);
    }
    else
    {
        handler.cardReleaseIntoSocket = false;
        handler.cardReleaseCardOnCardLvl = false;
        CardActionDetailsOnScreen.Instance.ShowNotSuitableSocket();
    }
}
}
