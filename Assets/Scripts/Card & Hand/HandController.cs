using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class HandController : MonoBehaviour
{
    #region Singleton + Initialization

    public static HandController Instance { get; private set; }

    public Transform handController;
    public Transform minPos, maxPos, spawnPosition;
    public GameObject cardOnMousePointerObject;

    public Vector3 distanceBetweenPoints;

    public MyEventHandler eventHandler;

    public bool handComplete;
    public bool allCardsInPosition = false;
    private bool checkPositionRunning = false;

    public TextMeshProUGUI textausgabeUI;

    public Button RecycleButton;
    public Button EndRoundButton;

    public int cardnumbers;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        EndRoundButton.interactable = false;
        RecycleButton.interactable = false;
    }
    #endregion

    #region Public Entry Points
    public void StartHandController()
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        activePlayer.ClearCardsInHand();
        activePlayer.ClearInstatiatedGameobjects();
        activePlayer.SetAmountCardsInHand(activePlayer.amountOfCardsInHand);

        //Karten in die Hand am Start
        StartRefillCards();
    }

    private void Update()
    {
        TryStartPositionCheck();
    }

    public void EndRound()
    {
        EndRoundButton.interactable = false;
        //Aktualisierung Resourcenanzeige nach Rundenende
        //UIResourceSummary.Instance.EndRoundSummary();
        GameUtils.Instance.SetGamemodeDefault();
        GameUtils.Instance.ResetHighlighting();
        TurnManager.Instance.NextTurn();
    }

    public void RecycleCardsInHand()
    {
        RecycleButton.interactable = false;

        StartCoroutine(StartRecycling());
    }

    public void StartRefillCards()
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();
        StartCoroutine(RefillCards());
    }

    public void NewPositionsIE()
    {
        CalculateCardPositions(); // Direkt, ohne Singleton
        StartCoroutine(CardMovement.Instance.MoveCardsToHandPosition());
    }

    //public bool CheckHandComplete()
    //{
    //    NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

    //    foreach (CardProperties card in activePlayer.cardsInHand)
    //    {
    //        if (card.moveRoutine != null)
    //        {
    //            return false;
    //        }
    //    }
    //    return true;
    //}

    public void UpdateCardCosts() // Kosten f�r Karten aktualisieren
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        foreach (CardProperties cardProps in activePlayer.cardsInHand)
        {
            cardProps.SetBasicCostResources();
        }
    }
    #endregion

    #region Card Handling

    private IEnumerator RefillCards()
    {
        //Karten in Ausgangsposition bringen
        ResetCardsInHand();
        //Karten hinzuf�gen
        yield return StartCoroutine(AddCardsWithDelay());
        //Runden z�hlen um weiteren verlauf weitere Karten freigeben
        CreateCardDeck.Instance.roundCounter++;
    }

    private IEnumerator AddCardsWithDelay()
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        // Überprüfe, ob die Liste voll ist
        if (activePlayer.cardsInHand.Count >= activePlayer.amountOfCardsInHand)
            yield break;

        // F�lle die Liste nur mit leeren Feldern auf
        for (int i = 0; i < activePlayer.amountOfCardsInHand; i++)
        {
            if (i < activePlayer.cardsInHand.Count && activePlayer.cardsInHand[i] != null)
                continue; // Füge vorhandene Karten hinzu

            if (activePlayer.isMainPlayer)
            {
                //Karten m�ssen wieder neu verschoben werden, also keine Finale Position mehr
                foreach (CardProperties cardProperties in activePlayer.cardsInHand)
                {
                    cardProperties.SetInFinalPosition(false);
                }
            }

            //neue zuf�llige Karte
            yield return StartCoroutine(Card.Instance.CreateNewCard());

            // Warte eine Sekunde, bevor die n�chste Karte hinzugef�gt wird Default: .25f
            yield return new WaitForSeconds(.1f);
        }

        //Neue Positionen festlegen
        allCardsInPosition = false;
        CalculateCardPositions();
        StartCoroutine(CardMovement.Instance.MoveCardsToHandPosition());
    }

    private IEnumerator StartRecycling()
    {
        //Aktualisierung Resourcenanzeige nach Rundenende
        //UIResourceSummary.Instance.EndRoundSummary();

        //Karten in das Ablagedeck verschieben
        yield return StartCoroutine(CardMovement.Instance.MoveCardsToCardStorageDeck());

        //Karten l�schen
        yield return StartCoroutine(RecycleCards());

        GameUtils.Instance.SetGamemodeDefault();
        GameUtils.Instance.ResetHighlighting();

        TurnManager.Instance.NextTurn();
    }

    private IEnumerator RecycleCards()
    {
        ResetCardsInHand();

        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        int cardCount = activePlayer.cardsInHand.Count;

        if (cardCount > 0)
        {
            yield return new WaitForSeconds(.6f * cardCount); // Wartezeit anpassen, falls n�tig

            // Lokale Kopie der Karten
            List<CardProperties> cardsToRemove = new List<CardProperties>(activePlayer.cardsInHand);

            for (int i = cardsToRemove.Count - 1; i >= 0; i--)
            {
                if (activePlayer.isMainPlayer)
                {
                    Destroy(activePlayer.cardsInHand[i].gameObject);
                }
                RemoveFromHand(activePlayer.cardsInHand[i].gameObject);
            }

            activePlayer.cardsInHand.Clear();
            activePlayer.instantiatedGameobjects.Clear();
        }
    }

    public void RemoveFromHand(GameObject card)
    {
        CardProperties cardProps = card.GetComponent<CardProperties>();
        int handPosition = cardProps.GetHandPosition();
        //cardsInHand.RemoveAt(handPosition);
        //cardPositions.RemoveAt(handPosition);
        //instatiatedGameobjects.RemoveAt(handPosition);

        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();


        foreach (CardProperties cardProperties in activePlayer.cardsInHand)
        {
            cardProperties.SetInFinalPosition(false);
        }
    }
    #endregion

    #region Position Handling

    private void TryStartPositionCheck()
    {
        if (!allCardsInPosition && !checkPositionRunning)
        {
            StartCoroutine(CheckStartPosition());
        }
    }

    private IEnumerator CheckStartPosition()
    {
        checkPositionRunning = true;

        yield return new WaitForSeconds(4f);

        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        cardnumbers = 0;

        foreach (CardProperties card in activePlayer.cardsInHand)
        {
            if (card != null)
            {
                int handPosition = card.GetHandPosition();

                GameObject gameObject = card.gameObject;

                if (activePlayer != null)
                {
                    if (Vector3.Distance(gameObject.transform.localPosition, activePlayer.GetCardPosition(handPosition)) < 1f)
                    {
                        cardnumbers++;
                    }
                }
            }
        }

        if (activePlayer.cardsInHand.Count > 0 && cardnumbers >= activePlayer.cardsInHand.Count)
        {
            allCardsInPosition = true;
        }

        checkPositionRunning = false;
    }

    public void CalculateCardPositions()
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        activePlayer.ClearCardPosition();
        distanceBetweenPoints = Vector3.zero;

        //Kartenpositionen ermitteln
        if (activePlayer.cardsInHand.Count > 1)
        {
            minPos = CardsPositions.Instance.GetMinPos();
            maxPos = CardsPositions.Instance.GetMaxPos();
            float distancePoints = CardsPositions.Instance.GetDistance();
            distanceBetweenPoints = new Vector3(distancePoints, 0f, 0f);
        }

        for (int i = 0; i < activePlayer.cardsInHand.Count; i++)
        {
            //Kartenposition der Liste hinzuf�gen
            activePlayer.cardPositions.Add(minPos.localPosition + (distanceBetweenPoints * i));

            //Karteneigenschaften
            activePlayer.cardsInHand[i].SetInHand(true);
            activePlayer.cardsInHand[i].SetHandPosition(i);
            activePlayer.cardsInHand[i].SetInFinalPosition(false);
        }

        allCardsInPosition = false;
    }
    #endregion

    #region Utilities

    public void ResetCardsInHand()
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        foreach (CardProperties cardProps in activePlayer.cardsInHand)
        {
            Vector3 startPos = new Vector3(0, 0, 0);
            cardProps.transform.localEulerAngles = Vector3.zero;
            cardProps.isSelected = false;

            if (activePlayer.isMainPlayer)
            {
                CardAnimationController cardAnimationController = cardProps.GetComponentInChildren<CardAnimationController>();

                //GameObject animationObject = cardAnimationController1.gameObject;

                if (cardAnimationController != null)
                {
                    cardAnimationController.transform.localEulerAngles = Vector3.zero;
                    CardMovement.Instance.BackToStartPosition(cardAnimationController.gameObject, startPos);
                }
            }
        }

        if (cardOnMousePointerObject != null)
            GameUtils.Instance.ClearParent(cardOnMousePointerObject);// Karte an Mauszeiger l�schen
    }


    public void HighlightCardOnClick(GameObject card)
    {
        StartCoroutine(ShowClickedCard(card));
    }

    private IEnumerator ShowClickedCard(GameObject card)
    {
        Vector3 targetpos = new Vector3(0, 20, 0);
        CardProperties cardProps = card.GetComponent<CardProperties>();
        CardAnimationController cardAnimationControllerNew = card.GetComponentInChildren<CardAnimationController>();

        cardProps.isSelected = true; // als ausgew�hlt markieren
        CardMovement.Instance.InTheForground(cardAnimationControllerNew.gameObject, targetpos);

        yield return null;
    }
    #endregion

}

