using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    private const float AreaRadius = 500f;
    private const float VillageClearRadius = 400f;
    private const float UpgradeChipDelay = 0.1f;

    public static StructureManager Instance { get; private set; }
    public MyEventHandler eventHandler;
    [Header("Settlement Prefabs")]
    public GameObject[] settlementPrefabs;
    [Header("ResourceTile Prefabs")]
    public GameObject[] resourcePrefabs;
    [Header("Road Prefabs")]
    public GameObject[] roadPrefabs;
    [Header("Upgrade Chip Prefab")]
    public GameObject upgradeChipPrefab;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    // BUTTON METHODEN
    public void BuyNewVillage()
    {
        if (eventHandler != null)
            eventHandler.gameMode = GameMode.FoundVillage;
    }
    public void BuyRoad()
    {
        if (eventHandler != null)
            eventHandler.gameMode = GameMode.BuildRoad;
    }

    // SETTLEMENT METHODEN
    public void FoundSettlement(Transform currentHex)
    {
        if (currentHex == null) return;
        var hexComponent = currentHex.GetComponent<Hex>();
        if (hexComponent == null || hexComponent.occupantType != OccupType.None) return;

        List<Hex> areaHex = GameUtils.Instance.GetAreaHex(currentHex.position, AreaRadius);
        List<Hex> nonVillList = new();

        var activePlayer = TurnManager.Instance.GetCurrentPlayer();

        foreach (Hex hex in areaHex)
        {
            if (hex == null) continue;

            if (hex == hexComponent)
            {
                CreateVillageOnHexField(hex);
                hex.PlaceHexPattern(currentHex.position);
                hex.SetSocketFromStructur();
                AddHexToActivePlayerSummary(hex);
            }
            else
            {
                nonVillList.Add(hex);
            }

        }

        CreateResourceFieldOnHexField(nonVillList, hexComponent);
    }

    private void AddHexToActivePlayerSummary(Hex hex)
    {
        var activePlayer = TurnManager.Instance.GetCurrentPlayer();
        var hexNew = hex.GetComponentInParent<Hex>();
        if (hexNew != null && activePlayer != null && !activePlayer.PlayerHexFieldSummaryList.Contains(hexNew.gameObject))
            activePlayer.PlayerHexFieldSummaryList.Add(hexNew.gameObject);
    }
    private void CreateResourceFieldOnHexField(List<Hex> resHexs, Hex settHex)
    {
        if (resHexs == null || settHex == null) return;
        var unoccupiedHexs = GetUnoccupiedHexes(resHexs);
        int count = unoccupiedHexs.Count;
        int placements = count switch
        {
            2 => 1,
            3 or 4 => 2,
            5 or 6 => 4,
            _ => 0
        };
        if (placements == 0) return;

        for (int j = 0; j < placements && unoccupiedHexs.Count > 0; j++)
        {
            int index = UnityEngine.Random.Range(0, unoccupiedHexs.Count);
            var chosen = unoccupiedHexs[index];

            chosen.occupantType = OccupType.Resource;
            chosen.GenerateDiceNumber();
            settHex.resTilesOnSettlement.Add(chosen.gameObject);

            chosen.SetHighlight(Color.yellow);

            int randomNumber = UnityEngine.Random.Range(0, resourcePrefabs.Length);
            var tileScript = chosen;
            tileScript.structure = Instantiate(resourcePrefabs[randomNumber], chosen.transform.position, Quaternion.identity, chosen.transform);
            var resTile = tileScript.structure.GetComponent<Resource>();
            if (resTile != null)
            {
                resTile.hexReference = tileScript;
                resTile.SetProductionRarity();
            }
            AddHexToActivePlayerSummary(chosen);
            unoccupiedHexs.RemoveAt(index);
        }
    }

    private void CreateVillageOnHexField(Hex hex)
    {
        if (hex == null) return;
        GameUtils.Instance.ClearObjectsInArea(hex.transform.position, VillageClearRadius);
        hex.occupantType = OccupType.Settlement;
        hex.structure = Instantiate(settlementPrefabs[0], hex.transform.position, Quaternion.identity, hex.transform);
        hex.SetHighlight(Color.red);
        RefreshRoadsAround(CheckAndProcessSurroundingHexes(hex.transform), hex.transform);
    }

    // UPGRADE-SETTLEMENT
    public IEnumerator ShowOptionsForUpgrade(Transform currentHex)
    {
        if (currentHex == null) yield break;
        var hex = currentHex.GetComponent<Hex>();
        if (hex == null) yield break;
        var setStruct = hex.structure;
        if (setStruct == null) yield break;
        var settlement = setStruct.GetComponent<Settlement>();
        if (settlement == null) yield break;
        var upList = settlement.upgradeList;
        var iconHolder = currentHex.Find("IconHolder");
        if (iconHolder == null) yield break;

        int maxElements = upList.Count;
        int zeigerLaenge = 400;
        float iterationsWinkel = 15f;
        float startWinkel = iterationsWinkel * (0.5f * (maxElements - 1));

        for (int i = 0; i < maxElements; i++)
        {
            var t = upList[i];
            float winkelGrad = startWinkel - i * iterationsWinkel;
            float winkelRad = winkelGrad * Mathf.Deg2Rad;

            float real = zeigerLaenge * Mathf.Cos(winkelRad);
            float imaginary = zeigerLaenge * Mathf.Sin(winkelRad);

            GameObject chip = Instantiate(upgradeChipPrefab, currentHex.position, Quaternion.identity, iconHolder);

            var upgradeChip = chip.GetComponent<UpgradeChip>();
            if (upgradeChip != null)
            {
                upgradeChip.SetChip(currentHex, t);
                upgradeChip.SpawnChip(new List<Vector3> { new Vector3(0, real, imaginary), new Vector3(0, real - 50, imaginary) });
            }

            yield return new WaitForSeconds(UpgradeChipDelay);
        }
    }

    public void HideOptionsForUpgrade(Transform chosenHexUpdate)
    {
        if (chosenHexUpdate == null) return;
        Transform iconHolder = chosenHexUpdate.Find("IconHolder");
        if (iconHolder == null) return;

        for (int i = iconHolder.childCount - 1; i >= 0; i--)
        {
            Transform child = iconHolder.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    public void UpgradeSettlement(GameObject currentChip)
    {
        if (currentChip == null) return;
        var upgradeChip = currentChip.GetComponent<UpgradeChip>();
        if (upgradeChip == null) return;
        Transform currentHex = upgradeChip.referenceHex;
        Settlement newSettl = upgradeChip.referenceSettlement;
        if (currentHex == null || newSettl == null) return;

        Hex hex = currentHex.GetComponent<Hex>();
        if (hex == null) return;

        Destroy(hex.structure);
        GameObject neueInstanceStructur = Instantiate(newSettl.gameObject, currentHex.position, Quaternion.identity, currentHex);
        hex.structure = neueInstanceStructur;
        hex.SetSocketFromStructur();
    }

    // STRASSEN METHODEN
    public void CreateRoadOnHex(Transform hex)
    {
        if (hex == null) return;
        var hexComp = hex.GetComponent<Hex>();
        if (hexComp == null || hexComp.occupantType != OccupType.None) return;

        hexComp.occupantType = OccupType.Road;
        hexComp.structure = Instantiate(roadPrefabs[0], hex.position, Quaternion.identity, hex);
        var hexMatch = CheckAndProcessSurroundingHexes(hex);

        AddHexToActivePlayerSummary(hexComp);
        CreateRoad(hexMatch, hex);
        RefreshRoadsAround(hexMatch, hex);
    }

    public List<Hex> CheckAndProcessSurroundingHexes(Transform hex)
    {
        List<Hex> hexMatch = new();
        if (hex == null) return hexMatch;

        Vector3 center = hex.position;
        List<Hex> surroundingHexes = GameUtils.Instance.GetAreaHex(center, AreaRadius);

        foreach (Hex tile in surroundingHexes)
        {
            if (tile == null) continue;
            Hex roh = tile.GetComponent<Hex>();
            if (roh != null && (roh.occupantType == OccupType.Settlement || roh.occupantType == OccupType.Road) && tile.transform != hex)
            {
                hexMatch.Add(tile);
            }
        }
        return hexMatch;
    }

    private void CreateRoad(List<Hex> matches, Transform hex)
    {
        if (matches == null || hex == null) return;
        foreach (Hex tile in matches)
        {
            if (tile == null) continue;
            GameObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject point in tile.directionalObjects)
            {
                if (point == null) continue;
                float distance = Vector3.Distance(point.transform.position, hex.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = point;
                }
            }
            if (closest != null)
            {
                SplineMeshCreator.Instance.CreateSplineMesh(closest.transform.position, hex.position, hex);
                ClearPath(hex);
            }
        }
    }

    public void RefreshRoadsAround(List<Hex> matches, Transform hex)
    {
        if (matches == null || hex == null) return;
        foreach (Hex tile in matches)
        {
            if (tile == null) continue;
            var tileHex = tile.GetComponent<Hex>();
            if (tileHex != null && tileHex.occupantType == OccupType.Road)
            {
                foreach (Transform child in tile.gameObject.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "SplineMesh")
                    {
                        Destroy(child.gameObject);
                    }
                }
                var hexMatch = CheckAndProcessSurroundingHexes(tile.transform);
                CreateRoad(hexMatch, tile.transform);
            }
        }
    }

    private void ClearPath(Transform hex)
    {
        if (hex == null) return;
        List<GameObject> splineMeshes = new();

        foreach (Transform child in hex)
        {
            if (child.name.Contains("SplineMesh"))
            {
                splineMeshes.Add(child.gameObject);
            }
        }

        foreach (GameObject obj in splineMeshes)
        {
            MeshFilter mf = obj.GetComponent<MeshFilter>();
            if (mf == null) continue;

            Bounds bounds = mf.mesh.bounds;
            bounds = TransformBounds(bounds, obj.transform);

            Vector3 center = bounds.center;
            Vector3 halfExtents = bounds.extents;

            Collider[] colliders = Physics.OverlapBox(center, halfExtents, obj.transform.rotation);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("EnvironmentObject"))
                {
                    Destroy(collider.gameObject);
                }
            }
        }

        Bounds TransformBounds(Bounds bounds, Transform transform)
        {
            Vector3 center = transform.TransformPoint(bounds.center);
            Vector3 extents = bounds.extents;
            Vector3 axisX = transform.TransformVector(extents.x, 0, 0);
            Vector3 axisY = transform.TransformVector(0, extents.y, 0);
            Vector3 axisZ = transform.TransformVector(0, 0, extents.z);

            extents = new Vector3(
                Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x),
                Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y),
                Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z)
            );

            return new Bounds(center, extents * 2);
        }
    }

    // UPGRADE-Road
    public void UpgradeRoad(Hex hex, RoadType type)
    {
        if (hex == null) return;
        if (hex.structure != null)
            Destroy(hex.structure);

        if ((int)type < 0 || (int)type >= roadPrefabs.Length) return;
        GameObject prefab = roadPrefabs[(int)type];
        GameObject go = Instantiate(prefab, hex.transform);

        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        hex.structure = go;
    }

    // ANDERES
    private void DestroySettlement(Transform tileHolder)
    {
        if (tileHolder == null) return;
        Hex tileScript = tileHolder.GetComponent<Hex>();
        if (tileScript != null && tileScript.structure != null)
        {
            Destroy(tileScript.structure);
            tileScript.structure = null;
        }
    }

    private List<Hex> GetUnoccupiedHexes(IEnumerable<Hex> hexes)
    {
        var result = new List<Hex>();
        if (hexes == null) return result;
        foreach (var hex in hexes)
            if (hex != null && hex.occupantType == OccupType.None)
                result.Add(hex);
        return result;
    }
}