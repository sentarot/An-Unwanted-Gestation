using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;
using UWG.UI;

/// <summary>
/// Editor utility that builds the entire UWG Canvas UI hierarchy,
/// attaches MonoBehaviour scripts, wires serialized fields, and creates prefabs.
/// Run from the menu bar: UWG > Build Canvas UI
/// </summary>
public static class CanvasBuilder
{
    // ───────────────────── Ref structs ─────────────────────

    private struct SetupRefs
    {
        public GameObject setupPanelGO;
        public SetupScreen setupScreen;
        public Transform hostButtonContainer;
        public Image hostPortrait;
        public TextMeshProUGUI hostNameText;
        public TextMeshProUGUI hostBackdropText;
        public TextMeshProUGUI hostStatsText;
        public Transform classButtonContainer;
        public TextMeshProUGUI classNameText;
        public TextMeshProUGUI classConceptText;
        public Button startButton;
    }

    private struct TopBarRefs
    {
        public HUDController hudController;
        public TextMeshProUGUI biomassText;
        public Slider gestationSlider;
        public TextMeshProUGUI gestationLabel;
        public Image gestationFill;
        public Slider interventionSlider;
        public TextMeshProUGUI interventionLabel;
        public Image interventionFill;
        public TextMeshProUGUI dayText;
        public SpeedControls speedControls;
        public Button pauseButton;
        public Button speed1Button;
        public Button speed2Button;
        public Button speed4Button;
        public TextMeshProUGUI speedLabel;
    }

    private struct LeftPanelRefs
    {
        public HostVitalsPanel vitalsPanel;
        public TextMeshProUGUI hostNameText;
        public TextMeshProUGUI archetypeText;
        public TextMeshProUGUI currentTaskText;
        public Image taskStatusIcon;
        public Slider humiliationBar;
        public TextMeshProUGUI humiliationText;
        public Slider discomfortBar;
        public TextMeshProUGUI discomfortText;
        public Slider socialBar;
        public TextMeshProUGUI socialText;
        public TextMeshProUGUI stateText;
    }

    private struct RightPanelRefs
    {
        public SkillTreePanel skillTreePanel;
        public Transform somaticContainer;
        public Transform endocrineContainer;
        public Transform temporalContainer;
        public Transform classAContainer;
        public Transform classBContainer;
        public Transform classCContainer;
        public GameObject tooltipPanel;
        public TextMeshProUGUI tooltipName;
        public TextMeshProUGUI tooltipDesc;
        public TextMeshProUGUI tooltipCost;
        public Button purchaseButton;
    }

    private struct BottomBarRefs
    {
        public EventLogPanel eventLogPanel;
        public TextMeshProUGUI logText;
        public ScrollRect scrollRect;
    }

    private struct GameOverRefs
    {
        public GameOverScreen gameOverScreen;
        public GameObject panel;
        public TextMeshProUGUI outcomeText;
        public TextMeshProUGUI summaryText;
        public Button restartButton;
    }

    // ───────────────────── Entry Point ─────────────────────

    [MenuItem("UWG/Build Canvas UI")]
    public static void BuildCanvasUI()
    {
        // 1. Canvas + EventSystem
        var canvasGO = FindOrCreateCanvas();
        FindOrCreateEventSystem();
        var canvasTransform = canvasGO.transform;

        // 2. Idempotency: destroy existing UWG panels
        DestroyExistingChild(canvasTransform, "SetupPanel");
        DestroyExistingChild(canvasTransform, "DashboardPanel");

        // 3. Build SetupPanel
        var setupRefs = BuildSetupPanel(canvasTransform);

        // 4. Build DashboardPanel (starts inactive)
        var dashboardGO = CreateUIObject("DashboardPanel", canvasTransform);
        StretchToFill(dashboardGO);
        dashboardGO.SetActive(false);

        var topBarRefs = BuildTopBar(dashboardGO.transform);
        var leftPanelRefs = BuildLeftPanel(dashboardGO.transform);
        var rightPanelRefs = BuildRightPanel(dashboardGO.transform);
        var bottomBarRefs = BuildBottomBar(dashboardGO.transform);
        var gameOverRefs = BuildGameOverOverlay(dashboardGO.transform);

        // 5. Create prefabs
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        var hostBtnPrefab = CreateButtonPrefab("HostButtonPrefab", "Host");
        var classBtnPrefab = CreateButtonPrefab("ClassButtonPrefab", "Class");
        var skillNodePrefab = CreateSkillNodePrefab();

        // 6. Wire all serialized fields
        WireSetupScreen(setupRefs, hostBtnPrefab, classBtnPrefab, dashboardGO);
        WireHUDController(topBarRefs);
        WireSpeedControls(topBarRefs);
        WireHostVitalsPanel(leftPanelRefs);
        WireSkillTreePanel(rightPanelRefs, skillNodePrefab);
        WireEventLogPanel(bottomBarRefs);
        WireGameOverScreen(gameOverRefs);

        // 7. Register undo and mark dirty
        Undo.RegisterCreatedObjectUndo(canvasGO, "Build UWG Canvas UI");
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        // 8. Summary
        Debug.Log(
            "[UWG CanvasBuilder] UI hierarchy built successfully.\n" +
            "  Created: SetupPanel, DashboardPanel (TopBar, LeftPanel, RightPanel, BottomBar, GameOverOverlay)\n" +
            "  Prefabs: Assets/Prefabs/HostButtonPrefab.prefab, ClassButtonPrefab.prefab, SkillNodePrefab.prefab\n" +
            "  TODO: Assign ScriptableObjects (HostProfile[], GestationClassData[], SkillTreeData) manually in the Inspector.\n" +
            "  Save the scene with Ctrl+S.");
    }

    // ───────────────────── Canvas / EventSystem ─────────────────────

    private static GameObject FindOrCreateCanvas()
    {
        var canvas = Object.FindObjectOfType<Canvas>();
        GameObject canvasGO;

        if (canvas != null)
        {
            canvasGO = canvas.gameObject;
        }
        else
        {
            canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        if (scaler == null) scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        return canvasGO;
    }

    private static void FindOrCreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
        }
    }

    // ───────────────────── Hierarchy Builders ─────────────────────

    private static SetupRefs BuildSetupPanel(Transform parent)
    {
        var setupGO = CreateUIObject("SetupPanel", parent);
        StretchToFill(setupGO);
        var setupScreen = setupGO.AddComponent<SetupScreen>();

        // Host Selection Area
        var hostArea = CreateUIObject("HostSelectionArea", setupGO.transform);
        AnchorLeftHalf(hostArea);

        var hostBtnContainer = CreateUIObject("HostButtonContainer", hostArea.transform);
        hostBtnContainer.AddComponent<VerticalLayoutGroup>();
        hostBtnContainer.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var hostPortrait = CreateImage("HostPortrait", hostArea.transform);
        var hostNameText = CreateTMPText("HostNameText", hostArea.transform, "Select a Host");
        var hostBackdropText = CreateTMPText("HostBackdropText", hostArea.transform, "");
        var hostStatsText = CreateTMPText("HostStatsText", hostArea.transform, "");

        // Class Selection Area
        var classArea = CreateUIObject("ClassSelectionArea", setupGO.transform);
        AnchorRightHalf(classArea);

        var classBtnContainer = CreateUIObject("ClassButtonContainer", classArea.transform);
        classBtnContainer.AddComponent<VerticalLayoutGroup>();
        classBtnContainer.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var classNameText = CreateTMPText("ClassNameText", classArea.transform, "Select a Class");
        var classConceptText = CreateTMPText("ClassConceptText", classArea.transform, "");

        // Start Button
        var (startBtn, _) = CreateButton("StartButton", setupGO.transform, "BEGIN GESTATION");

        return new SetupRefs
        {
            setupPanelGO = setupGO,
            setupScreen = setupScreen,
            hostButtonContainer = hostBtnContainer.transform,
            hostPortrait = hostPortrait,
            hostNameText = hostNameText,
            hostBackdropText = hostBackdropText,
            hostStatsText = hostStatsText,
            classButtonContainer = classBtnContainer.transform,
            classNameText = classNameText,
            classConceptText = classConceptText,
            startButton = startBtn
        };
    }

    private static TopBarRefs BuildTopBar(Transform parent)
    {
        var topBarGO = CreateUIObject("TopBar", parent);
        AnchorTop(topBarGO, 60f);
        topBarGO.AddComponent<HorizontalLayoutGroup>();
        var hud = topBarGO.AddComponent<HUDController>();

        var biomassText = CreateTMPText("BiomassText", topBarGO.transform, "BIOMASS: 0.0");

        // Gestation Bar
        var gestBarGO = CreateUIObject("GestationBar", topBarGO.transform);
        var (gestSlider, gestFill) = CreateSlider("GestationSlider", gestBarGO.transform);
        var gestLabel = CreateTMPText("GestationLabel", gestBarGO.transform, "GESTATION: 0.0%");

        // Intervention Bar
        var intBarGO = CreateUIObject("InterventionBar", topBarGO.transform);
        var (intSlider, intFill) = CreateSlider("InterventionSlider", intBarGO.transform);
        var intLabel = CreateTMPText("InterventionLabel", intBarGO.transform, "INTERVENTION: 0.0%");

        var dayText = CreateTMPText("DayText", topBarGO.transform, "DAY 0");

        // Speed Controls
        var speedGO = CreateUIObject("SpeedControls", topBarGO.transform);
        speedGO.AddComponent<HorizontalLayoutGroup>();
        var speedCtrl = speedGO.AddComponent<SpeedControls>();

        var (pauseBtn, _) = CreateButton("PauseButton", speedGO.transform, "||");
        var (speed1Btn, _) = CreateButton("Speed1Button", speedGO.transform, "1x");
        var (speed2Btn, _) = CreateButton("Speed2Button", speedGO.transform, "2x");
        var (speed4Btn, _) = CreateButton("Speed4Button", speedGO.transform, "4x");
        var speedLabel = CreateTMPText("SpeedLabel", speedGO.transform, "1x");

        return new TopBarRefs
        {
            hudController = hud,
            biomassText = biomassText,
            gestationSlider = gestSlider,
            gestationLabel = gestLabel,
            gestationFill = gestFill,
            interventionSlider = intSlider,
            interventionLabel = intLabel,
            interventionFill = intFill,
            dayText = dayText,
            speedControls = speedCtrl,
            pauseButton = pauseBtn,
            speed1Button = speed1Btn,
            speed2Button = speed2Btn,
            speed4Button = speed4Btn,
            speedLabel = speedLabel
        };
    }

    private static LeftPanelRefs BuildLeftPanel(Transform parent)
    {
        var leftGO = CreateUIObject("LeftPanel", parent);
        AnchorLeftStrip(leftGO, 280f, 60f, 120f);
        leftGO.AddComponent<VerticalLayoutGroup>();
        var vitals = leftGO.AddComponent<HostVitalsPanel>();

        var hostName = CreateTMPText("HostNameText", leftGO.transform, "Host Name");
        var archetype = CreateTMPText("ArchetypeText", leftGO.transform, "Archetype");
        var currentTask = CreateTMPText("CurrentTaskText", leftGO.transform, "Idle");
        var taskIcon = CreateImage("TaskStatusIcon", leftGO.transform);

        // Stat bars
        var (humBar, _) = CreateSlider("HumiliationBar", leftGO.transform);
        var humText = CreateTMPText("HumiliationText", leftGO.transform, "HUMILIATION: 0");
        var (disBar, _) = CreateSlider("DiscomfortBar", leftGO.transform);
        var disText = CreateTMPText("DiscomfortText", leftGO.transform, "DISCOMFORT: 0");
        var (socBar, _) = CreateSlider("SocialBar", leftGO.transform);
        var socText = CreateTMPText("SocialText", leftGO.transform, "SOCIAL: 0");

        var stateText = CreateTMPText("StateText", leftGO.transform, "STATUS: ACTIVE");

        return new LeftPanelRefs
        {
            vitalsPanel = vitals,
            hostNameText = hostName,
            archetypeText = archetype,
            currentTaskText = currentTask,
            taskStatusIcon = taskIcon,
            humiliationBar = humBar,
            humiliationText = humText,
            discomfortBar = disBar,
            discomfortText = disText,
            socialBar = socBar,
            socialText = socText,
            stateText = stateText
        };
    }

    private static RightPanelRefs BuildRightPanel(Transform parent)
    {
        var rightGO = CreateUIObject("RightPanel", parent);
        AnchorRightStrip(rightGO, 320f, 60f, 120f);
        var skillTree = rightGO.AddComponent<SkillTreePanel>();

        // 6 branch containers
        var somaticC = CreateBranchContainer("SomaticContainer", rightGO.transform);
        var endocrineC = CreateBranchContainer("EndocrineContainer", rightGO.transform);
        var temporalC = CreateBranchContainer("TemporalContainer", rightGO.transform);
        var classAC = CreateBranchContainer("ClassAContainer", rightGO.transform);
        var classBC = CreateBranchContainer("ClassBContainer", rightGO.transform);
        var classCC = CreateBranchContainer("ClassCContainer", rightGO.transform);

        // Tooltip
        var tooltipGO = CreateUIObject("TooltipPanel", rightGO.transform);
        tooltipGO.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        tooltipGO.SetActive(false);

        var tooltipName = CreateTMPText("TooltipName", tooltipGO.transform, "Skill Name");
        var tooltipDesc = CreateTMPText("TooltipDesc", tooltipGO.transform, "Description");
        var tooltipCost = CreateTMPText("TooltipCost", tooltipGO.transform, "COST: 0 Biomass");
        var (purchaseBtn, _) = CreateButton("PurchaseButton", tooltipGO.transform, "PURCHASE");

        return new RightPanelRefs
        {
            skillTreePanel = skillTree,
            somaticContainer = somaticC.transform,
            endocrineContainer = endocrineC.transform,
            temporalContainer = temporalC.transform,
            classAContainer = classAC.transform,
            classBContainer = classBC.transform,
            classCContainer = classCC.transform,
            tooltipPanel = tooltipGO,
            tooltipName = tooltipName,
            tooltipDesc = tooltipDesc,
            tooltipCost = tooltipCost,
            purchaseButton = purchaseBtn
        };
    }

    private static BottomBarRefs BuildBottomBar(Transform parent)
    {
        var bottomGO = CreateUIObject("BottomBar", parent);
        AnchorBottom(bottomGO, 120f);
        var eventLog = bottomGO.AddComponent<EventLogPanel>();

        var (scrollRect, content) = CreateScrollRect("EventScroll", bottomGO.transform);
        var logText = CreateTMPText("LogText", content, "");
        logText.fontSize = 14;

        return new BottomBarRefs
        {
            eventLogPanel = eventLog,
            logText = logText,
            scrollRect = scrollRect
        };
    }

    private static GameOverRefs BuildGameOverOverlay(Transform parent)
    {
        var overlayGO = CreateUIObject("GameOverOverlay", parent);
        StretchToFill(overlayGO);
        var gameOver = overlayGO.AddComponent<GameOverScreen>();

        // Panel child (this is what GameOverScreen toggles on/off)
        var panelGO = CreateUIObject("Panel", overlayGO.transform);
        StretchToFill(panelGO);
        panelGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
        panelGO.SetActive(false);

        var outcomeText = CreateTMPText("OutcomeText", panelGO.transform, "OUTCOME");
        outcomeText.fontSize = 36;
        outcomeText.alignment = TextAlignmentOptions.Center;

        var summaryText = CreateTMPText("SummaryText", panelGO.transform, "Summary...");
        summaryText.alignment = TextAlignmentOptions.Center;

        var (restartBtn, _) = CreateButton("RestartButton", panelGO.transform, "RESTART");

        return new GameOverRefs
        {
            gameOverScreen = gameOver,
            panel = panelGO,
            outcomeText = outcomeText,
            summaryText = summaryText,
            restartButton = restartBtn
        };
    }

    // ───────────────────── Prefab Creation ─────────────────────

    private static GameObject CreateButtonPrefab(string prefabName, string labelText)
    {
        var go = new GameObject(prefabName);
        var img = go.AddComponent<Image>();
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        // TMP child — we must add RectTransform context manually for unparented GO
        var labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        var tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text = labelText;
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.Center;

        string path = $"Assets/Prefabs/{prefabName}.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    private static GameObject CreateSkillNodePrefab()
    {
        var go = new GameObject("SkillNodePrefab");
        var bgImg = go.AddComponent<Image>();
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = bgImg;
        var skillNodeUI = go.AddComponent<SkillNodeUI>();

        // Icon
        var iconGO = new GameObject("IconImage");
        iconGO.transform.SetParent(go.transform, false);
        var iconImg = iconGO.AddComponent<Image>();

        // Name text
        var nameGO = new GameObject("NameText");
        nameGO.transform.SetParent(go.transform, false);
        var nameText = nameGO.AddComponent<TextMeshProUGUI>();
        nameText.text = "Skill";
        nameText.fontSize = 14;

        // Cost text
        var costGO = new GameObject("CostText");
        costGO.transform.SetParent(go.transform, false);
        var costText = costGO.AddComponent<TextMeshProUGUI>();
        costText.text = "0";
        costText.fontSize = 12;

        // Wire SkillNodeUI fields before saving
        var so = new SerializedObject(skillNodeUI);
        so.FindProperty("nameText").objectReferenceValue = nameText;
        so.FindProperty("costText").objectReferenceValue = costText;
        so.FindProperty("backgroundImage").objectReferenceValue = bgImg;
        so.FindProperty("iconImage").objectReferenceValue = iconImg;
        so.ApplyModifiedPropertiesWithoutUndo();

        string path = "Assets/Prefabs/SkillNodePrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
    }

    // ───────────────────── Field Wiring ─────────────────────

    private static void WireSetupScreen(SetupRefs refs, GameObject hostBtnPrefab, GameObject classBtnPrefab, GameObject dashboardPanel)
    {
        var so = new SerializedObject(refs.setupScreen);
        so.FindProperty("hostButtonContainer").objectReferenceValue = refs.hostButtonContainer;
        so.FindProperty("hostButtonPrefab").objectReferenceValue = hostBtnPrefab;
        so.FindProperty("hostPortrait").objectReferenceValue = refs.hostPortrait;
        so.FindProperty("hostNameText").objectReferenceValue = refs.hostNameText;
        so.FindProperty("hostBackdropText").objectReferenceValue = refs.hostBackdropText;
        so.FindProperty("hostStatsText").objectReferenceValue = refs.hostStatsText;
        so.FindProperty("classButtonContainer").objectReferenceValue = refs.classButtonContainer;
        so.FindProperty("classButtonPrefab").objectReferenceValue = classBtnPrefab;
        so.FindProperty("classNameText").objectReferenceValue = refs.classNameText;
        so.FindProperty("classConceptText").objectReferenceValue = refs.classConceptText;
        so.FindProperty("startButton").objectReferenceValue = refs.startButton;
        so.FindProperty("setupPanel").objectReferenceValue = refs.setupPanelGO;
        so.FindProperty("dashboardPanel").objectReferenceValue = dashboardPanel;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireHUDController(TopBarRefs refs)
    {
        var so = new SerializedObject(refs.hudController);
        so.FindProperty("biomassText").objectReferenceValue = refs.biomassText;
        so.FindProperty("gestationSlider").objectReferenceValue = refs.gestationSlider;
        so.FindProperty("gestationLabel").objectReferenceValue = refs.gestationLabel;
        so.FindProperty("gestationFill").objectReferenceValue = refs.gestationFill;
        so.FindProperty("interventionSlider").objectReferenceValue = refs.interventionSlider;
        so.FindProperty("interventionLabel").objectReferenceValue = refs.interventionLabel;
        so.FindProperty("interventionFill").objectReferenceValue = refs.interventionFill;
        so.FindProperty("dayText").objectReferenceValue = refs.dayText;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireSpeedControls(TopBarRefs refs)
    {
        var so = new SerializedObject(refs.speedControls);
        so.FindProperty("pauseButton").objectReferenceValue = refs.pauseButton;
        so.FindProperty("speed1Button").objectReferenceValue = refs.speed1Button;
        so.FindProperty("speed2Button").objectReferenceValue = refs.speed2Button;
        so.FindProperty("speed4Button").objectReferenceValue = refs.speed4Button;
        so.FindProperty("speedLabel").objectReferenceValue = refs.speedLabel;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireHostVitalsPanel(LeftPanelRefs refs)
    {
        var so = new SerializedObject(refs.vitalsPanel);
        so.FindProperty("hostNameText").objectReferenceValue = refs.hostNameText;
        so.FindProperty("archetypeText").objectReferenceValue = refs.archetypeText;
        so.FindProperty("currentTaskText").objectReferenceValue = refs.currentTaskText;
        so.FindProperty("taskStatusIcon").objectReferenceValue = refs.taskStatusIcon;
        so.FindProperty("humiliationBar").objectReferenceValue = refs.humiliationBar;
        so.FindProperty("humiliationText").objectReferenceValue = refs.humiliationText;
        so.FindProperty("discomfortBar").objectReferenceValue = refs.discomfortBar;
        so.FindProperty("discomfortText").objectReferenceValue = refs.discomfortText;
        so.FindProperty("socialBar").objectReferenceValue = refs.socialBar;
        so.FindProperty("socialText").objectReferenceValue = refs.socialText;
        so.FindProperty("stateText").objectReferenceValue = refs.stateText;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireSkillTreePanel(RightPanelRefs refs, GameObject skillNodePrefab)
    {
        var so = new SerializedObject(refs.skillTreePanel);
        so.FindProperty("skillNodePrefab").objectReferenceValue = skillNodePrefab;
        so.FindProperty("somaticContainer").objectReferenceValue = refs.somaticContainer;
        so.FindProperty("endocrineContainer").objectReferenceValue = refs.endocrineContainer;
        so.FindProperty("temporalContainer").objectReferenceValue = refs.temporalContainer;
        so.FindProperty("classAContainer").objectReferenceValue = refs.classAContainer;
        so.FindProperty("classBContainer").objectReferenceValue = refs.classBContainer;
        so.FindProperty("classCContainer").objectReferenceValue = refs.classCContainer;
        so.FindProperty("tooltipPanel").objectReferenceValue = refs.tooltipPanel;
        so.FindProperty("tooltipName").objectReferenceValue = refs.tooltipName;
        so.FindProperty("tooltipDesc").objectReferenceValue = refs.tooltipDesc;
        so.FindProperty("tooltipCost").objectReferenceValue = refs.tooltipCost;
        so.FindProperty("purchaseButton").objectReferenceValue = refs.purchaseButton;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireEventLogPanel(BottomBarRefs refs)
    {
        var so = new SerializedObject(refs.eventLogPanel);
        so.FindProperty("logText").objectReferenceValue = refs.logText;
        so.FindProperty("scrollRect").objectReferenceValue = refs.scrollRect;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void WireGameOverScreen(GameOverRefs refs)
    {
        var so = new SerializedObject(refs.gameOverScreen);
        so.FindProperty("panel").objectReferenceValue = refs.panel;
        so.FindProperty("outcomeText").objectReferenceValue = refs.outcomeText;
        so.FindProperty("summaryText").objectReferenceValue = refs.summaryText;
        so.FindProperty("restartButton").objectReferenceValue = refs.restartButton;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ───────────────────── Helpers: Object Creation ─────────────────────

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static TextMeshProUGUI CreateTMPText(string name, Transform parent, string defaultText)
    {
        var go = CreateUIObject(name, parent);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = defaultText;
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.Left;
        return tmp;
    }

    private static Image CreateImage(string name, Transform parent)
    {
        var go = CreateUIObject(name, parent);
        return go.AddComponent<Image>();
    }

    private static (Button btn, TextMeshProUGUI label) CreateButton(string name, Transform parent, string labelText)
    {
        var go = CreateUIObject(name, parent);
        var img = go.AddComponent<Image>();
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        var label = CreateTMPText("Label", go.transform, labelText);
        label.alignment = TextAlignmentOptions.Center;
        return (btn, label);
    }

    private static (Slider slider, Image fillImage) CreateSlider(string name, Transform parent)
    {
        var go = CreateUIObject(name, parent);
        var slider = go.AddComponent<Slider>();

        // Background
        var bgGO = CreateUIObject("Background", go.transform);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        var bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // Fill Area
        var fillArea = CreateUIObject("Fill Area", go.transform);
        var fillAreaRect = fillArea.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRect.offsetMin = new Vector2(5f, 0f);
        fillAreaRect.offsetMax = new Vector2(-5f, 0f);

        // Fill
        var fillGO = CreateUIObject("Fill", fillArea.transform);
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = Color.white;
        var fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.sizeDelta = Vector2.zero;

        // Handle Slide Area (empty — no visible handle for stat bars)
        var handleArea = CreateUIObject("Handle Slide Area", go.transform);
        var handleAreaRect = handleArea.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10f, 0f);
        handleAreaRect.offsetMax = new Vector2(-10f, 0f);

        // Wire slider
        slider.fillRect = fillRect;
        slider.handleRect = null;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;

        return (slider, fillImg);
    }

    private static (ScrollRect scrollRect, Transform content) CreateScrollRect(string name, Transform parent)
    {
        var go = CreateUIObject(name, parent);
        StretchToFill(go);
        var scrollRect = go.AddComponent<ScrollRect>();
        go.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        // Viewport
        var viewport = CreateUIObject("Viewport", go.transform);
        viewport.AddComponent<Image>().color = Color.white;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        var vpRect = viewport.GetComponent<RectTransform>();
        vpRect.anchorMin = Vector2.zero;
        vpRect.anchorMax = Vector2.one;
        vpRect.sizeDelta = Vector2.zero;

        // Content
        var content = CreateUIObject("Content", viewport.transform);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        content.AddComponent<VerticalLayoutGroup>();
        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = vpRect;
        scrollRect.content = contentRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;

        return (scrollRect, content.transform);
    }

    private static GameObject CreateBranchContainer(string name, Transform parent)
    {
        var go = CreateUIObject(name, parent);
        go.AddComponent<VerticalLayoutGroup>();
        go.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        return go;
    }

    // ───────────────────── Helpers: Anchoring / Layout ─────────────────────

    private static void StretchToFill(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void AnchorTop(GameObject go, float height)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(0f, -height);
        rt.offsetMax = Vector2.zero;
    }

    private static void AnchorBottom(GameObject go, float height)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = new Vector2(1f, 0f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = new Vector2(0f, height);
    }

    private static void AnchorLeftHalf(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void AnchorRightHalf(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void AnchorLeftStrip(GameObject go, float width, float topInset, float bottomInset)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = new Vector2(0f, 1f);
        rt.offsetMin = new Vector2(0f, bottomInset);
        rt.offsetMax = new Vector2(width, -topInset);
    }

    private static void AnchorRightStrip(GameObject go, float width, float topInset, float bottomInset)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(-width, bottomInset);
        rt.offsetMax = new Vector2(0f, -topInset);
    }

    // ───────────────────── Helpers: Misc ─────────────────────

    private static void DestroyExistingChild(Transform parent, string childName)
    {
        var existing = parent.Find(childName);
        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);
    }
}
