using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Colorable_Cozy_Lights
{
    [BepInPlugin("com.atomic.colorcozylights", "Color Cozy Lights", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ConfigEntry<int> colorConfigR;
        internal static ConfigEntry<int> colorConfigG;
        internal static ConfigEntry<int> colorConfigB;
        internal static ConfigEntry<bool> Rainbow;
        internal static ConfigEntry<float> RainbowSpeed;
        internal static ConfigEntry<float> Brightness;

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

            harmony = new Harmony("com.atomic.colorcozylights.patch");
            harmony.PatchAll();

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
                if (__instance.cozyLightsAnimator.GetBool("on") && !lastAnimatorState)
                {
                    var manager = GameObject.FindObjectOfType<CozyLightsManager>();
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

        void Awake()
        {
            Initialize();
        }

        void Update()
        {
            if (Plugin.Rainbow.Value)
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
                    foreach (Transform megaChild in child)
                    {
                        if (megaChild.name == "Light")
                        {
                            Light lightComponent = megaChild.GetComponent<Light>();
                            if (lightComponent != null)
                            {
                                lightComponent.intensity = brightness;
                                lightComponent.color = Plugin.Rainbow.Value
                                    ? new Color(normalizedR, normalizedG, normalizedB)
                                    : new Color(normalizedR, normalizedG, normalizedB);
                            }
                        }

                        if (megaChild.name == "CozyLightsLOD1")
                        {
                            Material material = megaChild.GetComponent<Material>();
                            if (material != null)
                            {
                                var newcolor = new Color(normalizedR, normalizedG, normalizedB);
                                material.SetColor("_EmissiveColor", newcolor);
                                Debug.Log("Changing the light color material");
                            }
                        }
                    }
                }
            }
        }
    }
}
