using HMUI;
using IllusionPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUI;
using System.Globalization;
using IllusionInjector;
namespace CustomColors
{
    class ColorsUI : MonoBehaviour
    {
        public static List<Tuple<Color, string>> ColorPresets = new List<Tuple<Color, string>>
        {
            {new Color(), "User"},
            {new Color(1, 0, 0), "Default Red"},
            {new Color(0, 0.5f, 1), "Default Blue"},
            {new Color(0, .98f, 2.157f), "Electric Blue"},
            {new Color(0, 1, 0), "Green"},
            {new Color(1.05f, 0, 2.188f), "Purple"},
            {new Color(2.157f ,.588f, 0), "Orange"},
            {new Color(2.157f, 1.76f, 0), "Yellow" },
            {new Color(1, 1, 1), "White"},
            {new Color(.3f ,.3f, .3f), "Black"},
        };
        public static List<Tuple<Color, string>> OtherPresets = new List<Tuple<Color, string>>
        {
            {new Color(), "Default" },
            {new Color(), "User Left" },
            {new Color(), "User Right" },
            {new Color(0, .98f, 2.157f), "Electric Blue"},
            {new Color(0, 1, 0), "Green"},
            {new Color(1.05f, 0, 2.188f), "Purple"},
            {new Color(2.157f ,.588f, 0), "Orange"},
            {new Color(2.157f, 1.76f, 0), "Yellow" },
            {new Color(1, 1, 1), "White"},
            {new Color(.3f ,.3f, .3f), "Black"},
        };

        public static void CreateSettingsUI()
        {
            var subMenuCC = SettingsUI.CreateSubMenu("Custom Colors");

            //Saber Override Setting for Left menu
            var saberOverrideL = subMenuCC.AddBool("Override Custom Saber Color");
            saberOverrideL.GetValue += delegate { return ModPrefs.GetBool(Plugin.Name, "OverrideCustomSabers", true, true); };
            saberOverrideL.SetValue += delegate (bool value) { ModPrefs.SetBool(Plugin.Name, "OverrideCustomSabers", value); };

            bool saberTailorInstalled = checkSaberTailor();
            if (saberTailorInstalled == true)
            {
                //Trail adjustment if Saber Tailor is installed
                var SaberTrails = subMenuCC.AddInt("Trail Length", 0, 100, 1);
                SaberTrails.GetValue += delegate { return ModPrefs.GetInt("SaberTailor", "TrailLength", 20, true); };
                SaberTrails.SetValue += delegate (int value) { ModPrefs.SetInt("SaberTailor", "TrailLength", (int)value); };
            }


            var subMenuPresets = SettingsUI.CreateSubMenu("Color Settings");
            float[] presetValues = new float[ColorPresets.Count];
            for (int i = 0; i < ColorPresets.Count; i++) presetValues[i] = i;

            //Left Color Preset
            var leftPreset = subMenuPresets.AddList("Left Color Preset", presetValues);
            leftPreset.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "leftColorPreset", 0, true); };
            leftPreset.SetValue += delegate (float value) { ModPrefs.SetInt(Plugin.Name, "leftColorPreset", (int)value); };
            leftPreset.FormatValue += delegate (float value) { return ColorPresets[(int)value].Item2; };

            //Right Color Preset
            var rightPreset = subMenuPresets.AddList("Right Color Preset", presetValues);
            rightPreset.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "rightColorPreset", 0, true); };
            rightPreset.SetValue += delegate (float value) { ModPrefs.SetInt(Plugin.Name, "rightColorPreset", (int)value); };
            rightPreset.FormatValue += delegate (float value) { return ColorPresets[(int)value].Item2; };

            float[] otherPresetValues = new float[OtherPresets.Count];
            for (int i = 0; i < OtherPresets.Count; i++) otherPresetValues[i] = i;
            // Wall Color Preset
            var customColoredWalls = subMenuPresets.AddList("Wall Color", otherPresetValues);
            customColoredWalls.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "customWallColor", 0, true); };
            customColoredWalls.SetValue += delegate (float value) { ModPrefs.SetInt(Plugin.Name, "customWallColor", (int)value); };
            customColoredWalls.FormatValue += delegate (float value) { return OtherPresets[(int)value].Item2; };

            //Left Light Preset
            var leftLightPreset = subMenuPresets.AddList("Left Light Preset", otherPresetValues);
            leftLightPreset.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "leftLightPreset", 0, true); };
            leftLightPreset.SetValue += delegate (float value) { ModPrefs.SetInt(Plugin.Name, "leftLightPreset", (int)value); };
            leftLightPreset.FormatValue += delegate (float value) { return OtherPresets[(int)value].Item2; };
            //Right Light Preset
            var rightLightPreset = subMenuPresets.AddList("Right Light Preset", otherPresetValues);
            rightLightPreset.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "rightLightPreset", 0, true); };
            rightLightPreset.SetValue += delegate (float value) { ModPrefs.SetInt(Plugin.Name, "rightLightPreset", (int)value); };
            rightLightPreset.FormatValue += delegate (float value) { return OtherPresets[(int)value].Item2; };



            /*
            var customColoredWalls = subMenuCC.AddList("Custom Wall Color", new float[] { 0, 1, 2 });
            var wallNames = new string[] { "Off", "Left Color", "Right Color" };
            customColoredWalls.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "customWallColor", 1, true); };
            customColoredWalls.SetValue += delegate (float value) { ModPrefs.SetInt(Plugin.Name, "customWallColor", (int)value); };
            customColoredWalls.FormatValue += delegate (float value) { return wallNames[(int)value]; };
            */

            var subMenuUser = SettingsUI.CreateSubMenu("User Colors");

            //Left Red
            var LeftR = subMenuUser.AddInt("User Left Red", Plugin.Min, Plugin.Max, Plugin.userIncrement);
            LeftR.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "LeftRed", 255, true); };
            LeftR.SetValue += delegate (int value) { ModPrefs.SetInt(Plugin.Name, "LeftRed", value); };

            //Left Green
            var LeftG = subMenuUser.AddInt("User Left Green", Plugin.Min, Plugin.Max, Plugin.userIncrement);
            LeftG.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "LeftGreen", 255, true); };
            LeftG.SetValue += delegate (int value) { ModPrefs.SetInt(Plugin.Name, "LeftGreen", value); };

            //Left Blue
            var LeftB = subMenuUser.AddInt("User Left Blue", Plugin.Min, Plugin.Max, Plugin.userIncrement);
            LeftB.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "LeftBlue", 255, true); };
            LeftB.SetValue += delegate (int value) { ModPrefs.SetInt(Plugin.Name, "LeftBlue", value); };

            //Right Red
            var RightR = subMenuUser.AddInt("User Right Red", Plugin.Min, Plugin.Max, Plugin.userIncrement);
            RightR.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "RightRed", 255, true); };
            RightR.SetValue += delegate (int value) { ModPrefs.SetInt(Plugin.Name, "RightRed", value); };

            //Right Green
            var RightG = subMenuUser.AddInt("User Right Green", Plugin.Min, Plugin.Max, Plugin.userIncrement);
            RightG.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "RightGreen", 255, true); };
            RightG.SetValue += delegate (int value) { ModPrefs.SetInt(Plugin.Name, "RightGreen", value); };

            //Right Blue
            var RightB = subMenuUser.AddInt("User Right Blue", Plugin.Min, Plugin.Max, Plugin.userIncrement);
            RightB.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "RightBlue", 255, true); };
            RightB.SetValue += delegate (int value) { ModPrefs.SetInt(Plugin.Name, "RightBlue", value); };
            
            // Increment Modifier
            var incrementValue = subMenuCC.AddList("User Color Increment", new float[] { 1, 5, 10, 25, 50, 100 });
            incrementValue.GetValue += delegate { return ModPrefs.GetInt(Plugin.Name, "userIncrement", 0, true); };
            incrementValue.SetValue += delegate (float value)
            {
                ModPrefs.SetInt(Plugin.Name, "userIncrement", (int)value);
                new IntViewController[] { LeftR, LeftG, LeftB, RightR, RightG, RightB }.ToList().ForEach((controller) => { controller.UpdateIncrement((int)value); });
            };
            incrementValue.FormatValue += delegate (float value) { return ((int)value).ToString(); };
        }

        public static bool checkSaberTailor()
        {
            bool result = false;
            foreach (IPlugin plugin in PluginManager.Plugins)
            {
                if (plugin.ToString() == "SaberTailor.Plugin")
                    result = true;
            }
            return result;
        }
    }
}
