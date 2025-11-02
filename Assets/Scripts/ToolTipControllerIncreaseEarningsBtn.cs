using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipControllerIncreaseEarningsBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool isHovering = false;

    private void Update()
    {
        if (isHovering)
            UpdateToolTip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        TextDetails();
    }

    private static void TextDetails()
    {
        CardActionDetailsOnScreen.Instance.TextHolder.gameObject.SetActive(true);
        CardModificationManager.Instance.BtnActionCardEarnings();
        CardModificationManager.Instance.toolTipControllerIncreaseEarningsBtnOnEnter = true;

        if (CardModificationManager.Instance.cardEarningsBtn.interactable == true)
        {
            CardActionDetailsOnScreen.Instance.ShowEarnigsInCardEarnings(); // In Karte anzeigen
            CardActionDetailsOnScreen.Instance.ShowCostsIncreasingEarnings(CardModificationManager.Instance.socketCardClone); // Text anzeigen
        }
        else
        {
            CardActionDetailsOnScreen.Instance.ShowNormalEarnings(); // In Karte anzeigen (ursprünglich)
            CardActionDetailsOnScreen.Instance.ShowCostsToHighIncreasingEarnings(CardModificationManager.Instance.socketCardClone); // Text anzeigen
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        CardModificationManager.Instance.tooltipControllerCardLvlBtnOnEnter = false;
        CardActionDetailsOnScreen.Instance.ShowNormalEarnings();
        CardActionDetailsOnScreen.Instance.NoCostText();
    }

    private void UpdateToolTip()
    {
        TextDetails();
    }
}
