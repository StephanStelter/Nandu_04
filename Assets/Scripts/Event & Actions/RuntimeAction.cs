using UnityEngine;

public class RuntimeAction : MonoBehaviour
{
    Transform activeHex = MyEventHandler.Instance.activeHex;

    public void RuntimeDefault()
    {
        GameUtils.Instance.ShowCloseSockets(MyEventHandler.Instance.activeHex);
    }

    public void RuntimeFound()
    {
        GameUtils.Instance.HighlightArea(MyEventHandler.Instance.activeHex.position, 500, Color.red, Color.yellow, MyEventHandler.Instance.activeHex);
        GameUtils.Instance.HighlightSocketUnderMouse(MyEventHandler.Instance.activeHex);
    }

    public void RuntimePlay()
    {
        GameUtils.Instance.HighlightArea(MyEventHandler.Instance.activeHex.position, 1000, Color.blue, Color.blue, MyEventHandler.Instance.activeHex);
        GameUtils.Instance.ShowCloseSockets(MyEventHandler.Instance.activeHex);
    }

    public void RuntimeBuild()
    {
        try { CostContentCheck.Instance.MouseWithCardOverObject(MyEventHandler.Instance.activeHex); }
        catch (System.Exception ex) { Debug.LogWarning($"Fehler bei MouseWithCardOverObject: {ex.Message}"); }

        GameUtils.Instance.ShowCloseSockets(MyEventHandler.Instance.activeHex);
    }

    public void RuntimeBuildRoad()
    { 
        GameUtils.Instance.HighlightArea(MyEventHandler.Instance.activeHex.position, 200, Color.magenta, Color.yellow, MyEventHandler.Instance.activeHex);
    }

    public void RuntimeUpgradeSettlement()
    { 
        GameUtils.Instance.ShowCloseSockets(MyEventHandler.Instance.activeHex); 
    }
        
}
