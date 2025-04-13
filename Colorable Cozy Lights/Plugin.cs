using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Colorable_Cozy_Lights
{
    [BepInPlugin("com.atomic.colorcozylights", "Color Cozy Lights", "1.9.0")]
    [BepInDependency("ainavt.lc.lethalconfig")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ConfigEntry<int> colorConfigR;
        internal static ConfigEntry<int> colorConfigG;
        internal static ConfigEntry<int> colorConfigB;
        internal static ConfigEntry<bool> Rainbow;
        internal static ConfigEntry<float> RainbowSpeed;
        internal static ConfigEntry<float> Brightness;
        internal static ConfigEntry<bool> AnimationActive;
        internal static ConfigEntry<string> AnimationValue;

        internal static RuntimeAnimatorController lightsAnimator;

        internal static new ManualLogSource Logger;

        private Harmony harmony;

        void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("Color Cozy Lights has loaded! Checking and setting configs.");

            colorConfigR = Config.Bind("Cozy Light Color", "Cozy Lights Color - R", 53, "RGB value (0-255) for Red. YOU DONT NEED TO RESET THE GAME");
            colorConfigG = Config.Bind("Cozy Light Color", "Cozy Lights Color - G", 135, "RGB value (0-255) for Green. YOU DONT NEED TO RESET THE GAME");
            colorConfigB = Config.Bind("Cozy Light Color", "Cozy Lights Color - B", 255, "RGB value (0-255) for Blue. YOU DONT NEED TO RESET THE GAME");
            Brightness = Config.Bind("Cozy Light Color", "Cozy Lights Brightness", 138f, "Decimals allowed. Brightness off the light. Please don't surpass 1000, its too bright. YOU DONT NEED TO RESET THE GAME");
            Rainbow = Config.Bind("Cozy Light Color", "Rainbow", false, "Cycles through colors!. YOU DONT NEED TO RESET THE GAME");
            RainbowSpeed = Config.Bind("Cozy Light Color", "Rainbow Speed", 0.05f, "Sets the speed of the rainbow, I am not responsible for any seizures people have for high amounts. YOU DONT NEED TO RESET THE GAME");
            AnimationActive = Config.Bind("Animations", "Animation Enabled", false, "Make light shows! Go to atomictyler.dev/tools/lights for easy creation!");
            AnimationValue = Config.Bind("Animations", "Animations", "{'255,255,255,1','5000'},{'0,255,144,0.5','2500'}", "atomictyler.dev/tools/lights recommended. It works by having each animation in {} and seperated with commas. you need RGBA and how long it will stay on that anim in ms.");

            harmony = new Harmony("com.atomic.colorcozylights.patch");
            harmony.PatchAll();

            try
            {
                AssetBundle lightbundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lightanimator"));
                lightsAnimator = lightbundle.LoadAsset("ShipCozyLightsClone", typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
            }
            catch
            {
                Logger.LogError("Encountered some error loading asset bundle. Did you install the plugin correctly?");
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "SampleSceneRelay")
            {
                GameObject environment = GameObject.Find("Environment");
                environment.AddComponent<CozyLightsManager>();
            }
        }

        [HarmonyPatch(typeof(CozyLights), "Update")]
        public class CozyLightsPatch
        {
            static bool lastAnimatorState = false;    

            static void Postfix(CozyLights __instance)
            {
                __instance.GetComponent<Animator>().runtimeAnimatorController = lightsAnimator;
                var manager = FindObjectOfType<CozyLightsManager>();
                if (__instance.cozyLightsAnimator.GetBool("on") && !lastAnimatorState)
                {
                    if (manager != null)
                    {
                        manager.Initialize();
                    }
                }
                lastAnimatorState = __instance.cozyLightsAnimator.GetBool("on");
            }
        }
    }

    public class CozyLightsManager : MonoBehaviour
    {
        private float normalizedR;
        private float normalizedG;
        private float normalizedB;
        private float hue = 0f;
        private float rainbowSpeed = Plugin.RainbowSpeed.Value;
        private float brightness = 0f;

        private List<(Color color, float duration)> animationSteps;
        private int currentStepIndex = 0;
        private float animationTimer = 0f;
        private bool animationInitialized = false;

        internal bool LightStatus = false;
        public bool MaterialMade = false;

        public Material cozyLightsNewMat;
        public Material cozyLightsOldMat;

        public GameObject cozyLights;
        public RuntimeAnimatorController lightsAnimator = Plugin.lightsAnimator;

        private List<(Color color, float duration)> ParseAnimationConfig(string configString)
        {
            var animationSteps = new List<(Color color, float duration)>();

            string[] entries = configString
                .Trim('{', '}')
                .Split(new[] { "},{" }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string entry in entries)
            {
                string cleanEntry = entry.Trim('{', '}');
                string[] parts = cleanEntry.Split('\'');

                if (parts.Length >= 2)
                {
                    string colorPart = parts[1];
                    string timePart = parts[3];

                    string[] rgba = colorPart.Split(',');
                    if (rgba.Length == 4 && float.TryParse(timePart, out float durationMs))
                    {
                        float r = int.Parse(rgba[0].Trim()) / 255f;
                        float g = int.Parse(rgba[1].Trim()) / 255f;
                        float b = int.Parse(rgba[2].Trim()) / 255f;
                        float a = float.Parse(rgba[3].Trim());

                        Color color = new Color(r, g, b, a);
                        animationSteps.Add((color, durationMs / 1000f));
                    }
                }
            }

            return animationSteps;
        }

        void Awake()
        {
            Initialize();
        }

        void Update()
        {
            if (Plugin.AnimationActive.Value)
            {
                if (!animationInitialized)
                {
                    animationSteps = ParseAnimationConfig(Plugin.AnimationValue.Value);
                    currentStepIndex = 0;
                    animationTimer = 0f;
                    animationInitialized = true;
                }

                if (animationSteps.Count > 0)
                {
                    animationTimer += Time.deltaTime;

                    var (color, duration) = animationSteps[currentStepIndex];

                    if (animationTimer >= duration)
                    {
                        animationTimer = 0f;
                        currentStepIndex = (currentStepIndex + 1) % animationSteps.Count;
                        color = animationSteps[currentStepIndex].color;
                    }

                    normalizedR = color.r;
                    normalizedG = color.g;
                    normalizedB = color.b;

                    SetCozyLights();
                }
            }
            if (Plugin.Rainbow.Value && !Plugin.AnimationActive.Value)
            {
                rainbowSpeed = Plugin.RainbowSpeed.Value;

                hue += rainbowSpeed * Time.deltaTime;
                if (hue > 1f) hue = 0f;
                Color rainbowColor = Color.HSVToRGB(hue, 0.8f, 0.8f);
                normalizedR = rainbowColor.r;
                normalizedG = rainbowColor.g;
                normalizedB = rainbowColor.b;
                SetCozyLights();
            }
        }

        public void MaterialHandler(MeshRenderer cozyLights)
        {

            if (MaterialMade)
            { 
                MaterialSwapper(cozyLights); 
                return; 
            }

            Material[] materials = cozyLights.materials;

            cozyLightsOldMat = materials[1];
            cozyLightsNewMat = new Material(Shader.Find("HDRP/Lit"));

            MaterialMade = true;

            MaterialSwapper(cozyLights);
        }

        public void MaterialSwapper(MeshRenderer cozyLights)
        {
            Material[] materials = cozyLights.materials;

            materials[1] = cozyLightsNewMat;
            cozyLights.materials = materials;
        }

        public void MaterialColourChanger(float R, float G, float B)
        {
            if (cozyLightsNewMat == null) { return; }
            if (!LightStatus) { cozyLightsNewMat.color = new Color(0, 0, 0); return; }
            cozyLightsNewMat.color = new Color(R, G, B);
            cozyLightsNewMat.SetColor("_BaseColor", new Color(R, G, B));
        }

        public void Initialize()
        {
            normalizedR = Plugin.colorConfigR.Value / 100f;
            normalizedG = Plugin.colorConfigG.Value / 100f;
            normalizedB = Plugin.colorConfigB.Value / 100f;
            SetCozyLights();
        }

        public void SetCozyLights()
        {
            brightness = Plugin.Brightness.Value;
            GameObject shipCozyLights = GameObject.Find("ShipCozyLights(Clone)");
            if (shipCozyLights != null)
            {
                foreach (Transform child in shipCozyLights.transform)
                {
                    MeshRenderer CozyLightMeshRenderer = child.GetComponent<MeshRenderer>();
                    foreach (Transform megaChild in child)
                    {
                        if (megaChild.name == "Light")
                        {
                            Light lightComponent = megaChild.GetComponent<Light>();
                            if (lightComponent != null)
                            {
                                if (!lightComponent.isActiveAndEnabled) { LightStatus = false; }
                                else { LightStatus = true; }
                                MaterialHandler(CozyLightMeshRenderer);
                                MaterialColourChanger(normalizedR, normalizedG, normalizedB);
                                lightComponent.intensity = brightness;
                                lightComponent.color = new Color(normalizedR, normalizedG, normalizedB);
                            }
                        }
                        else
                        {
                            MeshRenderer CozyLightMeshRenderer2 = megaChild.GetComponent<MeshRenderer>();
                            MaterialHandler(CozyLightMeshRenderer2);
                        }
                    }
                }
            }
        }
    }
}
