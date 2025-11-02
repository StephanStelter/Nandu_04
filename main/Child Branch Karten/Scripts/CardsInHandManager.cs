using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardsInHandHandler : MonoBehaviour
{
    public GameObject cardPrefab;

    public Transform startPosition; // Position, an die die Karte instanziert werden soll
    public Transform endPosition; // Position, an die die Karte gelöscht werden soll
    public Transform handPosition; // Position, an die die Karte bewegt werden soll

    public int maxCards = 5; // Anzahl der Karten, die instanziert werden sollen
    public int currentCards = 0; // Aktuelle Anzahl der Karten in der Hand
    public int distanceBetweenCards = 150;
    public Vector3[] cardPositions;
    public List<GameObject> cardsInHand;



    public void CreateCardsInHand()
    {
        // cardManager finden
        CardManager cardManager = Object.FindFirstObjectByType<CardManager>();
        if (cardManager == null) { Debug.LogWarning("CardManager nicht gefunden!"); return; }

        if (cardManager.cardDeck.Count == 0)
        {
            Debug.LogWarning("Kartendeck ist leer!");
            return;
        }

        // Kartenpositionen berechnen
        CreateCardPositions();

        // Solange Karten fehlen, neue Karten erstellen
        while (currentCards < maxCards)
        {
            //StartCoroutine(Cards());
            CreateNewCard();
        }

        // Neue Anordnung der Karten in der Hand
        StartCoroutine(NewOrderOfCardsInHand());
    }

    public void RemoveCardsFromHand()
    {
        // Alle Karten zur Endposition bewegen
        StartCoroutine(MoveCardsInEndPosition());
    }

    private void CreateNewCard()
    {
        // cardManager finden
        CardManager cardManager = Object.FindFirstObjectByType<CardManager>();
        if (cardManager == null || cardManager.cardDeck.Count == 0)
        {
            Debug.LogWarning("CardManager nicht gefunden oder Kartendeck ist leer!");
            return;
        }

        // Karte an Startposition erstellen 
        GameObject newCard = Instantiate(DrawRandomCard(cardManager), startPosition.position, Quaternion.identity);
        //GameObject newCard = Instantiate(cardManager.cardDeck[randomIndex].gameObject, startPosition.position, Quaternion.identity);

        // Karte skalieren (auf 30% der Originalgröße)
        newCard.transform.localScale = new Vector3(.3f, .3f, .3f);

        // Parent der Karte setzen, aber worldPosition beibehalten (damit Startposition korrekt bleibt)
        newCard.transform.SetParent(handPosition, true);

        // Status der Karte setzen (ob sie im Deck ist oder nicht)
        //CardBasicBahavior cardBehavior = newCard.GetComponent<CardBasicBahavior>();
        //if (cardBehavior != null)
        //{
        //    // Karte ist jetzt in der Hand
        //    cardBehavior.InactivateStatus();
        //    cardBehavior.isInHand = true;
        //}
        //else { Debug.LogWarning("CardBasicBahavior-Komponente nicht gefunden auf der neuen Karte!"); }

        // relativeChance der Karte updaten
        //CardPropertiesTest cardProperties = newCard.GetComponent<CardPropertiesTest>();
        //if (cardProperties == null) { Debug.Log("cardProperties == null"); }
        //cardProperties.UpdaterelativeChance();
        //cardProperties.DeactivateChanceText();

        // Karte zur Liste hinzufügen
        cardsInHand.Add(newCard);
        currentCards++;
    }

    private GameObject DrawRandomCard(CardManager cardManager)
    {
        // Überprüfen, ob das Deck leer ist
        if (cardManager.cardDeck == null || cardManager.cardDeck.Count == 0)
        {
            Debug.LogWarning("Deck ist leer!");
            return null;
        }

        // 1️ Gesamtgewicht berechnen
        float totalWeight = cardManager.cardDeck.Sum(card => card.GetComponent<CardPropertiesTest>().drawChance);

        // 2️ Zufallswert innerhalb der Gesamtgewichte wählen
        float randomValue = Random.Range(0, totalWeight);

        // 3️ Karte basierend auf Gewicht finden
        float runningSum = 0;
        foreach (var cardObj in cardManager.cardDeck)
        {
            var card = cardObj.GetComponent<CardPropertiesTest>();
            runningSum += card.drawChance; // Gewicht der aktuellen Karte hinzufügen

            if (randomValue < runningSum) // Wenn der Zufallswert innerhalb des aktuellen Gewichts liegt
            {
                // Karte gefunden – Kopie instanziieren
                GameObject instance = Instantiate(cardObj);

                // Eigenschaften der Karte anpassen
                var newProps = instance.GetComponent<CardPropertiesTest>();
                if (newProps == null) { Debug.LogWarning("CardPropertiesTest-Komponente nicht gefunden auf der instanzierten Karte!"); }
                newProps.DeactivateChanceText(); // Chance-Text deaktivieren

                var newBehavior = instance.GetComponent<CardBasicBahavior>();
                if (newBehavior == null) { Debug.LogWarning("CardBasicBahavior-Komponente nicht gefunden auf der instanzierten Karte!"); }
                newBehavior.isInHand = true; // isInHand setzen
                newBehavior.isInDeck = false; // isInDeck setzen
                newBehavior.InactivateStatus(); // Status inaktivieren

                return instance; // Karte zurückgeben
            }
        }

        return null; // sollte nie passieren
    }

    private void CreateCardPositions()
    {
        cardPositions = new Vector3[maxCards];

        float d = distanceBetweenCards;

        // Positionen berechnen
        for (int i = 0; i < maxCards; i++)
        {
            // Offset berechnen (Mitte = 0)
            float indexOffset = i - (maxCards - 1) / 2f;

            // Lokale Position relativ zu handPosition
            cardPositions[i] = new Vector3(indexOffset * d, 0f, 0f);
        }
    }

    private IEnumerator NewOrderOfCardsInHand()
    {
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            // Zielposition: lokale Position unter Hand
            Vector3 targetLocalPos = cardPositions[i];

            if (cardsInHand[i].transform.localPosition == startPosition.localPosition)
            {
                // Animation starten: von aktueller Position -> Zielposition
                yield return cardsInHand[i].GetComponent<CardBasicBahavior>().MoveCardInHandPosition(targetLocalPos);
            }
            else
            {
                // Sofort zur Zielposition bewegen (ohne Animation)
                yield return cardsInHand[i].GetComponent<CardBasicBahavior>().MoveCardNewOrder(targetLocalPos);
            }
        }
    }

    private IEnumerator MoveCardsInEndPosition()
    {
        int count = cardsInHand.Count - 1;
        // Karten von hinten nach vorne entfernen (damit die Liste korrekt bleibt)
        for (int i = count; i >= 0; i--)
        {
            // Animation starten: von aktueller Position -> Zielposition
            yield return cardsInHand[i].GetComponent<CardBasicBahavior>().MoveCardToEndPosition(endPosition.localPosition);

            // Karte aus der Liste entfernen und Zähler verringern
            currentCards--;
            cardsInHand.RemoveAt(i);
        }
    }

    public void ResetHighlightedCard()
    {
        foreach (GameObject gameObject in cardsInHand)
        {
            CardBasicBahavior card = gameObject.GetComponent<CardBasicBahavior>();
            if (card.moveUpToogle == true)
            {
                card.ClickedOnCard();
            }
        }
    }

    public void UseCard()
    {
        foreach (GameObject gameObject in cardsInHand)
        {
            // Nur die hervorgehobene Karte verwenden
            CardBasicBahavior card = gameObject.GetComponent<CardBasicBahavior>();
            if (card != null && card.moveUpToogle == true)
            {
                // Karte verwenden (hier kannst du die Logik zum Anwenden der Kartenfunktionalität hinzufügen)
                Debug.Log("Use Card" + gameObject.name);

                // Karte zerstören (Animation und Entfernen aus der Hand)
                card.DestroyCard();

                // Karte aus der Liste entfernen und Zähler verringern
                cardsInHand.Remove(gameObject);
                currentCards--;

                // Schleife beenden, da nur eine Karte verwendet werden soll
                break;
            }
        }
    }
}
