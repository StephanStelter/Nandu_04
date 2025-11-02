using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillObject : MonoBehaviour
{
    [Header("SkillOject Verknüpfungen")]
    public Image mainImage;
    public Image BackgroundImage;
    public Button skillButton;
    public GameObject tooltip;
    public GameObject UILinConnectorPrefab;

    [Header("Unlock Verknüpfungen")]
    public List<GameObject> nextSkillsToUnlock;

    [Header("Sprites Library")]
    public SpritesEnty resourceSpriteEntyScript;

    [Header("SkilltreeAndBonusesHandler")]
    public SkilltreeAndBonusesHandler skilltreeAndBonuses;

    [Header("Seetings")]
    public bool lockedSkill = true;
    public bool activatedSkill = false;

    [Header("Skill")]
    public string skillname;
    public string skillDescription;
    public ResourceType resourceForSkill;
    public bool trackResource = false;
    public int boostAmountInPercent = 0;
    public int boostAmountInTotalAmount = 0;

    [Header("Details")]
    public TextMeshProUGUI headlineText;
    public Image SkillImage;
    public TextMeshProUGUI detailText;
    public Button activateSkillButton;

    NewPlayer mainPlayer;

    public void UpdateSkillObject()
    {
        //if (trackResource)
        //{
        //    mainImage.sprite = resourceSpriteEntyScript.GetResourceSprite(resourceForSkill);
        //}

        //CheckButton();

        //SetConnectors();
    }

    private void SetConnectors()
    {
        if (nextSkillsToUnlock.Count != 0)
        {
            foreach (var item in nextSkillsToUnlock)
            {
                GameObject newLine = Instantiate(UILinConnectorPrefab, transform);
                if (newLine == null) { Debug.LogError("newLine == null"); }
                newLine.transform.localScale = new Vector3(.3f, .3f, .3f);

                UILineConnector uILineConnector = newLine.GetComponent<UILineConnector>();
                if (uILineConnector == null) { Debug.LogError("uILineConnector == null"); }

                uILineConnector.SetTargets(this.GetComponent<RectTransform>(), item.GetComponent<RectTransform>());
            }
        }
    }



    public void CheckButton()
    {
        //skillButton.interactable = true;

        if (lockedSkill)
        {
            activateSkillButton.interactable = false;
            BackgroundImage.color = Color.gray;
            return;
        }

        if (!lockedSkill)
        {
            activateSkillButton.interactable = true;
            BackgroundImage.color = Color.yellow;
        }
        if (activatedSkill)
        {
            activateSkillButton.interactable = false;
            BackgroundImage.color = Color.green;
        }
    }

    public void ActivateSkill()
    {
        mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) return;

        if (mainPlayer.skillPoints > 0)
        {
            activatedSkill = true;
            CheckButton();

            // Skill ausführen


            mainPlayer.skillPoints--;

            foreach (GameObject gameObject in nextSkillsToUnlock)
            {
                SkillObject skillObject = gameObject.GetComponent<SkillObject>();
                if (skillObject == null) { Debug.Log("skillObject == null"); }

                skillObject.lockedSkill = false;
            }
        }
        else
        {
            Debug.Log("Nicht genug Punkte.");
        }


    }

    public void OpenDetailWindow()
    {
        Debug.Log("skill");
        //CheckButton();

        //headlineText.text = skillname;
        //detailText.text = skillDescription;

        //if (skilltreeAndBonuses == null)
        //{
        //    Debug.LogError("skilltreeAndBonuses ist nicht gesetzt!");
        //    return;
        //}

        //skilltreeAndBonuses.GetCurrentSkill(this);

    }
}
