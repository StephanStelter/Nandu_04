using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePanelScript : MonoBehaviour
{
    public List<GameObject> skillObjectList;

    public void UpdateSkillObjects()
    {
        foreach (GameObject obj in skillObjectList)
        {
            SkillObject skillObject = obj.GetComponent<SkillObject>();
            if (skillObject == null) { Debug.Log("(skillObject == null"); };

            //skillObject.UpdateSkillObject();
        }

    }

}
