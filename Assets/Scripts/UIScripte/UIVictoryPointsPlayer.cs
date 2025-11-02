using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SkilltreeAndBonusesHandler;

public class UIVictoryPointsPlayer : MonoBehaviour
{
    [Header("Player Name")]
    public TextMeshProUGUI playerPositionText;
    public TextMeshProUGUI playerNameText;

    [Header("Summe")]
    public TextMeshProUGUI summaryText;

    [Header("Victory Signs Name")]
    public TextMeshProUGUI populationText;
    public TextMeshProUGUI religionText;
    public TextMeshProUGUI militaryText;
    public TextMeshProUGUI craftsmenText;
    public TextMeshProUGUI tradingText;
    public TextMeshProUGUI politicsText;

    public List<VictoryPointsSummary> victoryPointsSummaryList = new List<VictoryPointsSummary>();


    public void SetText(List<VictoryPointsSummary> _victoryPointsSummary, int playerNumber, List<HighestVictoryPoints> _highestVictoryPointsList)
    {
        victoryPointsSummaryList = _victoryPointsSummary;

        playerPositionText.text = (playerNumber + 1).ToString() + ".";
        playerNameText.text = victoryPointsSummaryList[playerNumber].playerName;

        int populationValue = _highestVictoryPointsList[0].populationVPSummaryHS;
        if (populationValue > 0 && populationValue == victoryPointsSummaryList[playerNumber].populationVPSummary)
            populationText.text = "<color=green>" + victoryPointsSummaryList[playerNumber].populationVPSummary + "</color>";
        else
            populationText.text = victoryPointsSummaryList[playerNumber].populationVPSummary.ToString();

        int religionValue = _highestVictoryPointsList[0].religionVPSummaryHS;
        if (religionValue > 0 && religionValue == victoryPointsSummaryList[playerNumber].religionVPSummary)
            religionText.text = "<color=green>" + victoryPointsSummaryList[playerNumber].religionVPSummary + "</color>";
        else
            religionText.text = victoryPointsSummaryList[playerNumber].religionVPSummary.ToString();

        int militaryValue = _highestVictoryPointsList[0].militaryVPSummaryHS;
        if (militaryValue > 0 && militaryValue == victoryPointsSummaryList[playerNumber].militaryVPSummary)
            militaryText.text = "<color=green>" + victoryPointsSummaryList[playerNumber].militaryVPSummary + "</color>";
        else
            militaryText.text = victoryPointsSummaryList[playerNumber].militaryVPSummary.ToString();

        int craftsmenValue = _highestVictoryPointsList[0].craftsmenVPSummaryHS;
        if (craftsmenValue > 0 && craftsmenValue == victoryPointsSummaryList[playerNumber].craftsmenVPSummary)
            craftsmenText.text = "<color=green>" + victoryPointsSummaryList[playerNumber].craftsmenVPSummary + "</color>";
        else
            craftsmenText.text = victoryPointsSummaryList[playerNumber].craftsmenVPSummary.ToString();

        int tradingValue = _highestVictoryPointsList[0].tradingVPSummaryHS;
        if (tradingValue > 0 && tradingValue == victoryPointsSummaryList[playerNumber].tradingVPSummary)
            tradingText.text = "<color=green>" + victoryPointsSummaryList[playerNumber].tradingVPSummary + "</color>";
        else
            tradingText.text = victoryPointsSummaryList[playerNumber].tradingVPSummary.ToString();

        int politicsValue = _highestVictoryPointsList[0].politicsVPSummaryHS;
        if (politicsValue > 0 && politicsValue == victoryPointsSummaryList[playerNumber].politicsVPSummary)
            politicsText.text = "<color=green>" + victoryPointsSummaryList[playerNumber].politicsVPSummary + "</color>";
        else
            politicsText.text = victoryPointsSummaryList[playerNumber].politicsVPSummary.ToString();

        int summaryValue = _highestVictoryPointsList[0].totalVPSummaryHS;
        if (summaryValue > 0 && summaryValue == victoryPointsSummaryList[playerNumber].totalVPSummary)
            summaryText.text = "<color=green>" + victoryPointsSummaryList[playerNumber].totalVPSummary + "</color>";
        else
            summaryText.text = victoryPointsSummaryList[playerNumber].totalVPSummary.ToString();
    }
}
