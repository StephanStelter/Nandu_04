using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
//using static ResourcesOnHex;

public class Settlement : Structure
{
    public List<Settlement> upgradeList; // Solte im Inspector auf den Defaultwert gesetzt werden und je nach 
    public int storage; // Vielleicht
    public List<Rarity> socketKey;

    void Start()
    {
        UpdateUpgradeList();
    }

    private void UpdateUpgradeList()
    {
        Biom type = this.GetComponentInParent<Hex>().biom;

        //if (type == HexFieldType.Berg){upgradeList.Add(this);}
        //if (type == HexFieldType.Feld) { upgradeList.Add(this); }
        //if (type == HexFieldType.Hügelig) { upgradeList.Add(this); }
        //if (type == HexFieldType.Moor) { upgradeList.Add(this); }
        //if (type == HexFieldType.Wald) { upgradeList.Add(this); }
        //if (type == HexFieldType.Wiese) { upgradeList.Add(this); }
        //if (type == HexFieldType.Wüste) { upgradeList.Add(this); }

        //Debug.Log("UpdateUpgradeList ausgeführt!");
    }
}
