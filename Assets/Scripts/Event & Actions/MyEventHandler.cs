using System.Collections.Generic;
using UnityEngine;

public class MyEventHandler : MonoBehaviour
{
    public static MyEventHandler Instance { get; set; }
    public GameObject testObject;


    #region
    public Transform activeHex;
    public GameMode gameMode = GameMode.Default;
    public Transform tileHolder; // Referenz zum Holder der Tiles
    public GameObject chosenCard;
    public Transform chosenHexUpdate;
    public GameObject chosenSocketUI;

    // zus�tzliche Variablen f�r MouseOver
    public bool cardReleaseIntoSocket;
    public bool cardReleaseCardOnCardLvl;

    private SelectAction selectAction;
    private ResetSelectAction resetSelectAction;
    private BuildAction buildAction;
    private ResetCardAction resetCardAction;
    private DecreeAction decreeAction;
    private FoundingAction foundingAction;
    private ReturnToDefaultAction returnToDefaultAction;
    private BuildRoadAction buildRoadAction;
    private UpgradeSettlementAction upgradeSettlementAction;

    private RuntimeAction runtimeAction;

    public List<Hex> highlightedHex = new List<Hex>(); // Speichert hervorgehobene Tiles
    #endregion

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        selectAction = gameObject.AddComponent<SelectAction>();
        resetSelectAction = gameObject.AddComponent<ResetSelectAction>();
        buildAction = gameObject.AddComponent<BuildAction>();
        resetCardAction = gameObject.AddComponent<ResetCardAction>();
        decreeAction = gameObject.AddComponent<DecreeAction>();
        foundingAction = gameObject.AddComponent<FoundingAction>();
        returnToDefaultAction = gameObject.AddComponent<ReturnToDefaultAction>();
        buildRoadAction = gameObject.AddComponent<BuildRoadAction>();
        upgradeSettlementAction = gameObject.AddComponent<UpgradeSettlementAction>();
        runtimeAction = gameObject.AddComponent<RuntimeAction>();
    }

    void Update()
    {
        activeHex = GameUtils.Instance.GetClosestObj();

        switch (gameMode)
        {
            //DEFAULT___________________________________________________________________________________________________________________
            case GameMode.Default:

                runtimeAction.RuntimeDefault();

                if (UnityEngine.Input.GetMouseButtonDown(0) && GameUtils.Instance.IsOnlyLeftClick() && !selectAction.IsRunning)
                {
                    StartCoroutine(selectAction.ExecuteAction(new Catch(activeHex)));
                }
                if ((UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetMouseButtonDown(2) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)) && !resetSelectAction.IsRunning)
                {
                    GameUtils.Instance.ResetHighlighting();
                    StartCoroutine(resetSelectAction.ExecuteAction(new Catch(activeHex)));
                }
                if (UnityEngine.Input.GetMouseButtonDown(1))
                {
                    selectAction.SelectHexForUpgrade(new Catch(activeHex));
                }
                if (UnityEngine.Input.GetKey(KeyCode.T))
                {
                    //ResVectorDemonstration demo = new ResVectorDemonstration();
                    //demo.ausgeben();

                    Temp_ResList temp = new Temp_ResList();
                    temp.ResListTestMethode();
                }
                if (UnityEngine.Input.GetMouseButtonDown(1) && UnityEngine.Input.GetKey(KeyCode.T))
                {
                    Debug.Log("Hat geklappt");
                }
                break;

            //FOUNDVILLAGE______________________________________________________________________________________________________________
            case GameMode.FoundVillage:

                runtimeAction.RuntimeFound();

                if (UnityEngine.Input.GetMouseButtonDown(0) && !foundingAction.IsRunning)
                {
                    StartCoroutine(foundingAction.ExecuteAction(new Catch(activeHex)));
                    GameUtils.Instance.ResetHighlighting();
                }
                if ((UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetMouseButtonDown(2) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)) && !returnToDefaultAction.IsRunning)
                {
                    GameUtils.Instance.ResetHighlighting();
                    StartCoroutine(returnToDefaultAction.ExecuteAction(new Catch(activeHex)));
                }
                break;

            //PLAYCARD__________________________________________________________________________________________________________________
            case GameMode.PlayCard:

                runtimeAction.RuntimePlay();

                if (UnityEngine.Input.GetMouseButtonDown(0) && !decreeAction.IsRunning)
                {
                    GameUtils.Instance.ResetHighlighting();
                    StartCoroutine(decreeAction.ExecuteAction(new Catch(activeHex,chosenCard)));
                }
                if ((UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetMouseButtonDown(2) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)) && !returnToDefaultAction.IsRunning)
                {
                    GameUtils.Instance.ResetHighlighting();
                    StartCoroutine(returnToDefaultAction.ExecuteAction(new Catch(activeHex, chosenCard)));
                }
                break;

            //BUILDCARD_________________________________________________________________________________________________________________
            case GameMode.BuildCard:

                runtimeAction.RuntimeBuild();

                if (UnityEngine.Input.GetMouseButtonDown(0) && !buildAction.IsRunning)
                {
                    StartCoroutine(buildAction.ExecuteAction(new Catch(activeHex,chosenCard)));
                    CostContentCheck.Instance.MouseWithCardOverObjectExit();
                }

                if (UnityEngine.Input.GetMouseButtonDown(1) && !buildAction.IsRunning)
                {
                    StartCoroutine(selectAction.ExecuteAction(new Catch(activeHex,chosenCard)));
                    CostContentCheck.Instance.MouseWithCardOverObjectExit();
                }

                if ((UnityEngine.Input.GetMouseButtonDown(2) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)) && !returnToDefaultAction.IsRunning)
                {
                    GameUtils.Instance.ResetHighlighting();
                    StartCoroutine(returnToDefaultAction.ExecuteAction(new Catch(activeHex, chosenCard)));
                    CostContentCheck.Instance.MouseWithCardOverObjectExit();
                }
                break;

            //BuildRoad______________________________________________________________________________________________________________
            case GameMode.BuildRoad:

                runtimeAction.RuntimeBuildRoad();

                if (UnityEngine.Input.GetMouseButtonDown(0) && !foundingAction.IsRunning)
                {
                    StartCoroutine(buildRoadAction.ExecuteAction(new Catch(activeHex)));
                    GameUtils.Instance.ResetHighlighting();
                }
                if ((UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetMouseButtonDown(2) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)) && !returnToDefaultAction.IsRunning)
                {
                    GameUtils.Instance.ResetHighlighting();
                    StartCoroutine(returnToDefaultAction.ExecuteAction(new Catch(activeHex)));
                }
                break;

            //UPGRADESettlement_______________________________________________________________________________________________________
            case GameMode.UpgradeSettlement:

                runtimeAction.RuntimeUpgradeSettlement();

                if (UnityEngine.Input.GetMouseButtonDown(0) && !foundingAction.IsRunning)
                {
                    StartCoroutine(upgradeSettlementAction.ExecuteAction(new Catch(activeHex,null,chosenHexUpdate)));
                    GameUtils.Instance.ResetHighlighting();
                }
                if ((UnityEngine.Input.GetMouseButtonDown(1) || UnityEngine.Input.GetMouseButtonDown(2) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)) && !returnToDefaultAction.IsRunning)
                {
                    StructureManager.Instance.HideOptionsForUpgrade(chosenHexUpdate);
                    chosenHexUpdate = null;
                    GameUtils.Instance.ResetHighlighting();
                    StartCoroutine(returnToDefaultAction.ExecuteAction(new Catch(activeHex, null, chosenHexUpdate)));
                }
                break;

            //SkillMenu_______________________________________________________________________________________________________
            case GameMode.SkillMenu:

                if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                {
                    StartCoroutine(returnToDefaultAction.ExecuteAction(new Catch(activeHex, null, chosenHexUpdate)));
                }
                break;
        }
    }
}