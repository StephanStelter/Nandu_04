using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSocketScript : MonoBehaviour
{
    public bool isOccupied = false; // Status, ob der Socket besetzt ist

    public void PlaceCard(GameObject card)
    {
        if (!isOccupied)
        {
            card.transform.position = transform.position; // Karte auf die Position des Sockets setzen
            isOccupied = true; // Socket als besetzt markieren
        }
        else
        {
            Debug.Log("Socket ist bereits besetzt!");
        }
    }

    public void RemoveCard()
    {
        if (isOccupied)
        {
            isOccupied = false; // Socket als frei markieren
        }
        else
        {
            Debug.Log("Socket ist bereits frei!");
        }
    }



}
