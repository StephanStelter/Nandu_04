using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerLevelManager : MonoBehaviour
{
    [Header("LevelObject Verknüpfungen")]
    public Button levelButton;
    public Image progressbar;
    public Image arrowUpSprite;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI toolTipText;
    public SkilltreeAndBonusesHandler skilltree;

    [Header("LevelObject Verknüpfungen")]
    public int currentLevel;
    public int xpForLevelUp;
    public int xpForLevelUpMultiplier;

    private NewPlayer mainPlayer;
    private int currentXp;
    private int saveRestXp;

    public void SetLevelStats()
    {
        mainPlayer = TurnManager.Instance.Players.Find(p => p.isMainPlayer);
        if (mainPlayer == null) return;

        currentLevel = mainPlayer.playerLevel;
        levelText.text = currentLevel.ToString();

        foreach (ResourceSummary resource in mainPlayer.SummaryResourcesList)
        {
            if (resource.ResourceName == ResourceType.XP)
            {
                if (saveRestXp != 0)
                {
                    resource.Amount += saveRestXp;
                    saveRestXp = 0;
                }
                currentXp = resource.Amount;
            }
        }

        FillBar();
        CheckLevelUp();
    }

    private void FillBar()
    {
        if (currentXp <= 0f)
        {
            progressbar.fillAmount = 0;
            return;
        }

        float ratio = (float)currentXp / (float)xpForLevelUp;
        progressbar.fillAmount = Mathf.Clamp01(ratio);
    }

    private void CheckLevelUp()
    {
        if (currentXp >= xpForLevelUp)
            LevelUp();
    }

    private void LevelUp()
    {
        saveRestXp = currentXp - xpForLevelUp;

        currentLevel++;
        mainPlayer.playerLevel = currentLevel;

        mainPlayer.skillPoints++;

        foreach (ResourceSummary resource in mainPlayer.SummaryResourcesList)
        {
            if (resource.ResourceName == ResourceType.XP)
                resource.Amount = 0;
        }

        xpForLevelUp *= xpForLevelUpMultiplier;

        SetLevelStats();
        UIResourceSummary.Instance.UpdateMainPlayerSummaryText();
    }

    public void ToolTippEnter()
    {
        SetLevelStats();

        toolTipText.text = string.Empty;
        toolTipText.text = UIResourceSummary.Instance.ZahlenFormatierung(currentXp) + " / " + "\n" + UIResourceSummary.Instance.ZahlenFormatierung(xpForLevelUp);
        toolTipText.gameObject.SetActive(true);
    }

    public void ToolTippExit()
    {
        toolTipText.gameObject.SetActive(false);
    }

}
