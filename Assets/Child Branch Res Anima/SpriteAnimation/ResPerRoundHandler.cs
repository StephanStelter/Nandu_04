using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RoundStartInformation;

public class ResPerRoundHandler : MonoBehaviour
{
    public static ResPerRoundHandler Instance { get; private set; }


    public GameObject resPerRoundPrefab;

    public Transform resSpawnPointWood;
    public Transform resSpawnPointStone;
    public Transform resSpawnPointFood;
    public Transform resSpawnPointIron;
    public Transform resSpawnPointWool;
    public Transform resSpawnPointGold;


    public Transform resTargetWood;
    public Transform resTargetStone;
    public Transform resTargetFood;
    public Transform resTargetIron;
    public Transform resTargetWool;
    public Transform resTargetGold;

    public SpritesEnty resourceSpriteEntyScript;
    public List<ResourcesPerRound> resourceListPerRoundMainPLayer = new List<ResourcesPerRound>() { };

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void GetResourceSummaryPerRound(List<ResourceSummary> resourceListPerRound)
    {
        foreach (ResourceSummary resourceSummary in resourceListPerRound)
        {
            if (resourceSummary.ResourceName != ResourceType.XP)
            {
                SetResourcetypeMode(resourceSummary.ResourceName, resourceSummary.Amount);
            }
        }
    }

    private void SetResourcetypeMode(ResourceType resourceType, int amount)
    {
        if (resourceType == ResourceType.Wood)
            SpawnIcon(resSpawnPointWood, resTargetWood, resourceType, amount);

        if (resourceType == ResourceType.Stone)
            SpawnIcon(resSpawnPointStone, resTargetStone, resourceType, amount);

        if (resourceType == ResourceType.Food)
            SpawnIcon(resSpawnPointFood, resTargetFood, resourceType, amount);

        if (resourceType == ResourceType.Iron)
            SpawnIcon(resSpawnPointIron, resTargetIron, resourceType, amount);

        if (resourceType == ResourceType.Wool)
            SpawnIcon(resSpawnPointWool, resTargetWool, resourceType, amount);

        if (resourceType == ResourceType.Gold)
            SpawnIcon(resSpawnPointGold, resTargetGold, resourceType, amount);

        //else { Debug.LogWarning("ResourceType " + resourceType + " nicht gefunden!"); }

    }

    private void SpawnIcon(Transform spawnPoint, Transform targetPoint, ResourceType resourceType, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Instanziiere das Icon am Spawn-Punkt
            GameObject icon = Instantiate(resPerRoundPrefab, spawnPoint.position, Quaternion.identity);
            icon.transform.SetParent(spawnPoint.transform, true);

            // Sprite dem Icon zuweisen
            Sprite sprite = resourceSpriteEntyScript.GetResourceSprite(resourceType);
            if (sprite != null)
            {
                icon.GetComponent<Image>().sprite = sprite;
            }
            else { Debug.LogWarning("Sprite für ResourceType " + resourceType + " nicht gefunden!"); }

            // Setze die Objekt-Statistiken
            icon.GetComponent<SpriteMovement>().SetObjectStats(resourceType, 1);

            // Starte die Bewegung des Icons zum Zielpunkt
            icon.GetComponent<SpriteMovement>().StartMovement(targetPoint.position);
        }
    }




}
