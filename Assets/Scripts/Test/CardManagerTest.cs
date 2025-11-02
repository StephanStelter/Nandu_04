using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManagerTest : MonoBehaviour
{

    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform spawnPointTransform;
    [SerializeField] private Transform handPositionTransform;
    [SerializeField] private Transform deletePointTransform;
    [SerializeField] private GameObject cardsInHandGameobject;

    public GameObject instatiatedCardObject = null;

    public List<GameObject> cardsInHand;
    public List<Vector3> cardPositionsInHand;

    public int cardAmountInHand = 5;

    private int cardOffset = 10;


    // Start is called before the first frame update
    void Start()
    {
        if (spawnPointTransform == null)
        {
            Debug.LogError("Kein Spanwpoint gefunden");

            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("Kein CardPrefab gefunden");

            return;
        }

        if (cardsInHandGameobject == null)
        {
            Debug.LogError("Kein cardsInHandGameobject gefunden");

            return;
        }
        cardsInHand = new List<GameObject>();

        cardPositionsInHand = new List<Vector3>();

        //SetCardPositionsInHand();

        StartCoroutine(InstantiateCardsInHand());
    }

    private IEnumerator InstantiateCardsInHand()
    {
        int cardPositionOffset = 0;

        for (int i = 0; i < cardAmountInHand; i++)
        {
            InstatiateCard(i, cardPositionOffset);

            yield return new WaitForSeconds(.5f);

            cardPositionOffset += cardOffset;
        }

    }

    private void InstatiateCard(int i, int cardOffset)
    {
        //Erzeugen an SpawnPosition
        instatiatedCardObject = Instantiate(cardPrefab, spawnPointTransform.transform.position, transform.rotation);

        //Neues Parent
        instatiatedCardObject.transform.SetParent(cardsInHandGameobject.transform, true);

        CardMoveInHands cardMoveInHands = instatiatedCardObject.GetComponent<CardMoveInHands>();

        if (cardMoveInHands == null)
        {
            Debug.LogError("Kein cardMoveInHands gefunden");

            return;
        }

        // Karte hinzufügen
        if (i < cardsInHand.Count)
        {
            cardsInHand[i] = instatiatedCardObject;
        }
        else
        {
            cardsInHand.Add(instatiatedCardObject);
        }

        //Kartenposition mit Offset übergeben und Karten bewegen
        cardMoveInHands.SetHandPosition(handPositionTransform, cardOffset);

        //KartenLöschpunkt übergebebn
        DeleteCard deleteCard = instatiatedCardObject.GetComponent<DeleteCard>();

        if (deleteCard == null)
        {
            Debug.LogError("Kein deleteCard gefunden");

            return;
        }

        deleteCard.SetDeletePoint(deletePointTransform);
    }

    public void DeleteCards()
    {
        StartCoroutine(DeleteCardsCoroutine());
    }

    private IEnumerator DeleteCardsCoroutine()
    {
        for (int i = cardsInHand.Count - 1; i >= 0; i--)
        {
            //Skript zum löschen finden
            DeleteCard deleteCard = cardsInHand[i].gameObject.GetComponent<DeleteCard>();

            if (deleteCard == null)
            {
                Debug.LogError("Kein deleteCard gefunden");

                continue;
            }

            //Karte aus Liste entfernen
            cardsInHand.RemoveAt(i);

            deleteCard.startDeleteCards = true;

            yield return new WaitForSeconds(.5f);
        }
    }

    //private void SetCardPositionsInHand()
    //{
    //    cardPositionsInHand.Clear();

    //    int startPosition;

    //    //int cardPositionStart = cardOffset * cardsInHand.Count;

    //    if (IstGerade(5))
    //    {
    //        int cardPositionStart = cardAmountInHand / 2;

    //        startPosition = - cardPositionStart * cardOffset;
    //    }

    //    for (int i = 0; i < cardAmountInHand; i++)
    //    {
    //        //Vector3 newPosition = new Vector3(startPosition, handPositionTransform.position.y, handPositionTransform.position.z);
    //        cardPositionsInHand.Add(newPosition);
    //    }
    //}

    public bool IstGerade(int number)
    {
        return number % 2 == 0;
    }

}
