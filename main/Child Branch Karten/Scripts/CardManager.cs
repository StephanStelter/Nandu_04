using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class CardManager : MonoBehaviour
{
    [Header("Kartendeck")]
    private int rows = 3; // Anzahl der Reihen im Deck
    private int columns = 12; // Anzahl der Spalten im Deck
    private float cardWidth = 150f; // Breite einer Karte
    private float cardHeight = 220f; // Höhe einer Karte
    private float spacing = -50f; // Abstand zwischen den Karten
    private Vector3 startPosition = new Vector3(-550f, 110f, 0f); // Startposition des Decks
    public GameObject cardPrefabEmptySocket; // Prefab der Karte
    public GameObject cardDeckParent;

    public int unlockedSockets = 12; // Anzahl der freigeschalteten Kartenplätze
    public TextMeshProUGUI cardsInDeckCounter; // Textfeld für die Anzeige der Karten im Deck

    public int cardpointsLimit = 14; // Maximale Anzahl der Kartenpunkte im Deck
    public TextMeshProUGUI cardpointsCurrentAmount; // Textfeld für die Anzeige der aktuellen Kartenpunkte im Deck




    //public List<GameObject> cardDeckSockets; // Liste der Kartensockel im Deck
    public List<GameObject> cardDeck; // Liste der Karten im Deck
    public List<GameObject> tempCardDeck; // Temporäre Liste der Karten im Deck

    [Header("Kartenbibliothek")]
    public GameObject cardLibrary; // Objekt, das alle Karten in der Bibliothek enthält
    public GameObject cardLibraryParent; // Elternelement für die Karten in der Bibliothek in der UI

    private CardPropertiesTest cardPropertiesTest;
    private CardBasicBahavior cardBasicBahavior;
    public UnityEngine.UI.Image resourceImage;
    public TMPro.TextMeshProUGUI resourceText;
    public TMPro.TextMeshProUGUI buttonText;


    private void Start()
    {
        // Karten IDs setzen
        SetCardIDs();
        // Kartenbibliothek und Deck erstellen
        UpdateLibraryAnDeck();
    }

    public void UpdateLibraryAnDeck()
    {
        CreateCardLibrary();
        CreateCardDeck();

        // Kartenwahrscheinlichkeiten im Deck berechnen
        //CalculateCardProbabilities();
    }


    private void CreateCardLibrary()
    {
        // Vorherige Karten in der Bibliothek löschen 
        ClearCardLibrary();
        // Karten in der Bibliothek positionieren
        CardBasicBahavior[] cardBasicBahaviorsArray = cardLibrary.GetComponentsInChildren<CardBasicBahavior>(true);
        if (cardBasicBahaviorsArray == null || cardBasicBahaviorsArray.Length == 0) { Debug.Log("cardBasicBahaviorsArray == null"); return; }
        for (int i = 0; i < cardBasicBahaviorsArray.Length; i++)
        {
            int row = i / columns;   // Reihenindex berechnen
            int col = i % columns;   // Spaltenindex berechnen

            // Position der Karte berechnen
            float posX = startPosition.x + col * (cardWidth + spacing);
            float posY = startPosition.y - row * (cardHeight + spacing);
            Vector3 localPos = new Vector3(posX, posY, startPosition.z);

            // Karte aus der Bibliothek instanziieren
            GameObject newCard = Instantiate(cardBasicBahaviorsArray[i].gameObject, cardLibraryParent.transform);
            newCard.name = "Card_" + (i + 1);

            // Lokale Position setzen
            newCard.transform.localPosition = localPos;

            // Karte skalieren (auf 40% der Originalgröße)
            newCard.transform.localScale = new Vector3(.4f, .4f, .4f);
        }
        
        StartCoroutine(SetStatusInLibrary()); // Status der Karte setzen (ob sie im Deck ist oder nicht)
    }

    private IEnumerator SetStatusInLibrary()
    {
        yield return new WaitForEndOfFrame(); // Warten, bis alle Karten fertig instanziiert sind

        // Karte ist in Bibliothek UI --> Status setzen
        foreach (Transform child in cardLibraryParent.transform)
        {
            var cardBehavior = child.GetComponent<CardBasicBahavior>();
            if (cardBehavior == null) { Debug.Log("cardBehavior == null"); continue; }
            cardBehavior.SetStatus(); // Status der Karte setzen (ob sie im Deck ist oder nicht)

            var cardProperties = child.GetComponent<CardPropertiesTest>();
            if (cardProperties == null) { Debug.Log("cardProperties == null on card: " + child.name); }
            cardProperties.SetDrawChance(); // Ziehchance der Karte setzen
        }

    }

    private void CreateCardDeck()
    {
        PrepareForNewDeck();

        int cardsInDeck = 0;
        int cardpointsCurrent = 0;

        // Karten im Deck positionieren
        for (int i = 0; i < unlockedSockets; i++)
        {
            int row = i / columns;   // Reihenindex berechnen
            int col = i % columns;   // Spaltenindex berechnen

            // Position der Karte berechnen
            float posX = startPosition.x + col * (cardWidth + spacing);
            float posY = startPosition.y - row * (cardHeight + spacing);
            Vector3 localPos = new Vector3(posX, posY, startPosition.z);

            // Karte / Socket instanziieren
            if (i < cardDeck.Count)
            {
                // Karte aus der Bibliothek instanziieren
                GameObject newCard = Instantiate(cardDeck[i], cardDeckParent.transform);
                newCard.name = "Card_" + (i + 1);

                CardPropertiesTest cardProperties = newCard.GetComponent<CardPropertiesTest>();
                if (cardProperties == null) { Debug.Log("cardProperties == null on card: " + newCard.name); }
                int cardValueRest = cardpointsLimit - cardpointsCurrent; // verbleibende Kartenpunkte im Deck

                // Überprüfen, ob die Karte das Kartenpunkte-Limit überschreitet
                if (cardProperties.cardValue > cardValueRest)
                {
                    Debug.Log("Karte " + newCard.name + " mit Kartenwert " + cardProperties.cardValue + " überschreitet das Kartenpunkte-Limit. Maximaler Restwert: " + cardValueRest);
                    Destroy(newCard); // zerstöre die Karte wieder
                    // leeres Kartensocket erstellen
                    GameObject emptySocket = Instantiate(cardPrefabEmptySocket, cardDeckParent.transform);
                    emptySocket.name = "CardSocket_" + (i + 1);
                    // Lokale Position setzen
                    emptySocket.transform.localPosition = localPos;
                    continue; // nächste Karte
                }

                newCard.transform.localPosition = localPos; // Lokale Position setzen

                newCard.transform.localScale = new Vector3(.4f, .4f, .4f); // Karte skalieren (auf 40% der Originalgröße)

                SetStatusCardsInDeck(newCard); // Status der Karte setzen (ob sie im Deck ist oder nicht)

                cardsInDeck++; // Karten im Deck Zähler aktualisieren

                cardpointsCurrent += cardProperties.cardValue; // aktuelle Kartenpunkte im Deck aktualisieren

            }
            else
            {
                // leeres Kartensocket erstellen
                GameObject newCard = Instantiate(cardPrefabEmptySocket, cardDeckParent.transform);
                newCard.name = "CardSocket_" + (i + 1);

                // Lokale Position setzen
                newCard.transform.localPosition = localPos;
            }
        }
        // Karten im Deck Zähler aktualisieren
        cardsInDeckCounter.text = cardsInDeck + " / " + unlockedSockets;
        // Kartenpunkte im Deck Zähler aktualisieren
        cardpointsCurrentAmount.text = cardpointsCurrent + " / " + cardpointsLimit;
        // Kartenpunkte im Deck Zähler aktualisieren
        StartCoroutine(UpdateDeckChancesAfterBuild());

    }

    private void PrepareForNewDeck()
    {
        // Vorherige Karten im Deck löschen
        ClearCardDeck();
        // Karte im Deck leeren
        cardDeck = new List<GameObject>();
        // Karten im Deck hinzufügen
        foreach (Transform child in cardLibrary.transform)
        {
            CardBasicBahavior gameobjectBasicBehavior = child.GetComponent<CardBasicBahavior>();
            if (gameobjectBasicBehavior == null) { Debug.Log("gameobjectBasicBehavior == null"); }

            if (gameobjectBasicBehavior.isInDeck)
            {
                // Karte hinzufügen
                cardDeck.Add(child.gameObject);
            }
        }
    }

    private void SetStatusCardsInDeck(GameObject instantiatedObject)
    {
        // Karte ist im Deck (sichtbares Objekt) --> Status nicht anzeigen 
        CardBasicBahavior cardBehaviorInstantiated = instantiatedObject.GetComponent<CardBasicBahavior>();
        if (cardBehaviorInstantiated == null) { Debug.Log("cardBehaviorInstantiated == null"); return; }
        cardBehaviorInstantiated.InactivateStatus();
    }

    private IEnumerator UpdateDeckChancesAfterBuild()
    {
        yield return new WaitForEndOfFrame(); // Warten, bis alle Karten fertig instanziiert sind

        // Alle Karten im sichtbaren Deck holen
        var instancedCards = new List<GameObject>();
        foreach (Transform child in cardDeckParent.transform)
        {
            var cardProps = child.GetComponent<CardPropertiesTest>();
            if (cardProps != null)
                instancedCards.Add(child.gameObject);
        }

        if (instancedCards.Count == 0) yield break;

        float totalWeight = instancedCards.Sum(c => c.GetComponent<CardPropertiesTest>().drawChance); // Gesamtgewicht berechnen
        foreach (var obj in instancedCards)
        {
            var cp = obj.GetComponent<CardPropertiesTest>();
            float rel = cp.drawChance / totalWeight; // relative Chance berechnen
            cp.SetRelativeChance(rel); // ruft Anzeigeaktualisierung auf
        }

        // Ersetze cardDeck mit den Instanzen
        cardDeck = instancedCards;

        Debug.Log("✅ Wahrscheinlichkeiten im Deck aktualisiert (" + cardDeck.Count + " Karten)");
    }







    // Details der Karte im Fenster setzen
    public void SetCardDetailsInWindow(CardPropertiesTest _cardPropertiesTest, CardBasicBahavior _cardBasicBahavior)
    {
        cardPropertiesTest = _cardPropertiesTest;
        cardBasicBahavior = _cardBasicBahavior;

        cardPropertiesTest.ResourceOFCard();
        cardBasicBahavior.CardScriptTest();

        // Ressource Sprite setzen
        SpritesEnty spritesEnty = UnityEngine.Object.FindFirstObjectByType<SpritesEnty>();
        if (spritesEnty == null) { Debug.Log("spritesEnty == null"); return; }
        resourceImage.sprite = spritesEnty.GetResourceSprite(cardPropertiesTest.resourceType);


        // Ressource Text setzen
        resourceText.text = "";
        resourceText.text += "\nRessource: " + cardPropertiesTest.resourceType.ToString();
        resourceText.text += "\nResourcenmenge: " + cardPropertiesTest.resourceAmount.ToString();
        resourceText.text += "\nIm Deck: " + cardBasicBahavior.isInDeck;

        // Button Text setzen
        // Hauptspieler finden
        NewPlayer mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) return;

        // Karte ist gesperrt
        if (cardBasicBahavior.isLocked)
        {
            buttonText.text = "Freischalten (" + cardBasicBahavior.skillpointsToUnlock + " SP)";
            if (mainPlayer.skillPoints < cardBasicBahavior.skillpointsToUnlock)
            {
                buttonText.text = "Nicht genug SP";
            }
            return;
        }
        // Karte ist im Deck
        if (cardBasicBahavior.isInDeck) { buttonText.text = "Entfernen"; }
        else { buttonText.text = "Hinzufügen"; }
    }

    // Karte per Button hinzufügen oder entfernen
    public void SwitchCard()
    {
        Debug.Log("SwitchCard aufgerufen für: " + cardBasicBahavior.gameObject.name);

        // Karte ist gesperrt
        if (cardBasicBahavior.isLocked)
        {
            NewPlayer mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
            if (mainPlayer == null) return;

            if (mainPlayer.skillPoints >= cardBasicBahavior.skillpointsToUnlock)
            {
                // hier weiter machen. Karte freischalten funktioniert. Skillpoints müssen noch abgezogen werden. 
                mainPlayer.skillPoints -= cardBasicBahavior.skillpointsToUnlock;

                // Skillpunkte im Skillbaum aktualisieren
                SkillTreeManager skillTreeManager = UnityEngine.Object.FindFirstObjectByType<SkillTreeManager>();
                if (skillTreeManager == null) { Debug.Log("skillTreeManager == null"); return; }
                skillTreeManager.SetSkilltree(); // Skillbaum aktualisieren

                // Karte freischalten
                cardBasicBahavior.UnlockCard();
                // Details im Fenster aktualisieren
                SetCardDetailsInWindow(cardPropertiesTest, cardBasicBahavior);
                Debug.Log("Karte freigeschaltet: " + cardBasicBahavior.gameObject.name);

                // Status in der Kartenbibliothek aktualisieren
                foreach (Transform child in cardLibrary.transform)
                {
                    CardBasicBahavior gameobjectBasicBehavior = child.GetComponent<CardBasicBahavior>();
                    if (gameobjectBasicBehavior == null) { Debug.Log("gameobjectBasicBehavior == null"); }
                    if (gameobjectBasicBehavior.cardID == cardBasicBahavior.cardID)
                    {
                        gameobjectBasicBehavior.UnlockCard();// Karte als freigeschaltet markieren                     
                        //SetStatusInLibrary(child.gameObject, gameobjectBasicBehavior.gameObject); // Status der Karte setzen (ob sie im Deck ist oder nicht)
                    }
                }

                StartCoroutine(SetStatusInLibrary()); // Status der Karte setzen (ob sie im Deck ist oder nicht)

                return;
            }
        }

        // Karte ist im Deck
        if (cardBasicBahavior.isInDeck)
        {
            // Karte entfernen
            RemoveCardFromDeck();
            Debug.Log("Karte entfernen: " + cardBasicBahavior.gameObject.name);
        }
        else
        {
            // Karte hinzufügen
            if (cardDeck.Count < unlockedSockets)
            {
                int currentCardPoints = 0;
                // Aktuelle Kartenpunkte im Deck berechnen
                foreach (GameObject cardInDeck in cardDeck)
                {
                    CardPropertiesTest cardPropertiesInDeck = cardInDeck.GetComponent<CardPropertiesTest>();
                    if (cardPropertiesInDeck == null) { Debug.Log("cardPropertiesInDeck == null on card: " + cardInDeck.name); }
                    currentCardPoints += cardPropertiesInDeck.cardValue;
                }

                // Karte aus dem Deck entfernen
                foreach (Transform child in cardLibrary.transform)
                {
                    CardBasicBahavior gameobjectBasicBehavior = child.GetComponent<CardBasicBahavior>();
                    if (gameobjectBasicBehavior == null) { Debug.Log("gameobjectBasicBehavior == null"); }

                    if (gameobjectBasicBehavior.cardID == cardBasicBahavior.cardID)
                    {
                        CardPropertiesTest cardProperties = gameobjectBasicBehavior.gameObject.GetComponent<CardPropertiesTest>();
                        if (cardProperties == null) { Debug.Log("cardProperties == null on card: " + cardProperties.name); }
                        int cardValueRest = cardpointsLimit - currentCardPoints; // verbleibende Kartenpunkte im Deck

                        // Überprüfen, ob die Karte das Kartenpunkte-Limit überschreitet
                        if (cardValueRest >= cardProperties.cardValue)
                        {
                            // Karte hinzufügen
                            Debug.Log("Karte hinzufügen: " + child.name);

                            gameobjectBasicBehavior.CardInDeck(); // Karte als im Deck markieren
                            cardBasicBahavior = gameobjectBasicBehavior; // Lokale Referenz aktualisieren
                            // Details im Fenster aktualisieren
                            SetCardDetailsInWindow(cardPropertiesTest, cardBasicBahavior); // Aktualisiere die Details im Fenster
                            cardDeck.Add(child.gameObject); // Karte zum Deck hinzufügen
                        }
                        else
                        {
                            Debug.Log("Karte " + child.name + " mit Kartenwert " + cardProperties.cardValue +
                                " überschreitet das Kartenpunkte-Limit. Verbleibende Punkte im Deck: " + cardValueRest);
                        }
                    }
                    else { Debug.Log("Karte nicht gefunden: " + child.name); }
                }

                Debug.Log("Karte hinzugefügt: " + cardBasicBahavior.gameObject.name);
                // Deck und Bibliothek neu erstellen
                UpdateLibraryAnDeck();
            }
            else
            {
                Debug.Log("Deck ist voll. Maximal " + unlockedSockets + " Karten erlaubt.");
            }
        }
    }

    private void RemoveCardFromDeck()
    {
        // Karte aus dem Deck entfernen
        foreach (Transform child in cardLibrary.transform)
        {
            CardBasicBahavior gameobjectBasicBehavior = child.GetComponent<CardBasicBahavior>();
            if (gameobjectBasicBehavior == null) { Debug.Log("gameobjectBasicBehavior == null"); }

            if (gameobjectBasicBehavior.cardID == cardBasicBahavior.cardID)
            {
                // Karte entfernen
                Debug.Log("Karte entfernt: " + child.name);
                gameobjectBasicBehavior.CardNotInDeck();
                cardBasicBahavior = gameobjectBasicBehavior;
                // Details im Fenster aktualisieren
                SetCardDetailsInWindow(cardPropertiesTest, cardBasicBahavior);
                cardDeck.Remove(child.gameObject);
            }
            else { Debug.Log("Karte nicht gefunden: " + child.name); }
        }

        // Deck und Bibliothek neu erstellen
        UpdateLibraryAnDeck();
    }

    private void ClearCardLibrary()
    {
        // Alle Kinder des Kartenbibliothek-Elternelements löschen
        foreach (Transform child in cardLibraryParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearCardDeck()
    {
        // Alle Kinder des Kartendeck-Elternelements löschen
        foreach (Transform child in cardDeckParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetCardIDs()
    {
        int i = 1;
        foreach (Transform child in cardLibrary.transform)
        {
            CardBasicBahavior cardBehavior = child.GetComponent<CardBasicBahavior>();
            if (cardBehavior != null)
            {
                cardBehavior.cardID = i; // IDs beginnen bei 1
                i++;
            }
        }
    }

    // Hilfsmethode zum Konvertieren von CardPropertiesTest.ResourceNames zu ResourceType
    //private ResourceType ConvertToResourceType(CardPropertiesTest.Resource resourceName)
    //{
    //    // Annahme: Die Enum-Werte stimmen in Reihenfolge und Bedeutung überein
    //    return (ResourceType)resourceName;
    //}
}

