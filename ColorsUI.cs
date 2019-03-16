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
using CustomUI;
using CustomUI.Settings;
namespace CustomColors
{
    class ColorsUI : MonoBehaviour
    {
        public static List<Tuple<Color, string>> ColorPresets = new List<Tuple<Color, string>>
        {
            {new Color(), "User"},
            {new Color(1, 0.0156862745f, 0.0156862745f), "Default Red"},
            {new Color(0.00001413f, .706f, 1), "Default Blue"},
            {new Color(0, .98f, 2.157f), "Electric Blue"},
            {new Color(0.00001413f, 1, 0), "Green"},
            {new Color(1.05f, 0, 2.188f), "Purple"},
            {new Color(2.157f ,.588f, 0), "Orange"},
            {new Color(2.157f, 1.76f, 0), "Yellow" },
            {new Color(1, 1, 1), "White"},
            {new Color(.3f ,.3f, .3f), "Black"},
            {new Color(1.000f, 0.396f, 0.243f), "OST Orange"},
            {new Color(0.761f, 0.125f, 0.867f), "OST Purple"},
            {new Color(0.349f, 0.69f, 0.957f), "Klouder Blue"},
            {new Color(0.0352941176f, 0.929411765f, 0.764705882f), "Miku"},
            {new Color(0f, 0.28000000000000003f, 0.55000000000000004f), "Dark Blue"},
            {new Color(1f, 0.388f, .7724f), "Pink"},
        };
        public static List<Tuple<Color, string>> OtherPresets = new List<Tuple<Color, string>>
        {
            {new Color(), "Default" },
            {new Color(), "Left Color" },
            {new Color(), "Right Color" },
            {new Color(), "User Left" },
            {new Color(), "User Right" },
            {new Color(0, .98f, 2.157f), "Electric Blue"},
            {new Color(0.00001413f, 1, 0), "Green"},
            {new Color(1.05f, 0, 2.188f), "Purple"},
            {new Color(2.157f ,.588f, 0), "Orange"},
            {new Color(2.157f, 1.76f, 0), "Yellow" },
            {new Color(1, 1, 1), "White"},
            {new Color(.3f ,.3f, .3f), "Black"},
            {new Color(0f ,0f, 0f), "Pure Black"},
            {new Color(1.000f, 0.396f, 0.243f), "OST Orange"},
            {new Color(0.761f, 0.125f, 0.867f), "OST Purple"},
            {new Color(0.349f, 0.69f, 0.957f), "Klouder Blue"},
            {new Color(0.0352941176f, 0.929411765f, 0.764705882f), "Miku"},
            {new Color(0f, 0.28000000000000003f, 0.55000000000000004f), "Dark Blue"},
            {new Color(1f, 0.388f, .7724f), "Pink"},
        };
        
        public static void CreateSettingsUI()
        {
            var subMenuCC = SettingsUI.CreateSubMenu("Custom Colors");

            //Saber Override Setting for Left menu
            var disableOption = subMenuCC.AddBool("Disable the Plugin");
            disableOption.GetValue += delegate { return Plugin.Config.GetBool("Core", "disablePlugin", false, true); };
            disableOption.SetValue += delegate (bool value) 
            {
                if(value == true)
                {
                Plugin.Config.SetBool("Core", "disablePlugin", value);
                    Plugin.disablePlugin = value;
                   Plugin.queuedDisable = true;
                }
                if (value == false)
                {
                    Plugin.Config.SetBool("Core", "disablePlugin", value);
                    Plugin.disablePlugin = value;
                    Plugin.queuedDisable = false;
                }


            };
            var environmentColorsOption = subMenuCC.AddBool("Allow Color Overrides");
            environmentColorsOption.GetValue += delegate { return Plugin.Config.GetBool("Core", "allowEnvironmentColors", true, true); };
            environmentColorsOption.SetValue += delegate (bool value) { Plugin.Config.SetBool("Core", "allowEnvironmentColors", value); };

            var saberOverrideL = subMenuCC.AddBool("Override Custom Saber Color");
            saberOverrideL.GetValue += delegate { return Plugin.Config.GetBool("Core", "OverrideCustomSabers", true, true); };
            saberOverrideL.SetValue += delegate (bool value) { Plugin.Config.SetBool("Core", "OverrideCustomSabers", value); };

            //Light brightness
            float[] brightnessValues = new float[11] { 0, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f };
            var Brightness = subMenuCC.AddList("Light Brightness", brightnessValues);
            Brightness.GetValue += delegate { return Plugin.Config.GetFloat("Core", "Brightness", 1, true); };
            Brightness.SetValue += delegate (float value) { Plugin.Config.SetFloat("Core", "Brightness", value); };
             Brightness.FormatValue += delegate (float value) { return value.ToString(); };

            var subMenuPresets = SettingsUI.CreateSubMenu("Color Settings");
            float[] presetValues = new float[ColorPresets.Count];
            for (int i = 0; i < ColorPresets.Count; i++) presetValues[i] = i;

            //Left Color Preset
            var leftPreset = subMenuPresets.AddList("Left Color Preset", presetValues);
            leftPreset.GetValue += delegate { return Plugin.Config.GetInt("Presets", "leftNoteColorPreset", 0, true); };
            leftPreset.SetValue += delegate (float value) { Plugin.Config.SetInt("Presets", "leftNoteColorPreset", (int)value); };
            leftPreset.FormatValue += delegate (float value) { return ColorPresets[(int)value].Item2; };

            //Right Color Preset
            var rightPreset = subMenuPresets.AddList("Right Color Preset", presetValues);
            rightPreset.GetValue += delegate { return Plugin.Config.GetInt("Presets", "rightNoteColorPreset", 0, true); };
            rightPreset.SetValue += delegate (float value) { Plugin.Config.SetInt("Presets", "rightNoteColorPreset", (int)value); };
            rightPreset.FormatValue += delegate (float value) { return ColorPresets[(int)value].Item2; };

            float[] otherPresetValues = new float[OtherPresets.Count];
            for (int i = 0; i < OtherPresets.Count; i++) otherPresetValues[i] = i;
            // Wall Color Preset
            var customColoredWalls = subMenuPresets.AddList("Wall Color", otherPresetValues);
            customColoredWalls.GetValue += delegate { return Plugin.Config.GetInt("Presets", "wallColorPreset", 0, true); };
            customColoredWalls.SetValue += delegate (float value) { Plugin.Config.SetInt("Presets", "wallColorPreset", (int)value); };
            customColoredWalls.FormatValue += delegate (float value) { return OtherPresets[(int)value].Item2; };

            //Left Light Preset
            var leftLightPreset = subMenuPresets.AddList("Left Light Preset", otherPresetValues);
            leftLightPreset.GetValue += delegate { return Plugin.Config.GetInt("Presets", "leftLightPreset", 0, true); };
            leftLightPreset.SetValue += delegate (float value) { Plugin.Config.SetInt("Presets", "leftLightPreset", (int)value); };
            leftLightPreset.FormatValue += delegate (float value) { return OtherPresets[(int)value].Item2; };
            //Right Light Preset
            var rightLightPreset = subMenuPresets.AddList("Right Light Preset", otherPresetValues);
            rightLightPreset.GetValue += delegate { return Plugin.Config.GetInt("Presets", "rightLightPreset", 0, true); };
            rightLightPreset.SetValue += delegate (float value) { Plugin.Config.SetInt("Presets", "rightLightPreset", (int)value); };
            rightLightPreset.FormatValue += delegate (float value) { return OtherPresets[(int)value].Item2; };

            //Rainbow Walls
            var rainbowWallOverride = subMenuPresets.AddBool("Rainbow Wall Override");
            rainbowWallOverride.GetValue += delegate { return Plugin.Config.GetBool("Presets", "rainbowWalls", false, true); };
            rainbowWallOverride.SetValue += delegate (bool value) { Plugin.Config.SetBool("Presets", "rainbowWalls", value); };


            var subMenuUser = SettingsUI.CreateSubMenu("User Colors");
            var leftUser = subMenuUser.AddColorPicker("Left User Color Preset", "Left User Color Selection", Plugin.LeftUserColor);
            leftUser.GetValue += delegate { return Plugin.LeftUserColor; };
            leftUser.SetValue += delegate (Color value) 
            {
                Plugin.Config.SetFloat("User Preset Colors", "Left User Preset R", value.r * 255f);
                Plugin.Config.SetFloat("User Preset Colors", "Left User Preset G", value.g  * 255f);
                Plugin.Config.SetFloat("User Preset Colors", "Left User Preset B", value.b * 255f);
                Plugin.LeftUserColor = value;
            };
            var rightUser = subMenuUser.AddColorPicker("Right User Color Preset", "Right User Color Selection", Plugin.RightUserColor);
            rightUser.GetValue += delegate { return Plugin.RightUserColor; };
            rightUser.SetValue += delegate (Color value)
            {
                Plugin.Config.SetFloat("User Preset Colors", "Right User Preset R", value.r *255f);
                Plugin.Config.SetFloat("User Preset Colors", "Right User Preset G", value.g *255f);
                Plugin.Config.SetFloat("User Preset Colors", "Right User Preset B", value.b *255f);
                Plugin.RightUserColor = value;
            };

            /*
        //Left Red
        var LeftR = subMenuUser.AddInt("User Left Red", Plugin.Min, Plugin.Max, Plugin.userIncrement);
        LeftR.GetValue += delegate { return Plugin.Config.GetInt(Plugin.Name, "LeftRed", 255, true); };
        LeftR.SetValue += delegate (int value) { Plugin.Config.SetInt(Plugin.Name, "LeftRed", value); };

        //Left Green
        var LeftG = subMenuUser.AddInt("User Left Green", Plugin.Min, Plugin.Max, Plugin.userIncrement);
        LeftG.GetValue += delegate { return Plugin.Config.GetInt(Plugin.Name, "LeftGreen", 255, true); };
        LeftG.SetValue += delegate (int value) { Plugin.Config.SetInt(Plugin.Name, "LeftGreen", value); };

        //Left Blue
        var LeftB = subMenuUser.AddInt("User Left Blue", Plugin.Min, Plugin.Max, Plugin.userIncrement);
        LeftB.GetValue += delegate { return Plugin.Config.GetInt(Plugin.Name, "LeftBlue", 255, true); };
        LeftB.SetValue += delegate (int value) { Plugin.Config.SetInt(Plugin.Name, "LeftBlue", value); };

        //Right Red
        var RightR = subMenuUser.AddInt("User Right Red", Plugin.Min, Plugin.Max, Plugin.userIncrement);
        RightR.GetValue += delegate { return Plugin.Config.GetInt(Plugin.Name, "RightRed", 255, true); };
        RightR.SetValue += delegate (int value) { Plugin.Config.SetInt(Plugin.Name, "RightRed", value); };

        //Right Green
        var RightG = subMenuUser.AddInt("User Right Green", Plugin.Min, Plugin.Max, Plugin.userIncrement);
        RightG.GetValue += delegate { return Plugin.Config.GetInt(Plugin.Name, "RightGreen", 255, true); };
        RightG.SetValue += delegate (int value) { Plugin.Config.SetInt(Plugin.Name, "RightGreen", value); };

        //Right Blue
        var RightB = subMenuUser.AddInt("User Right Blue", Plugin.Min, Plugin.Max, Plugin.userIncrement);
        RightB.GetValue += delegate { return Plugin.Config.GetInt(Plugin.Name, "RightBlue", 255, true); };
        RightB.SetValue += delegate (int value) { Plugin.Config.SetInt(Plugin.Name, "RightBlue", value); };

        // Increment Modifier
        var incrementValue = subMenuCC.AddList("User Color Increment", new float[] { 1, 5, 10, 25, 50, 100 });
        incrementValue.GetValue += delegate { return Plugin.Config.GetInt(Plugin.Name, "userIncrement", 0, true); };
        incrementValue.SetValue += delegate (float value)
        {
            Plugin.Config.SetInt(Plugin.Name, "userIncrement", (int)value);
            new IntViewController[] { LeftR, LeftG, LeftB, RightR, RightG, RightB }.ToList().ForEach((controller) => { controller.UpdateIncrement((int)value); });
        };
        incrementValue.FormatValue += delegate (float value) { return ((int)value).ToString(); };
        */
        }
        
        public static bool CheckSaberTailor()
        {
            bool result = false;
            foreach (IPlugin plugin in PluginManager.Plugins)
            {
                if (plugin.ToString() == "SaberTailor.Plugin")
                    result = true;
            }
            return result;
        }
        public static void CheckCT()
        {
            foreach (IPlugin plugin in PluginManager.Plugins)
            {
                if (plugin.ToString() == "Chroma.Plugin")
                {
                    Plugin.Log("ChromaToggle Detected, Disabling Custom Colors");
                    Plugin.disablePlugin = true;
                    Plugin.ctInstalled = true;
                }

            }
           
        }
    }
}
