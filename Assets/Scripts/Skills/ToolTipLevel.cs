using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipLevel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerLevelManager playerLevelManager;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        playerLevelManager.ToolTippEnter();
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        playerLevelManager.ToolTippExit();
    }



}
