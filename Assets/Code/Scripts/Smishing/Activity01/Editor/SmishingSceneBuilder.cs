#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Smishing01.Editor
{
    /// <summary>
    /// One-click scene builder. Menu: CyberSafeVR → Smishing → Build Scene.
    /// Creates Smishing01.unity and Smishing01_End.unity with every GameObject,
    /// component, reference, canvas, button, and material already wired up.
    ///
    /// ZERO external prefab dependencies. ZERO GUID issues.
    /// All visuals built from Unity primitives + UI components.
    /// </summary>
    public static class SmishingSceneBuilder
    {
        private const string SceneFolder    = "Assets/Scenes/Smishing";
        private const string MainScenePath  = SceneFolder + "/Smishing01.unity";
        private const string EndScenePath   = SceneFolder + "/Smishing01_End.unity";

        // ── Palette (clear, consistent) ──────────────────────────────────────
        private static readonly Color ColBg       = new Color(0.07f, 0.08f, 0.11f);
        private static readonly Color ColPanel    = new Color(0.13f, 0.14f, 0.18f);
        private static readonly Color ColAccent   = new Color(0.25f, 0.55f, 1.00f);
        private static readonly Color ColSuccess  = new Color(0.23f, 0.80f, 0.48f);
        private static readonly Color ColDanger   = new Color(0.92f, 0.30f, 0.30f);
        private static readonly Color ColWarning  = new Color(1.00f, 0.78f, 0.25f);
        private static readonly Color ColText     = new Color(0.94f, 0.94f, 0.96f);
        private static readonly Color ColMuted    = new Color(0.65f, 0.68f, 0.75f);

        [MenuItem("CyberSafeVR/Smishing/Build Complete Smishing01 Scene")]
        public static void BuildAll()
        {
            Directory.CreateDirectory(SceneFolder);

            // 1. Create the 6 scenarios
            var scenarios = SmishingScenarioCreator.CreateAll();

            // 2. Build main scene
            BuildMainScene(scenarios);

            // 3. Build debrief scene
            BuildDebriefScene();

            // 4. Register in Build Settings
            AddScenesToBuildSettings();

            // 5. Open main scene for the user
            EditorSceneManager.OpenScene(MainScenePath);

            EditorUtility.DisplayDialog(
                "Smishing Scene Built",
                "Successfully created:\n• Smishing01.unity\n• Smishing01_End.unity\n• 6 scenario ScriptableObjects\n\nPress Play to test!",
                "OK");
        }

        // ════════════════════════════════════════════════════════════════════
        //  MAIN SCENE
        // ════════════════════════════════════════════════════════════════════

        private static void BuildMainScene(SmishingScenarioData[] scenarios)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Ambient + fog
            RenderSettings.ambientMode     = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.35f, 0.42f, 0.55f);
            RenderSettings.ambientEquatorColor = new Color(0.25f, 0.28f, 0.32f);
            RenderSettings.ambientGroundColor  = new Color(0.18f, 0.18f, 0.20f);

            // Directional light
            var light = new GameObject("Directional Light").AddComponent<Light>();
            light.type      = LightType.Directional;
            light.intensity = 1.1f;
            light.color     = new Color(1f, 0.96f, 0.88f);
            light.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            light.shadows   = LightShadows.Soft;

            // Environment
            BuildEnvironment();

            // Camera
            var cam = new GameObject("Main Camera").AddComponent<Camera>();
            cam.tag            = "MainCamera";
            cam.transform.position = new Vector3(0f, 1.6f, 0f);
            cam.transform.LookAt(new Vector3(0f, 1.2f, 1.5f));
            cam.nearClipPlane  = 0.05f;
            cam.backgroundColor = new Color(0.05f, 0.07f, 0.12f);
            cam.gameObject.AddComponent<AudioListener>();
            cam.gameObject.AddComponent<SimpleCameraController>();

            // Managers
            var managersGO = new GameObject("Managers");
            var manager    = managersGO.AddComponent<SmishingManager>();
            var tracker    = managersGO.AddComponent<ScenarioProgressTracker>();
            var audio      = managersGO.AddComponent<ScenarioAudioController>();
            managersGO.AddComponent<AudioSource>(); // second source for ambient

            // Event System for UI input
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            // UI root (world space canvases live under here for organisation)
            var uiRoot = new GameObject("WorldSpaceUI").transform;

            // Intro Screen
            var intro = BuildIntroCanvas(uiRoot);

            // Phone Canvas (hidden on start)
            var phone = BuildPhoneCanvas(uiRoot);

            // Confirm Dialog (hidden)
            var confirm = BuildConfirmDialog(uiRoot);
            phone.confirmDialog = confirm;

            // HUD
            var hud = BuildHUDCanvas(uiRoot);

            // Outcome (Feedback) panels
            var outcome = BuildOutcomePanels(uiRoot);

            // Wire up the manager
            manager.scenarios        = scenarios;
            manager.introScreen      = intro;
            manager.phoneUI          = phone;
            manager.hud              = hud;
            manager.outcomeHandler   = outcome;
            manager.progressTracker  = tracker;
            manager.audioController  = audio;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, MainScenePath);
        }

        // ── Environment ──────────────────────────────────────────────────────

        private static void BuildEnvironment()
        {
            var env = new GameObject("Environment").transform;

            // Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(env);
            floor.transform.localScale = new Vector3(3f, 1f, 3f);
            ApplyMaterial(floor, new Color(0.22f, 0.24f, 0.28f), 0.0f, 0.15f);

            // 4 walls
            BuildWall(env, "Wall_N", new Vector3(0f,  2.5f,  8f), new Vector3(16f, 5f, 0.3f));
            BuildWall(env, "Wall_S", new Vector3(0f,  2.5f, -8f), new Vector3(16f, 5f, 0.3f));
            BuildWall(env, "Wall_E", new Vector3( 8f, 2.5f, 0f),  new Vector3(0.3f, 5f, 16f));
            BuildWall(env, "Wall_W", new Vector3(-8f, 2.5f, 0f),  new Vector3(0.3f, 5f, 16f));

            // Desk
            var desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            desk.name = "Desk";
            desk.transform.SetParent(env);
            desk.transform.position   = new Vector3(0f, 0.75f, 1.3f);
            desk.transform.localScale = new Vector3(1.5f, 0.05f, 0.7f);
            ApplyMaterial(desk, new Color(0.45f, 0.30f, 0.22f), 0.0f, 0.25f);

            for (int i = 0; i < 4; i++)
            {
                var leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.name = $"DeskLeg_{i}";
                leg.transform.SetParent(desk.transform);
                leg.transform.localScale    = new Vector3(0.04f, 15f, 0.04f);
                float x = (i % 2 == 0 ? -0.45f : 0.45f);
                float z = (i < 2 ? -0.45f : 0.45f);
                leg.transform.localPosition = new Vector3(x, -7.5f, z);
                ApplyMaterial(leg, new Color(0.35f, 0.22f, 0.15f), 0.0f, 0.3f);
            }

            // Subtle accent light from above
            var accent = new GameObject("AccentLight").AddComponent<Light>();
            accent.transform.SetParent(env);
            accent.transform.position = new Vector3(0f, 3.5f, 1f);
            accent.type       = LightType.Point;
            accent.color      = new Color(0.4f, 0.6f, 1f);
            accent.intensity  = 2.5f;
            accent.range      = 6f;
        }

        private static void BuildWall(Transform parent, string name, Vector3 pos, Vector3 scale)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position   = pos;
            wall.transform.localScale = scale;
            ApplyMaterial(wall, new Color(0.18f, 0.20f, 0.24f), 0.0f, 0.35f);
        }

        // ── Intro Canvas ─────────────────────────────────────────────────────

        private static IntroScreenController BuildIntroCanvas(Transform parent)
        {
            var go = CreateWorldCanvas("IntroCanvas", parent,
                new Vector3(0f, 1.6f, 1.5f), new Vector2(800f, 550f), 0.0015f);

            var cg = go.AddComponent<CanvasGroup>();

            // Dim backdrop
            var bg = CreateImage("Backdrop", go.transform, new Color(0.05f, 0.06f, 0.09f, 0.98f));
            StretchFill(bg);

            // Title
            var title = CreateText("Title", go.transform, "SMISHING AWARENESS",
                54, ColAccent, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(title.rectTransform, 80f, 380f);

            // Subtitle
            var subtitle = CreateText("Subtitle", go.transform,
                "Spot the scam before you get scammed.", 26,
                ColMuted, TextAlignmentOptions.Center, FontStyles.Italic);
            AnchorTop(subtitle.rectTransform, 150f, 50f);

            // Briefing body
            var briefing = CreateText("Briefing", go.transform,
                "You'll receive <b>6 text messages</b> on your phone.\n\n" +
                "For each one, decide:\n" +
                "   <color=#5EE8A6><b>Report</b></color> — if you think it's a scam\n" +
                "   <color=#C9CCD4><b>Ignore</b></color> — if it's safe and needs no action\n" +
                "   <color=#FFB84D><b>Open Link</b></color> — if you trust the URL\n\n" +
                "Use the <b>Hint</b> button if you're stuck — but only once per scenario.",
                22, ColText, TextAlignmentOptions.TopLeft);
            AnchorCentre(briefing.rectTransform, new Vector2(0f, 20f), new Vector2(680f, 260f));

            // Start button
            var startBtn = CreateButton("StartButton", go.transform, "BEGIN  →",
                ColAccent, new Vector2(280f, 70f));
            AnchorBottom(startBtn.GetComponent<RectTransform>(), 50f);

            // Controller
            var controller          = go.AddComponent<IntroScreenController>();
            controller.canvasGroup  = cg;
            controller.titleText    = title;
            controller.briefingText = briefing;
            controller.startButton  = startBtn;

            return controller;
        }

        // ── Phone Canvas ─────────────────────────────────────────────────────

        private static PhoneMessageUI BuildPhoneCanvas(Transform parent)
        {
            var go = CreateWorldCanvas("PhoneScreenCanvas", parent,
                new Vector3(-0.45f, 1.4f, 1.2f), new Vector2(450f, 780f), 0.0012f);
            go.transform.localRotation = Quaternion.Euler(0f, -15f, 0f);

            var cg = go.AddComponent<CanvasGroup>();

            // Phone body (rounded-ish dark rectangle via Image)
            var phoneBody = CreateImage("PhoneBody", go.transform, new Color(0.05f, 0.05f, 0.07f, 1f));
            StretchFill(phoneBody, inset: -15f);

            // Screen area
            var screen = CreateImage("Screen", phoneBody.transform, ColBg);
            StretchFill(screen, inset: 10f);

            // Status bar (top)
            var statusBar = CreateImage("StatusBar", screen.transform, new Color(0.04f, 0.04f, 0.06f));
            AnchorTop(statusBar.rectTransform, 0f, 45f);

            var time = CreateText("Time", statusBar.transform, "09:41",
                18, ColText, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            AnchorLeft(time.rectTransform, 20f);

            var signal = CreateText("SignalBars", statusBar.transform, "▂▄▆█  5G  100%",
                16, ColText, TextAlignmentOptions.MidlineRight);
            AnchorRight(signal.rectTransform, 20f);

            // Sender row
            var senderRow = CreateImage("SenderRow", screen.transform, ColPanel);
            AnchorTop(senderRow.rectTransform, 55f, 80f);

            var senderIcon = CreateImage("SenderIcon", senderRow.transform, ColAccent);
            var iconRT = senderIcon.rectTransform;
            iconRT.anchorMin = iconRT.anchorMax = new Vector2(0f, 0.5f);
            iconRT.pivot     = new Vector2(0f, 0.5f);
            iconRT.anchoredPosition = new Vector2(20f, 0f);
            iconRT.sizeDelta  = new Vector2(50f, 50f);

            var sender = CreateText("SenderName", senderRow.transform, "Sender",
                22, ColText, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            var srt = sender.rectTransform;
            srt.anchorMin = new Vector2(0f, 0f);
            srt.anchorMax = new Vector2(1f, 1f);
            srt.offsetMin = new Vector2(85f, 5f);
            srt.offsetMax = new Vector2(-15f, -5f);

            // Message body
            var msgBg = CreateImage("MessageBg", screen.transform, ColPanel);
            AnchorTop(msgBg.rectTransform, 145f, 340f);
            var msgRT = msgBg.rectTransform;
            msgRT.anchorMin = new Vector2(0f, 1f);
            msgRT.anchorMax = new Vector2(1f, 1f);
            msgRT.offsetMin = new Vector2(15f, 0f);
            msgRT.offsetMax = new Vector2(-15f, 0f);
            msgRT.sizeDelta = new Vector2(-30f, 340f);
            msgRT.anchoredPosition = new Vector2(0f, -145f);

            var msgBody = CreateText("MessageBody", msgBg.transform, "Message body…",
                20, ColText, TextAlignmentOptions.TopLeft);
            StretchFill(msgBody, inset: 20f);
            msgBody.enableWordWrapping = true;

            // URL (if present)
            var urlBg = CreateImage("UrlBg", screen.transform, new Color(0.10f, 0.14f, 0.22f));
            var urlRT = urlBg.rectTransform;
            urlRT.anchorMin = new Vector2(0f, 1f);
            urlRT.anchorMax = new Vector2(1f, 1f);
            urlRT.offsetMin = new Vector2(15f, 0f);
            urlRT.offsetMax = new Vector2(-15f, 0f);
            urlRT.sizeDelta = new Vector2(-30f, 60f);
            urlRT.anchoredPosition = new Vector2(0f, -500f);

            var urlText = CreateText("UrlText", urlBg.transform, "",
                18, ColAccent, TextAlignmentOptions.MidlineLeft, FontStyles.Underline);
            StretchFill(urlText, inset: 15f);

            // Hint text (below URL, hidden by default)
            var hintText = CreateText("HintText", screen.transform, "",
                16, ColWarning, TextAlignmentOptions.TopLeft, FontStyles.Italic);
            var hintRT = hintText.rectTransform;
            hintRT.anchorMin = new Vector2(0f, 1f);
            hintRT.anchorMax = new Vector2(1f, 1f);
            hintRT.offsetMin = new Vector2(20f, 0f);
            hintRT.offsetMax = new Vector2(-20f, 0f);
            hintRT.sizeDelta = new Vector2(-40f, 80f);
            hintRT.anchoredPosition = new Vector2(0f, -575f);
            hintText.enableWordWrapping = true;
            hintText.gameObject.SetActive(false);

            // Hint button
            var hintBtn = CreateButton("HintButton", screen.transform, "?  Hint",
                new Color(0.4f, 0.35f, 0.15f), new Vector2(120f, 40f));
            var hbRT = hintBtn.GetComponent<RectTransform>();
            hbRT.anchorMin = new Vector2(1f, 1f);
            hbRT.anchorMax = new Vector2(1f, 1f);
            hbRT.pivot     = new Vector2(1f, 1f);
            hbRT.anchoredPosition = new Vector2(-20f, -60f);

            // Action buttons row (at bottom)
            var btnRow = CreateImage("ButtonRow", screen.transform, Color.clear);
            var brRT = btnRow.rectTransform;
            brRT.anchorMin = new Vector2(0f, 0f);
            brRT.anchorMax = new Vector2(1f, 0f);
            brRT.pivot     = new Vector2(0.5f, 0f);
            brRT.sizeDelta = new Vector2(-20f, 90f);
            brRT.anchoredPosition = new Vector2(0f, 15f);

            var reportBtn = CreateButton("ReportButton",   btnRow.transform, "⚑  Report",   ColDanger,  new Vector2(130f, 80f));
            var ignoreBtn = CreateButton("IgnoreButton",   btnRow.transform, "✕  Ignore",   new Color(0.4f, 0.4f, 0.45f), new Vector2(130f, 80f));
            var clickBtn  = CreateButton("ClickLinkButton",btnRow.transform, "↗  Open",     ColAccent,  new Vector2(130f, 80f));

            var hlg = btnRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.childAlignment = TextAnchor.MiddleCenter;

            // Controller
            var pm = go.AddComponent<PhoneMessageUI>();
            pm.senderNameText  = sender;
            pm.messageBodyText = msgBody;
            pm.embeddedUrlText = urlText;
            pm.timeStampText   = time;
            pm.hintText        = hintText;
            pm.reportButton    = reportBtn;
            pm.ignoreButton    = ignoreBtn;
            pm.clickLinkButton = clickBtn;
            pm.hintButton      = hintBtn;
            pm.canvasGroup     = cg;
            pm.phoneBody       = phoneBody.transform;

            return pm;
        }

        // ── Confirm Dialog ───────────────────────────────────────────────────

        private static ConfirmDialogController BuildConfirmDialog(Transform parent)
        {
            var go = CreateWorldCanvas("ConfirmDialog", parent,
                new Vector3(0f, 1.55f, 1.3f), new Vector2(520f, 280f), 0.0015f);
            go.transform.localPosition = new Vector3(0f, 1.55f, 1.2f);

            var cg = go.AddComponent<CanvasGroup>();

            var bg = CreateImage("Bg", go.transform, new Color(0.08f, 0.09f, 0.12f, 0.98f));
            StretchFill(bg);

            // Warning border
            var border = CreateImage("Border", go.transform, ColWarning);
            var bRT = border.rectTransform;
            bRT.anchorMin = Vector2.zero; bRT.anchorMax = Vector2.one;
            bRT.offsetMin = Vector2.zero; bRT.offsetMax = Vector2.zero;
            border.transform.SetAsFirstSibling();

            var title = CreateText("Title", go.transform, "⚠  Are you sure?",
                28, ColWarning, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(title.rectTransform, 30f, 50f);

            var body = CreateText("Body", go.transform,
                "Clicking unknown links can lead to credential theft or malware.",
                18, ColText, TextAlignmentOptions.Center);
            AnchorCentre(body.rectTransform, new Vector2(0f, 0f), new Vector2(460f, 90f));
            body.enableWordWrapping = true;

            var row = new GameObject("ButtonRow");
            row.transform.SetParent(go.transform, false);
            var rowRT = row.AddComponent<RectTransform>();
            rowRT.anchorMin = new Vector2(0.5f, 0f);
            rowRT.anchorMax = new Vector2(0.5f, 0f);
            rowRT.pivot     = new Vector2(0.5f, 0f);
            rowRT.sizeDelta = new Vector2(400f, 60f);
            rowRT.anchoredPosition = new Vector2(0f, 25f);

            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;

            var cancelBtn  = CreateButton("CancelButton",  row.transform, "Cancel",       new Color(0.35f, 0.35f, 0.4f), new Vector2(160f, 55f));
            var confirmBtn = CreateButton("ConfirmButton", row.transform, "Open anyway",  ColDanger,                     new Vector2(200f, 55f));

            var cd = go.AddComponent<ConfirmDialogController>();
            cd.canvasGroup   = cg;
            cd.titleText     = title;
            cd.bodyText      = body;
            cd.confirmButton = confirmBtn;
            cd.cancelButton  = cancelBtn;

            return cd;
        }

        // ── HUD Canvas ───────────────────────────────────────────────────────

        private static HUDController BuildHUDCanvas(Transform parent)
        {
            var go = CreateWorldCanvas("HUDCanvas", parent,
                new Vector3(0f, 2.3f, 1.4f), new Vector2(900f, 260f), 0.0014f);

            var bg = CreateImage("Bg", go.transform, new Color(0.08f, 0.09f, 0.12f, 0.90f));
            StretchFill(bg);

            // Title (scenario title)
            var title = CreateText("Title", go.transform, "Scenario Title",
                32, ColAccent, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(title.rectTransform, 20f, 45f);

            // Narration
            var narration = CreateText("Narration", go.transform, "Narration goes here.",
                20, ColText, TextAlignmentOptions.Center);
            AnchorCentre(narration.rectTransform, new Vector2(0f, 10f), new Vector2(820f, 80f));
            narration.enableWordWrapping = true;

            // Step label
            var step = CreateText("StepLabel", go.transform, "Step 1 of 6",
                16, ColMuted, TextAlignmentOptions.MidlineLeft);
            var stRT = step.rectTransform;
            stRT.anchorMin = new Vector2(0f, 0f); stRT.anchorMax = new Vector2(0f, 0f);
            stRT.pivot = new Vector2(0f, 0f);
            stRT.anchoredPosition = new Vector2(25f, 20f);
            stRT.sizeDelta = new Vector2(200f, 30f);

            // Score
            var score = CreateText("Score", go.transform, "Score: 0 / 6",
                16, ColMuted, TextAlignmentOptions.MidlineRight);
            var scRT = score.rectTransform;
            scRT.anchorMin = new Vector2(1f, 0f); scRT.anchorMax = new Vector2(1f, 0f);
            scRT.pivot = new Vector2(1f, 0f);
            scRT.anchoredPosition = new Vector2(-25f, 20f);
            scRT.sizeDelta = new Vector2(200f, 30f);

            // Dots row
            var dotsRowGO = new GameObject("Dots");
            dotsRowGO.transform.SetParent(go.transform, false);
            var dotsRT = dotsRowGO.AddComponent<RectTransform>();
            dotsRT.anchorMin = new Vector2(0.5f, 0f); dotsRT.anchorMax = new Vector2(0.5f, 0f);
            dotsRT.pivot     = new Vector2(0.5f, 0f);
            dotsRT.anchoredPosition = new Vector2(0f, 25f);
            dotsRT.sizeDelta = new Vector2(400f, 30f);

            var dotsLayout = dotsRowGO.AddComponent<HorizontalLayoutGroup>();
            dotsLayout.spacing = 12;
            dotsLayout.childAlignment = TextAnchor.MiddleCenter;
            dotsLayout.childControlWidth  = false;
            dotsLayout.childControlHeight = false;

            // Template dot
            var dot = CreateImage("DotTemplate", dotsRowGO.transform, new Color(0.25f, 0.25f, 0.28f));
            var dotRT = dot.rectTransform;
            dotRT.sizeDelta = new Vector2(20f, 20f);
            dot.gameObject.SetActive(false);

            var hud = go.AddComponent<HUDController>();
            hud.titleText      = title;
            hud.narrationText  = narration;
            hud.stepLabel      = step;
            hud.scoreText      = score;
            hud.dotsContainer  = dotsRowGO.transform;
            hud.dotTemplate    = dot;

            return hud;
        }

        // ── Outcome Panels ───────────────────────────────────────────────────

        private static ScenarioOutcomeHandler BuildOutcomePanels(Transform parent)
        {
            // Correct panel
            var correctGO = CreateWorldCanvas("CorrectPanel", parent,
                new Vector3(0f, 1.5f, 1.5f), new Vector2(700f, 500f), 0.0015f);
            var correctCG = correctGO.AddComponent<CanvasGroup>();

            var cBg = CreateImage("Bg", correctGO.transform, new Color(0.08f, 0.15f, 0.10f, 0.97f));
            StretchFill(cBg);

            var cBorder = CreateImage("BorderTop", correctGO.transform, ColSuccess);
            var cbRT = cBorder.rectTransform;
            cbRT.anchorMin = new Vector2(0f, 1f); cbRT.anchorMax = new Vector2(1f, 1f);
            cbRT.pivot = new Vector2(0.5f, 1f);
            cbRT.sizeDelta = new Vector2(0f, 10f);

            // Big check icon (built from 2 rotated rectangles)
            BuildCheckIcon(correctGO.transform, new Vector2(0f, 140f), ColSuccess);

            var cTitle = CreateText("Title", correctGO.transform, "CORRECT",
                46, ColSuccess, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(cTitle.rectTransform, 210f, 60f);

            var cBody = CreateText("Body", correctGO.transform, "",
                22, ColText, TextAlignmentOptions.Center);
            AnchorCentre(cBody.rectTransform, new Vector2(0f, -30f), new Vector2(620f, 180f));
            cBody.enableWordWrapping = true;

            var cContinue = CreateButton("Continue", correctGO.transform, "Continue  →",
                ColSuccess, new Vector2(240f, 60f));
            AnchorBottom(cContinue.GetComponent<RectTransform>(), 40f);

            correctGO.SetActive(false);

            // Incorrect panel
            var wrongGO = CreateWorldCanvas("IncorrectPanel", parent,
                new Vector3(0f, 1.5f, 1.5f), new Vector2(700f, 600f), 0.0015f);
            var wrongCG = wrongGO.AddComponent<CanvasGroup>();

            var wBg = CreateImage("Bg", wrongGO.transform, new Color(0.18f, 0.08f, 0.08f, 0.97f));
            StretchFill(wBg);

            var wBorder = CreateImage("BorderTop", wrongGO.transform, ColDanger);
            var wbRT = wBorder.rectTransform;
            wbRT.anchorMin = new Vector2(0f, 1f); wbRT.anchorMax = new Vector2(1f, 1f);
            wbRT.pivot = new Vector2(0.5f, 1f);
            wbRT.sizeDelta = new Vector2(0f, 10f);

            BuildCrossIcon(wrongGO.transform, new Vector2(0f, 180f), ColDanger);

            var wTitle = CreateText("Title", wrongGO.transform, "INCORRECT",
                46, ColDanger, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(wTitle.rectTransform, 250f, 60f);

            var wBody = CreateText("Body", wrongGO.transform, "",
                20, ColText, TextAlignmentOptions.Center);
            AnchorCentre(wBody.rectTransform, new Vector2(0f, 20f), new Vector2(620f, 130f));
            wBody.enableWordWrapping = true;

            var redFlags = CreateText("RedFlags", wrongGO.transform, "",
                18, ColWarning, TextAlignmentOptions.TopLeft);
            AnchorCentre(redFlags.rectTransform, new Vector2(0f, -120f), new Vector2(580f, 160f));
            redFlags.enableWordWrapping = true;

            var wContinue = CreateButton("Continue", wrongGO.transform, "Continue  →",
                ColDanger, new Vector2(240f, 60f));
            AnchorBottom(wContinue.GetComponent<RectTransform>(), 40f);

            wrongGO.SetActive(false);

            // Handler on parent root
            var handlerGO = new GameObject("OutcomeHandler");
            handlerGO.transform.SetParent(parent, false);
            var handler = handlerGO.AddComponent<ScenarioOutcomeHandler>();
            handler.correctPanel            = correctGO;
            handler.incorrectPanel          = wrongGO;
            handler.correctGroup            = correctCG;
            handler.incorrectGroup          = wrongCG;
            handler.correctFeedbackText     = cBody;
            handler.incorrectFeedbackText   = wBody;
            handler.redFlagsText            = redFlags;
            handler.correctContinueButton   = cContinue;
            handler.incorrectContinueButton = wContinue;

            return handler;
        }

        // ── Primitive icons ──────────────────────────────────────────────────

        private static void BuildCheckIcon(Transform parent, Vector2 anchorPos, Color color)
        {
            var container = new GameObject("CheckIcon");
            container.transform.SetParent(parent, false);
            var crt = container.AddComponent<RectTransform>();
            crt.anchorMin = new Vector2(0.5f, 1f); crt.anchorMax = new Vector2(0.5f, 1f);
            crt.pivot = new Vector2(0.5f, 1f);
            crt.anchoredPosition = anchorPos;
            crt.sizeDelta = new Vector2(120f, 120f);

            // Two rotated bars forming a check
            var shortBar = CreateImage("Short", container.transform, color);
            var sRT = shortBar.rectTransform;
            sRT.sizeDelta = new Vector2(14f, 60f);
            sRT.anchoredPosition = new Vector2(-28f, -8f);
            sRT.rotation = Quaternion.Euler(0, 0, 45);

            var longBar = CreateImage("Long", container.transform, color);
            var lRT = longBar.rectTransform;
            lRT.sizeDelta = new Vector2(14f, 100f);
            lRT.anchoredPosition = new Vector2(15f, 5f);
            lRT.rotation = Quaternion.Euler(0, 0, -45);
        }

        private static void BuildCrossIcon(Transform parent, Vector2 anchorPos, Color color)
        {
            var container = new GameObject("CrossIcon");
            container.transform.SetParent(parent, false);
            var crt = container.AddComponent<RectTransform>();
            crt.anchorMin = new Vector2(0.5f, 1f); crt.anchorMax = new Vector2(0.5f, 1f);
            crt.pivot = new Vector2(0.5f, 1f);
            crt.anchoredPosition = anchorPos;
            crt.sizeDelta = new Vector2(120f, 120f);

            var bar1 = CreateImage("Bar1", container.transform, color);
            var b1RT = bar1.rectTransform;
            b1RT.sizeDelta = new Vector2(14f, 110f);
            b1RT.rotation = Quaternion.Euler(0, 0, 45);

            var bar2 = CreateImage("Bar2", container.transform, color);
            var b2RT = bar2.rectTransform;
            b2RT.sizeDelta = new Vector2(14f, 110f);
            b2RT.rotation = Quaternion.Euler(0, 0, -45);
        }

        // ════════════════════════════════════════════════════════════════════
        //  DEBRIEF SCENE
        // ════════════════════════════════════════════════════════════════════

        private static void BuildDebriefScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            RenderSettings.ambientMode     = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.25f, 0.30f, 0.40f);

            var light = new GameObject("Directional Light").AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.9f;
            light.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            var cam = new GameObject("Main Camera").AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.transform.position = new Vector3(0f, 1.6f, 0f);
            cam.transform.LookAt(new Vector3(0f, 1.6f, 1f));
            cam.nearClipPlane = 0.05f;
            cam.backgroundColor = new Color(0.04f, 0.05f, 0.09f);
            cam.gameObject.AddComponent<AudioListener>();

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            // Debrief canvas
            var go = CreateWorldCanvas("DebriefCanvas", null,
                new Vector3(0f, 1.6f, 1.5f), new Vector2(800f, 650f), 0.0015f);

            var bg = CreateImage("Bg", go.transform, new Color(0.07f, 0.08f, 0.11f, 0.98f));
            StretchFill(bg);

            var border = CreateImage("BorderTop", go.transform, ColAccent);
            var bRT = border.rectTransform;
            bRT.anchorMin = new Vector2(0f, 1f); bRT.anchorMax = new Vector2(1f, 1f);
            bRT.pivot = new Vector2(0.5f, 1f);
            bRT.sizeDelta = new Vector2(0f, 10f);

            var title = CreateText("Title", go.transform, "ACTIVITY COMPLETE",
                48, ColAccent, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(title.rectTransform, 40f, 70f);

            var scoreLabel = CreateText("ScoreLabel", go.transform, "Your Score",
                22, ColMuted, TextAlignmentOptions.Center);
            AnchorTop(scoreLabel.rectTransform, 130f, 35f);

            var score = CreateText("Score", go.transform, "— / —",
                64, ColText, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(score.rectTransform, 170f, 90f);

            var grade = CreateText("Grade", go.transform, "—",
                30, ColSuccess, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(grade.rectTransform, 270f, 50f);

            var hints = CreateText("Hints", go.transform, "",
                18, ColMuted, TextAlignmentOptions.Center, FontStyles.Italic);
            AnchorTop(hints.rectTransform, 330f, 30f);

            var takeaways = CreateText("Takeaways", go.transform,
                "<b>Key takeaways</b>\n" +
                "• Never click links in unexpected SMS messages\n" +
                "• Check domains for misspellings or wrong TLDs\n" +
                "• Urgency language is a manipulation tactic\n" +
                "• Legitimate organisations never request credentials via SMS",
                18, ColText, TextAlignmentOptions.TopLeft);
            AnchorCentre(takeaways.rectTransform, new Vector2(0f, -75f), new Vector2(680f, 150f));
            takeaways.enableWordWrapping = true;

            // Buttons row
            var row = new GameObject("Buttons");
            row.transform.SetParent(go.transform, false);
            var rRT = row.AddComponent<RectTransform>();
            rRT.anchorMin = new Vector2(0.5f, 0f); rRT.anchorMax = new Vector2(0.5f, 0f);
            rRT.pivot = new Vector2(0.5f, 0f);
            rRT.anchoredPosition = new Vector2(0f, 40f);
            rRT.sizeDelta = new Vector2(700f, 70f);

            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;

            var retry = CreateButton("RetryButton", row.transform, "↻  Retry",
                new Color(0.4f, 0.4f, 0.45f), new Vector2(200f, 60f));
            var menu  = CreateButton("MenuButton",  row.transform, "≡  Main Menu",
                new Color(0.4f, 0.4f, 0.45f), new Vector2(200f, 60f));
            var next  = CreateButton("NextButton",  row.transform, "Next Module  →",
                ColAccent, new Vector2(240f, 60f));

            var dbc = go.AddComponent<DebriefSceneController>();
            dbc.scoreText   = score;
            dbc.gradeText   = grade;
            dbc.hintsText   = hints;
            dbc.retryButton = retry;
            dbc.menuButton  = menu;
            dbc.nextButton  = next;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, EndScenePath);
        }

        // ════════════════════════════════════════════════════════════════════
        //  BUILD SETTINGS
        // ════════════════════════════════════════════════════════════════════

        private static void AddScenesToBuildSettings()
        {
            var existing = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            AddIfMissing(existing, MainScenePath);
            AddIfMissing(existing, EndScenePath);
            EditorBuildSettings.scenes = existing.ToArray();
        }

        private static void AddIfMissing(List<EditorBuildSettingsScene> list, string path)
        {
            if (!list.Exists(s => s.path == path))
                list.Add(new EditorBuildSettingsScene(path, true));
        }

        // ════════════════════════════════════════════════════════════════════
        //  UI HELPERS
        // ════════════════════════════════════════════════════════════════════

        private static GameObject CreateWorldCanvas(string name, Transform parent,
            Vector3 worldPos, Vector2 size, float scale)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);

            go.transform.position   = worldPos;
            go.transform.localScale = Vector3.one * scale;

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;

            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            return go;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            return img;
        }

        private static TMP_Text CreateText(string name, Transform parent, string text,
            float fontSize, Color color, TextAlignmentOptions alignment,
            FontStyles style = FontStyles.Normal)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.color     = color;
            tmp.alignment = alignment;
            tmp.fontStyle = style;
            tmp.enableWordWrapping = false;
            tmp.richText  = true;
            return tmp;
        }

        private static Button CreateButton(string name, Transform parent, string label,
            Color bgColor, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = size;

            var img = go.AddComponent<Image>();
            img.color = bgColor;

            var btn = go.AddComponent<Button>();
            var cb = btn.colors;
            cb.highlightedColor = Color.Lerp(bgColor, Color.white, 0.2f);
            cb.pressedColor     = Color.Lerp(bgColor, Color.black, 0.25f);
            cb.selectedColor    = bgColor;
            cb.disabledColor    = new Color(bgColor.r * 0.5f, bgColor.g * 0.5f, bgColor.b * 0.5f, 0.6f);
            btn.colors = cb;

            go.AddComponent<ButtonHoverEffect>();

            var labelText = CreateText("Label", go.transform, label, 22,
                ColText, TextAlignmentOptions.Center, FontStyles.Bold);
            StretchFill(labelText, inset: 5f);

            return btn;
        }

        private static void ApplyMaterial(GameObject go, Color color, float metallic, float smoothness)
        {
            var rend = go.GetComponent<Renderer>();
            if (rend == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader);
            mat.color = color;
            if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
            if (mat.HasProperty("_BaseColor"))  mat.SetColor("_BaseColor",  color);
            rend.sharedMaterial = mat;
        }

        // ── Anchor shortcuts ─────────────────────────────────────────────────

        private static void StretchFill(Graphic g, float inset = 0f)
        {
            var rt = g.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(inset, inset);
            rt.offsetMax = new Vector2(-inset, -inset);
        }

        private static void AnchorTop(RectTransform rt, float topOffset, float height)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot     = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(0f, height);
            rt.anchoredPosition = new Vector2(0f, -topOffset);
        }

        private static void AnchorBottom(RectTransform rt, float bottomOffset)
        {
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot     = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, bottomOffset);
        }

        private static void AnchorLeft(RectTransform rt, float leftOffset)
        {
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot     = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(leftOffset, 0f);
        }

        private static void AnchorRight(RectTransform rt, float rightOffset)
        {
            rt.anchorMin = new Vector2(1f, 0.5f);
            rt.anchorMax = new Vector2(1f, 0.5f);
            rt.pivot     = new Vector2(1f, 0.5f);
            rt.anchoredPosition = new Vector2(-rightOffset, 0f);
        }

        private static void AnchorCentre(RectTransform rt, Vector2 offset, Vector2 size)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = offset;
        }
    }
}
#endif
