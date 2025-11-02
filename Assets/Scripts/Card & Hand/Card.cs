using System.Collections;
using UnityEngine;


public class Card : MonoBehaviour
{
    public static Card Instance { get; private set; }


    private void Awake()
    {
        // Sicher stellen, dass nur eine Instanz existiert
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {

            Destroy(gameObject); // Eine Instanz existiert bereits, diese sollte zerstört werden
            DontDestroyOnLoad(gameObject); // optional: behält die Instanz über Szenenwechsel hinweg 
        }
    }
    // Startmethode *********************************************
    public IEnumerator CreateNewCard()
    {
        yield return StartCoroutine(CreateFromCardDeck());

        yield return StartCoroutine(InstantiateCard());
    }

    private IEnumerator CreateFromCardDeck()
    {
        if (CreateCardDeck.Instance != null)
        {
            CreateCardDeck.Instance.CreateDeckCardsinHand();
        }

        yield return null;
    }

    private IEnumerator InstantiateCard()
    {
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        if (activePlayer.isMainPlayer)
        {
            // neue Karte instazieren
            GameObject cardObject = Instantiate(activePlayer.cardsInHand[activePlayer.cardsInHand.Count - 1].gameObject,
            transform.position, Quaternion.identity, HandController.Instance.handController);

            //neue Karte an SpawnPosition setzen
            if (cardObject.transform.position != HandController.Instance.spawnPosition.transform.position)
            {
                cardObject.transform.position = HandController.Instance.spawnPosition.transform.position;
            }

            //Karte der Liste CardMovement hinzufügen
            activePlayer.instantiatedGameobjects.Add(cardObject);

            CardProperties cardProperties = cardObject.GetComponent<CardProperties>();

            if (cardProperties != null)
            {
                activePlayer.cardsInHand[activePlayer.cardsInHand.Count - 1] = cardProperties;
            }

            //Startposition erreicht
            cardProperties.SetInStartPosition(true); // Karte ist auf Startposition
            yield return null;
        }
    }
}
