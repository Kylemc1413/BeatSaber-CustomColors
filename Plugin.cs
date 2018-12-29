using IllusionPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace CustomColors
{
    public class Plugin : IPlugin
    {
        public const string Name = "CustomColorsEdit";
        public const string Version = "1.8.7";

        public static Color ColorLeft = new Color(1, 0, 0);
        public static Color ColorRight = new Color(0, 0, 1);
        public static Color ColorLeftLight = new Color(1, 0, 0);
        public static Color ColorRightLight = new Color(0, 0, 1);
        public static bool _overrideCustomSabers = true;
        public static int leftColorPreset = 0;
        public static int rightColorPreset = 0;
        public static int wallColorPreset = 0;
        public static int leftLightPreset = 0;
        public static int rightLightPreset = 0;
        public static int userIncrement;
        public static bool disablePlugin = false;
        public static bool queuedDisable = false;
        public static bool allowEnvironmentColors = true;
        public static bool ctInstalled = false;
        public const int Max = 3000;
        public const int Min = 0;
        public static float brightness = 1f;
        public static bool rainbowWall = false;
        public static int previewChangeAttempts = 5;
        string IPlugin.Name => Name;
        string IPlugin.Version => Version;
        bool _colorInit = false;
        static bool _customsInit = false;
        bool overrideSaberOverride = false;
        bool safeSaberOverride = false;
        static EnvironmentColorsSetter colorsSetter = null;
        readonly List<Material> _environmentLights = new List<Material>();
        SimpleColorSO[] _scriptableColors;
        TubeBloomPrePassLight[] _prePassLights;
        private static bool CustomSabersPresent;
        public void OnApplicationStart()
        {
            ReadPreferences();
            ColorsUI.CheckCT();
            _colorInit = false;
            if (ctInstalled == false)
                GameHooks.Initialize();

            CustomSabersPresent = IllusionInjector.PluginManager.Plugins.Any(x => x.Name == "Saber Mod");
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;


        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (ctInstalled == false)
                if (arg0.name == "Menu")
                {
                    ColorsUI.CreateSettingsUI();

                }
        }


        public void OnApplicationQuit()
        {
            GameHooks.Shutdown();

            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {

            ReadPreferences();
            GetObjects();
            InvalidateColors();

            if (CustomSabersPresent && scene.name == "Menu")
                _customsInit = true;

            if (disablePlugin)
            {
                var colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().FirstOrDefault();
                if (colorManager == null) return;

                var leftColor = ReflectionUtil.GetPrivateField<SimpleColorSO>(colorManager, "_colorA");
                var rightColor = ReflectionUtil.GetPrivateField<SimpleColorSO>(colorManager, "_colorB");

                foreach (var scriptableColor in _scriptableColors)
                {
                    if (scriptableColor != null)
                    {
                        //      Log(scriptableColor.name + " " + scriptableColor.color.ToString());
                        if (scriptableColor.name == "Color0")
                            scriptableColor.SetColor(new Color(1, 0, 0));

                        if (scriptableColor.name == "Color1")
                            scriptableColor.SetColor(new Color(0, .706f, 1));

                        //    Log("TWO " + scriptableColor.name + " " + scriptableColor.color.ToString());

                    }

                }
            }

        }

        void SwapLightColors()
        {
            Color tmp = ColorLeftLight;

        }

        void ReadPreferences()
        {
            _overrideCustomSabers = ModPrefs.GetBool(Name, "OverrideCustomSabers", true, true);
            allowEnvironmentColors = ModPrefs.GetBool(Plugin.Name, "allowEnvironmentColors", true, true);
            if (disablePlugin == false)
            {
                disablePlugin = ModPrefs.GetBool(Name, "disablePlugin", false, true);
                if (disablePlugin) queuedDisable = true;

            }

            if (queuedDisable)
            {
                ColorLeft = new Color(
                                       ModPrefs.GetInt(Name, "LeftRed", 255, true) / 255f,
                                       ModPrefs.GetInt(Name, "LeftGreen", 4, true) / 255f,
                                       ModPrefs.GetInt(Name, "LeftBlue", 4, true) / 255f
                                   );
                ColorRight = new Color(
                      ModPrefs.GetInt(Name, "RightRed", 0, true) / 255f,
                      ModPrefs.GetInt(Name, "RightGreen", 192, true) / 255f,
                      ModPrefs.GetInt(Name, "RightBlue", 255, true) / 255f
                  );
                ColorLeftLight = new Color(1, 4 / 255f, 4 / 255f);
                ColorRightLight = new Color(0, 192 / 255f, 1);
                wallColorPreset = 0;
            }

            if (disablePlugin == false)
            {
                userIncrement = ModPrefs.GetInt(Name, "userIncrement", 10, true);
                leftColorPreset = ModPrefs.GetInt(Name, "leftColorPreset", 0, true);
                rightColorPreset = ModPrefs.GetInt(Name, "rightColorPreset", 0, true);
                wallColorPreset = ModPrefs.GetInt(Name, "wallColorPreset", 0, true);
                leftLightPreset = ModPrefs.GetInt(Name, "leftLightPreset", 1, true);
                rightLightPreset = ModPrefs.GetInt(Name, "rightLightPreset", 2, true);

                brightness = ModPrefs.GetFloat(Name, "Brightness", 1, true);
                rainbowWall = ModPrefs.GetBool(Name, "rainbowWalls", false, true);
                //Make sure preset exists, else default to user
                if (leftColorPreset > ColorsUI.ColorPresets.Count) leftColorPreset = 0;
                if (rightColorPreset > ColorsUI.ColorPresets.Count) rightColorPreset = 0;
                if (leftLightPreset > ColorsUI.OtherPresets.Count) leftLightPreset = 0;
                if (rightLightPreset > ColorsUI.OtherPresets.Count) rightLightPreset = 0;
                if (wallColorPreset > ColorsUI.OtherPresets.Count) wallColorPreset = 0;

                //If preset is user get modprefs for colors, otherwise use preset
                if (leftColorPreset == 0)
                    ColorLeft = new Color(
                        ModPrefs.GetInt(Name, "LeftRed", 255, true) / 255f,
                        ModPrefs.GetInt(Name, "LeftGreen", 4, true) / 255f,
                        ModPrefs.GetInt(Name, "LeftBlue", 4, true) / 255f
                    );
                else
                    ColorLeft = ColorsUI.ColorPresets[leftColorPreset].Item1;

                if (rightColorPreset == 0)
                    ColorRight = new Color(
                        ModPrefs.GetInt(Name, "RightRed", 0, true) / 255f,
                        ModPrefs.GetInt(Name, "RightGreen", 192, true) / 255f,
                        ModPrefs.GetInt(Name, "RightBlue", 255, true) / 255f
                    );
                else
                    ColorRight = ColorsUI.ColorPresets[rightColorPreset].Item1;

                //Set Light colors
                switch (leftLightPreset)
                {
                    case 0:
                        ColorLeftLight = new Color(1, 4 / 255f, 4 / 255f);
                        break;
                    case 1:
                        ColorLeftLight = ColorLeft;
                        if (leftColorPreset != 1 && leftColorPreset != 2)
                            ColorLeftLight *= .8f;
                        break;
                    case 2:
                        ColorLeftLight = ColorRight;
                        if (rightColorPreset != 1 && rightColorPreset != 2)
                            ColorLeftLight *= .8f;
                        break;
                    case 3:
                        ColorLeftLight = new Color(
                        ModPrefs.GetInt(Name, "LeftRed", 255, true) / 255f,
                        ModPrefs.GetInt(Name, "LeftGreen", 4, true) / 255f,
                        ModPrefs.GetInt(Name, "LeftBlue", 4, true) / 255f
                    );
                        ColorLeftLight *= .8f;
                        break;
                    case 4:
                        ColorLeftLight = new Color(
                        ModPrefs.GetInt(Name, "RightRed", 0, true) / 255f,
                        ModPrefs.GetInt(Name, "RightGreen", 192, true) / 255f,
                        ModPrefs.GetInt(Name, "RightBlue", 255, true) / 255f
                    );
                        ColorLeftLight *= .8f;
                        break;
                    default:
                        ColorLeftLight = ColorsUI.OtherPresets[leftLightPreset].Item1;
                        ColorLeftLight *= .8f;
                        break;

                }
                switch (rightLightPreset)
                {
                    case 0:
                        ColorRightLight = new Color(0, 192 / 255f, 1);
                        break;
                    case 1:
                        ColorRightLight = ColorLeft;
                        if (leftColorPreset != 1 && leftColorPreset != 2)
                            ColorRightLight *= .8f;
                        break;
                    case 2:
                        ColorRightLight = ColorRight;
                        if (rightColorPreset != 1 && rightColorPreset != 2)
                            ColorRightLight *= .8f;
                        break;
                    case 3:
                        ColorRightLight = new Color(
                        ModPrefs.GetInt(Name, "LeftRed", 255, true) / 255f,
                        ModPrefs.GetInt(Name, "LeftGreen", 4, true) / 255f,
                        ModPrefs.GetInt(Name, "LeftBlue", 4, true) / 255f
                    );
                        ColorRightLight *= .8f;
                        break;
                    case 4:
                        ColorRightLight = new Color(
                        ModPrefs.GetInt(Name, "RightRed", 0, true) / 255f,
                        ModPrefs.GetInt(Name, "RightGreen", 192, true) / 255f,
                        ModPrefs.GetInt(Name, "RightBlue", 255, true) / 255f
                    );
                        ColorRightLight *= .8f;
                        break;
                    default:
                        ColorRightLight = ColorsUI.OtherPresets[rightLightPreset].Item1;
                        ColorRightLight *= .8f;
                        break;

                }
                ColorLeftLight *= brightness;
                ColorRightLight *= brightness;
            }

        }

        void GetObjects()
        {

            _scriptableColors = Resources.FindObjectsOfTypeAll<SimpleColorSO>();
            _prePassLights = UnityEngine.Object.FindObjectsOfType<TubeBloomPrePassLight>();
            var renderers = UnityEngine.Object.FindObjectsOfType<Renderer>();
            _environmentLights.Clear();
            _environmentLights.AddRange(
                renderers
                    .Where(renderer => renderer.materials.Length > 0)
                    .Select(renderer => renderer.material)
                    .Where(material => material.shader.name == "Custom/ParametricBox" || material.shader.name == "Custom/ParametricBoxOpaque")
            );
        }

        void InvalidateColors()
        {
            _colorInit = false;
            _customsInit = false;
            safeSaberOverride = false;
            colorsSetter = null;
            overrideSaberOverride = false;
        }

        void EnsureCustomSabersOverridden()
        {
            if (!CustomSabersPresent)
            {
                if (SceneManager.GetActiveScene().name != "GameCore") return;
            }
            else
              if (SceneManager.GetActiveScene().name != "Menu" && SceneManager.GetActiveScene().name != "GameCore") return;
            if (_customsInit) return;
            if (disablePlugin && !allowEnvironmentColors) return;

            if (!overrideSaberOverride)
            {
                if (disablePlugin) return;
      //          Log("Attempting Override of Custom Sabers");
                _customsInit = OverrideSaber("LeftSaber", ColorLeft) && OverrideSaber("RightSaber", ColorRight);
            }
            else
            {
    //            Log("Attempting Override of Custom Sabers");
                _customsInit = OverrideSaber("LeftSaber", colorsSetter.GetPrivateField<Color>("_overrideColorB")) && OverrideSaber("RightSaber", colorsSetter.GetPrivateField<Color>("_overrideColorA"));
            }
            
        }

        public static void ForceOverrideCustomSabers(bool loading)
        {
       //     Log("Force Override Called");
            if (!_overrideCustomSabers) return;
            if (loading)
            {
                _customsInit = false;
                previewChangeAttempts = 4;
            }

            else
                _customsInit = true;

        }

        public static bool OverrideSaber(string objectName, Color color)
        {
     //       Log("Attempting Override of  Saber");
            Transform saberObject = null;
            if (previewChangeAttempts <= 0 && SceneManager.GetActiveScene().name == "Menu") return true;
            bool alreadyChanged = true;
            if (SceneManager.GetActiveScene().name == "Menu" && CustomSabersPresent)
            {
       //         Log("Finding Preview");
                saberObject = GameObject.Find("Saber Preview").transform.Find(objectName);
            }

            else
                saberObject = GameObject.Find(objectName).transform;
            if (saberObject == null) return false;
    //        Log(previewChangeAttempts.ToString());
            var saberRenderers = saberObject.GetComponentsInChildren<Renderer>();
            if (saberRenderers == null) return false;

            foreach (var renderer in saberRenderers)
            {
                if (renderer != null)
                {
                    foreach (var renderMaterial in renderer.sharedMaterials)
                    {
                        if (renderMaterial == null)
                        {
                            continue;
                        }

                        if (renderMaterial.HasProperty("_Glow") && renderMaterial.GetFloat("_Glow") > 0 ||
                            renderMaterial.HasProperty("_Bloom") && renderMaterial.GetFloat("_Bloom") > 0)
                        {
                            if (renderMaterial.GetColor("_Color") != color)
                            {
                                alreadyChanged = false;

                            }
                            renderMaterial.SetColor("_Color", color);
                        }
                    }
                }

            }
            if (!alreadyChanged)
                return true;
            else
            {
                previewChangeAttempts -= 1;
                return false;
            }

        }

        public void OnUpdate()
        {
            ApplyColors();
        }

        public void ApplyColors()
        {
            if (_colorInit && _overrideCustomSabers && safeSaberOverride)
                EnsureCustomSabersOverridden();
            if (SceneManager.GetActiveScene().name == "Menu" && CustomSabersPresent && _overrideCustomSabers)
                EnsureCustomSabersOverridden();

            if (disablePlugin == false || queuedDisable)
            {
               
                if (_colorInit) return;


                var colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().FirstOrDefault();
                if (colorManager == null) return;

                var leftColor = ReflectionUtil.GetPrivateField<SimpleColorSO>(colorManager, "_colorA");
                var rightColor = ReflectionUtil.GetPrivateField<SimpleColorSO>(colorManager, "_colorB");

                leftColor.SetColor(ColorLeft);
                rightColor.SetColor(ColorRight);

                Log("ColorManager colors set!");

                foreach (var scriptableColor in _scriptableColors)
                {
                    if (scriptableColor != null)
                    {
                        //     Log(scriptableColor.name);
                        //     Log(scriptableColor.color.ToString());
                        /*
                        if (scriptableColor.name == "Color Red" || scriptableColor.name == "BaseColor1")
                        {
                            scriptableColor.SetColor(ColorLeft);
                        }
                        else if (scriptableColor.name == "Color Blue" || scriptableColor.name == "BaseColor0")
                        {
                            scriptableColor.SetColor(ColorRight);
                        }
                        */
                        if (scriptableColor.name == "Color0")
                            scriptableColor.SetColor(ColorLeft);
                        else if (scriptableColor.name == "BaseColor0")
                            scriptableColor.SetColor(ColorRightLight);
                        else if (scriptableColor.name == "Color1")
                            scriptableColor.SetColor(ColorRight);
                        else if (scriptableColor.name == "BaseColor1")
                            scriptableColor.SetColor(ColorLeftLight);
                        else if (scriptableColor.name == "MenuEnvLight0")
                            scriptableColor.SetColor(ColorRightLight);

                        //      Log(scriptableColor.name);
                        //      Log(scriptableColor.color.ToString());
                    }
                    //         Log($"Set scriptable color: {scriptableColor.name}");
                }
                Log("ScriptableColors modified!");
                colorManager.RefreshColors();

                foreach (var prePassLight in _prePassLights)
                {

                    if (prePassLight != null)
                    {
                        if (prePassLight.name.Contains("NeonLight (6)"))
                        {
                            prePassLight.color = ColorLeftLight;

                        }
                        if (prePassLight.name.Contains("NeonLight (8)"))
                        {
                            if (prePassLight.gameObject.transform.position.ToString() == "(0.0, 17.2, 24.8)")
                            {
                                prePassLight.color = ColorLeftLight;
                            }

                        }
                        if (prePassLight.name.Contains("BATNeon") || prePassLight.name.Contains("ENeon"))
                            prePassLight.color = ColorRightLight;

                        //    Log($"PrepassLight: {prePassLight.name}");
                    }
                }

                Log("PrePassLight colors set!");
                SpriteRenderer[] sprites = Resources.FindObjectsOfTypeAll<SpriteRenderer>();
                foreach (var sprite in sprites)
                {
                    if (sprite != null)
                    {
                        if (sprite.name == "LogoSABER")
                            sprite.color = ColorLeftLight;
                        if (sprite.name == "LogoBAT" || sprite.name == "LogoE")
                            sprite.color = ColorRightLight;
                    }

                }


                //Logo Disable if needed
                /*
               GameObject logo = GameObject.Find("Logo");
               if(logo != null)
               GameObject.Destroy(logo.gameObject);
               */

                if (SceneManager.GetActiveScene().name == "Menu")
                {

                    var flickeringLetter = UnityEngine.Object.FindObjectOfType<FlickeringNeonSign>();
                    if (flickeringLetter != null)
                        ReflectionUtil.SetPrivateField(flickeringLetter, "_onColor", ColorRightLight);

                    Log("Menu colors set!");
                }


                _colorInit = true;
                queuedDisable = false;
                colorsSetter = Resources.FindObjectsOfTypeAll<EnvironmentColorsSetter>().FirstOrDefault();
                if (allowEnvironmentColors)
                {
                    if (colorsSetter != null)
                    {
                        colorsSetter.Awake();
                        overrideSaberOverride = true;
                    }
                }
                else
                {
                    colorManager.RefreshColors();
                }
                safeSaberOverride = true;
            }
            if(disablePlugin && allowEnvironmentColors && _overrideCustomSabers)
            {
                colorsSetter = Resources.FindObjectsOfTypeAll<EnvironmentColorsSetter>().FirstOrDefault();
                if (colorsSetter != null)
                {
                    overrideSaberOverride = true;
                    _colorInit = true;
                }
                    safeSaberOverride = true;
            }

        }

        public static void Log(string message)
        {
            Console.WriteLine("[{0}] {1}", Name, message);
        }
        public static void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        #region Unused IPlugin Members

        void IPlugin.OnFixedUpdate() { }
        void IPlugin.OnLevelWasLoaded(int level) { }
        void IPlugin.OnLevelWasInitialized(int level) { }

        #endregion

    }
}