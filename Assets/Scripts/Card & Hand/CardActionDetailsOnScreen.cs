using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CardProperties;

public class CardActionDetailsOnScreen : MonoBehaviour
{
    public static CardActionDetailsOnScreen Instance { get; private set; }

    public TextMeshProUGUI costText;

    private string addCardInSocket = "Karte in freien Sockel ablegen.";
    private string caseCardInSocketOnSocket = "Karte in diesen Sockel ablegen.";
    private string addCardNotInSocketSocket = "Kein freier Sockel!";
    private string addCardNotInSuitableSocket = "Kein geeignerter Sockel!";
    private string addCardNotInSocketCosts = "Nicht genügend Resourcen!";
    private string AddCardForCardLvlNormal = "Karte aufwerten";
    private string noCardLvlNormalCosts = "Nicht genügend Resourcen!";
    private string noCardLvlNormal = "Aufwerten nicht möglich";
    private string earningsCosts = "Erträge steigern";
    private string earningsCostsToHeigh = "Nicht genügend XP!";

    private bool waitBool = false;

    public Transform TextHolder;
    public RectTransform TextHolderRect;
    private float rectHeightPerRow = 3.2f; // Zeilenhöhe
    private int rowCounter = 0;

    private float rectWidthNormal = 32;
    private float rectWidthShort = 25;
    private float rectWidthVeryShort = 19;

    public GameObject myEventHandlerGameObject;


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        TextHolder.gameObject.SetActive(false);

        MyEventHandler myEventHandler = myEventHandlerGameObject.GetComponent<MyEventHandler>();
        if (myEventHandler == null) { Debug.Log("myEventHandler == null"); }
    }

    public void ShowAddCardInSocketHex(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = addCardInSocket + "\n";
            rowCounter++;

            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            foreach (ResourceCosts resource in cardProperties.resourceCostsList)
            {
                costText.text += resource.resourceName + ": " + resource.resourceAmount + "\n";
                rowCounter++;
            }

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowNoEmptySocket()
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = "<color=red>" + addCardNotInSocketSocket + "</color>\n";
            rowCounter++;

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthVeryShort, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowNotSuitableSocket()
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = "<color=red>" + addCardNotInSuitableSocket + "</color>\n";
            rowCounter++;

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthShort, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowNoAddCardCostsToHeight(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = addCardNotInSocketCosts + "\n";
            rowCounter++;

            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            foreach (ResourceCosts resource in cardProperties.resourceCostsList)
            {
                if (CostHandler.Instance.problemCosts.Contains(resource.resourceName.ToString())) // Kosten die NICHT gedeckt sind ROT anzeigen
                {
                    costText.text += resource.resourceName + ": " + "<color=red>" + resource.resourceAmount + "</color>\n";
                    rowCounter++;
                }
                else
                {
                    costText.text += resource.resourceName + ": " + resource.resourceAmount + "\n"; // Kosten die gedeckt sind normal anzeigen
                    rowCounter++;
                }
            }

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowAddCardInSocket(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = caseCardInSocketOnSocket + "\n";
            rowCounter++;

            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            foreach (ResourceCosts resource in cardProperties.resourceCostsList)
            {
                costText.text += resource.resourceName + ": " + resource.resourceAmount + "\n";
                rowCounter++;
            }

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowAddCardLvl(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = AddCardForCardLvlNormal + "\n";
            rowCounter++;

            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            foreach (ResourceCosts resource in cardProperties.resourceCostsListLvlCardOnCard)
            {
                if (CostHandler.Instance.problemCosts.Contains(resource.resourceName.ToString())) // Kosten die NICHT gedeckt sind ROT anzeigen
                {
                    costText.text += resource.resourceName + ": " + "<color=red>" + resource.resourceAmount + "</color>\n";
                    rowCounter++;
                }
                else
                {
                    costText.text += resource.resourceName + ": " + resource.resourceAmount + "\n"; // Kosten die gedeckt sind normal anzeigen
                    rowCounter++;
                }
            }

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowNoAddCardLvl()
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = "<color=red>" + noCardLvlNormal + "</color>\n";
            rowCounter++;

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowAddCardLvlPerBtn(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = AddCardForCardLvlNormal + "\n";
            rowCounter++;

            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            foreach (ResourceCosts resource in cardProperties.resourceCostsListLvlCardNormal)
            {
                if (CostHandler.Instance.problemCosts.Contains(resource.resourceName.ToString())) // Kosten die NICHT gedeckt sind ROT anzeigen
                {
                    costText.text += resource.resourceName + ": " + "<color=red>" + resource.resourceAmount + "</color>\n";
                    rowCounter++;
                }
                else
                {
                    costText.text += resource.resourceName + ": " + resource.resourceAmount + "\n"; // Kosten die gedeckt sind normal anzeigen
                    rowCounter++;
                }
            }

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
        }
    }

    public void ShowEarnigsInCard()
    {
        if (!waitBool)
        {
            CardProperties cardProperties = CardModificationManager.Instance.socketCardClone.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            cardProperties.CardLvlPreview(cardProperties);
        }
    }

    public void ShowNormalEarnings()
    {
        if (!waitBool)
        {
            CardProperties cardProperties = CardModificationManager.Instance.socketCardClone.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            cardProperties.DisplayCardEarnings();
        }
    }

    public void ShowNoCardLvlNormalCostsToHeight(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = addCardNotInSocketCosts + "\n";
            rowCounter++;

            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            foreach (ResourceCosts resource in cardProperties.resourceCostsListLvlCardNormal)
            {
                if (CostHandler.Instance.problemCosts.Contains(resource.resourceName.ToString())) // Kosten die NICHT gedeckt sind ROT anzeigen
                {
                    costText.text += resource.resourceName + ": " + "<color=red>" + resource.resourceAmount + "</color>\n";
                    rowCounter++;
                }
                else
                {
                    costText.text += resource.resourceName + ": " + resource.resourceAmount + "\n"; // Kosten die gedeckt sind normal anzeigen
                    rowCounter++;
                }
            }

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
        }
    }

    public void ShowAddCardLvlCostsToHeigh(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            StartingPosition(); // Ausgangsposition

            costText.text = noCardLvlNormalCosts + "\n";
            rowCounter++;

            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            foreach (ResourceCosts resource in cardProperties.resourceCostsListLvlCardOnCard)
            {
                if (CostHandler.Instance.problemCosts.Contains(resource.resourceName.ToString())) // Kosten die NICHT gedeckt sind ROT anzeigen
                {
                    costText.text += resource.resourceName + ": " + "<color=red>" + resource.resourceAmount + "</color>\n";
                    rowCounter++;
                }
                else
                {
                    costText.text += resource.resourceName + ": " + resource.resourceAmount + "\n"; // Kosten die gedeckt sind normal anzeigen
                    rowCounter++;
                }
            }

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowCostsIncreasingEarnings(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            StartingPosition(); // Ausgangsposition

            costText.text = earningsCosts + "\n";
            rowCounter++;
            costText.text += cardProperties.xpEarningsCosts + " XP" + "\n";
            rowCounter++;

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowCostsToHighIncreasingEarnings(GameObject cardToWorkWithGameobject)
    {
        if (!waitBool)
        {
            CardProperties cardProperties = cardToWorkWithGameobject.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            StartingPosition(); // Ausgangsposition

            costText.text = earningsCostsToHeigh + "\n";
            rowCounter++;
            costText.text += "<color=red>" + cardProperties.xpEarningsCosts + " XP" + "</color>\n";
            rowCounter++;

            //Anzeigen
            TextHolderRect.sizeDelta = new Vector2(rectWidthNormal, (rectHeightPerRow * rowCounter));
            TextHolder.gameObject.SetActive(true);
        }
    }

    public void ShowEarnigsInCardEarnings()
    {
        if (!waitBool)
        {
            CardProperties cardProperties = CardModificationManager.Instance.socketCardClone.GetComponent<CardProperties>();
            if (cardProperties == null) { Debug.Log("Keine CardProperties"); }

            cardProperties.CardIncreasingEarningsPreview(cardProperties);
        }
    }

    private void StartingPosition()
    {
        costText.text = string.Empty;
        rowCounter = 0;
    }

    public IEnumerator ShowIdividuellText(string textToShow)
    {
        rowCounter = 0;
        waitBool = true;
        costText.text = string.Empty;
        yield return new WaitForSeconds(.1f);

        costText.text = "<color=red>" + textToShow + "</color>\n";
        rowCounter++;
        TextHolderRect.sizeDelta = new Vector2(rectWidthVeryShort, (rectHeightPerRow * rowCounter));
        TextHolder.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        waitBool = false;
        TextHolder.gameObject.SetActive(false);
    }

    // Text wieder ausblenden
    public void NoCostText()
    {
        TextHolder.gameObject.SetActive(false);
    }
}
