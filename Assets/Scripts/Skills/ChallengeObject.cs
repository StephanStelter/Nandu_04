using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChallengeObject : MonoBehaviour
{
    [Header("ChallengeObject Verknüpfungen")]
    [Header("kleine UI")]
    public Image mainImageSmall;
    public Image panelSmall;
    public Button uiButton;

    [Header("Detail Window")]
    public GameObject DetailWindowObject;
    public TextMeshProUGUI challengeText;
    public string challengeTextString;
    public Image mainImageBig;
    public Image panelBig;
    public TextMeshProUGUI panelText;
    public TextMeshProUGUI detailText;
    public Button redeemButton;
    public SkilltreeAndBonusesHandler skilltree;

    [Header("SkillOject Verknüpfungen")]
    public ResourceType resourceForSkill;
    public bool trackResource = false;

    [Header("Sprites Library")]
    public SpritesEnty resourceSpriteEntyScript;

    [Header("Goals")]
    public int xpToEarn = 0;
    public int xpMultiplierperLevel = 0;
    public int amountOfResourcesForEarning = 0;
    public int forEarningMultiplierperLevel = 0;

    private int level = 1;
    private int amountOfCurrentResources = 0;
    NewPlayer mainPlayer;
    public bool challengeCompleted;

    public void UpdatUI()
    {
        if (trackResource)
        {
            mainImageSmall.sprite = resourceSpriteEntyScript.GetResourceSprite(resourceForSkill);
        }

        MainPlayerResourceCounter();
        FillBar();
        CheckButton();
    }

    private void MainPlayerResourceCounter()
    {
        mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) return;

        foreach (ResourceSummaryTotalCounter resource in mainPlayer.ResourceSummaryTotalCounterList)
        {
            if (resource.ResourceName == resourceForSkill)
                amountOfCurrentResources = resource.Amount;
        }
    }

    private void FillBar()
    {
        if (amountOfResourcesForEarning <= 0f)
        {
            panelSmall.fillAmount = 0;
            panelBig.fillAmount = 0;
            return;
        }

        float ratio = (float)amountOfCurrentResources / (float)amountOfResourcesForEarning;
        panelSmall.fillAmount = Mathf.Clamp01(ratio);
        panelBig.fillAmount = Mathf.Clamp01(ratio);
    }



    public void ClickUiButton()
    {
        UpdatUI();
        DetailWindow();
        skilltree.GetChallengeObject(gameObject);
    }

    public void ClickRedeemButton()
    {
        RedeemAwards();
        LevelUp();
        UpdatUI();
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText();
        DetailWindow();
    }

    private void RedeemAwards()
    {
        mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) return;

        foreach (ResourceSummaryTotalCounter resource in mainPlayer.ResourceSummaryTotalCounterList)
        {
            if (resource.ResourceName == ResourceType.XP)
                resource.Amount += xpToEarn;
        }

        foreach (ResourceSummary resource in mainPlayer.SummaryResourcesList)
        {
            if (resource.ResourceName == ResourceType.XP)
                resource.Amount += xpToEarn;
        }
    }

    private void LevelUp()
    {
        amountOfResourcesForEarning *= forEarningMultiplierperLevel;
        xpToEarn *= xpMultiplierperLevel;
        level++;
    }


    private void CheckButton()
    {
        if (amountOfCurrentResources <= 0f)
        {
            redeemButton.interactable = false;
            panelSmall.fillAmount = 0;
            panelBig.fillAmount = 0;
            challengeCompleted = false;
            return;
        }

        float ratio = (float)amountOfCurrentResources / (float)amountOfResourcesForEarning;

        if (ratio >= 1)
        {
            redeemButton.interactable = true;
            panelSmall.fillAmount = 1;
            panelSmall.color = Color.green;
            panelBig.fillAmount = 1;
            panelBig.color = Color.green;
            challengeCompleted = true;
        }
        else
        {
            redeemButton.interactable = false;

            panelSmall.fillAmount = ratio;
            panelSmall.color = Color.yellow;
            panelBig.fillAmount = ratio;
            panelBig.color = Color.yellow;
            challengeCompleted = false;
        }
    }


    private void DetailWindow()
    {
        mainImageBig.sprite = resourceSpriteEntyScript.GetResourceSprite(resourceForSkill);
        challengeText.text = challengeTextString;

        panelText.text = UIResourceSummary.Instance.ZahlenFormatierung(amountOfCurrentResources) + " / " +
            UIResourceSummary.Instance.ZahlenFormatierung(amountOfResourcesForEarning);

        detailText.text =
            "Level: " + level + "\n" +
            "Reward: " + xpToEarn + " XP";
    }
}
