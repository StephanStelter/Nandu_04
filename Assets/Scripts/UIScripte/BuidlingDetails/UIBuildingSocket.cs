using UnityEngine;
using UnityEngine.EventSystems;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

public class UIBuildingSocket : MonoBehaviour, IPointerClickHandler
{
    public GameObject referenceSocket;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameMode gameMode = MyEventHandler.Instance.gameMode;

        switch (gameMode)
        {
            //DEFAULT___________________________________________________________________________________________________________________
            case GameMode.Default:

                break;

            //FOUNDVILLAGE______________________________________________________________________________________________________________
            case GameMode.FoundVillage:

                break;

            //PLAYCARD__________________________________________________________________________________________________________________
            case GameMode.PlayCard:

                break;

            //BUILDCARD_________________________________________________________________________________________________________________
            case GameMode.BuildCard:
                //if (referenceSocket.GetComponent<Socket>().isBlocked) { break; }
                //Catch caCa = new Catch(MyEventHandler.Instance.activeHex, MyEventHandler.Instance.chosenCard);
                //MyEventHandler.Instance.chosenSocketUI = referenceSocket;
                //CostHandler.Instance.payCostsForCardIntoSocketNormal(caCa.ChosenCard);  // 5. Kosten abrechnen
                //GameUtils.Instance.AddCardToPlayedListOfPlayer(caCa.ChosenCard);      // 6. Karte in den ersten freien Platz der Liste einfügen
                //HandController.Instance.UpdateCardCosts();
                //StartCoroutine(GameUtils.Instance.ManageCardSocketPlacement(referenceSocket, caCa.ChosenCard));// 7. Neue Kosten für Karten in der Hand berechnen
                //HandController.Instance.RemoveFromHand(caCa.ChosenCard);                   // 11. Card aus der Hand entfernen
                //UIResourceSummary.Instance.UpdateEPR_neu();                                   // 10. ??? (eigentlich Punkt 12)
                //ShowDetailWindowScript.Instance.UpdateSockets(caCa.UIRefrenceHex);              // 11. Socket updaten
                //ShowDetailWindowScript.Instance.UpdateDetailsHex(caCa.UIRefrenceHex);           // 12. Detailfenster aktualisieren
                //CostContentCheck.Instance.MouseWithCardOverObjectExit();                     // 13. Hier wird die Mauskostenanzeige ausgeschaltet
                //HandController.Instance.NewPositionsIE();                                  // 14. Cards in Hand anordnen
                //GameUtils.Instance.ResetToDefault();
                //break;

            //BuildRoad______________________________________________________________________________________________________________
            case GameMode.BuildRoad:

                break;

            //UPGRADESettlement_______________________________________________________________________________________________________
            case GameMode.UpgradeSettlement:

                break;
        }

    }
}
