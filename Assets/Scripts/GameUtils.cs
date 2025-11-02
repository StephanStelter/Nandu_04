using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ───────────────────────────── SINGLETON & INITIALISIERUNG ─────────────────────────────
public class GameUtils : MonoBehaviour
{
    public static GameUtils Instance { get; private set; }
    public Transform tileHolder;
    public GameObject eventHandler;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

// ───────────────────────────── KOLLIDER & AUSWAHL ─────────────────────────────
    public Transform GetClosestObj()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mouseWorldPos = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.origin + ray.direction * 100f;

        Transform closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform child in tileHolder)
        {
            float distance = (mouseWorldPos - child.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = child;
            }
        }
        return closestObject;
    }

    public Transform GetTransform()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue);
        return raycastHit.transform;
    }

    public RaycastHit GetRaycastHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue);
        return raycastHit;
    }

    public List<Hex> GetAreaHex(Vector3 position, float range)
    {
        List<Hex> foundHex = new();
        Vector3 point1 = position + Vector3.down * 200f;
        Vector3 point2 = position + Vector3.up * 200f;
        Collider[] colliders = Physics.OverlapCapsule(point1, point2, range);

        foreach (Collider collider in colliders)
        {
            Hex hex = collider.transform.parent.GetComponent<Hex>();
            if (hex != null && !foundHex.Contains(hex))
                foundHex.Add(hex);
        }
        return foundHex;
    }

// ───────────────────────────── SOCKETS & FINDER ─────────────────────────────
    public void HighlightSocketUnderMouse(Transform activeHex)
    {
        Hex roh = activeHex.GetComponent<Hex>();
        if (roh == null || roh.occupantType != OccupType.Settlement) return;

        Transform socketHolder = activeHex.Find("SocketHolder");
        if (socketHolder == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, 10000f)) return;
        Vector3 hitPoint = hitInfo.point;

        Socket[] sockets = socketHolder.GetComponentsInChildren<Socket>();
        Transform closestSocket = null;
        float closestDistSqr = float.PositiveInfinity;

        foreach (var sock in sockets)
        {
            float d = (sock.transform.position - hitPoint).sqrMagnitude;
            if (d < closestDistSqr)
            {
                closestDistSqr = d;
                closestSocket = sock.transform;
            }
        }

        foreach (var sock in sockets)
            sock.ResetHighlight();

        if (closestSocket != null)
        {
            Socket socketComp = closestSocket.GetComponent<Socket>();
            if (socketComp != null)
                socketComp.HighlightSocket();
        }
    }

    public List<Socket> GetFreeSocketsOnHex(Transform hex)
    {
        var socketHolder = hex.Find("SocketHolder");
        if (socketHolder == null) return new List<Socket>();

        return socketHolder
            .Cast<Transform>()
            .Select(t => t.GetComponent<Socket>())
            .Where(s => s != null && !s.isBlocked)
            .ToList();
    }

    public List<Socket> GetAllSocketsOnHex(Transform hex)
    {
        return hex.GetComponentsInChildren<Socket>().ToList();
    }

    public GameObject FindTwinByName(GameObject original)
    {
        foreach (Transform child in tileHolder)
        {
            if (child.name == original.name)
                return child.gameObject;
        }
        Debug.LogWarning($"Kein Zwilling mit dem Namen '{original.name}' in '{tileHolder.name}' gefunden.");
        return null;
    }

    public Socket FindSocketInHex(Transform hex, Socket socket)
    {
        foreach (Transform child in hex)
        {
            if (!child.gameObject.activeInHierarchy)
                continue;
            if (child.name == socket.name)
                return child.GetComponent<Socket>();

            Socket found = FindSocketInHex(child, socket);
            if (found != null)
                return found;
        }
        return null;
    }

    public Socket FindFreeFittingSocket(Transform hex, GameObject card)
    {
        Transform socketHolderTransform = hex.Find("SocketHolder");
        if (socketHolderTransform == null) return null;
        var cardRarity = card.GetComponent<CardProperties>().rarity;

        var validRarity = cardRarity switch
        {
            Rarity.None => new List<Rarity>(),
            Rarity.Common => new() { Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic, Rarity.Legendary },
            Rarity.Uncommon => new() { Rarity.Uncommon, Rarity.Rare, Rarity.Epic, Rarity.Legendary },
            Rarity.Rare => new() { Rarity.Rare, Rarity.Epic, Rarity.Legendary },
            Rarity.Epic => new() { Rarity.Epic, Rarity.Legendary },
            Rarity.Legendary => new() { Rarity.Legendary },
            _ => new List<Rarity>()
        };

        var matches = new List<Socket>();
        foreach (Transform t in socketHolderTransform)
        {
            Socket socket = t.GetComponent<Socket>();
            if (socket != null && !socket.isBlocked && validRarity.Contains(socket.socketType))
                matches.Add(socket);
        }
        return matches.OrderBy(s => s.socketType).FirstOrDefault();
    }

// ───────────────────────────── UI & HIGHLIGHTING ─────────────────────────────
    public bool CheckUI()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            string hitName = hit.collider.gameObject.name;
            if (hitName == "BackGroundBottom" || hitName == "BackGoundTop")
                return true;
        }
        return false;
    }

    public void HighlightActiveHex(Color color, Transform activeHex)
    {
        ResetHighlighting();
        var handler = eventHandler.GetComponent<MyEventHandler>();
        List<Hex> highlightedHex = handler.highlightedHex;

        Hex hex = activeHex.GetComponent<Hex>();
        if (hex != null)
        {
            hex.SetHighlight(color);
            highlightedHex.Add(hex);
        }
        else
        {
            Debug.LogWarning("Kein hex-Component gefunden!");
        }
    }

    public void HighlightArea(Vector3 position, int range, Color centerColor, Color surroundingColor, Transform activeHex)
    {
        ResetHighlighting();
        var handler = eventHandler.GetComponent<MyEventHandler>();
        List<Hex> highlightedHex = handler.highlightedHex;

        Collider[] colliders = Physics.OverlapSphere(position, range);
        foreach (Collider collider in colliders)
        {
            Hex hex = collider.transform.parent.GetComponent<Hex>();
            if (hex != null)
            {
                if (hex.transform.position == activeHex.transform.position)
                    hex.SetHighlight(centerColor);
                else
                    hex.SetHighlight(surroundingColor);
                highlightedHex.Add(hex);
            }
        }
    }

    public void ResetHighlighting()
    {
        var handler = eventHandler.GetComponent<MyEventHandler>();
        List<Hex> highlightedHex = handler.highlightedHex;

        foreach (Hex hex in highlightedHex)
            hex.ResetHighlighting();
        highlightedHex.Clear();
    }

// ───────────────────────────── VERSCHIEDENES & HILFSMETHODEN ─────────────────────────────
    public void ClearParent(GameObject parent)
    {
        if (parent == null) return;
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject.layer == 8)
                Destroy(child.gameObject);
        }
        CardActionDetailsOnScreen.Instance.NoCostText();
    }

    public void SetGamemodeByCard(GameObject currentCard)
    {
        var handler = eventHandler.GetComponent<MyEventHandler>();
        var cardProps = currentCard.GetComponent<CardProperties>();
        if (cardProps == null) return;

        string cardType = cardProps.GetCardType();
        if (cardType == "Building")
            handler.gameMode = GameMode.BuildCard;
        else if (cardType == "Action")
            handler.gameMode = GameMode.PlayCard;
    }

    public void SetGamemodeDefault()
    {
        var handler = eventHandler.GetComponent<MyEventHandler>();
        handler.gameMode = GameMode.Default;
    }

    public void CardOnCardLvlAction(GameObject socketCardGameobject, GameObject currentCardInHand)
    {
        var cardInSocket = socketCardGameobject.GetComponent<CardProperties>();
        var cardInHand = currentCardInHand.GetComponent<CardProperties>();
        if (cardInSocket == null || cardInHand == null)
        {
            Debug.Log("CardProperties nicht gefunden!");
            return;
        }
        cardInSocket.CardLvln(cardInHand);
    }

    public void DestroyUsedCard(GameObject cardToDestroyGameobject)
    {
        var cardProps = cardToDestroyGameobject.GetComponent<CardProperties>();
        if (cardProps == null)
        {
            Debug.Log("cardPropertiesCurrentCardInHand == null");
            return;
        }
        cardProps.DestroyCard();
    }

    public void UpdateResourceOnHex(Transform hexTrans)
    {
        Hex hex = hexTrans.GetComponent<Hex>();
        if (hex != null)
            hex.ChangeListeEPR();
    }

    public void EnableAllComponents(GameObject obj)
    {
        foreach (var component in obj.GetComponentsInChildren<MonoBehaviour>())
            component.enabled = true;
    }

    public bool IsOnlyLeftClick()
    {
        return Input.GetMouseButtonDown(0) &&
               !Input.GetMouseButton(1) &&
               !Input.GetMouseButton(2) &&
               !Input.GetKey(KeyCode.LeftShift) &&
               !Input.GetKey(KeyCode.RightShift) &&
               !Input.GetKey(KeyCode.LeftControl) &&
               !Input.GetKey(KeyCode.RightControl) &&
               !Input.GetKey(KeyCode.LeftAlt) &&
               !Input.GetKey(KeyCode.RightAlt) &&
               !Input.GetKey(KeyCode.Escape) &&
               !Input.GetKey(KeyCode.T);
    }

    public void ClearObjectsInArea(Vector3 position, float range)
    {
        Vector3 point1 = position + Vector3.down * 200f;
        Vector3 point2 = position + Vector3.up * 200f;
        Collider[] colliders = Physics.OverlapCapsule(point1, point2, range);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("EnvironmentObject"))
                Destroy(collider.gameObject);
        }
    }

    public void AddCardToPlayedListOfPlayer(GameObject card)
    {
        CardProperties cardProps = card.GetComponent<CardProperties>();
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();
        if (activePlayer != null && cardProps != null)
            activePlayer.AddCardToPlayedList(cardProps);
        else
            Debug.Log("cardProps oder activePlayer == null");
    }

    public void ShowCloseSockets(Transform activeHex)
    {
        List<Hex> surHex = GetAreaHex(activeHex.position, 1000);
        foreach (Hex hex in surHex)
            hex.HideAllSockets();

        surHex = GetAreaHex(activeHex.position, 500);
        foreach (Hex hex in surHex)
        {
            if (hex.occupantType == OccupType.Settlement)
            {
                hex.ShowAllSockets();
                hex.ResetAllSocketHighlighting();
                HighlightSocketUnderMouse(activeHex);
            }
        }
    }

    public void ResetToDefault()
    {
        var handler = eventHandler.GetComponent<MyEventHandler>();
        handler.chosenCard = null;
        ClearParent(GameObject.Find("Card on Mouse Pointer"));
        SetGamemodeDefault();
    }

    public void SetCurentCard(GameObject card)
    {
        var handler = eventHandler.GetComponent<MyEventHandler>();
        handler.chosenCard = card;
    }

    public void PlaceCardInSocket(GameObject target, GameObject card)
    {
        GameObject socketGO = null;
        if (target.GetComponent<Socket>() != null)
        {
            socketGO = target;
        }
        else
        {
            Socket freeSocket = FindFreeFittingSocket(target.transform, card);
            if (freeSocket != null)
                socketGO = freeSocket.gameObject;
            else
            {
                Debug.LogWarning($"Kein freier, passender Socket gefunden auf Hex „{target.name}“");
                return;
            }
        }
        StartCoroutine(ManageCardSocketPlacement(socketGO, card));
        SetBuilding(socketGO, card);
    }

    public IEnumerator ManageCardSocketPlacement(GameObject chosenSocket, GameObject chosenCard)
    {
        Socket socket = chosenSocket.GetComponent<Socket>();
        CardProperties card = chosenCard.GetComponent<CardProperties>();

        Transform cardTransform = card.gameObject.transform;
        cardTransform.SetParent(socket.transform, worldPositionStays: true);
        cardTransform.localPosition = Vector3.zero;
        cardTransform.localRotation = Quaternion.identity;
        cardTransform.localScale = new Vector3(.055f, .042f, .1f);

        foreach (var rend in cardTransform.GetComponentsInChildren<Renderer>())
            rend.enabled = false;

        Vector3 socketPos = socket.transform.position;
        Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        GameObject tempModel = Instantiate(card.model, socketPos, randomYRotation);
        socket.structure = tempModel;
        tempModel.transform.SetParent(socket.transform, worldPositionStays: true);

        socket.card = card;
        socket.isBlocked = true;
        card.SetInHand(false);
        card.SetCardInSlot(true);

        yield return null;
    }

    private void SetBuilding(GameObject socket, GameObject card)
    {
        Hex hex = socket.GetComponentInParent<Hex>();
        if (hex == null) return;
        List<CardProperties.Resources> resourceEarningList = card.GetComponent<CardProperties>().resourceEarningList;

        foreach (var resource in resourceEarningList)
        {
            int index = hex.resourcesList_EPR.FindIndex(item => item.resourceName == resource.resourceName);
            if (index != -1)
            {
                hex.resourcesList_EPR[index].resourceAmount += resource.resourceAmount;
                hex.resourcesList_EPR[index].isBlocked = resource.isBlocked;
                hex.resourcesList_EPR[index].playerName = resource.playerName.ToString();
            }
            int index2 = hex.resourcesList_MEPR.FindIndex(item2 => item2.resourceName == resource.resourceName);
            if (index2 != -1)
            {
                //hex.GenerateResourceAmount(resource.resourceName.ToString(), false);
            }
        }
    }

    public void SelectHexForUpgrade(Catch caCa)
    {
        var hex = caCa.CurrentHex.GetComponent<Hex>();
        if (hex != null && hex.structure != null)
        {
            StartCoroutine(StructureManager.Instance.ShowOptionsForUpgrade(caCa.CurrentHex));
            var handler = eventHandler.GetComponent<MyEventHandler>();
            handler.chosenHexUpdate = caCa.CurrentHex;
            handler.gameMode = GameMode.UpgradeSettlement;
        }
    }

    public ResList CardPropsToResVec(GameObject card)
    {
        CardProperties cardProps = card.GetComponent<CardProperties>();
        if (cardProps == null) return new ResList();
        return new ResList(
            cardProps.resourceCostsList[0].resourceAmount,
            cardProps.resourceCostsList[1].resourceAmount,
            cardProps.resourceCostsList[2].resourceAmount,
            cardProps.resourceCostsList[3].resourceAmount,
            cardProps.resourceCostsList[4].resourceAmount,
            0, 0, 0);
    }
}
