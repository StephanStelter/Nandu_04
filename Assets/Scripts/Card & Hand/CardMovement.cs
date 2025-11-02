using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    public static CardMovement Instance { get; private set; }

    [SerializeField] private Transform spawnPointCardTransform;

    int moveSpeed = 200;

    private Vector3 firstMoveToHand;
    private Vector3 secondMoveToHand;
    private Vector3 thirdMoveToHand;
    private Vector3 horizontalMove;

    //Kartenbewegung in Ablagedeck
    [SerializeField] private Transform despawnPointTransform;
    [SerializeField] private Transform despawnPointCradTransform;

    private Vector3 firstMoveToStorage;
    private Vector3 secondMoveToStorage;
    private Vector3 thirdMoveToStorage;

    private bool backToStorage = false;
    public bool animationIsRunning = false;


    private void Awake()
    {
        // Sicher stellen, dass nur eine Instanz existiert
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Eine Instanz existiert bereits, diese sollte zerstört werden
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        spawnPointCardTransform.localPosition = new Vector3(0f, .3f, -1);
        despawnPointCradTransform.localPosition = new Vector3(-.3f, .3f, -1f);
    }

    // allgemeine Kartenbewegungsmethode ****************************************************************************************
    private IEnumerator MoveCard(GameObject card, Vector3 targetPos)
    {
        while (Vector3.Distance(card.transform.localPosition, targetPos) > 0.001f)
        {
            float step = moveSpeed * Time.deltaTime;
            card.transform.localPosition = Vector3.MoveTowards(card.transform.localPosition, targetPos, step);

            yield return null;
        }

        card.transform.localPosition = targetPos;

        animationIsRunning = false;
    }

    // Kartenbewegung zu HandPosition ******************************************************************************************
    public IEnumerator MoveCardsToHandPosition()
    {
        yield return new WaitForSeconds(.1f);

        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        for (int i = 0; i < activePlayer.instantiatedGameobjects.Count; i++)
        {
            GameObject currentCard = activePlayer.instantiatedGameobjects[i];

            CardProperties cardProps =activePlayer.instantiatedGameobjects[i].GetComponent<CardProperties>();

            GetSpeedAdjustment(cardProps);

            if (cardProps.GetInStartPosition() && !cardProps.GetInFinalPosition())
            {
                //TEST
                List<Vector3> zielpfad = new List<Vector3>
                {
                    GetFirstMoveToHand(),
                    GetSecondMoveToHand(activePlayer.instantiatedGameobjects[i]),
                    GetThirdMoveToHand(activePlayer.instantiatedGameobjects[i])
                };

                // Skalierung für diese spezifische Karte starten
                CardScaler.Instance.StartScaleUpRoutine(currentCard); // Jede Karte wird skaliert

                cardProps.MoveAlongPath(zielpfad,800,0,"global");
                // Skalierung für diese spezifische Karte starten
                CardScaler.Instance.StartScaleUpRoutine(currentCard); // Jede Karte wird skaliert
            }
            else if (!cardProps.GetInStartPosition() && !cardProps.GetInFinalPosition())
            {
                //Horizontale Bewegung - Rechts - Links
                yield return StartCoroutine(MoveCard(activePlayer.instantiatedGameobjects[i],
                    GetHorizontalMove(activePlayer.instantiatedGameobjects[i])));
            }


            //if (cardProps.GetInStartPosition() && !cardProps.GetInFinalPosition())
            //{
            //    //Vertikale Bewegung - Hoch
            //    yield return StartCoroutine(MoveCard(HandController.Instance.instatiatedGameobjects[i], GetFirstMoveToHand()));
            //    //Horizontale Bewegung - Rechts
            //    yield return StartCoroutine(MoveCard(HandController.Instance.instatiatedGameobjects[i],
            //        GetSecondMoveToHand(HandController.Instance.instatiatedGameobjects[i])));

            //    // Skalierung für diese spezifische Karte starten
            //    CardScaler.Instance.StartScaleUpRoutine(currentCard); // Jede Karte wird skaliert

            //    //Vertikale Bewegung - Runter
            //    yield return StartCoroutine(MoveCard(HandController.Instance.instatiatedGameobjects[i],
            //        GetThirdMoveToHand(HandController.Instance.instatiatedGameobjects[i])));
            //}
            //else if (!cardProps.GetInStartPosition() && !cardProps.GetInFinalPosition())
            //{
            //    //Horizontale Bewegung - Rechts - Links
            //    yield return StartCoroutine(MoveCard(HandController.Instance.instatiatedGameobjects[i],
            //        GetHorizontalMove(HandController.Instance.instatiatedGameobjects[i])));
            //}
        }
    }

    private Vector3 GetFirstMoveToHand()
    {
        firstMoveToHand = Vector3.zero;

        return firstMoveToHand = HandController.Instance.spawnPosition.localPosition + new Vector3(0, 200, 0);
    }

    private Vector3 GetSecondMoveToHand(GameObject card)
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        Vector3 cardPos = activePlayer.cardPositions[card.GetComponent<CardProperties>().GetHandPosition()];
        float movedirectionX = Mathf.Abs(cardPos.x - firstMoveToHand.x);

        return secondMoveToHand = firstMoveToHand + new Vector3(movedirectionX, 0, 0);
    }

    private Vector3 GetThirdMoveToHand(GameObject card)
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        CardProperties cardProps = card.GetComponent<CardProperties>();

        Vector3 cardPos = activePlayer.cardPositions[cardProps.GetHandPosition()];

        cardProps.SetInFinalPosition(true); //finale Position erreicht
        cardProps.SetInStartPosition(false); //keine Startposition mehr

        return thirdMoveToHand = cardPos;
    }

    private Vector3 GetHorizontalMove(GameObject card)
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        CardProperties cardProps = card.GetComponent<CardProperties>();

        Vector3 cardPos = activePlayer.cardPositions[cardProps.GetHandPosition()];

        cardProps.SetInFinalPosition(true); //finale Position erreicht
        cardProps.SetInStartPosition(false); //keine Startposition mehr

        return horizontalMove = cardPos;
    }

    // Kartenbewegung zu Ablagedeck ******************************************************************************************
    public IEnumerator MoveCardsToCardStorageDeck()
    {
        yield return new WaitForSeconds(.1f);

        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        backToStorage = true;

        for (int i = activePlayer.instantiatedGameobjects.Count - 1; i >= 0; i--)
        {
            GameObject currentCard = activePlayer.instantiatedGameobjects[i];

            CardProperties cardProps = activePlayer.instantiatedGameobjects[i].GetComponent<CardProperties>();

            GetSpeedAdjustment(cardProps);

            //Karte wieder kleiner skalieren
            GameObject _gameObject = cardProps.gameObject;

            // Skalierung für diese spezifische Karte starten
            CardScaler.Instance.StartScaleDownRoutine(currentCard); // Jede Karte wird skaliert

            //Vertikale Bewegung - Hoch
            yield return StartCoroutine(MoveCard(activePlayer.instantiatedGameobjects[i],
                GetFirstMoveToStorage(activePlayer.instantiatedGameobjects[i])));
            //Horizontale Bewegung - Rechts
            yield return StartCoroutine(MoveCard(activePlayer.instantiatedGameobjects[i],
                GetSecondMoveToStorage(activePlayer.instantiatedGameobjects[i])));
            ////Vertikale Bewegung - Runter
            yield return StartCoroutine(MoveCard(activePlayer.instantiatedGameobjects[i],
            GetThirdMoveToStorage(activePlayer.instantiatedGameobjects[i])));

            SpecialCardEventHandler.Instance.GetPoitsForRecycledCard(currentCard);
        }
        yield return backToStorage = false;
    }

    private Vector3 GetFirstMoveToStorage(GameObject card)
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        CardProperties cardProps = card.GetComponent<CardProperties>();

        Vector3 cardPos = activePlayer.cardPositions[cardProps.GetHandPosition()];

        return firstMoveToStorage = cardPos + new Vector3(0, 200, 0);
    }

    private Vector3 GetSecondMoveToStorage(GameObject card)
    {
        float movedirectionX = Mathf.Abs(despawnPointTransform.localPosition.x - firstMoveToStorage.x);
        float movedirectionZ = Mathf.Abs(despawnPointTransform.localPosition.z - firstMoveToStorage.z);

        return secondMoveToStorage = firstMoveToStorage + new Vector3(movedirectionX, 0, -movedirectionZ);
    }

    private Vector3 GetThirdMoveToStorage(GameObject card)
    {
        return thirdMoveToStorage = despawnPointTransform.localPosition;
    }

    private void GetSpeedAdjustment(CardProperties cardProperties)
    {
        if (!cardProperties.GetInStartPosition() && !cardProperties.GetInFinalPosition())
        {
            moveSpeed = 500;
        }

        if (backToStorage)
        {
            moveSpeed = 1500;
        }

        else
        {
            moveSpeed = 1200;
        }
    }

    public void InTheForground(GameObject card, Vector3 targetPos)
    {
        card.GetComponentInParent<CardProperties>().MoveAlongPath(new List<Vector3> { targetPos }, 50, 360, "local");
    }

    public void BackToStartPosition(GameObject card, Vector3 targetPos)
    {
        card.GetComponentInParent<CardProperties>().MoveAlongPath(new List<Vector3> { targetPos }, 200,0,"local");   
    }
}
