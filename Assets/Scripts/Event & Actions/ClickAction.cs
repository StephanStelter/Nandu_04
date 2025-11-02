using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public abstract class ClickAction : MonoBehaviour
{
    protected bool isRunning = false;
    public bool IsRunning => isRunning;
    public IEnumerator ExecuteAction(Catch caCa)
    {
        if (isRunning) yield break;
        isRunning = true;
        PerformAction(caCa);
        yield return new WaitForSeconds(0.1f); // Simulierte Dauer
        isRunning = false;
    }
    protected abstract void PerformAction(Catch caCa);
}


public class SelectAction : ClickAction
{
    protected override void PerformAction(Catch caCa)
    {//Debug.Log(caCa.caKey);

        switch (caCa.caKey)
        {
            case string v when v[1] == '1':
                SelectButtonAction();
                break;

            case string v when v[4] == '1':
                break;

            case "100000":
            case "100001":
                SelectHexAction(caCa);
                break;

            case "100100":
            case "100101":
                SelectCardAction(caCa);
                break;

            case "101000":
            case "101001":
                SelectSocketAction(caCa);
                break;

            case "101100":
            case "101101":
            case "101110":
            case "101111":
                SelectedSlottedCardAction(caCa);
                break;

            default:
                Debug.Log("Kein passender Fall." + caCa.caKey);
                break;
        }

    }

    public void SelectHexAction(Catch caCa)
    {//Debug.Log("SelectHexAction");

        GameUtils.Instance.HighlightActiveHex(Color.blue, caCa.CurrentHex);   // 1. Ausgewähltes Hex Highlighten
        DetailWindowManager.Instance.SetUpDetailWindow("HexDetails", caCa);             // 2. UI Anzeigen für Hex/Settlement
    }
    public void SelectCardAction(Catch caCa)
    {//Debug.Log("SelectCardAction");

        GameUtils.Instance.SetCurentCard(caCa.CurrentCard);                   // 0. Setzt die CurrentCard im Eventhandler auf die augewählte Karte       
        GameUtils.Instance.SetGamemodeByCard(caCa.CurrentCard);               // 1. Gamemode setzen je nach Kartentyp     
        HandController.Instance.ResetCardsInHand();                                // 2. Handkarten reseten      
        MouseFollow.Instance.CardOnMousePointer(caCa.CurrentCard);              // 3. Karte an den Mauszeiger hängen // aktuell noch direkt aus dieser Klasse Urspung in CardmouseHandler      
        HandController.Instance.HighlightCardOnClick(caCa.CurrentCard);            // 4. angeklickte Karte hervorheben // aktuell noch direkt aus dieser Klasse Urspung in CardmouseHandler
        DetailWindowManager.Instance.SetUpDetailWindow("CardDetails", caCa);             // 2. UI Anzeigen für Hex/Settlement

    }
    public void SelectSocketAction(Catch caCa)
    {//Debug.Log("SelectSocketAction");

        GameUtils.Instance.HighlightActiveHex(Color.blue, caCa.CurrentHex);   // 1. Ausgewähltes Hex Highlighten     
        DetailWindowManager.Instance.SetUpDetailWindow("SocketDetails", caCa);             // 2. UI Anzeigen für Hex/Settlement
    }
    public void SelectButtonAction()
    {//Debug.Log("SelectButtonAction");

    }
    public void SelectedSlottedCardAction(Catch caCa)
    {//Debug.Log("SelectedSlottedCardAction");

        CardModificationManager.Instance.StartCardOverlayForModification(caCa.CurrentCard); // 1. UI anzeigen und Karte übergeben
    }
    public void SelectHexForUpgrade(Catch caCa)
    {//Debug.Log("SelectHexForUpgrade");

        GameUtils.Instance.SelectHexForUpgrade(caCa);
    }
}


public class FoundingAction : ClickAction
{
    protected override void PerformAction(Catch caCa)
    {//Debug.Log(caCa.caKey);

        switch (caCa.caKey)
        {
            case "100000":
            case "100001":
            case "101000":
            case "101001":
                FoundVillageAction(caCa);
                break;

            case "100100":
            case "100101":
            case "101100":
            case "101101":
                SelectCardAction(caCa);
                break;

            case string v when v[1] == '1': // z. B. "11000"
                SelectButtonAction();
                break;

            default:
                Debug.Log("Kein passender Fall.");
                break;
        }

    }

    public void FoundVillageAction(Catch caCa)
    {//Debug.Log("FoundVillageAction");

        StructureManager.Instance.FoundSettlement(caCa.CurrentHex);                  // 1. Die gesamte Founding Logik einschlieöich Siedlung erstellen und Resourcen Felder erstellen    
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText();                                   // 4. UI Anzeigen
        DetailWindowManager.Instance.SetUpDetailWindow("SettlementDetails", caCa);
        GameUtils.Instance.ResetToDefault();
    }
    public void SelectCardAction(Catch caCa)
    {//Debug.Log("SelectCardAction");

        GameUtils.Instance.SetGamemodeByCard(caCa.CurrentCard);               // 1. Gamemode setzen je nach Kartentyp       
        HandController.Instance.ResetCardsInHand();                                // 2. Handkarten reseten        
        MouseFollow.Instance.CardOnMousePointer(caCa.CurrentCard);              // 3. Karte an den Mauszeiger hängen // aktuell noch direkt aus dieser Klasse Urspung in CardmouseHandler       
        HandController.Instance.HighlightCardOnClick(caCa.CurrentCard);            // 4. angeklickte Karte hervorheben// aktuell noch direkt aus dieser Klasse Urspung in CardmouseHandler     
        DetailWindowManager.Instance.SetUpDetailWindow("CardDetails", caCa);             // 2. UI Anzeigen für Hex/Settlement
    }
    public void SelectButtonAction()
    {//Debug.Log("SelectButtonAction");

        HandController.Instance.ResetCardsInHand();                                // 1. Handkartenzurücksetzena
        GameUtils.Instance.ResetHighlighting();                               // 2. Highlighting zurücksetzen 
    }
}


public class BuildAction : ClickAction
{
    protected override void PerformAction(Catch caCa)
    {
        if (MyEventHandler.Instance.cardReleaseIntoSocket == true || MyEventHandler.Instance.cardReleaseCardOnCardLvl == true || caCa.caKey == "10010")
        {
            switch (caCa.caKey)
            {
                case "100000":
                case "100001":
                    BuildOnHexAction(caCa);
                    break;

                case "100100" when MyEventHandler.Instance.chosenCard != null:
                case "100101" when MyEventHandler.Instance.chosenCard != null:
                    SelectCardAction(caCa);
                    break;

                case "101100":
                case "101101":
                    SelectForLevelAction(caCa);
                    break;

                case "101000":
                case "101001":
                    BuildOnSocketAction(caCa);
                    break;

                case string v when v[1] == '1':
                    SelectButtonAction();
                    break;

                default:
                    Debug.Log("Kein passender Fall." + caCa.caKey);
                    break;
            }
        }
        else
        {
            Debug.Log(caCa.caKey);
            Debug.Log("Kann nicht platziert werden");
            StartCoroutine(CardActionDetailsOnScreen.Instance.ShowIdividuellText("Du Arsch!!!"));
        }
    }

    public void BuildOnHexAction(Catch caCa)
    {
        GameUtils.Instance.HighlightActiveHex(Color.blue, caCa.CurrentHex);
        CostHandler.Instance.PayCostsForCard(caCa.ChosenCard); // NEU
        GameUtils.Instance.AddCardToPlayedListOfPlayer(caCa.ChosenCard);
        HandController.Instance.UpdateCardCosts();
        GameUtils.Instance.PlaceCardInSocket(caCa.CurrentHex.gameObject, caCa.ChosenCard);
        HandController.Instance.RemoveFromHand(caCa.ChosenCard);
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText();
        CostContentCheck.Instance.MouseWithCardOverObjectExit();
        HandController.Instance.NewPositionsIE();
        DetailWindowManager.Instance.SetUpDetailWindow("SettlementDetails", caCa);
        GameUtils.Instance.ResetToDefault();
    }
    public void SelectCardAction(Catch caCa)
    {
        GameUtils.Instance.SetGamemodeByCard(caCa.CurrentCard);
        HandController.Instance.ResetCardsInHand();
        MouseFollow.Instance.CardOnMousePointer(caCa.CurrentCard);
        HandController.Instance.HighlightCardOnClick(caCa.CurrentCard);
        DetailWindowManager.Instance.SetUpDetailWindow("CardDetails", caCa);
    }
    public void SelectForLevelAction(Catch caCa)
    {
        GameUtils.Instance.HighlightActiveHex(Color.blue, caCa.CurrentHex);
        CostHandler.Instance.PayCostsForSocketCardsLvl(caCa.CurrentCard); // NEU
        GameUtils.Instance.CardOnCardLvlAction(caCa.CurrentCard, caCa.ChosenCard);
        GameUtils.Instance.UpdateResourceOnHex(caCa.CurrentHex);
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText();
        HandController.Instance.RemoveFromHand(caCa.ChosenCard);
        GameUtils.Instance.DestroyUsedCard(caCa.ChosenCard);
        CostContentCheck.Instance.MouseWithCardOverObjectExit();
        HandController.Instance.NewPositionsIE();
        GameUtils.Instance.ResetToDefault();
    }
    public void BuildOnSocketAction(Catch caCa)
    {
        GameUtils.Instance.HighlightActiveHex(Color.blue, caCa.CurrentHex);
        CostHandler.Instance.PayCostsForCard(caCa.ChosenCard); // NEU
        GameUtils.Instance.AddCardToPlayedListOfPlayer(caCa.ChosenCard);
        HandController.Instance.UpdateCardCosts();
        GameUtils.Instance.PlaceCardInSocket(caCa.CurrentSocket, caCa.ChosenCard);
        HandController.Instance.RemoveFromHand(caCa.ChosenCard);
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText();
        CostContentCheck.Instance.MouseWithCardOverObjectExit();
        HandController.Instance.NewPositionsIE();
        DetailWindowManager.Instance.SetUpDetailWindow("SocketDetails", caCa);
        GameUtils.Instance.ResetToDefault();
    }
    public void SelectButtonAction()
    {
        HandController.Instance.ResetCardsInHand();
        GameUtils.Instance.ClearParent(GameObject.Find("Card on Mouse Pointer"));
        CostContentCheck.Instance.MouseWithCardOverObjectExit();
    }
}


public class DecreeAction : ClickAction
{
    [SerializeField] Transform actionCardForDelete;
    GameObject cardToWorkWith;

    protected override void PerformAction(Catch caCa)
    {//Debug.Log(caCa.caKey);

        switch (caCa.caKey)
        {
            case "100000":
            case "100001":
            case "101000":
            case "101001":
                PlayDecreeAction(caCa);
                break;

            case "100100":
            case "100101":
            case "101100":
            case "101101":
                SelectCardAction(caCa);
                break;

            case string v when v[1] == '1':
                SelectButtonAction();
                break;

            default:
                Debug.Log("Kein passender Fall.");
                break;
        }

    }

    public void PlayDecreeAction(Catch caCa)
    {//Debug.Log("PlayDecreeAction);

        DetailWindowManager.Instance.Hide(); // 1. Hier wird das UI geschlossen – wurde so aus alter Logik übernommen, weiß nicht, ob noch notwendig
        HandleLeftClickAction(caCa.CurrentHex, caCa.ChosenCard); // 2. Zunächst noch einfach aufgerufen wie es vorher war... muss noch angepasst werden
        GameUtils.Instance.ResetToDefault();
    }
    public void SelectCardAction(Catch caCa)
    {//Debug.Log("SelectCardAction");

        GameUtils.Instance.SetGamemodeByCard(caCa.CurrentCard);               // 1. Gamemode setzen je nach Kartentyp
        HandController.Instance.ResetCardsInHand();                                // 2. Handkarten reseten
        MouseFollow.Instance.CardOnMousePointer(caCa.CurrentCard);              // 3. Karte an den Mauszeiger hängen – aktuell noch direkt aus dieser Klasse, Ursprung in CardMouseHandler
        HandController.Instance.HighlightCardOnClick(caCa.CurrentCard);            // 4. Angeklickte Karte hervorheben – aktuell noch direkt aus dieser Klasse, Ursprung in CardMouseHandler
        DetailWindowManager.Instance.SetUpDetailWindow("CardDetails", caCa);             // 2. UI Anzeigen für Hex/Settlement
    }
    public void SelectButtonAction()
    {//Debug.Log("SelectButtonAction");

        HandController.Instance.ResetCardsInHand();
    }

    //Ab hier sind in dieser Klasse nur noch hilfsMethoden die Später in einem Cardhandler abgelegt werden, sind aber Funktionstüchtig und sind für das aktuelle Ausführen des Threads unerlässlich (30.03.2025)
    public void HandleLeftClickAction(Transform hexTransform, GameObject card)
    {
        List<Hex> areaHex = GameUtils.Instance.GetAreaHex(hexTransform.position, 1000);
        NewPlayer activePlayer = TurnManager.Instance.GetCurrentPlayer();

        // Gespielte Karte aus der Hand suchen
        foreach (CardProperties cardInHand in activePlayer.cardsInHand)
        {
            if (cardInHand.isSelected)
            {
                cardToWorkWith = cardInHand.gameObject;
                break; // Nur das erste gefundene Objekt wird verwendet
            }
        }

        CardProperties cardProps = cardToWorkWith.GetComponent<CardProperties>();
        string actionArea = cardProps.GetActionChangeArea();

        // === Action: Geologe ===
        if (cardProps.isGeologist && actionArea == ActionChangeArea.MEPR.ToString())
        {
            foreach (Hex hex in areaHex)
            {
                if (cardProps.isReroll)
                {
                    if (cardProps.isRerollAllRecources)
                    {
                        //hex.GenerateAllResourceAmounts(true); // Alle bekannten Resourcen neu würfeln
                    }
                    else
                    {
                        //hex.GenerateResourceAmount(cardProps.resourceNameToReroll.ToString(), true); // Eine Resource neu würfeln
                    }
                }
                else
                {
                    //hex.GenerateAllResourceAmounts(false); // Alle Resourcen das erste Mal würfeln
                }
            }
        }
        // === Sonstige Actions ===
        else
        {
            foreach (Hex hex in areaHex)
            {
                if (actionArea == ActionChangeArea.MEPR.ToString())
                {
                    hex.ChangeListMEPR(cardProps.resourceEarningList); // Resourcenliste aktualisieren
                }

                if (actionArea == ActionChangeArea.EPR.ToString())
                {
                    EPRAction(cardProps, hex); // Karten in Slots auf Hexfeld ändern
                }
            }
        }

        StartCoroutine(EndOfActionMethod(cardProps));
    }

    private static void EPRAction(CardProperties cardProps, Hex hex)
    {
        CardProperties[] cardPropertiesArray = hex.GetComponentsInChildren<CardProperties>();

        if (cardPropertiesArray.Length > 0)
        {
            foreach (CardProperties cardProperties in cardPropertiesArray)
            {
                cardProperties.ChangeResourceEarningList(cardProps.resourceEarningList); // Werte auf Karten in Slots �ndern
            }
        }

        hex.ChangeListeEPR(); // EPR Liste auf Hexfeld aktualisieren
    }
    private IEnumerator EndOfActionMethod(CardProperties cardProps)
    {
        cardProps.transform.SetParent(actionCardForDelete, true);
        HandController.Instance.RemoveFromHand(cardProps.gameObject);                //Karte von Hand entfernen
        HandController.Instance.NewPositionsIE();
        GameUtils.Instance.ResetToDefault();

        Destroy(cardProps.gameObject);
        yield return new WaitForSeconds(1f);
    }
}


public class BuildRoadAction : ClickAction
{
    protected override void PerformAction(Catch caCa)
    {//Debug.Log(caCa.caKey);

        switch (caCa.caKey)
        {
            case "100000":
            case "100001":
            case "101000":
            case "101001":
                PlaceRoadAction(caCa);
                break;

            case "100100":
            case "100101":
            case "101100":
            case "101101":
                SelectCardAction(caCa);
                break;

            case string v when v[1] == '1': // z.B. "11000"
                SelectButtonAction();
                break;

            default:
                Debug.Log("Kein passender Fall.");
                break;
        }

    }

    public void PlaceRoadAction(Catch caCa)
    {//Debug.Log("PlaceRoadAction");

        StructureManager.Instance.CreateRoadOnHex(caCa.CurrentHex);                      // 1. Die gesamte Founding-Logik einschließlich Siedlung erstellen und Resourcenfelder erstellen
        DetailWindowManager.Instance.SetUpDetailWindow("RoadDetails", caCa);                     // 2. UI Anzeigen für Hex/Settlement
        GameUtils.Instance.ResetToDefault();
    }
    public void SelectCardAction(Catch caCa)
    {//Debug.Log("SelectCardAction");

        GameUtils.Instance.SetGamemodeByCard(caCa.CurrentCard);                   // 1. Gamemode setzen je nach Kartentyp
        HandController.Instance.ResetCardsInHand();                                    // 2. Handkarten reseten
        MouseFollow.Instance.CardOnMousePointer(caCa.CurrentCard);                  // 3. Karte an den Mauszeiger hängen – aktuell noch direkt aus dieser Klasse, Ursprung in CardMouseHandler
        HandController.Instance.HighlightCardOnClick(caCa.CurrentCard);                // 4. Angeklickte Karte hervorheben – aktuell noch direkt aus dieser Klasse, Ursprung in CardMouseHandler
        DetailWindowManager.Instance.SetUpDetailWindow("CardDetails", caCa);             // 2. UI Anzeigen für Hex/Settlement
    }
    public void SelectButtonAction()
    {//Debug.Log("SelectButtonAction");

        HandController.Instance.ResetCardsInHand();                                    // 1. Handkartenzurücksetzen
    }
}

public class UpgradeSettlementAction : ClickAction
{
    protected override void PerformAction(Catch caCa)
    {//Debug.Log(caCa.caKey);

        switch (caCa.caKey)
        {
            case string v when v[1] == '1': // z. B. "11000"
                SelectButtonAction(caCa);
                break;

            case string v when v[5] == '1': // z. B. "11000"
                SelectChipAction(caCa);
                break;

            case "100000":
            case "101000":
                SelectHexAction(caCa);
                break;

            case "100100":
            case "101100":
                SelectCardAction(caCa);
                break;

            default:
                Debug.Log("Kein passender Fall.");
                break;
        }
    }

    public void SelectChipAction(Catch caCa) // muss noch überarbietet werden
    {//Debug.Log("SelectChipAction");

        StructureManager.Instance.UpgradeSettlement(caCa.CurrentChip);                   // 1. Settlement upgraden
        StructureManager.Instance.HideOptionsForUpgrade(caCa.ChosenHexUpdate);           // 2. Chips verstecken
        GameUtils.Instance.ResetToDefault();                                      // 3. GameMode auf Default setzen
        DetailWindowManager.Instance.SetUpDetailWindow("SettlementDetails", caCa);          // 2. UI Anzeigen für Hex/Settlement
    }
    public void SelectHexAction(Catch caCa) // muss noch überarbietet werden
    {//Debug.Log("SelectHexAction");

        StructureManager.Instance.HideOptionsForUpgrade(caCa.ChosenHexUpdate);           // 1. Chips verstecken
        GameUtils.Instance.HighlightActiveHex(Color.blue, caCa.CurrentHex);       // 2. Ausgewähltes Hex highlighten
        DetailWindowManager.Instance.SetUpDetailWindow("HexDetails", caCa);             // 2. UI Anzeigen für Hex/Settlement
    }
    public void SelectCardAction(Catch caCa) // muss noch überarbietet werden
    {//Debug.Log("SelectCardAction");

        StructureManager.Instance.HideOptionsForUpgrade(caCa.ChosenHexUpdate);   // 1. Chips verstecken
        GameUtils.Instance.SetGamemodeByCard(caCa.CurrentCard);                   // 2. Gamemode setzen je nach Kartentyp
        HandController.Instance.ResetCardsInHand();                                    // 3. Handkarten reseten
        MouseFollow.Instance.CardOnMousePointer(caCa.CurrentCard);                  // 4. Karte an den Mauszeiger hängen (aktuell noch direkt aus dieser Klasse, Ursprung in CardmouseHandler)
        HandController.Instance.HighlightCardOnClick(caCa.CurrentCard);                // 5. angeklickte Karte hervorheben (aktuell noch direkt aus dieser Klasse, Ursprung in CardmouseHandler)
        DetailWindowManager.Instance.SetUpDetailWindow("CardDetails", caCa);             // 2. UI Anzeigen für Hex/Settlement
    }
    public void SelectButtonAction(Catch caCa) // muss noch überarbitet werden
    {//Debug.Log("SelectButtonAction");

        StructureManager.Instance.HideOptionsForUpgrade(caCa.ChosenHexUpdate);   // 1. Chips verstecken
        HandController.Instance.ResetCardsInHand();                                    // 2. Handkartenzurücksetzen
        GameUtils.Instance.ResetToDefault();                                      // 3. GameMode auf Default setzen
    }
}

public class ReturnToDefaultAction : ClickAction
{
    protected override void PerformAction(Catch caCa)
    {
        HandController.Instance.ResetCardsInHand();                                    // 1. Handkartenzurücksetzen
        GameUtils.Instance.ResetToDefault();                                      // 3. GameMode auf Default setzen
        DetailWindowManager.Instance.Hide();                                                // 5. UI schließen
        SkilltreeAndBonusesHandler.Instance.CloseSkilltree();          // 6. Skilltree und Bonus Fenster schließen
    }
}
public class ResetSelectAction : ClickAction
{
    protected override void PerformAction(Catch caCa)
    {//Debug.Log("Reset Select Action ausgeführt!");

        DetailWindowManager.Instance.Hide();                                                // 1. UI zurücksetzen
        GameUtils.Instance.ResetToDefault();                                      // 3. GameMode auf Default setzen
    }
}
public class ResetCardAction : ClickAction
{
    protected override void PerformAction(Catch caCa)
    {//Debug.Log("Karte zurückgesetzt!");

    }
}

