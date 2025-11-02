using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ResMoni : MonoBehaviour
{
    [Header("Produktions Anzeige")]
    public TextMeshProUGUI production;
    public Image productionIcon;

    [Header("Storage Anzeige")]
    public List<Image> storageSlots; // 9 Images
    public Sprite activeStorageImage;
    public Sprite emptyStorageImage;
    public Sprite blockedStorageImage;
    private int storageCapacity;
    private int currentStorage;
    private int blockedStorage;

    [Header("Resourcentyp")]
    public List<Sprite> resourceTypeSprites; // 6 Sprites
    public Image resourceTypeImage;

    [Header("Zeichen")]
    public List<Sprite> signSprites; // 8 Sprites
    public Image sign;

    public void SetUpResMoni(GameObject hexObj)
    {
        Resource res = hexObj.GetComponent<Hex>().structure.GetComponent<Resource>();
        SetProduction(res);
        SetStorageCapacity(res);
        SetCurrentStorage(res);
        SetBlockedStorage(res);
        SetResourceType(res);
        SetZeichen(res);
        SetRarity(res); // ← NEU
        SetPanelColor(res);
    }
    public void SetProduction(Resource res)
    {
        this.production.text = res.production.ToString();
    }
    public void SetStorageCapacity(Resource res)
    {
        var capacity = res.capacity;
        storageCapacity = Mathf.Clamp(capacity, 0, storageSlots.Count);
        UpdateStorageVisuals();
    }
    public void SetCurrentStorage(Resource res)
    {
        var amount = res.amount;
        currentStorage = Mathf.Clamp(amount, 0, storageCapacity);
        UpdateStorageVisuals();
    }
    public void SetBlockedStorage(Resource res)
    {
        var blocked = res.blocked;
        blockedStorage = Mathf.Clamp(blocked, 0, storageCapacity);
        UpdateStorageVisuals();
    }
    private void UpdateStorageVisuals()
    {
        for (int i = 0; i < storageSlots.Count; i++)
        {
            if (i < blockedStorage)
            {
                storageSlots[i].sprite = blockedStorageImage;
            }
            else if (i < currentStorage)
            {
                storageSlots[i].sprite = activeStorageImage;
            }
            else if (i < storageCapacity)
            {
                storageSlots[i].sprite = emptyStorageImage;
            }
            else
            {
                storageSlots[i].enabled = false; // Slot außerhalb der Kapazität
            }
        }
    }
    public void SetResourceType(Resource res)
    {
        ResourceType resType = res.resourceType;

        switch (resType)
        {
            case ResourceType.Food:
                resourceTypeImage.sprite = resourceTypeSprites[0];
                break;

            case ResourceType.Gold:
                resourceTypeImage.sprite = resourceTypeSprites[1];
                break;

            case ResourceType.Iron:
                resourceTypeImage.sprite = resourceTypeSprites[2];
                break;

            case ResourceType.Stone:
                resourceTypeImage.sprite = resourceTypeSprites[3];
                break;

            case ResourceType.Wood:
                resourceTypeImage.sprite = resourceTypeSprites[4];
                break;

            case ResourceType.Wool:
                resourceTypeImage.sprite = resourceTypeSprites[5];
                break;

            default:
                // Optional: falls nichts passt
                break;
        }

    }
    public void SetZeichen(Resource res)
    {
        Sign tileSign = res.sign;

        switch (tileSign)
        {
            case Sign.Bear:
                sign.sprite = signSprites[0];
                break;

            case Sign.Boar:
                sign.sprite = signSprites[1];
                break;

            case Sign.Deer:
                sign.sprite = signSprites[2];
                break;

            case Sign.Eagel:
                sign.sprite = signSprites[3];
                break;

            case Sign.Goat:
                sign.sprite = signSprites[4];
                break;

            case Sign.Nandu:
                sign.sprite = signSprites[5];
                break;

            case Sign.Ox:
                sign.sprite = signSprites[6];
                break;

            case Sign.Wolf:
                sign.sprite = signSprites[7];
                break;
        }
    }
    public void SetRarity(Resource res)
    {
        Rarity productionRarity = res.productionRarity;
        string hexColor = "#FFFFFF"; // Standard: Weiß

        switch (productionRarity)
        {
            case Rarity.Common:
                hexColor = "#898989"; // GRau
                break;

            case Rarity.Uncommon:
                hexColor = "#1EFF00"; // Grün
                break;

            case Rarity.Rare:
                hexColor = "#0070DD"; // Blau
                break;

            case Rarity.Epic:
                hexColor = "#A335EE"; // Lila
                break;

            case Rarity.Legendary:
                hexColor = "#FF8000"; // Orange
                break;
        }

        // Hex in Unity-Farbe umwandeln und setzen
        Color color;
        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            productionIcon.color = color;
        }
    }
    public void SetPanelColor(Resource res)
    {
        ResourceType type = res.resourceType;
        Image img = GetComponent<Image>();
        switch (type)
        {
            case ResourceType.Food:
                img.color = ColorUtility.TryParseHtmlString("#B8A36580", out var colfood) ? colfood : Color.white;
                break;
            case ResourceType.Gold:
                img.color = ColorUtility.TryParseHtmlString("#C4961280", out var colgold) ? colgold : Color.white;
                break;
            case ResourceType.Wood:
                img.color = ColorUtility.TryParseHtmlString("#5D714480", out var colwood) ? colwood : Color.white;
                break;
            case ResourceType.Iron:
                img.color = ColorUtility.TryParseHtmlString("#87412980", out var coliron) ? coliron : Color.white;
                break;
            case ResourceType.Stone:
                img.color = ColorUtility.TryParseHtmlString("#89898980", out var colstone) ? colstone : Color.white;
                break;
            case ResourceType.Wool:
                img.color = ColorUtility.TryParseHtmlString("#8DAFCC80", out var colwool) ? colwool : Color.white;
                break;
        }
    }
}
