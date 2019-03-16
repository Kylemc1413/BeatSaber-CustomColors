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
        public static BS_Utils.Utilities.Config Config = new BS_Utils.Utilities.Config("CustomColors");

        public const string Name = "CustomColorsEdit";
        public const string Version = "1.11.2";
        public delegate void ColorsApplied();
        public delegate void SettingsChanged();
        public static event SettingsChanged CCSettingsChanged;
        public static event ColorsApplied ColorsAppliedEvent;
        public static Color ColorLeft = new Color(1, 0, 0);
        public static Color ColorRight = new Color(0, 0, 1);
        public static Color ColorLeftLight = new Color(1, 0, 0);
        public static Color ColorRightLight = new Color(0, 0, 1);
        public static Color LeftUserColor;
        public static Color RightUserColor;

        public static Color CurrentWallColor;
        public static Color wallColor;
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
        public static float lerpControl = 0;
        public static bool gameScene = false;
        string IPlugin.Name => Name;
        string IPlugin.Version => Version;
        static bool _colorInit = false;
        static bool _customsInit = false;
        bool overrideSaberOverride = false;
        bool safeSaberOverride = false;
        static EnvironmentColorsSetter colorsSetter = null;
        readonly List<Material> _environmentLights = new List<Material>();
        SimpleColorSO[] _scriptableColors;
        TubeBloomPrePassLight[] _prePassLights;
        private static bool CustomSabersPresent;
        internal static bool safe = false;

        public static IEnumerable<Material> coreObstacleMaterials;
        public static IEnumerable<Material> frameObstacleMaterials;


        public void OnApplicationStart()
        {
            ReadPreferences();
            ColorsUI.CheckCT();
            _colorInit = false;

            CustomSabersPresent = IllusionInjector.PluginManager.Plugins.Any(x => x.Name == "Saber Mod");
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;


        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (ctInstalled == false)
                if (arg0.name == "MenuCore")
                {
                    ColorsUI.CreateSettingsUI();

                }
        }


        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            safe = false;
            gameScene = false;
            ReadPreferences();
            GetObjects();
            InvalidateColors();
            if (scene.name == "GameCore" || scene.name == "MenuCore")
                safe = true;
            if (CustomSabersPresent && scene.name == "MenuCore")
                _customsInit = true;
            if (scene.name == "GameCore")
                gameScene = true;

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
            _overrideCustomSabers = Config.GetBool("Core", "OverrideCustomSabers", true, true);
            allowEnvironmentColors = Config.GetBool("Core", "allowEnvironmentColors", true, true);
            disablePlugin = Config.GetBool("Core", "disablePlugin", false, true) || ctInstalled;
            if (disablePlugin) queuedDisable = true;

            if (queuedDisable)
            {
                ColorLeft = new Color(
                                       255f,
                                       4f / 255f,
                                       4f / 255f
                                   );
                ColorRight = new Color(
                      0,
                      192f / 255f,
                      1f
                  );
                ColorLeftLight = new Color(1, 4 / 255f, 4 / 255f);
                ColorRightLight = new Color(0, 192 / 255f, 1);
                wallColorPreset = 0;
            }

            if (disablePlugin == false)
            {
                leftColorPreset = Config.GetInt("Presets", "leftNoteColorPreset", 0, true);
                rightColorPreset = Config.GetInt("Presets", "rightNoteColorPreset", 0, true);
                wallColorPreset = Config.GetInt("Presets", "wallColorPreset", 0, true);
                leftLightPreset = Config.GetInt("Presets", "leftLightPreset", 1, true);
                rightLightPreset = Config.GetInt("Presets", "rightLightPreset", 2, true);

                brightness = Config.GetFloat("Core", "Brightness", 1, true);
                rainbowWall = Config.GetBool("Presets", "rainbowWalls", false, true);
                //Make sure preset exists, else default to user
                if (leftColorPreset > ColorsUI.ColorPresets.Count) leftColorPreset = 0;
                if (rightColorPreset > ColorsUI.ColorPresets.Count) rightColorPreset = 0;
                if (leftLightPreset > ColorsUI.OtherPresets.Count) leftLightPreset = 0;
                if (rightLightPreset > ColorsUI.OtherPresets.Count) rightLightPreset = 0;
                if (wallColorPreset > ColorsUI.OtherPresets.Count) wallColorPreset = 0;

                //If preset is user get config values for colors, otherwise use preset
                LeftUserColor = new Color(
                   Config.GetFloat("User Preset Colors", "Left User Preset R", 255, true) / 255f,
                   Config.GetFloat("User Preset Colors", "Left User Preset G", 4, true) / 255f,
                   Config.GetFloat("User Preset Colors", "Left User Preset B", 4, true) / 255f
               );
                RightUserColor = new Color(
                      Config.GetFloat("User Preset Colors", "Right User Preset R", 0, true) / 255f ,
                      Config.GetFloat("User Preset Colors", "Right User Preset G", 192, true) / 255f,
                      Config.GetFloat("User Preset Colors", "Right User Preset B", 255, true) / 255f
                  );
                if (leftColorPreset == 0)
                    ColorLeft = LeftUserColor;
                else
                    ColorLeft = ColorsUI.ColorPresets[leftColorPreset].Item1;

                if (rightColorPreset == 0)
                    ColorRight = RightUserColor;
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
                        ColorLeftLight = LeftUserColor;
                        ColorLeftLight *= .8f;
                        break;
                    case 4:
                        ColorLeftLight = RightUserColor;
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
                        ColorRightLight = LeftUserColor;
                        ColorRightLight *= .8f;
                        break;
                    case 4:
                        ColorRightLight = RightUserColor;
                        ColorRightLight *= .8f;
                        break;
                    default:
                        ColorRightLight = ColorsUI.OtherPresets[rightLightPreset].Item1;
                        ColorRightLight *= .8f;
                        break;

                }
                ColorLeftLight *= brightness;
                ColorRightLight *= brightness;
                GetWallColor();

            }
            CCSettingsChanged?.Invoke();
        }

        void GetObjects()
        {
            _scriptableColors = Resources.FindObjectsOfTypeAll<SimpleColorSO>();
            _prePassLights = UnityEngine.Object.FindObjectsOfType<TubeBloomPrePassLight>();
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
              if (SceneManager.GetActiveScene().name != "MenuCore" && SceneManager.GetActiveScene().name != "GameCore") return;
            if (_customsInit) return;
            if (disablePlugin && !allowEnvironmentColors) return;

            if (!overrideSaberOverride)
            {
                if (disablePlugin) return;
                //          Log("Attempting Override of Custom Sabers");
                _customsInit = OverrideSaber("LeftSaber", ColorLeft) || OverrideSaber("RightSaber", ColorRight);
                if (_customsInit)
                {
                    //Reoverride attempt both once one attempt succeeds, to try and account for one saber cases, etc
                    OverrideSaber("LeftSaber", ColorLeft);
                    OverrideSaber("RightSaber", ColorRight);
                }
            }
            else
            {
                //            Log("Attempting Override of Custom Sabers");
                _customsInit = OverrideSaber("LeftSaber", colorsSetter.GetPrivateField<Color>("_overrideColorB")) || OverrideSaber("RightSaber", colorsSetter.GetPrivateField<Color>("_overrideColorA"));
                if (_customsInit)
                {
                    //Reoverride attempt both once one attempt succeeds, to try and account for one saber cases, etc
                    OverrideSaber("LeftSaber", colorsSetter.GetPrivateField<Color>("_overrideColorB"));
                    OverrideSaber("RightSaber", colorsSetter.GetPrivateField<Color>("_overrideColorA"));
                }
            }

        }
        public static void OverrideCustomSaberColors(Color left, Color right)
        {
            if (!_overrideCustomSabers || !allowEnvironmentColors) return;
            OverrideSaber("LeftSaber", left);
            OverrideSaber("RightSaber", right);
        }
        public static void ForceOverrideCustomSabers(bool loading)
        {
            //     Log("Force Override Called");
            if (!_overrideCustomSabers) return;
            if (loading)
            {
                _customsInit = false;
            }

            else
                _customsInit = true;

        }

        public static bool OverrideSaber(string objectName, Color color)
        {
            //       Log("Attempting Override of  Saber");
            Transform saberObject = null;
            if (SceneManager.GetActiveScene().name == "MenuCore" && CustomSabersPresent)
            {
                //         Log("Finding Preview");
                saberObject = GameObject.Find("Saber Preview").transform.Find(objectName);
            }

            else
                saberObject = GameObject.Find(objectName)?.transform;
            if (saberObject == null) return false;
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
                            renderMaterial.SetColor("_Color", color);
                        }
                    }
                }

            }
            return true;

        }

        public void OnUpdate()
        {
            if(safe)
            ApplyColors();


        }

        private static Color GetWallColor()
        {
            Color col;
            if (!Plugin.rainbowWall)
            {
                if (Plugin.wallColorPreset == 1)
                    col = Plugin.ColorLeft;
                else if (Plugin.wallColorPreset == 2)
                    col = Plugin.ColorRight;
                else if (Plugin.wallColorPreset == 3)
                    col = LeftUserColor;
                else if (Plugin.wallColorPreset == 4)
                    col = RightUserColor;
                else
                    col = ColorsUI.OtherPresets[Plugin.wallColorPreset].Item1;
            }
            else
            {
                col = Rainbow.GetRandomColor();
                if (!_colorInit)
                    CurrentWallColor = col;
            }
            return col;
        }

        public void ApplyColors()
        {
            if (_colorInit && _overrideCustomSabers && safeSaberOverride)
                EnsureCustomSabersOverridden();
            if (SceneManager.GetActiveScene().name == "MenuCore" && CustomSabersPresent && _overrideCustomSabers)
                EnsureCustomSabersOverridden();

            if (disablePlugin == false || queuedDisable)
            {

                //                [CustomColorsEdit] Mesh renderer material name is ObstacleCore(Instance)
                //[CustomColorsEdit] Mesh renderer material name is ObstacleCoreInside(Instance)
                //[CustomColorsEdit] Mesh renderer material name is ObstacleFrame(Instance)

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
                        //         Log(scriptableColor.name);
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
                        if (scriptableColor.name == "BaseNoteColor0")
                            scriptableColor.SetColor(ColorRight);
                        else if (scriptableColor.name == "BaseColor0")
                            scriptableColor.SetColor(ColorRightLight);
                        else if (scriptableColor.name == "BaseNoteColor1")
                            scriptableColor.SetColor(ColorLeft);
                        else if (scriptableColor.name == "BaseColor1")
                            scriptableColor.SetColor(ColorLeftLight);
                        else if (scriptableColor.name == "MenuEnvLight0")
                            scriptableColor.SetColor(ColorRightLight);
                        //   else if (scriptableColor.name == "MenuEnvLight1")
                        //      scriptableColor.SetColor(ColorRightLight);

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
                /*
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
                */

                if (Plugin.wallColorPreset != 0)
                {

                    coreObstacleMaterials = Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "ObstacleCore" || m.name == "ObstacleCoreInside");
                    frameObstacleMaterials = Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "ObstacleFrame");
                    if (gameScene && rainbowWall)
                        SharedCoroutineStarter.instance.StartCoroutine(RainbowWalls());
                    else
                        SetWallColors();
                }

                //Logo Disable if needed
                /*
               GameObject logo = GameObject.Find("Logo");
               if(logo != null)
               GameObject.Destroy(logo.gameObject);
               */

                if (SceneManager.GetActiveScene().name == "MenuCore")
                {
                    /*
                    var flickeringLetter = UnityEngine.Object.FindObjectOfType<FlickeringNeonSign>();
                    if (flickeringLetter != null)
                        ReflectionUtil.SetPrivateField(flickeringLetter, "_onColor", ColorRightLight);
                        */
                    Log("Menu colors set!");
                }


                _colorInit = true;
                queuedDisable = false;
                colorsSetter = Resources.FindObjectsOfTypeAll<EnvironmentColorsSetter>().FirstOrDefault();
                if (allowEnvironmentColors)
                {
                    if (colorsSetter != null)
                    {
                        try
                        {
                            colorsSetter.Awake();
                        }
                        catch
                        {
                            Log("Failed to Initialize Color Setter");
                        }

                        overrideSaberOverride = true;
                    }
                }
                else
                {
                    colorManager.RefreshColors();
                }
                safeSaberOverride = true;
                ColorsAppliedEvent?.Invoke();
            }
            if (disablePlugin && allowEnvironmentColors && _overrideCustomSabers)
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

        public static void SetWallColors()
        {

            wallColor = GetWallColor();
            if (coreObstacleMaterials != null && frameObstacleMaterials != null)
            {
                foreach (Material m in coreObstacleMaterials)
                {
                    m.color = wallColor;
                    m.SetColor("_AddColor", (wallColor / 4f).ColorWithAlpha(0f));
                }
                foreach (Material m in frameObstacleMaterials)
                {
                    m.color = wallColor;
                }
            }


        }


        public static IEnumerator RainbowWalls()
        {
            yield return new WaitForSeconds(0.08f);
            if (!gameScene) yield break;
            if (rainbowWall)
            {
                if (CurrentWallColor == wallColor)
                {
                    wallColor = GetWallColor();
                    lerpControl = 0;
                }
                if (lerpControl < 1)
                    lerpControl += 0.08f / 1.25f;
                CurrentWallColor = Color.Lerp(CurrentWallColor, wallColor, lerpControl);

                if (coreObstacleMaterials != null && frameObstacleMaterials != null)
                {
                    foreach (Material m in coreObstacleMaterials)
                    {
                        m.color = CurrentWallColor;
                        m.SetColor("_AddColor", (CurrentWallColor / 4f).ColorWithAlpha(0f));
                    }
                    foreach (Material m in frameObstacleMaterials)
                    {
                        m.color = CurrentWallColor;
                    }
                }

            }
            SharedCoroutineStarter.instance.StartCoroutine(RainbowWalls());



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