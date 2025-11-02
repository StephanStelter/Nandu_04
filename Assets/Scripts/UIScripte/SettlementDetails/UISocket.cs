using UnityEngine;
using UnityEngine.EventSystems;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

public class UISocket : MonoBehaviour, IPointerClickHandler
{
    public GameObject referenceSocket;
    [Header("UI Setups")]
    public GameObject uiBuildDetails;
    public GameObject uiSettlementDetails;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameMode gameMode = MyEventHandler.Instance.gameMode;

        switch (gameMode)
        {
            //DEFAULT___________________________________________________________________________________________________________________
            case GameMode.Default:
                if (uiBuildDetails != null) uiBuildDetails.SetActive(true);
                StartCoroutine(DeactivateSelfNextFrame());
                break;

            //FOUNDVILLAGE______________________________________________________________________________________________________________
            case GameMode.FoundVillage:
                break;

            //PLAYCARD__________________________________________________________________________________________________________________
            case GameMode.PlayCard:
                break;

            //BUILDCARD_________________________________________________________________________________________________________________
            case GameMode.BuildCard:
                if (referenceSocket.GetComponent<Socket>().isBlocked) { break; }
                Catch caCa = new Catch(MyEventHandler.Instance.activeHex, MyEventHandler.Instance.chosenCard);
                MyEventHandler.Instance.chosenSocketUI = referenceSocket;
                CostHandler.Instance.PayCostsForCard(caCa.ChosenCard);  // NEU: Kosten abrechnen
                GameUtils.Instance.AddCardToPlayedListOfPlayer(caCa.ChosenCard);
                HandController.Instance.UpdateCardCosts();
                StartCoroutine(GameUtils.Instance.ManageCardSocketPlacement(referenceSocket, caCa.ChosenCard));
                HandController.Instance.RemoveFromHand(caCa.ChosenCard);
                UIResourceSummary.Instance.UpdateMainPlayerSummaryText();
                DetailWindowManager.Instance.UpdateSockets(caCa.UIRefrenceHex);
                DetailWindowManager.Instance.UpdateDetailsHex(caCa.UIRefrenceHex);
                CostContentCheck.Instance.MouseWithCardOverObjectExit();
                HandController.Instance.NewPositionsIE();
                GameUtils.Instance.ResetToDefault();
                break;

            //BuildRoad______________________________________________________________________________________________________________
            case GameMode.BuildRoad:
                break;

            //UPGRADESettlement_______________________________________________________________________________________________________
            case GameMode.UpgradeSettlement:
                break;
        }
    }

    private System.Collections.IEnumerator DeactivateSelfNextFrame()
    {
        uiSettlementDetails.SetActive(false);
        yield return null;
    }
}
