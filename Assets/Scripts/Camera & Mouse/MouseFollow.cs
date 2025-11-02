using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardProperties;

public class MouseFollow : MonoBehaviour
{
    public GameObject card;  // Karte für Mauszeiger
    public static MouseFollow Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }
    private void Update()
    {
        transform.position = GameUtils.Instance.GetRaycastHit().point;
    }

    public void CardOnMousePointer(GameObject clickedCard)
    {
        card = Instantiate(clickedCard, new Vector3(0f, 0f, 0f), Quaternion.identity);

        CardScaler.Instance.StartScaleRoutine(card, new Vector3(2.5f, 2.5f, 2.5f), .1f);

        card.transform.SetParent(transform, false);
        card.transform.localPosition = new Vector3(-5f, 0f, -5f); // Karte am Mauszeiger ausrichten

        //Skript deaktivieren
        DeactivateCardComponents(card);
    }

    //Für mich später eher bei Card

    public void DeactivateCardComponents(GameObject card)
    {
        CardProperties cardToDeactivate = card.GetComponent<CardProperties>();
        BoxCollider colliderToDeactivate = card.GetComponent<BoxCollider>();
        BoxCollider[] colliderToDeactivateArray = card.GetComponentsInChildren<BoxCollider>();

        cardToDeactivate.enabled = false;
        colliderToDeactivate.enabled = false;

        foreach (BoxCollider boxCollider in colliderToDeactivateArray)
        {
            boxCollider.enabled = false;
        }

    }
}
