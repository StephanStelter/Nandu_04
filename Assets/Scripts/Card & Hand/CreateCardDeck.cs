using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CardProperties;

public class CreateCardDeck : MonoBehaviour
{
    public static CreateCardDeck Instance { get; set; }


    public List<CardProperties> commonCards = new List<CardProperties>();
    public List<CardProperties> uncommonCards = new List<CardProperties>();
    public List<CardProperties> rareCards = new List<CardProperties>();
    public List<CardProperties> legendaryCards = new List<CardProperties>();
    public List<CardProperties> goldCards = new List<CardProperties>();

    public int roundCounter;
    private int randomNumber;

    public int roundInt;

    private int cardID = 0;

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

        randomNumber = 0;
        roundCounter = 0;
    }


    public void CreateDeckCardsinHand()
    {
        //int randomNumber = Random.Range(0, 100);

        RoundCounternumber();

        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        //Common Cards
        if (randomNumber >= 0 && randomNumber <= 50)
        {
            int randomIndex = Random.Range(0, commonCards.Count); //Zufälliger Index in Liste
            activePlayer.cardsInHand.Add(commonCards[randomIndex]); //Füge zufällige Karte der Hand hinzu

            SetCardID();
            SetCosts(commonCards[randomIndex]);
        }

        //Uncommon Cards
        if (randomNumber > 50 && randomNumber <= 70)
        {
            int randomIndex = Random.Range(0, uncommonCards.Count); //Zufälliger Index in Liste
            activePlayer.cardsInHand.Add(uncommonCards[randomIndex]); //Füge zufällige Karte der Hand hinzu

            SetCardID();
            SetCosts(commonCards[randomIndex]);
        }

        //Rare Cards
        if (randomNumber > 70 && randomNumber <= 85)
        {
            int randomIndex = Random.Range(0, rareCards.Count); //Zufälliger Index in Liste
            activePlayer.cardsInHand.Add(rareCards[randomIndex]); //Füge zufällige Karte der Hand hinzu

            SetCardID();
            SetCosts(commonCards[randomIndex]);
        }

        //Legendary Cards
        if (randomNumber > 85 && randomNumber <= 95)
        {
            int randomIndex = Random.Range(0, legendaryCards.Count); //Zufälliger Index in Liste
            activePlayer.cardsInHand.Add(legendaryCards[randomIndex]); //Füge zufällige Karte der Hand hinzu

            SetCardID();
            SetCosts(commonCards[randomIndex]);
        }

        //Legendary Cards
        if (randomNumber > 95 && randomNumber <= 100)
        {
            int randomIndex = Random.Range(0, goldCards.Count); //Zufälliger Index in Liste
           activePlayer.cardsInHand.Add(goldCards[randomIndex]); //Füge zufällige Karte der Hand hinzu

            SetCardID();
            SetCosts(commonCards[randomIndex]);
        }
    }

    private void RoundCounternumber()
    {
        if (roundCounter < 100)
        {
            randomNumber = Random.Range(0, 50);
        }
        //if (roundCounter >= 10 && roundCounter < 20)
        //{
        //    randomNumber = Random.Range(0, 70);
        //}
        //if (roundCounter >= 20 && roundCounter < 30)
        //{
        //    randomNumber = Random.Range(0, 85);
        //}
        //if (roundCounter >= 30)
        //{
        //    randomNumber = Random.Range(0, 100);
        //}
    }

    private void SetCardID()
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        CardProperties cardProperties = activePlayer.cardsInHand[activePlayer.cardsInHand.Count - 1];
        cardProperties.SetCardID(cardID);

        cardID++;
    }

    private void SetCosts(CardProperties cardPropertiesCostCheck)
    {
        cardPropertiesCostCheck.SetBasicCostResources();
    }
}
