using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipControllerCardLvlBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
        CardModificationManager.Instance.BtnActionCardLvl();
        CardModificationManager.Instance.tooltipControllerCardLvlBtnOnEnter = true;

        if (CardModificationManager.Instance.cardLvlBtn.interactable == true)
        {
            CardActionDetailsOnScreen.Instance.ShowEarnigsInCard(); // In Karte anzeigen
            CardActionDetailsOnScreen.Instance.ShowAddCardLvlPerBtn(CardModificationManager.Instance.socketCardClone); // Text anzeigen
        }
        else
        {
            CardActionDetailsOnScreen.Instance.ShowNormalEarnings(); // In Karte anzeigen (ursprünglich)
            CardActionDetailsOnScreen.Instance.ShowNoCardLvlNormalCostsToHeight(CardModificationManager.Instance.socketCardClone); // Text anzeigen
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        CardModificationManager.Instance.toolTipControllerIncreaseEarningsBtnOnEnter = false;
        CardActionDetailsOnScreen.Instance.ShowNormalEarnings();
        CardActionDetailsOnScreen.Instance.NoCostText();
    }

    private void UpdateToolTip()
    {
        TextDetails();
    }

}
