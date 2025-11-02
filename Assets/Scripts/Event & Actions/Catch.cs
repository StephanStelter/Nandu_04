using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Catch
{
    public Transform tileHolder; // Referenz zum Holder der Tiles
    public Transform CurrentHex = null;
    public GameObject CurrentSocket = null;
    public GameObject CurrentCard = null;
    public Button CurrentButton = null;
    public bool OverUI;
    public GameObject CurrentChip = null;

    public GameObject ChosenCard = null;
    public Transform ChosenHexUpdate = null;
    public Transform UIRefrenceHex = null;
    public GameObject UIRefrenceSocket = null;
    public string caKey;

    public Catch(Transform activeHex, GameObject chosenCard = null,Transform chosenHexUpdate = null)
    {
        try
        {
            CurrentHex = activeHex;
            ChosenCard = chosenCard;
            ChosenHexUpdate = chosenHexUpdate;
            CurrentSocket = GetCurrentSocket();
            CurrentCard = GetCurrentCard();
            CurrentButton = GetCurrentButton();
            OverUI = GameUtils.Instance.CheckUI();
            if (OverUI)
            {
                UIRefrenceHex = DetailWindowManager.Instance.ReferenceHex;
            }
            CurrentChip = GetChip(); 
            UIRefrenceSocket = MyEventHandler.Instance.chosenSocketUI;
            caKey = CreateCatchKey();
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Catch-Konstruktor] Fehler in Catch-Initialisierung: {ex.GetType().Name} – {ex.Message}\n{ex.StackTrace}");
        }
    }

    private string CreateCatchKey()
    {
        string key = "";

        key += (CurrentHex != null) ? "1" : "0";
        key += (CurrentButton != null) ? "1" : "0";
        key += (CurrentSocket != null) ? "1" : "0";
        key += (CurrentCard != null) ? "1" : "0";
        key += OverUI ? "1" : "0";
        key += (CurrentChip != null) ? "1" : "0";

        //Debug.Log("key: " + key);

        return key;
    }

    private GameObject GetCurrentSocket()
    {//Debug.Log("Catch fängt Socket");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            Transform currentTransform = hit.collider.transform;

            if (currentTransform.gameObject.CompareTag("Socket") && currentTransform.GetComponent<Socket>().socketType != Rarity.None)
            {
                return currentTransform.gameObject;
            }
        }
        return null;
    }
    private GameObject GetCurrentCard()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            Transform currentTransform = hit.collider.transform;
            while (currentTransform != null)
            {
                if (currentTransform.parent?.parent != null && currentTransform.parent.parent.CompareTag("Card"))
                {
                    return currentTransform.parent.parent.gameObject;
                }
                currentTransform = currentTransform.parent;
            }
        }
        return null;
    }
    private Button GetCurrentButton()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null)
            {
                return button;
            }
        }
        return null;

    }
    private GameObject GetChip()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            Transform currentTransform = hit.collider.transform;
            while (currentTransform != null)
            {
                if (currentTransform.parent != null && currentTransform.parent.CompareTag("Icon"))
                {
                    return currentTransform.parent.gameObject;
                }
                currentTransform = currentTransform.parent;
            }
        }
        return null;
    }
}
