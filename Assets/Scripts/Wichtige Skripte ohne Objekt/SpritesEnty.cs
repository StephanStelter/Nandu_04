using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpritesEnty : MonoBehaviour
{
    [System.Serializable]
    public class ResourceSpriteMapping
    {
        public ResourceType resourceName;
        public Sprite resourceSprite;
    }

    [System.Serializable]
    public class VictoryPointsSpriteMapping
    {
        public VictoryNames victoryName;
        public Sprite victorySprite;
    }

    public enum VictoryNames
    {
        Bevölkerung,
        Religion,
        Militär,
        Handel,
        Politik,
        Handwerk
    }

    [System.Serializable]
    public class SignSpriteMapping
    {
        public Sign signName;
        public Sprite signSprite;
    }

    public enum Sign
    {
        Bear,
        Boar,
        Deer,
        Eagel,
        Goat,
        Nandu,
        Ox,
        Wolf
    }

    [Header("Sprite Mappings")]
    [SerializeField]
    private List<ResourceSpriteMapping> resourceSprites = new List<ResourceSpriteMapping>();

    [Header("Sprite Mappings")]
    [SerializeField]
    private List<VictoryPointsSpriteMapping> victorySprites = new List<VictoryPointsSpriteMapping>();

    [Header("Sprite Mappings")]
    [SerializeField]
    public List<SignSpriteMapping> signSprites = new List<SignSpriteMapping>();


    public Sprite GetResourceSprite(ResourceType resourceType)
    {
        // Suche nach passender Resource in summaryList anhand des Resourcentyps
        ResourceSpriteMapping existingResource = resourceSprites.FirstOrDefault(r => r.resourceName == resourceType);
        if (existingResource != null)
        {
            return existingResource.resourceSprite;
        }
        else
        {
            Debug.Log("Kein Sprite gefunden!");
            return null;
        }
    }

    public Sprite GetVictorySprite(VictoryNames victoryName)
    {
        // Suche nach passender Resource in summaryList anhand des Resourcentyps
        VictoryPointsSpriteMapping existingName = victorySprites.FirstOrDefault(r => r.victoryName == victoryName);
        if (existingName != null)
        {
            return existingName.victorySprite;
        }
        else
        {
            Debug.Log("Kein Sprite gefunden!");
            return null;
        }
    }

    public Sprite GetSignSprite(Sign signName)
    {
        // Suche nach passender Resource in summaryList anhand des Resourcentyps
        SignSpriteMapping existingName = signSprites.FirstOrDefault(r => r.signName == signName);
        if (existingName != null)
        {
            return existingName.signSprite;
        }
        else
        {
            Debug.Log("Kein Sprite gefunden!");
            return null;
        }
    }
}
