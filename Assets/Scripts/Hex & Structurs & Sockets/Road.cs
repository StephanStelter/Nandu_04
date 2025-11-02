using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Road : Structure
{
    public RoadType type;
    public List<Road> upgradeList; // Solte im Inspector auf den Defaultwert gesetzt werden und je nach 
    public Sprite icon;

}
