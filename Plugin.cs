using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Xft;

namespace CustomColors
{
    public class Plugin : IPlugin
    {
        Color _colorLeft = new Color(1, 0, 0);
        Color _colorRight = new Color(0, 0, 1);
        int _trailLength = 20;
        bool _overrideCustomSabers = true;

        public string Name => "CustomColors";
        public string Version => "1.1";

        bool _init;
        bool _colorInit;
        bool _customsInit;

        //Color objects
        readonly List<SimpleColorSO> _protectedScriptableColors = new List<SimpleColorSO>();

        SimpleColorSO[] _scriptableColors;
        Material[] _allMaterials;
        readonly List<Material> _environmentLights = new List<Material>();
        Saber[] _sabers;
        XWeaponTrail[] _saberTrails;
        TubeBloomPrePassLight[] _prePassLights;

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            _colorInit = false;

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            ReadPreferences();

            GetObjects();
            InvalidateColors();
        }

        void ReadPreferences()
        {
            _colorLeft = new Color(
                ModPrefs.GetFloat(Name, "LeftRed", 255, true) / 255f,
                ModPrefs.GetFloat(Name, "LeftGreen", 0, true) / 255f,
                ModPrefs.GetFloat(Name, "LeftBlue", 0, true) / 255f
            );

            _colorRight = new Color(
                ModPrefs.GetFloat(Name, "RightRed", 0, true) / 255f,
                ModPrefs.GetFloat(Name, "RightGreen", 0, true) / 255f,
                ModPrefs.GetFloat(Name, "RightBlue", 255, true) / 255f
            );

            _trailLength = ModPrefs.GetInt(Name, "TrailLength", 20, true);

            _overrideCustomSabers = ModPrefs.GetBool(Name, "OverrideCustomSabers", true, true);
        }

        void GetObjects()
        {
            _scriptableColors = Resources.FindObjectsOfTypeAll<SimpleColorSO>();

            _allMaterials = Resources.FindObjectsOfTypeAll<Material>();
            _sabers = UnityEngine.Object.FindObjectsOfType<Saber>();
            _saberTrails = UnityEngine.Object.FindObjectsOfType<XWeaponTrail>();
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
            _protectedScriptableColors.Clear();
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

            _protectedScriptableColors.Add(leftColor);
            _protectedScriptableColors.Add(rightColor);
            Log("ColorManager colors set!");


            foreach (var scriptableColor in _scriptableColors.Except(_protectedScriptableColors))
                scriptableColor.SetColor(scriptableColor.color.r > 0.5 ? _colorLeft : _colorRight);
            Log("ScriptableColors modified!");


            foreach (var trail in _saberTrails)
            {
                ReflectionUtil.SetPrivateField(trail, "_maxFrame", _trailLength);
            }
            Log("SaberTrail length set!");


            foreach (var prePassLight in _prePassLights)
            {
                var oldCol = ReflectionUtil.GetPrivateField<Color>(prePassLight, "_color");

                ReflectionUtil.SetPrivateField(prePassLight, "_color", oldCol.r > 0.5 ? _colorLeft : _colorRight);
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
                    var oldCol = ReflectionUtil.GetPrivateField<Color>(text, "m_fontColor");

                    ReflectionUtil.SetPrivateField(text, "m_fontColor", oldCol.r > 0.5 ? _colorLeft : _colorRight);
                }

                var flickeringLetter = UnityEngine.Object.FindObjectOfType<FlickeringNeonSign>();
                if (flickeringLetter != null)
                    ReflectionUtil.SetPrivateField(flickeringLetter, "_onColor", _colorRight);

                Log("Menu colors set!");
            }


            _colorInit = true;
        }

        public void OnFixedUpdate() { }
        public void OnLevelWasInitialized(int level) { }
        public void OnLevelWasLoaded(int level) { }

        void Log(string message)
        {
            Console.WriteLine("[{0}] {1}", Name, message);
        }
        void Log(string format, params object[] args)
        {
            Log(string.Format(format, args));
        }
    }
}
