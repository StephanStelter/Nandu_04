using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPropertiesTest : MonoBehaviour
{
    public TextMeshProUGUI cadrValueText;
    [Range(0f, 1f)]
    public Image drawChanceImage;
    public Image victoryTargetImage;
    public Image signImage;
    public Image resourceImage;
    public float relativeChance;
    public TextMeshProUGUI chanceText;
    public GameObject chanceTextParent;

    [Header("Karten Variablen")]
    public Victorytypes victorytypes;
    public Sign sign;
    public ResourceType resourceType;
    public int resourceAmount;
    public int cardValue;
    public float drawChance; // z. B. 0.1 = 10 %, 0.5 = 50 %, usw.


    private void Start()
    {
        cadrValueText.text = cardValue.ToString(); // Setze den Text auf den Kartenwerte

        // Sprites setzen
        bool flowControl = SetSprites();
        if (!flowControl)
        {
            return;
        }
    }

    private bool SetSprites()
    {
        SpritesEnty spritesEnty = FindAnyObjectByType<SpritesEnty>();
        if (spritesEnty == null) { Debug.LogError("SpritesEnty-Instanz nicht gefunden!"); return false; }

        if (victorytypes == Victorytypes.Nothing)
        {
            victoryTargetImage.gameObject.SetActive(false);
        }
        else
        {
            victoryTargetImage.gameObject.SetActive(true);
            victoryTargetImage.sprite = spritesEnty.GetVictorySprite((SpritesEnty.VictoryNames)(int)victorytypes);
        }

        if (sign == Sign.Nothing)
        {
            signImage.gameObject.SetActive(false);
        }
        else
        {
            signImage.gameObject.SetActive(true);
            signImage.sprite = spritesEnty.GetSignSprite((SpritesEnty.Sign)(int)sign);
        }

        if (resourceType == ResourceType.Nothing)
        {
            resourceImage.gameObject.SetActive(false);
        }
        else
        {
            resourceImage.gameObject.SetActive(true);
            resourceImage.sprite = spritesEnty.GetResourceSprite(resourceType);
        }

        return true;
    }

    public void ResourceOFCard()
    {
        Debug.Log("Ressource: " + resourceType + " Amount: " + resourceAmount);
    }

    private void SetFillAmountColor(float _chance)
    {
        if (_chance >= 0.75f)
        {
            drawChanceImage.color = Color.green; // Grün für hohe Chancen
        }
        else if (_chance >= 0.25f)
        {
            drawChanceImage.color = Color.yellow; // Gelb für mittlere Chancen
        }
        else
        {
            drawChanceImage.color = Color.red; // Rot für niedrige Chancen
        }
    }

    public void SetDrawChance()
    {
        drawChanceImage.fillAmount = drawChance; // Aktualisiere den Füllstand des Bildes
        SetFillAmountColor(drawChance); // Aktualisiere die Farbe basierend auf der neuen Ziehchance
        chanceText.text = (drawChance * 100).ToString("F1") + "%"; // Aktualisiere den Text mit der prozentualen Chance
    }

    // Diese Methode aktualisiert das Füllbild basierend auf der relativen Chance
    public void SetRelativeChance(float _relativeChance)
    {
        relativeChance = _relativeChance;
        drawChanceImage.fillAmount = relativeChance; // Aktualisiere den Füllstand des Bildes
        SetFillAmountColor(relativeChance); // Aktualisiere die Farbe basierend auf der neuen relativen Chance
        chanceText.text = (relativeChance * 100).ToString("F1") + "%"; // Aktualisiere den Text mit der prozentualen Chance
    }

    public void UpdaterelativeChance()
    {
        SetRelativeChance(relativeChance);
    }

    public void DeactivateChanceText()
    {
        chanceTextParent.SetActive(false);
    }
}
