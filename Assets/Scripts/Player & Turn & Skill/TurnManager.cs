using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public List<NewPlayer> Players = new List<NewPlayer>();
    public TextMeshProUGUI activPlayerText;
    public TextMeshProUGUI roundText;
    public int rundenZähler = 0;

    private int currentPlayerIndex = 0;

    public SkilltreeAndBonusesHandler skilltreeScript;
    public PlayerLevelManager playerLevelManager;

    public static TurnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartTurn()
    {
        NewTurnText();
        SetNewRound();

        HandController.Instance.StartHandController();

        //StartCoroutine(DiceAction());
        StartCoroutine(Dice.Instance.StartDiceRoll());

        MyEventHandler.Instance.gameMode = GameMode.Default;
        playerLevelManager.SetLevelStats();

    }

    private void NewTurnText()
    {
        //Text in UI
        GetCurrentPlayer()?.BeginTurn();
        SetActivPlayertext();
    }

    public NewPlayer GetCurrentPlayer()
    {
        if (Players.Count == 0) return null;
        return Players[currentPlayerIndex];
    }

    public void NextTurn()
    {
        if (Players.Count == 0) return;

        //skilltreeScript.CloseSkilltree();

        // Spielerwechsel
        currentPlayerIndex = (currentPlayerIndex + 1) % Players.Count;

        // Prüfen ob die Liste von vorne beginnt (neue Runde)
        if (currentPlayerIndex == 0)
            SetNewRound();

        NewTurnText();

        StartCoroutine(NextTurnRoutine());
    }

    private IEnumerator NextTurnRoutine()
    {
        // Warten bis die Würfel-Animation abgeschlossen ist
        yield return StartCoroutine(Dice.Instance.StartDiceRoll());

        // Nachfolgende Methoden werden erst jetzt ausgeführt
        UIResourceSummary.Instance.StartRoundSummary();
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText(); ;

        if (GetCurrentPlayer().isMainPlayer)
        {
            Debug.Log("Main Player ist am Zug");
            RoundStartInformation.Instance.SetTextForUI();
            RoundStartInformation.Instance.OpenUI();
            RoundStartInformation.Instance.OpenUI();
        }


        HandController.Instance.StartRefillCards();

        skilltreeScript.UpdateChallengeObjects();
        playerLevelManager.SetLevelStats();
        skilltreeScript.SetVictoryPoints();

        SetActivPlayertext();
    }


    private void SetActivPlayertext()
    {
        activPlayerText.text = "Am Zug: " + GetCurrentPlayer().name;
    }

    private void SetNewRound()
    {
        rundenZähler++;
        roundText.text = "Runde: " + rundenZähler;
    }
}
