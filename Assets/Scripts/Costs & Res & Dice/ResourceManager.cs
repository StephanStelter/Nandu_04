using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    private void Awake() { if (Instance == null) { Instance = this; } else { Destroy(gameObject); } }

    public ResList GetTotalYield()
    {
        ResList yieldReturn = new ResList(0, 0, 0, 0, 0, 0, 0, 0); // Initialisierung mit 0
        var activePlayer = TurnManager.Instance.GetCurrentPlayer();

        foreach (GameObject obj in activePlayer.PlayerHexFieldSummaryList)
        {
            var hex = obj.GetComponent<Hex>();
            if (hex != null && hex.occupantType == OccupType.Resource && hex.structure != null && hex.diceNumber == Dice.Instance.GetSpriteNumberRound())
            {
                var resource = hex.structure.GetComponent<Resource>();
                if (resource != null)
                {
                    ResList tempRes = resource.produce();
                    yieldReturn = yieldReturn + tempRes;
                }
            }
        }
        Debug.Log(yieldReturn.ToString());
        return yieldReturn;
    }

    private void RerollResource(Hex hex, ResourceType? type = null)
    {
        if (type == null)
        {
            // alle neu
            hex.actValues = hex.minValues & hex.maxValues;
        }
        else
        {
            // nur eine Ressource neu
            var act = hex.actValues;
            int i = (int)type.Value;
            act[i] = UnityEngine.Random.Range(
                (int)hex.minValues[i],
                (int)hex.maxValues[i] + 1
            );
            hex.actValues = act;
        }
    }
}
public class GenerateResourceListEPRScript
{
    List<Res> tempList;


    public void GenerateList_EPR(Hex hex)
    {
        // Tiefe Kopie der Basisliste veränderliche Werte erstellen
        tempList = new List<Res>();

        foreach (Res resource in hex.resourcesList_MEPR)
        {
            this.tempList.Add(new Res(resource.resourceName, resource.resourceAmount, resource.isBlocked, resource.playerName, false));
        }

        // alle Werte auf 0 setzen
        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i].resourceAmount = 0;
        }

        if (tempList.Count == 0)
        {
            Debug.LogError("Änderungs Liste konnte nicht erstellt werden!");
        }
    }

    public List<Res> GetResourceList()
    {
        return tempList;
    }
}
public class GenerateResourceListMEPRScript
{
    List<Res> tempList;


    public void GenerateResourceList_MEPR(Hex hex)
    {
        this.tempList = new List<Res>();

        BasicResourcesGenerator generator = new BasicResourcesGenerator();
        generator.GenerateResourcelist();

        List<Res> tempList = generator.resourcesList;

        //1. Basisliste erstellen 
        foreach (Res resource in tempList)
        {
            this.tempList.Add(new Res(resource.resourceName, resource.resourceAmount, resource.isBlocked, resource.playerName, false));
        }

        Biom hexFielType;
        hexFielType = hex.biom;

        //2. Werte Hinzufügen
        foreach (Res resource in this.tempList)
        {
            // Die Menge der Resource für Hexfelder berechnen und aktualisieren
            int newAmount = generator.GeneratResourcesAmountOnHexfield(resource.resourceName, hex.biom);// Hier wird die Menge basierend auf der Funktion berechnet
            resource.resourceAmount = newAmount;
        }

        if (this.tempList.Count == 0)
        {
            Debug.LogError("Basic Liste konnte nicht erstellt werden!");
        }
    }

    public List<Res> GetResourceList()
    {
        return tempList;
    }

}
public class BasicResourcesGenerator
{
    public List<Res> resourcesList;

    // Basisliste mit allen möglichen Resourcen vorbereiten    q   
    public void GenerateResourcelist()
    {
        resourcesList = new List<Res>
        {
            new Res(ResourceType.Food, 0, false, Player.None, true),
            new Res(ResourceType.Gold, 0, false, Player.None, true),
            new Res(ResourceType.Wood, 0, false, Player.None, true),
            new Res(ResourceType.Iron, 0, false, Player.None, true),
            new Res(ResourceType.Stone, 0, false, Player.None, true),
            new Res(ResourceType.Wool, 0, false, Player.None, true),
        };
    }

    // Resourcenmenge je nach Terrain erzeugen
    public int GeneratResourcesAmountOnHexfield(ResourceType resourceName, Biom hexFieldType)
    {
        switch (resourceName)
        {
            case ResourceType.Food:
                return GrainGenerator(hexFieldType);
            case ResourceType.Gold:
                return GoldGenerator(hexFieldType);
            case ResourceType.Wood:
                return WoodGenerator(hexFieldType);
            case ResourceType.Iron:
                return MetalGenerator(hexFieldType);
            case ResourceType.Stone:
                return StoneGenerator(hexFieldType);
            case ResourceType.Wool:
                return WoolGenerator(hexFieldType);
            default:
                return 0;
        }
    }

    private int GoldGenerator(Biom type)
    {
        return type switch
        {
            Biom.Berg => Random.Range(0, 50),
            Biom.Hügelig => Random.Range(0, 5),
            _ => 0
        };
    }

    private int GrainGenerator(Biom type)
    {
        return type switch
        {
            Biom.Feld => Random.Range(50, 100),
            Biom.Hügelig => Random.Range(5, 30),
            Biom.Wiese => Random.Range(30, 80),
            Biom.Wald => Random.Range(0, 20),
            _ => 0
        };
    }

    private int MetalGenerator(Biom type)
    {
        return type switch
        {
            Biom.Berg => Random.Range(10, 80),
            Biom.Hügelig => Random.Range(2, 10),
            _ => 0
        };
    }

    private int StoneGenerator(Biom type)
    {
        return type switch
        {
            Biom.Berg => Random.Range(70, 100),
            Biom.Feld => Random.Range(0, 5),
            Biom.Hügelig => Random.Range(10, 30),
            Biom.Moor => Random.Range(0, 20),
            Biom.Wald => Random.Range(0, 10),
            Biom.Wiese => Random.Range(0, 10),
            Biom.Wüste => Random.Range(0, 10),
            _ => 0
        };
    }

    private int WoodGenerator(Biom type)
    {
        return type switch
        {
            Biom.Feld => Random.Range(0, 20),
            Biom.Hügelig => Random.Range(5, 30),
            Biom.Moor => Random.Range(10, 40),
            Biom.Wald => Random.Range(50, 100),
            Biom.Wiese => Random.Range(0, 30),
            Biom.Wüste => Random.Range(0, 5),
            _ => 0
        };
    }

    private int WoolGenerator(Biom type)
    {
        return type switch
        {
            Biom.Feld => Random.Range(30, 60),
            Biom.Hügelig => Random.Range(20, 50),
            Biom.Wald => Random.Range(0, 10),
            Biom.Wiese => Random.Range(50, 100),
            _ => 0
        };
    }
}

