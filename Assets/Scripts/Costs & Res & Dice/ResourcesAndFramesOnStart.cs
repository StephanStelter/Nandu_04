using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesAndFramesOnStart : MonoBehaviour
{
    public static ResourcesAndFramesOnStart Instance { get; private set; }

    [SerializeField] private Transform gridHexfieldContentHolder;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void Start()
    {
        //StartCoroutine(SetFramesOnHexCoroutine());

        StartCoroutine(SetResourcesOnHexCoroutine());
    }
    private IEnumerator SetResourcesOnHexCoroutine()
    {
        Debug.Log("Test hat geklappt");
        yield return new WaitForSeconds(5f);

        GenerateResourceListMEPRScript generateResourceListMEPRScript = new GenerateResourceListMEPRScript();

        GenerateResourceListEPRScript generateResourceListEPRScript = new GenerateResourceListEPRScript();

        Hex[] hexChilds = gridHexfieldContentHolder.GetComponentsInChildren<Hex>();

        foreach (Hex hex in hexChilds)
        {
            if (hex != null)
            {
                //MEPR Liste erstellen
                generateResourceListMEPRScript.GenerateResourceList_MEPR(hex);

                hex.resourcesList_MEPR = generateResourceListMEPRScript.GetResourceList();

                //EPR Liste erstellen
                generateResourceListEPRScript.GenerateList_EPR(hex);

                hex.resourcesList_EPR = generateResourceListEPRScript.GetResourceList();
            }
            else
            {
                Debug.Log("Kein childResourcesOnHex");
            }
        }
    }

}
