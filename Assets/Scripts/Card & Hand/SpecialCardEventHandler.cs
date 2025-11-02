using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpecialCardEventHandler : MonoBehaviour
{
    public static SpecialCardEventHandler Instance { get; private set; }

    public TextMeshProUGUI xpAmountText;

    [SerializeField] private GameObject xpDisplayObject;
    [SerializeField] private GameObject targetObject;


    private int commonXP = 1;
    private int uncommonXP = 10;
    private int rareXP = 25;
    private int epicXP = 35;
    private int legendaryXP = 50;
    private int goldXP = 100;

    private void Awake()
    {
        // Sicher stellen, dass nur eine Instanz existiert
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Eine Instanz existiert bereits, diese sollte zerstört werden
            Destroy(gameObject);
        }
    }

    public void GetPoitsForRecycledCard(GameObject recycledCard)
    {
        CardProperties cardProperties = recycledCard.GetComponent<CardProperties>();
        if (cardProperties != null)
            XpParameters(cardProperties.rarity);

    }

    private void XpParameters(Rarity cardrarity)
    {
        switch (cardrarity)
        {
            case Rarity.Common:
                UIResourceSummary.Instance.SetXP(commonXP);
                StartCoroutine(XPDisplay(commonXP));
                break;

            case Rarity.Uncommon:
                UIResourceSummary.Instance.SetXP(uncommonXP);
                StartCoroutine(XPDisplay(uncommonXP));
                break;

            case Rarity.Rare:
                UIResourceSummary.Instance.SetXP(rareXP);
                StartCoroutine(XPDisplay(rareXP));
                break;

            case Rarity.Legendary:
                UIResourceSummary.Instance.SetXP(legendaryXP);
                StartCoroutine(XPDisplay(legendaryXP));
                break;

            case Rarity.Mythical:
                UIResourceSummary.Instance.SetXP(goldXP);
                StartCoroutine(XPDisplay(goldXP));
                break;

            default:
                break;
        }
    }

    private IEnumerator XPDisplay(int xpAmount)
    {
        // neues Objekt instanzieren
        GameObject showXp;
        showXp = Instantiate(xpDisplayObject, targetObject.transform.localPosition, Quaternion.identity);
        showXp.transform.SetParent(targetObject.transform, false);
        showXp.transform.localPosition = new Vector3(Random.Range(-9, 2), -55 + (Random.Range(-7, 7)), 0);

        // Text setzen
        TextMeshProUGUI xpAmountTextNew = showXp.GetComponentInChildren<TextMeshProUGUI>();
        xpAmountTextNew.text = "+" + xpAmount.ToString() + "XP";

        // Bewegung nach oben und Blende das Objekt langsam aus
        CanvasGroup canvasGroup = showXp.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = showXp.AddComponent<CanvasGroup>(); // Falls nicht vorhanden, hinzufügen

        float fadeDuration = 0.6f;
        float moveDuration = 0.6f;
        float elapsedTime = 0f;
        Vector3 startPos = showXp.transform.localPosition;
        Vector3 targetPos = startPos + new Vector3(0, 50f, 0);

        while (elapsedTime < moveDuration)
        {
            showXp.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / moveDuration); // bewegen
            canvasGroup.alpha = 1 - (elapsedTime / fadeDuration); // ausblenden
            elapsedTime += Time.deltaTime;
            yield return null; // Warte einen Frame
        }

        yield return new WaitForSeconds(.6f);

        Destroy(showXp); // Objekt wieder zerstören
    }
}
