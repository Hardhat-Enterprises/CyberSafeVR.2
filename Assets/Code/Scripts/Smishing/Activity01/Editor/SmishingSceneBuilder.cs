#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Smishing01.Editor
{
    /// <summary>
    /// v4 — Polished, project-quality scene builder.
    /// Menu: CyberSafeVR → Smishing → Build Complete Smishing01 Scene.
    /// </summary>
    public static class SmishingSceneBuilder
    {
        private const string SceneFolder    = "Assets/Scenes/Smishing";
        private const string MainScenePath  = SceneFolder + "/Smishing01.unity";
        private const string EndScenePath   = SceneFolder + "/Smishing01_End.unity";

        // ── Palette ──────────────────────────────────────────────────────────
        private static readonly Color ColBgTop    = new Color(0.13f, 0.16f, 0.24f);
        private static readonly Color ColBgBot    = new Color(0.05f, 0.06f, 0.10f);
        private static readonly Color ColPanelTop = new Color(0.16f, 0.18f, 0.24f);
        private static readonly Color ColPanelBot = new Color(0.10f, 0.11f, 0.15f);
        private static readonly Color ColAccent   = new Color(0.30f, 0.65f, 1.00f);
        private static readonly Color ColAccent2  = new Color(0.55f, 0.40f, 1.00f);
        private static readonly Color ColSuccess  = new Color(0.25f, 0.85f, 0.55f);
        private static readonly Color ColDanger   = new Color(0.95f, 0.32f, 0.35f);
        private static readonly Color ColWarning  = new Color(1.00f, 0.78f, 0.30f);
        private static readonly Color ColText     = new Color(0.96f, 0.97f, 1.00f);
        private static readonly Color ColMuted    = new Color(0.65f, 0.70f, 0.80f);

        [MenuItem("CyberSafeVR/Smishing/Build Complete Smishing01 Scene")]
        public static void BuildAll()
        {
            Directory.CreateDirectory(SceneFolder);

            var scenarios = SmishingScenarioCreator.CreateAll();
            BuildMainScene(scenarios);
            BuildDebriefScene();
            AddScenesToBuildSettings();

            EditorSceneManager.OpenScene(MainScenePath);

            EditorUtility.DisplayDialog("Smishing Scene Built ✓",
                "v4 scene generated:\n" +
                "• Polished office environment (desk, monitor, lamp, posters)\n" +
                "• Animated phone with idle bob + tilt\n" +
                "• Gradient UI panels\n" +
                "• 6 scenario ScriptableObjects\n\n" +
                "Press Cmd+S, then Play to test.",
                "OK");
        }

        // ════════════════════════════════════════════════════════════════════
        //  MAIN SCENE
        // ════════════════════════════════════════════════════════════════════

        private static void BuildMainScene(SmishingScenarioData[] scenarios)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            ConfigureLighting();

            var light = new GameObject("Sun").AddComponent<Light>();
            light.type      = LightType.Directional;
            light.intensity = 0.85f;
            light.color     = new Color(1f, 0.94f, 0.82f);
            light.transform.rotation = Quaternion.Euler(38f, -42f, 0f);
            light.shadows   = LightShadows.Soft;

            BuildEnvironment();
            BuildDesk();
            BuildPosters();
            BuildAccentLights();
            BuildAmbientParticles();

            // Camera looks at the desk where phone will hover
            var cam = new GameObject("Main Camera").AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.transform.position = new Vector3(0f, 1.55f, -0.6f);
            cam.transform.rotation = Quaternion.Euler(5f, 0f, 0f);
            cam.nearClipPlane  = 0.05f;
            cam.farClipPlane   = 100f;
            cam.backgroundColor = new Color(0.04f, 0.05f, 0.10f);
            cam.fieldOfView    = 60f;
            cam.gameObject.AddComponent<AudioListener>();
            cam.gameObject.AddComponent<SimpleCameraController>();

            // Managers
            var managersGO = new GameObject("Managers");
            var manager    = managersGO.AddComponent<SmishingManager>();
            var tracker    = managersGO.AddComponent<ScenarioProgressTracker>();
            var audio      = managersGO.AddComponent<ScenarioAudioController>();
            managersGO.AddComponent<AudioSource>(); // ambient source

            // Event System
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();

            // World UI
            var uiRoot = new GameObject("WorldSpaceUI").transform;

            var intro   = BuildIntroCanvas(uiRoot);
            var phone   = BuildPhoneCanvas(uiRoot);
            var confirm = BuildConfirmDialog(uiRoot);
            phone.confirmDialog = confirm;

            var hud     = BuildHUDCanvas(uiRoot);
            var outcome = BuildOutcomePanels(uiRoot);

            // Wire manager
            manager.scenarios       = scenarios;
            manager.introScreen     = intro;
            manager.phoneUI         = phone;
            manager.hud             = hud;
            manager.outcomeHandler  = outcome;
            manager.progressTracker = tracker;
            manager.audioController = audio;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, MainScenePath);
        }

        // ── Lighting / atmosphere ────────────────────────────────────────────

        private static void ConfigureLighting()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor     = new Color(0.18f, 0.22f, 0.32f);
            RenderSettings.ambientEquatorColor = new Color(0.13f, 0.14f, 0.18f);
            RenderSettings.ambientGroundColor  = new Color(0.06f, 0.06f, 0.07f);

            RenderSettings.fog        = true;
            RenderSettings.fogColor   = new Color(0.07f, 0.08f, 0.12f);
            RenderSettings.fogMode    = FogMode.Linear;
            RenderSettings.fogStartDistance = 6f;
            RenderSettings.fogEndDistance   = 22f;
        }

        // ── Environment (room) ───────────────────────────────────────────────

        private static void BuildEnvironment()
        {
            var env = new GameObject("Environment").transform;

            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.SetParent(env);
            floor.transform.localScale = new Vector3(2.5f, 1f, 2.5f);
            ApplyMaterial(floor, new Color(0.16f, 0.17f, 0.20f), 0.05f, 0.4f);

            // Floor accent stripes (visible glow lines)
            for (int i = -2; i <= 2; i++)
            {
                if (i == 0) continue;
                var stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stripe.name = $"FloorStripe_{i}";
                stripe.transform.SetParent(env);
                stripe.transform.position   = new Vector3(i * 2.5f, 0.005f, 0f);
                stripe.transform.localScale = new Vector3(0.04f, 0.01f, 24f);
                ApplyEmissive(stripe, ColAccent * 0.6f);
            }

            // Walls
            BuildWall(env, "Wall_N", new Vector3(0f,  2.5f,  9f), new Vector3(20f, 5f, 0.3f), new Color(0.13f, 0.14f, 0.18f));
            BuildWall(env, "Wall_S", new Vector3(0f,  2.5f, -9f), new Vector3(20f, 5f, 0.3f), new Color(0.13f, 0.14f, 0.18f));
            BuildWall(env, "Wall_E", new Vector3( 9f, 2.5f, 0f),  new Vector3(0.3f, 5f, 18f), new Color(0.11f, 0.12f, 0.16f));
            BuildWall(env, "Wall_W", new Vector3(-9f, 2.5f, 0f),  new Vector3(0.3f, 5f, 18f), new Color(0.11f, 0.12f, 0.16f));

            // Ceiling
            var ceil = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceil.name = "Ceiling";
            ceil.transform.SetParent(env);
            ceil.transform.position   = new Vector3(0f, 5f, 0f);
            ceil.transform.localScale = new Vector3(20f, 0.2f, 18f);
            ApplyMaterial(ceil, new Color(0.08f, 0.09f, 0.12f), 0.0f, 0.2f);
        }

        private static void BuildWall(Transform parent, string name, Vector3 pos, Vector3 scale, Color color)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent);
            wall.transform.position   = pos;
            wall.transform.localScale = scale;
            ApplyMaterial(wall, color, 0.0f, 0.3f);
        }

        // ── Desk + monitor + lamp + plant ────────────────────────────────────

        private static void BuildDesk()
        {
            var furniture = new GameObject("Furniture").transform;

            // Desk top
            var top = GameObject.CreatePrimitive(PrimitiveType.Cube);
            top.name = "DeskTop";
            top.transform.SetParent(furniture);
            top.transform.position   = new Vector3(0f, 0.74f, 1.2f);
            top.transform.localScale = new Vector3(2.2f, 0.05f, 0.9f);
            ApplyMaterial(top, new Color(0.32f, 0.22f, 0.16f), 0.02f, 0.35f);

            // Legs
            for (int i = 0; i < 4; i++)
            {
                var leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.name = $"DeskLeg_{i}";
                leg.transform.SetParent(top.transform);
                float x = (i % 2 == 0 ? -0.45f : 0.45f);
                float z = (i < 2 ? -0.45f : 0.45f);
                leg.transform.localPosition = new Vector3(x, -7.5f, z);
                leg.transform.localScale    = new Vector3(0.04f, 15f, 0.04f);
                ApplyMaterial(leg, new Color(0.20f, 0.14f, 0.10f), 0.05f, 0.4f);
            }

            // Monitor on the desk (back wall)
            BuildMonitor(furniture, new Vector3(0.55f, 1.05f, 1.4f));

            // Desk lamp (left side)
            BuildLamp(furniture, new Vector3(-0.7f, 0.78f, 1.4f));

            // Plant (right side)
            BuildPlant(furniture, new Vector3(0.95f, 0.78f, 1.4f));

            // Coffee mug (centre-right)
            BuildMug(furniture, new Vector3(0.3f, 0.78f, 1.0f));
        }

        private static void BuildMonitor(Transform parent, Vector3 basePos)
        {
            var monitor = new GameObject("Monitor").transform;
            monitor.SetParent(parent);
            monitor.position = basePos;

            var stand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stand.name = "Stand";
            stand.transform.SetParent(monitor);
            stand.transform.localPosition = new Vector3(0f, 0f, 0f);
            stand.transform.localScale    = new Vector3(0.06f, 0.18f, 0.04f);
            ApplyMaterial(stand, new Color(0.10f, 0.10f, 0.12f), 0.4f, 0.6f);

            var foot = GameObject.CreatePrimitive(PrimitiveType.Cube);
            foot.name = "Foot";
            foot.transform.SetParent(monitor);
            foot.transform.localPosition = new Vector3(0f, -0.08f, 0f);
            foot.transform.localScale    = new Vector3(0.30f, 0.02f, 0.20f);
            ApplyMaterial(foot, new Color(0.10f, 0.10f, 0.12f), 0.4f, 0.6f);

            var bezel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bezel.name = "Bezel";
            bezel.transform.SetParent(monitor);
            bezel.transform.localPosition = new Vector3(0f, 0.20f, 0f);
            bezel.transform.localScale    = new Vector3(0.7f, 0.42f, 0.04f);
            ApplyMaterial(bezel, new Color(0.06f, 0.06f, 0.08f), 0.5f, 0.7f);

            var screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.name = "Screen";
            screen.transform.SetParent(bezel.transform);
            screen.transform.localPosition = new Vector3(0f, 0f, -0.55f);
            screen.transform.localScale    = new Vector3(0.95f, 0.92f, 0.05f);
            ApplyEmissive(screen, ColAccent * 0.8f);
        }

        private static void BuildLamp(Transform parent, Vector3 basePos)
        {
            var lamp = new GameObject("Lamp").transform;
            lamp.SetParent(parent);
            lamp.position = basePos;

            var baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseObj.name = "Base";
            baseObj.transform.SetParent(lamp);
            baseObj.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            baseObj.transform.localScale    = new Vector3(0.15f, 0.02f, 0.15f);
            ApplyMaterial(baseObj, new Color(0.08f, 0.08f, 0.10f), 0.5f, 0.7f);

            var pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(lamp);
            pole.transform.localPosition = new Vector3(0f, 0.20f, 0f);
            pole.transform.localScale    = new Vector3(0.02f, 0.20f, 0.02f);
            ApplyMaterial(pole, new Color(0.10f, 0.10f, 0.12f), 0.5f, 0.7f);

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(lamp);
            head.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            head.transform.localScale    = Vector3.one * 0.12f;
            ApplyEmissive(head, ColWarning);

            // Light on the lamp head
            var lampLight = new GameObject("LampLight").AddComponent<Light>();
            lampLight.transform.SetParent(lamp);
            lampLight.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            lampLight.type      = LightType.Point;
            lampLight.color     = ColWarning;
            lampLight.intensity = 1.8f;
            lampLight.range     = 2.5f;
            lampLight.gameObject.AddComponent<PulseGlow>();
        }

        private static void BuildPlant(Transform parent, Vector3 basePos)
        {
            var plant = new GameObject("Plant").transform;
            plant.SetParent(parent);
            plant.position = basePos;

            var pot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pot.name = "Pot";
            pot.transform.SetParent(plant);
            pot.transform.localPosition = new Vector3(0f, 0.04f, 0f);
            pot.transform.localScale    = new Vector3(0.12f, 0.05f, 0.12f);
            ApplyMaterial(pot, new Color(0.45f, 0.30f, 0.22f), 0.0f, 0.3f);

            // Leaves (sphere clusters)
            for (int i = 0; i < 5; i++)
            {
                var leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                leaf.name = $"Leaf_{i}";
                leaf.transform.SetParent(plant);
                float angle = i * 72f * Mathf.Deg2Rad;
                leaf.transform.localPosition = new Vector3(
                    Mathf.Cos(angle) * 0.05f,
                    0.10f + i * 0.02f,
                    Mathf.Sin(angle) * 0.05f);
                leaf.transform.localScale = Vector3.one * 0.10f;
                ApplyMaterial(leaf, new Color(0.20f + Random.Range(0f, 0.1f), 0.45f, 0.18f), 0.0f, 0.4f);
            }
        }

        private static void BuildMug(Transform parent, Vector3 basePos)
        {
            var mug = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mug.name = "CoffeeMug";
            mug.transform.SetParent(parent);
            mug.transform.position   = basePos;
            mug.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            ApplyMaterial(mug, new Color(0.85f, 0.85f, 0.88f), 0.0f, 0.5f);
        }

        // ── Posters on walls ─────────────────────────────────────────────────

        private static void BuildPosters()
        {
            var posters = new GameObject("Posters").transform;

            // Cybersecurity-themed posters using tinted Quads + text
            CreatePoster(posters, new Vector3(-3.5f, 2.2f, 8.85f), "STAY ALERT", ColAccent);
            CreatePoster(posters, new Vector3( 3.5f, 2.2f, 8.85f), "THINK BEFORE YOU CLICK", ColSuccess);
            CreatePoster(posters, new Vector3(-8.85f, 2.2f, 3f), "VERIFY THE SOURCE", ColWarning, rotY: 90f);
            CreatePoster(posters, new Vector3( 8.85f, 2.2f, -3f), "REPORT • DON'T RESPOND", ColDanger, rotY: -90f);
        }

        private static void CreatePoster(Transform parent, Vector3 pos, string text, Color tint, float rotY = 0f)
        {
            var posterRoot = new GameObject($"Poster_{text}");
            posterRoot.transform.SetParent(parent);
            posterRoot.transform.position = pos;
            posterRoot.transform.rotation = Quaternion.Euler(0f, rotY, 0f);

            // Frame
            var frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frame.name = "Frame";
            frame.transform.SetParent(posterRoot.transform);
            frame.transform.localPosition = Vector3.zero;
            frame.transform.localScale    = new Vector3(1.6f, 1.0f, 0.05f);
            ApplyMaterial(frame, new Color(0.15f, 0.15f, 0.18f), 0.5f, 0.7f);

            // Inner
            var inner = GameObject.CreatePrimitive(PrimitiveType.Cube);
            inner.name = "Inner";
            inner.transform.SetParent(posterRoot.transform);
            inner.transform.localPosition = new Vector3(0f, 0f, -0.03f);
            inner.transform.localScale    = new Vector3(1.5f, 0.9f, 0.02f);
            ApplyEmissive(inner, tint * 0.4f);

            // Floating text via world-space canvas
            var canvasGO = new GameObject("PosterText");
            canvasGO.transform.SetParent(posterRoot.transform);
            canvasGO.transform.localPosition = new Vector3(0f, 0f, -0.05f);
            canvasGO.transform.localRotation = Quaternion.identity;
            canvasGO.transform.localScale    = Vector3.one * 0.005f;

            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var rt = canvasGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(280f, 160f);
            canvasGO.AddComponent<CanvasScaler>();

            var tmpGO = new GameObject("Label");
            tmpGO.transform.SetParent(canvasGO.transform, false);
            var tmp = tmpGO.AddComponent<TextMeshProUGUI>();
            tmp.text       = text;
            tmp.fontSize   = 36;
            tmp.color      = Color.white;
            tmp.alignment  = TextAlignmentOptions.Center;
            tmp.fontStyle  = FontStyles.Bold;
            tmp.enableWordWrapping = true;
            var tmpRT = tmp.GetComponent<RectTransform>();
            tmpRT.anchorMin = Vector2.zero;
            tmpRT.anchorMax = Vector2.one;
            tmpRT.offsetMin = Vector2.zero;
            tmpRT.offsetMax = Vector2.zero;
        }

        // ── Accent lights ────────────────────────────────────────────────────

        private static void BuildAccentLights()
        {
            var lights = new GameObject("AccentLights").transform;

            // Blue rim from behind camera
            var rim = new GameObject("RimBlue").AddComponent<Light>();
            rim.transform.SetParent(lights);
            rim.transform.position = new Vector3(0f, 3.5f, -3f);
            rim.type      = LightType.Point;
            rim.color     = ColAccent;
            rim.intensity = 2.2f;
            rim.range     = 12f;

            // Purple key from above-right
            var key = new GameObject("KeyPurple").AddComponent<Light>();
            key.transform.SetParent(lights);
            key.transform.position = new Vector3(3f, 4f, 0f);
            key.type      = LightType.Point;
            key.color     = ColAccent2;
            key.intensity = 1.8f;
            key.range     = 10f;
            key.gameObject.AddComponent<PulseGlow>();
        }

        // ── Ambient particle dust ────────────────────────────────────────────

        private static void BuildAmbientParticles()
        {
            var go = new GameObject("DustParticles");
            go.transform.position = new Vector3(0f, 2.5f, 1f);

            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 12f;
            main.startSpeed    = 0.05f;
            main.startSize     = 0.015f;
            main.startColor    = new Color(1f, 1f, 1f, 0.35f);
            main.maxParticles  = 200;
            main.gravityModifier = -0.02f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 8f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale     = new Vector3(10f, 4f, 10f);

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        // ── Intro Canvas ─────────────────────────────────────────────────────

        private static IntroScreenController BuildIntroCanvas(Transform parent)
        {
            var go = CreateWorldCanvas("IntroCanvas", parent,
                new Vector3(0f, 1.55f, 1.2f), new Vector2(900f, 620f), 0.0014f);

            var cg = go.AddComponent<CanvasGroup>();

            var bg = CreateImage("Backdrop", go.transform, Color.white);
            StretchFill(bg);
            AddGradient(bg, ColPanelTop, ColPanelBot);

            // Top accent stripe
            var stripe = CreateImage("AccentStripe", go.transform, ColAccent);
            var sRT = stripe.rectTransform;
            sRT.anchorMin = new Vector2(0f, 1f); sRT.anchorMax = new Vector2(1f, 1f);
            sRT.pivot = new Vector2(0.5f, 1f);
            sRT.sizeDelta = new Vector2(0f, 6f);

            // Brand
            var brand = CreateText("Brand", go.transform, "CyberSafeVR",
                18, ColAccent, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(brand.rectTransform, 30f, 30f);

            var title = CreateText("Title", go.transform, "SMISHING AWARENESS",
                64, ColText, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(title.rectTransform, 75f, 80f);

            var subtitle = CreateText("Subtitle", go.transform,
                "Spot the scam before it spots you.", 24,
                ColMuted, TextAlignmentOptions.Center, FontStyles.Italic);
            AnchorTop(subtitle.rectTransform, 165f, 35f);

            var divider = CreateImage("Divider", go.transform, ColAccent);
            var dRT = divider.rectTransform;
            dRT.anchorMin = new Vector2(0.5f, 1f); dRT.anchorMax = new Vector2(0.5f, 1f);
            dRT.pivot = new Vector2(0.5f, 1f);
            dRT.sizeDelta = new Vector2(120f, 2f);
            dRT.anchoredPosition = new Vector2(0f, -210f);

            var briefing = CreateText("Briefing", go.transform,
                "You'll receive <b>6 text messages</b> on your phone.\n\n" +
                "For each, choose how to respond:\n\n" +
                "  <color=#5EE8A6><b>⚑ Report</b></color>   — if it looks like a scam\n" +
                "  <color=#C9CCD4><b>✕ Ignore</b></color>   — if it's safe and needs no action\n" +
                "  <color=#FFB84D><b>↗ Open</b></color>   — if you trust the URL\n\n" +
                "Use <color=#FFB84D><b>?</b></color> for a hint — but only once per message.",
                22, ColText, TextAlignmentOptions.Center);
            AnchorCentre(briefing.rectTransform, new Vector2(0f, -10f), new Vector2(720f, 280f));
            briefing.enableWordWrapping = true;

            var startBtn = CreateButton("StartButton", go.transform, "BEGIN  →",
                ColAccent, new Vector2(300f, 70f));
            AnchorBottom(startBtn.GetComponent<RectTransform>(), 50f);

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
                new Vector3(0f, 1.45f, 1.0f), new Vector2(450f, 820f), 0.0014f);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            // Note: PhoneIdleAnimation removed — was moving phone out of camera frame

            var cg = go.AddComponent<CanvasGroup>();

            // Phone outer frame (dark grey)
            var phoneBody = CreateImage("PhoneBody", go.transform, new Color(0.06f, 0.06f, 0.08f));
            StretchFill(phoneBody, inset: -16f);

            // Inner highlight bezel
            var bezel = CreateImage("Bezel", phoneBody.transform, new Color(0.12f, 0.13f, 0.16f));
            StretchFill(bezel, inset: 6f);

            // Screen (gradient bg)
            var screen = CreateImage("Screen", bezel.transform, Color.white);
            StretchFill(screen, inset: 6f);
            AddGradient(screen, ColBgTop, ColBgBot);

            // Top notch
            var notch = CreateImage("Notch", screen.transform, new Color(0.04f, 0.04f, 0.06f));
            var nRT = notch.rectTransform;
            nRT.anchorMin = new Vector2(0.5f, 1f); nRT.anchorMax = new Vector2(0.5f, 1f);
            nRT.pivot = new Vector2(0.5f, 1f);
            nRT.sizeDelta = new Vector2(150f, 28f);
            nRT.anchoredPosition = new Vector2(0f, -10f);

            // Status bar
            var time = CreateText("Time", screen.transform, "09:41",
                18, ColText, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            var tRT = time.rectTransform;
            tRT.anchorMin = new Vector2(0f, 1f); tRT.anchorMax = new Vector2(0f, 1f);
            tRT.pivot = new Vector2(0f, 1f);
            tRT.anchoredPosition = new Vector2(20f, -15f);
            tRT.sizeDelta = new Vector2(80f, 30f);

            var signal = CreateText("Signal", screen.transform, "▂▄▆█  5G  100%",
                14, ColText, TextAlignmentOptions.MidlineRight);
            var sigRT = signal.rectTransform;
            sigRT.anchorMin = new Vector2(1f, 1f); sigRT.anchorMax = new Vector2(1f, 1f);
            sigRT.pivot = new Vector2(1f, 1f);
            sigRT.anchoredPosition = new Vector2(-20f, -15f);
            sigRT.sizeDelta = new Vector2(180f, 30f);

            // Sender row card
            var senderRow = CreateImage("SenderRow", screen.transform, Color.white);
            AddGradient(senderRow, ColPanelTop, ColPanelBot);
            AnchorTop(senderRow.rectTransform, 60f, 90f);
            var srRT = senderRow.rectTransform;
            srRT.anchorMin = new Vector2(0f, 1f); srRT.anchorMax = new Vector2(1f, 1f);
            srRT.offsetMin = new Vector2(15f, 0f); srRT.offsetMax = new Vector2(-15f, 0f);
            srRT.sizeDelta = new Vector2(-30f, 90f);
            srRT.anchoredPosition = new Vector2(0f, -60f);

            // Sender icon (circle made from a square w/ tint)
            var senderIcon = CreateImage("SenderIcon", senderRow.transform, ColAccent);
            var iconRT = senderIcon.rectTransform;
            iconRT.anchorMin = iconRT.anchorMax = new Vector2(0f, 0.5f);
            iconRT.pivot = new Vector2(0f, 0.5f);
            iconRT.anchoredPosition = new Vector2(15f, 0f);
            iconRT.sizeDelta = new Vector2(56f, 56f);

            var sender = CreateText("SenderName", senderRow.transform, "Sender",
                22, ColText, TextAlignmentOptions.MidlineLeft, FontStyles.Bold);
            var sendRT = sender.rectTransform;
            sendRT.anchorMin = new Vector2(0f, 0f); sendRT.anchorMax = new Vector2(1f, 1f);
            sendRT.offsetMin = new Vector2(85f, 5f); sendRT.offsetMax = new Vector2(-15f, -5f);

            // Message body card
            var msgBg = CreateImage("MessageBg", screen.transform, Color.white);
            AddGradient(msgBg, ColPanelTop, ColPanelBot);
            var msgRT = msgBg.rectTransform;
            msgRT.anchorMin = new Vector2(0f, 1f); msgRT.anchorMax = new Vector2(1f, 1f);
            msgRT.offsetMin = new Vector2(15f, 0f); msgRT.offsetMax = new Vector2(-15f, 0f);
            msgRT.sizeDelta = new Vector2(-30f, 360f);
            msgRT.anchoredPosition = new Vector2(0f, -160f);

            var msgBody = CreateText("MessageBody", msgBg.transform, "Message body…",
                20, ColText, TextAlignmentOptions.TopLeft);
            StretchFill(msgBody, inset: 22f);
            msgBody.enableWordWrapping = true;

            // URL card (highlighted)
            var urlBg = CreateImage("UrlBg", screen.transform, new Color(0.10f, 0.16f, 0.26f));
            var urlRT = urlBg.rectTransform;
            urlRT.anchorMin = new Vector2(0f, 1f); urlRT.anchorMax = new Vector2(1f, 1f);
            urlRT.offsetMin = new Vector2(15f, 0f); urlRT.offsetMax = new Vector2(-15f, 0f);
            urlRT.sizeDelta = new Vector2(-30f, 65f);
            urlRT.anchoredPosition = new Vector2(0f, -535f);

            var urlText = CreateText("UrlText", urlBg.transform, "",
                17, ColAccent, TextAlignmentOptions.MidlineLeft, FontStyles.Underline);
            StretchFill(urlText, inset: 15f);

            // Hint label
            var hintText = CreateText("HintText", screen.transform, "",
                15, ColWarning, TextAlignmentOptions.TopLeft, FontStyles.Italic);
            var hintRT = hintText.rectTransform;
            hintRT.anchorMin = new Vector2(0f, 1f); hintRT.anchorMax = new Vector2(1f, 1f);
            hintRT.offsetMin = new Vector2(20f, 0f); hintRT.offsetMax = new Vector2(-20f, 0f);
            hintRT.sizeDelta = new Vector2(-40f, 80f);
            hintRT.anchoredPosition = new Vector2(0f, -610f);
            hintText.enableWordWrapping = true;
            hintText.gameObject.SetActive(false);

            // Hint button
            var hintBtn = CreateButton("HintButton", screen.transform, "?",
                new Color(0.45f, 0.35f, 0.10f), new Vector2(50f, 50f));
            var hbRT = hintBtn.GetComponent<RectTransform>();
            hbRT.anchorMin = new Vector2(1f, 1f); hbRT.anchorMax = new Vector2(1f, 1f);
            hbRT.pivot = new Vector2(1f, 1f);
            hbRT.anchoredPosition = new Vector2(-20f, -60f);

            // Action buttons row
            var btnRow = new GameObject("ButtonRow");
            btnRow.transform.SetParent(screen.transform, false);
            var brRT = btnRow.AddComponent<RectTransform>();
            brRT.anchorMin = new Vector2(0f, 0f); brRT.anchorMax = new Vector2(1f, 0f);
            brRT.pivot = new Vector2(0.5f, 0f);
            brRT.sizeDelta = new Vector2(-30f, 95f);
            brRT.anchoredPosition = new Vector2(0f, 18f);

            var hlg = btnRow.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;

            var reportBtn = CreateButton("ReportButton",   btnRow.transform, "⚑\nReport", ColDanger,                     new Vector2(125f, 85f));
            var ignoreBtn = CreateButton("IgnoreButton",   btnRow.transform, "✕\nIgnore", new Color(0.4f, 0.4f, 0.45f),  new Vector2(125f, 85f));
            var clickBtn  = CreateButton("ClickLinkButton",btnRow.transform, "↗\nOpen",   ColAccent,                     new Vector2(125f, 85f));

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
                new Vector3(0f, 1.55f, 1.2f), new Vector2(560f, 320f), 0.0015f);

            var cg = go.AddComponent<CanvasGroup>();

            var bg = CreateImage("Bg", go.transform, Color.white);
            StretchFill(bg);
            AddGradient(bg, new Color(0.20f, 0.10f, 0.06f, 0.98f), new Color(0.10f, 0.05f, 0.04f, 0.98f));

            var border = CreateImage("BorderTop", go.transform, ColWarning);
            var bRT = border.rectTransform;
            bRT.anchorMin = new Vector2(0f, 1f); bRT.anchorMax = new Vector2(1f, 1f);
            bRT.pivot = new Vector2(0.5f, 1f);
            bRT.sizeDelta = new Vector2(0f, 5f);

            var title = CreateText("Title", go.transform, "⚠  Are you sure?",
                32, ColWarning, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(title.rectTransform, 35f, 50f);

            var body = CreateText("Body", go.transform,
                "Clicking unknown links can lead to credential theft, malware, or financial loss.",
                18, ColText, TextAlignmentOptions.Center);
            AnchorCentre(body.rectTransform, new Vector2(0f, 10f), new Vector2(480f, 100f));
            body.enableWordWrapping = true;

            var row = new GameObject("ButtonRow");
            row.transform.SetParent(go.transform, false);
            var rowRT = row.AddComponent<RectTransform>();
            rowRT.anchorMin = new Vector2(0.5f, 0f); rowRT.anchorMax = new Vector2(0.5f, 0f);
            rowRT.pivot = new Vector2(0.5f, 0f);
            rowRT.sizeDelta = new Vector2(440f, 65f);
            rowRT.anchoredPosition = new Vector2(0f, 30f);

            var hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;

            var cancelBtn  = CreateButton("CancelButton",  row.transform, "Cancel",      new Color(0.4f, 0.4f, 0.45f), new Vector2(170f, 60f));
            var confirmBtn = CreateButton("ConfirmButton", row.transform, "Open anyway", ColDanger,                    new Vector2(220f, 60f));

            var cd = go.AddComponent<ConfirmDialogController>();
            cd.canvasGroup   = cg;
            cd.titleText     = title;
            cd.bodyText      = body;
            cd.confirmButton = confirmBtn;
            cd.cancelButton  = cancelBtn;
            return cd;
        }

        // ── HUD ──────────────────────────────────────────────────────────────

        private static HUDController BuildHUDCanvas(Transform parent)
        {
            var go = CreateWorldCanvas("HUDCanvas", parent,
                new Vector3(0f, 2.55f, 1.8f), new Vector2(1000f, 280f), 0.0013f);

            var bg = CreateImage("Bg", go.transform, Color.white);
            StretchFill(bg);
            AddGradient(bg, new Color(ColPanelTop.r, ColPanelTop.g, ColPanelTop.b, 0.92f),
                            new Color(ColPanelBot.r, ColPanelBot.g, ColPanelBot.b, 0.92f));

            var stripe = CreateImage("AccentStripe", go.transform, ColAccent);
            var stRT = stripe.rectTransform;
            stRT.anchorMin = new Vector2(0f, 1f); stRT.anchorMax = new Vector2(1f, 1f);
            stRT.pivot = new Vector2(0.5f, 1f);
            stRT.sizeDelta = new Vector2(0f, 4f);

            var title = CreateText("Title", go.transform, "Scenario Title",
                32, ColAccent, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(title.rectTransform, 25f, 45f);

            var narration = CreateText("Narration", go.transform, "Narration goes here.",
                20, ColText, TextAlignmentOptions.Center);
            AnchorCentre(narration.rectTransform, new Vector2(0f, 15f), new Vector2(880f, 80f));
            narration.enableWordWrapping = true;

            var step = CreateText("StepLabel", go.transform, "Step 1 of 6",
                16, ColMuted, TextAlignmentOptions.MidlineLeft);
            var stpRT = step.rectTransform;
            stpRT.anchorMin = new Vector2(0f, 0f); stpRT.anchorMax = new Vector2(0f, 0f);
            stpRT.pivot = new Vector2(0f, 0f);
            stpRT.anchoredPosition = new Vector2(30f, 25f);
            stpRT.sizeDelta = new Vector2(220f, 30f);

            var score = CreateText("Score", go.transform, "Score: 0 / 6",
                16, ColMuted, TextAlignmentOptions.MidlineRight);
            var scRT = score.rectTransform;
            scRT.anchorMin = new Vector2(1f, 0f); scRT.anchorMax = new Vector2(1f, 0f);
            scRT.pivot = new Vector2(1f, 0f);
            scRT.anchoredPosition = new Vector2(-30f, 25f);
            scRT.sizeDelta = new Vector2(220f, 30f);

            var dotsRowGO = new GameObject("Dots");
            dotsRowGO.transform.SetParent(go.transform, false);
            var dotsRT = dotsRowGO.AddComponent<RectTransform>();
            dotsRT.anchorMin = new Vector2(0.5f, 0f); dotsRT.anchorMax = new Vector2(0.5f, 0f);
            dotsRT.pivot = new Vector2(0.5f, 0f);
            dotsRT.anchoredPosition = new Vector2(0f, 28f);
            dotsRT.sizeDelta = new Vector2(450f, 30f);

            var dotsLayout = dotsRowGO.AddComponent<HorizontalLayoutGroup>();
            dotsLayout.spacing = 14;
            dotsLayout.childAlignment = TextAnchor.MiddleCenter;
            dotsLayout.childControlWidth = false;
            dotsLayout.childControlHeight = false;

            var dot = CreateImage("DotTemplate", dotsRowGO.transform, new Color(0.25f, 0.25f, 0.28f));
            dot.rectTransform.sizeDelta = new Vector2(22f, 22f);
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

        // ── Outcome panels ───────────────────────────────────────────────────

        private static ScenarioOutcomeHandler BuildOutcomePanels(Transform parent)
        {
            // Correct
            var correctGO = CreateWorldCanvas("CorrectPanel", parent,
                new Vector3(0f, 1.55f, 1.3f), new Vector2(740f, 540f), 0.0015f);
            var correctCG = correctGO.AddComponent<CanvasGroup>();

            var cBg = CreateImage("Bg", correctGO.transform, Color.white);
            StretchFill(cBg);
            AddGradient(cBg, new Color(0.06f, 0.18f, 0.10f, 0.97f), new Color(0.04f, 0.12f, 0.07f, 0.97f));

            var cBorder = CreateImage("BorderTop", correctGO.transform, ColSuccess);
            var cbRT = cBorder.rectTransform;
            cbRT.anchorMin = new Vector2(0f, 1f); cbRT.anchorMax = new Vector2(1f, 1f);
            cbRT.pivot = new Vector2(0.5f, 1f);
            cbRT.sizeDelta = new Vector2(0f, 8f);

            BuildCheckIcon(correctGO.transform, new Vector2(0f, 140f), ColSuccess);

            var cTitle = CreateText("Title", correctGO.transform, "CORRECT",
                52, ColSuccess, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(cTitle.rectTransform, 220f, 70f);

            var cBody = CreateText("Body", correctGO.transform, "",
                22, ColText, TextAlignmentOptions.Center);
            AnchorCentre(cBody.rectTransform, new Vector2(0f, -25f), new Vector2(660f, 200f));
            cBody.enableWordWrapping = true;

            var cContinue = CreateButton("Continue", correctGO.transform, "Continue  →",
                ColSuccess, new Vector2(260f, 65f));
            AnchorBottom(cContinue.GetComponent<RectTransform>(), 40f);
            correctGO.SetActive(false);

            // Incorrect
            var wrongGO = CreateWorldCanvas("IncorrectPanel", parent,
                new Vector3(0f, 1.55f, 1.3f), new Vector2(740f, 640f), 0.0015f);
            var wrongCG = wrongGO.AddComponent<CanvasGroup>();

            var wBg = CreateImage("Bg", wrongGO.transform, Color.white);
            StretchFill(wBg);
            AddGradient(wBg, new Color(0.20f, 0.06f, 0.06f, 0.97f), new Color(0.12f, 0.04f, 0.04f, 0.97f));

            var wBorder = CreateImage("BorderTop", wrongGO.transform, ColDanger);
            var wbRT = wBorder.rectTransform;
            wbRT.anchorMin = new Vector2(0f, 1f); wbRT.anchorMax = new Vector2(1f, 1f);
            wbRT.pivot = new Vector2(0.5f, 1f);
            wbRT.sizeDelta = new Vector2(0f, 8f);

            BuildCrossIcon(wrongGO.transform, new Vector2(0f, 180f), ColDanger);

            var wTitle = CreateText("Title", wrongGO.transform, "INCORRECT",
                52, ColDanger, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(wTitle.rectTransform, 260f, 70f);

            var wBody = CreateText("Body", wrongGO.transform, "",
                20, ColText, TextAlignmentOptions.Center);
            AnchorCentre(wBody.rectTransform, new Vector2(0f, 25f), new Vector2(660f, 140f));
            wBody.enableWordWrapping = true;

            var redFlags = CreateText("RedFlags", wrongGO.transform, "",
                18, ColWarning, TextAlignmentOptions.TopLeft);
            AnchorCentre(redFlags.rectTransform, new Vector2(0f, -130f), new Vector2(620f, 160f));
            redFlags.enableWordWrapping = true;

            var wContinue = CreateButton("Continue", wrongGO.transform, "Continue  →",
                ColDanger, new Vector2(260f, 65f));
            AnchorBottom(wContinue.GetComponent<RectTransform>(), 40f);
            wrongGO.SetActive(false);

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

        // ── Icons ────────────────────────────────────────────────────────────

        private static void BuildCheckIcon(Transform parent, Vector2 anchorPos, Color color)
        {
            var c = new GameObject("CheckIcon");
            c.transform.SetParent(parent, false);
            var crt = c.AddComponent<RectTransform>();
            crt.anchorMin = new Vector2(0.5f, 1f); crt.anchorMax = new Vector2(0.5f, 1f);
            crt.pivot = new Vector2(0.5f, 1f);
            crt.anchoredPosition = anchorPos;
            crt.sizeDelta = new Vector2(140f, 140f);

            // Soft glow background circle
            var glow = CreateImage("Glow", c.transform, new Color(color.r, color.g, color.b, 0.25f));
            glow.rectTransform.sizeDelta = new Vector2(140f, 140f);

            var s = CreateImage("Short", c.transform, color);
            var sRT = s.rectTransform;
            sRT.sizeDelta = new Vector2(16f, 70f);
            sRT.anchoredPosition = new Vector2(-32f, -8f);
            sRT.rotation = Quaternion.Euler(0, 0, 45);

            var l = CreateImage("Long", c.transform, color);
            var lRT = l.rectTransform;
            lRT.sizeDelta = new Vector2(16f, 115f);
            lRT.anchoredPosition = new Vector2(18f, 8f);
            lRT.rotation = Quaternion.Euler(0, 0, -45);
        }

        private static void BuildCrossIcon(Transform parent, Vector2 anchorPos, Color color)
        {
            var c = new GameObject("CrossIcon");
            c.transform.SetParent(parent, false);
            var crt = c.AddComponent<RectTransform>();
            crt.anchorMin = new Vector2(0.5f, 1f); crt.anchorMax = new Vector2(0.5f, 1f);
            crt.pivot = new Vector2(0.5f, 1f);
            crt.anchoredPosition = anchorPos;
            crt.sizeDelta = new Vector2(140f, 140f);

            var glow = CreateImage("Glow", c.transform, new Color(color.r, color.g, color.b, 0.25f));
            glow.rectTransform.sizeDelta = new Vector2(140f, 140f);

            var b1 = CreateImage("Bar1", c.transform, color);
            b1.rectTransform.sizeDelta = new Vector2(16f, 130f);
            b1.rectTransform.rotation = Quaternion.Euler(0, 0, 45);

            var b2 = CreateImage("Bar2", c.transform, color);
            b2.rectTransform.sizeDelta = new Vector2(16f, 130f);
            b2.rectTransform.rotation = Quaternion.Euler(0, 0, -45);
        }

        // ════════════════════════════════════════════════════════════════════
        //  DEBRIEF SCENE
        // ════════════════════════════════════════════════════════════════════

        private static void BuildDebriefScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            ConfigureLighting();
            RenderSettings.ambientSkyColor = new Color(0.20f, 0.18f, 0.30f);

            var light = new GameObject("Sun").AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.9f;
            light.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            var cam = new GameObject("Main Camera").AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.transform.position = new Vector3(0f, 1.6f, 0f);
            cam.transform.LookAt(new Vector3(0f, 1.6f, 1f));
            cam.nearClipPlane = 0.05f;
            cam.backgroundColor = new Color(0.04f, 0.05f, 0.10f);
            cam.gameObject.AddComponent<AudioListener>();

            // Backdrop floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(2f, 1f, 2f);
            ApplyMaterial(floor, new Color(0.10f, 0.10f, 0.14f), 0.0f, 0.3f);

            // Accent rim light
            var rim = new GameObject("Rim").AddComponent<Light>();
            rim.transform.position = new Vector3(0f, 3f, 3f);
            rim.type = LightType.Point;
            rim.color = ColAccent;
            rim.intensity = 2.5f;
            rim.range = 12f;
            rim.gameObject.AddComponent<PulseGlow>();

            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();

            var go = CreateWorldCanvas("DebriefCanvas", null,
                new Vector3(0f, 1.6f, 1.5f), new Vector2(820f, 700f), 0.0014f);

            var bg = CreateImage("Bg", go.transform, Color.white);
            StretchFill(bg);
            AddGradient(bg, ColPanelTop, ColPanelBot);

            var stripe = CreateImage("AccentStripe", go.transform, ColAccent);
            var stRT = stripe.rectTransform;
            stRT.anchorMin = new Vector2(0f, 1f); stRT.anchorMax = new Vector2(1f, 1f);
            stRT.pivot = new Vector2(0.5f, 1f);
            stRT.sizeDelta = new Vector2(0f, 6f);

            var brand = CreateText("Brand", go.transform, "CyberSafeVR",
                16, ColAccent, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(brand.rectTransform, 25f, 25f);

            var title = CreateText("Title", go.transform, "ACTIVITY COMPLETE",
                52, ColText, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(title.rectTransform, 60f, 70f);

            var divider = CreateImage("Divider", go.transform, ColAccent);
            var dRT = divider.rectTransform;
            dRT.anchorMin = new Vector2(0.5f, 1f); dRT.anchorMax = new Vector2(0.5f, 1f);
            dRT.pivot = new Vector2(0.5f, 1f);
            dRT.sizeDelta = new Vector2(120f, 2f);
            dRT.anchoredPosition = new Vector2(0f, -150f);

            var scoreLabel = CreateText("ScoreLabel", go.transform, "Your Score",
                22, ColMuted, TextAlignmentOptions.Center);
            AnchorTop(scoreLabel.rectTransform, 170f, 35f);

            var score = CreateText("Score", go.transform, "— / —",
                72, ColText, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(score.rectTransform, 210f, 100f);

            var grade = CreateText("Grade", go.transform, "—",
                32, ColSuccess, TextAlignmentOptions.Center, FontStyles.Bold);
            AnchorTop(grade.rectTransform, 320f, 50f);

            var hints = CreateText("Hints", go.transform, "",
                18, ColMuted, TextAlignmentOptions.Center, FontStyles.Italic);
            AnchorTop(hints.rectTransform, 380f, 30f);

            var takeaways = CreateText("Takeaways", go.transform,
                "<b>Key takeaways</b>\n" +
                "• Never click links in unexpected SMS messages\n" +
                "• Check domains for misspellings or wrong TLDs\n" +
                "• Urgency language is a manipulation tactic\n" +
                "• Legitimate organisations never request credentials via SMS",
                18, ColText, TextAlignmentOptions.TopLeft);
            AnchorCentre(takeaways.rectTransform, new Vector2(0f, -90f), new Vector2(700f, 160f));
            takeaways.enableWordWrapping = true;

            var row = new GameObject("Buttons");
            row.transform.SetParent(go.transform, false);
            var rRT = row.AddComponent<RectTransform>();
            rRT.anchorMin = new Vector2(0.5f, 0f); rRT.anchorMax = new Vector2(0.5f, 0f);
            rRT.pivot = new Vector2(0.5f, 0f);
            rRT.anchoredPosition = new Vector2(0f, 40f);
            rRT.sizeDelta = new Vector2(720f, 70f);

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
        //  HELPERS
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
            go.GetComponent<RectTransform>().sizeDelta = size;
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

        private static void AddGradient(Image img, Color top, Color bottom)
        {
            var grad = img.gameObject.AddComponent<UIGradient>();
            grad.topColor    = top;
            grad.bottomColor = bottom;
        }

        private static TMP_Text CreateText(string name, Transform parent, string text,
            float fontSize, Color color, TextAlignmentOptions alignment, FontStyles style = FontStyles.Normal)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.fontStyle = style;
            tmp.enableWordWrapping = false;
            tmp.richText = true;
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
            cb.highlightedColor = Color.Lerp(bgColor, Color.white, 0.25f);
            cb.pressedColor     = Color.Lerp(bgColor, Color.black, 0.30f);
            cb.selectedColor    = bgColor;
            cb.disabledColor    = new Color(bgColor.r * 0.4f, bgColor.g * 0.4f, bgColor.b * 0.4f, 0.55f);
            btn.colors = cb;

            // Soft gradient on the button itself
            AddGradient(img, Color.Lerp(bgColor, Color.white, 0.10f),
                              Color.Lerp(bgColor, Color.black, 0.15f));

            go.AddComponent<ButtonHoverEffect>();

            var labelText = CreateText("Label", go.transform, label, 22,
                ColText, TextAlignmentOptions.Center, FontStyles.Bold);
            StretchFill(labelText, inset: 4f);
            return btn;
        }

        private static void ApplyMaterial(GameObject go, Color color, float metallic, float smoothness)
        {
            var rend = go.GetComponent<Renderer>();
            if (rend == null) return;

            // Try shaders in order of preference
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("HDRP/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Legacy Shaders/Diffuse");
            if (shader == null) { Debug.LogWarning("[SmishingSceneBuilder] No suitable shader found."); return; }

            var mat = new Material(shader);
            mat.name = $"Mat_{go.name}";

            // Set colour properties for whichever shader we got
            if (mat.HasProperty("_BaseColor"))      mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color"))          mat.SetColor("_Color",     color);
            mat.color = color;

            if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   metallic);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);

            rend.sharedMaterial = mat;
        }

        private static void ApplyEmissive(GameObject go, Color emission)
        {
            var rend = go.GetComponent<Renderer>();
            if (rend == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader);
            mat.color = emission * 0.5f;
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emission * 2f);
            }
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", emission * 0.5f);
            rend.sharedMaterial = mat;
        }

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
