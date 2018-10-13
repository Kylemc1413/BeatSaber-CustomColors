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
        public const string Name = "CustomColors";
        public const string Version = "1.3.0";

        Color _colorLeft = new Color(1, 0, 0);
        Color _colorRight = new Color(0, 0, 1);
        bool _overrideCustomSabers = true;
        public static int leftColorPreset = 0;
        public static int rightColorPreset = 0;
        public static int userIncrement;

        public const int Max = 3000;
        public const int Min = 0;

        string IPlugin.Name => Name;
        string IPlugin.Version => Version;

        bool _init;
        bool _colorInit;
        bool _customsInit;

        SimpleColorSO[] _scriptableColors;
        readonly List<Material> _environmentLights = new List<Material>();
        TubeBloomPrePassLight[] _prePassLights;

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            _colorInit = false;

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            userIncrement = ModPrefs.GetInt(Plugin.Name, "userIncrement", 10, true);
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (scene.name == "Menu")
            {
                ColorsUI.CreateSettingsUI();
            }

            SharedCoroutineStarter.instance.StartCoroutine(DelayedOnActiveSceneChanged(scene));
            ReadPreferences();

            GetObjects();
            InvalidateColors();
        }

        IEnumerator DelayedOnActiveSceneChanged(Scene scene)
        {
            InvalidateColors();
            yield return new WaitForSeconds(0.01f);

            ReadPreferences();
            GetObjects();
            InvalidateColors();
        }

        void ReadPreferences()
        {
            leftColorPreset = ModPrefs.GetInt(Name, "leftColorPreset", 0, true);
            rightColorPreset = ModPrefs.GetInt(Name, "rightColorPreset", 0, true);
            //Make sure preset exists, else default to user
            if (leftColorPreset > ColorsUI.ColorPresets.Count)
                leftColorPreset = 0;
            if (rightColorPreset > ColorsUI.ColorPresets.Count)
                rightColorPreset = 0;
            //If preset is user get modprefs for colors, otherwise use preset
            if (leftColorPreset == 0)
                _colorLeft = new Color(
                    ModPrefs.GetInt(Name, "LeftRed", 255, true) / 255f,
                    ModPrefs.GetInt(Name, "LeftGreen", 4, true) / 255f,
                    ModPrefs.GetInt(Name, "LeftBlue", 4, true) / 255f
                );
            else
                _colorLeft = ColorsUI.ColorPresets[leftColorPreset].Item1;

            if (rightColorPreset == 0)
                _colorRight = new Color(
                    ModPrefs.GetInt(Name, "RightRed", 0, true) / 255f,
                    ModPrefs.GetInt(Name, "RightGreen", 192, true) / 255f,
                    ModPrefs.GetInt(Name, "RightBlue", 255, true) / 255f
                );
            else
                _colorRight = ColorsUI.ColorPresets[rightColorPreset].Item1;

            _overrideCustomSabers = ModPrefs.GetBool(Name, "OverrideCustomSabers", true, true);

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
        }

        void EnsureCustomSabersOverridden()
        {
            if (!_colorInit) return;
            if (_customsInit) return;

            _customsInit = OverrideSaber("LeftSaber", _colorLeft) && OverrideSaber("RightSaber", _colorRight);
        }
        bool OverrideSaber(string objectName, Color color)
        {
            var saberObject = GameObject.Find(objectName);
            if (saberObject == null) return false;

            var saberRenderers = saberObject.GetComponentsInChildren<Renderer>();
            if (saberRenderers == null) return false;

            foreach (var renderer in saberRenderers)
            {
                foreach (var renderMaterial in renderer.sharedMaterials)
                {
                    if (renderMaterial.HasProperty("_Glow") && renderMaterial.GetFloat("_Glow") > 0 ||
                        renderMaterial.HasProperty("_Bloom") && renderMaterial.GetFloat("_Bloom") > 0)
                    {
                        renderMaterial.SetColor("_Color", color);
                    }
                }
            }

            return true;
        }

        public void OnUpdate()
        {
            if (_colorInit && _overrideCustomSabers)
                EnsureCustomSabersOverridden();

            if (_colorInit) return;


            var colorManager = Resources.FindObjectsOfTypeAll<ColorManager>().FirstOrDefault();
            if (colorManager == null) return;

            var leftColor = ReflectionUtil.GetPrivateField<SimpleColorSO>(colorManager, "_colorA");
            var rightColor = ReflectionUtil.GetPrivateField<SimpleColorSO>(colorManager, "_colorB");

            leftColor.SetColor(_colorLeft);
            rightColor.SetColor(_colorRight);

            Log("ColorManager colors set!");


            foreach (var scriptableColor in _scriptableColors)
            {
                if (scriptableColor.name == "Color Red" || scriptableColor.name == "BaseColor1")
                {
                    scriptableColor.SetColor(_colorLeft);
                }
                else if (scriptableColor.name == "Color Blue" || scriptableColor.name == "BaseColor0")
                {
                    scriptableColor.SetColor(_colorRight);
                }
                //Log($"Set scriptable color: {scriptableColor.name}");
            }
            Log("ScriptableColors modified!");


            foreach (var prePassLight in _prePassLights)
            {
                if (prePassLight.name.Contains("-RightColor") || prePassLight.name.Contains("-LeftColor")) continue;

                var oldCol = ReflectionUtil.GetPrivateField<Color>(prePassLight, "_color");
                if (oldCol.r > 0.5)
                {
                    prePassLight.name += "-LeftColor";
                    ReflectionUtil.SetPrivateField(prePassLight, "_color", _colorLeft);
                }
                else
                {
                    prePassLight.name += "-RightColor";
                    ReflectionUtil.SetPrivateField(prePassLight, "_color", _colorRight);
                }
                //Log($"PrepassLight: {prePassLight.name}");
            }
            Log("PrePassLight colors set!");


            foreach (var light in _environmentLights)
            {
                light.SetColor("_Color", new Color(_colorRight.r * 0.5f, _colorRight.g * 0.5f, _colorRight.b * 0.5f, 1.0f));
            }
            Log("Environment light colors set!");

            if (SceneManager.GetActiveScene().name == "Menu")
            {
                var texts = UnityEngine.Object.FindObjectsOfType<TextMeshPro>();
                foreach (var text in texts)
                {
                    if (text.name == "PP" || text.name == "SABER")
                    {
                        ReflectionUtil.SetPrivateField(text, "m_fontColor", _colorLeft);
                    }
                    else if (text.name == "AT" || text.name == "B" || text.name == "E")
                    {
                        ReflectionUtil.SetPrivateField(text, "m_fontColor", _colorRight);
                    }
                    //Log($"text: {text.name}");
                }

                var flickeringLetter = UnityEngine.Object.FindObjectOfType<FlickeringNeonSign>();
                if (flickeringLetter != null)
                    ReflectionUtil.SetPrivateField(flickeringLetter, "_onColor", _colorRight);

                Log("Menu colors set!");
            }


            _colorInit = true;
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
