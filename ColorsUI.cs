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


        public static void CreateSettingsUI()
        {
    
            var subMenuCC = SettingsUI.CreateSubMenu("Custom Colors");
            float[] presetValues = new float[ColorPresets.Count];
            fillArray(presetValues, 0, ColorPresets.Count());

            //Left Color Preset
            var leftPreset = subMenuCC.AddList("Left Color Preset", presetValues);

            leftPreset.GetValue += delegate
            {
                return ModPrefs.GetInt(Plugin.Name, "leftColorPreset", 0, true);
            };
            leftPreset.SetValue += delegate (float value)
            {
                ModPrefs.SetInt(Plugin.Name, "leftColorPreset", (int)value);
            };
            leftPreset.FormatValue += delegate (float value)
           {
               return ColorPresets[(int)value].Item2;
           };

            //Right Color Preset
            var rightPreset = subMenuCC.AddList("Right Color Preset", presetValues);

            rightPreset.GetValue += delegate
            {
                return ModPrefs.GetInt(Plugin.Name, "rightColorPreset", 0, true);
            };
            rightPreset.SetValue += delegate (float value)
            {
                ModPrefs.SetInt(Plugin.Name, "rightColorPreset", (int)value);
            };
            rightPreset.FormatValue += delegate (float value)
            {
                return ColorPresets[(int)value].Item2;
            };

            //Saber Override Setting for Left menu
            var saberOverrideL = subMenuCC.AddBool("Override Custom Saber Color");

            saberOverrideL.GetValue += delegate
            {
                return ModPrefs.GetBool(Plugin.Name, "OverrideCustomSabers", true, true);
            };

            saberOverrideL.SetValue += delegate (bool value)
            {
                ModPrefs.SetBool(Plugin.Name, "OverrideCustomSabers", value);
            };


            bool saberTailorInstalled = checkSaberTailor();
            float[] validTrailLength = new float[101];
            fillArray(validTrailLength, 0, 101);

            if (saberTailorInstalled == true)
            {
                //Trail adjustment if Saber Tailor is installed
                var SaberTrails = subMenuCC.AddList("Trail Length", validTrailLength);
                SaberTrails.SetValues(validTrailLength);
                SaberTrails.GetValue += delegate
                {
                    return ModPrefs.GetInt("SaberTailor", "TrailLength", 20, true);
                };

                SaberTrails.SetValue += delegate (float value)
                {
                    ModPrefs.SetInt("SaberTailor", "TrailLength", (int)value);

                };

                SaberTrails.FormatValue += delegate (float value)
                {
                    return value.ToString();
                };
            }



       
            //User Color Menu
            float[] ColorRange = new float[3000 / Plugin.userIncrement];
            fillArrayIncremented(ColorRange, 0, 1000, Plugin.userIncrement);

            //Force user colors to be valid for increment
            feexColors(Plugin.userIncrement);
           
            var subMenuUser = SettingsUI.CreateSubMenu("User Colors");

            //Left Red
            var LeftR = subMenuUser.AddList("User Left Red", ColorRange);

            LeftR.GetValue += delegate
            {
                return ModPrefs.GetFloat(Plugin.Name, "LeftRed", 255, true);
            };

            LeftR.SetValue += delegate (float value)
            {
                ModPrefs.SetFloat(Plugin.Name, "LeftRed", value);

            };

            LeftR.FormatValue += delegate (float value)
            {
                return value.ToString();
            };

            //Left Green
            var LeftG = subMenuUser.AddList("User Left Green", ColorRange);

            LeftG.GetValue += delegate
            {
                return ModPrefs.GetFloat(Plugin.Name, "LeftGreen", 255, true);
            };

            LeftG.SetValue += delegate (float value)
            {
                ModPrefs.SetFloat(Plugin.Name, "LeftGreen", value);

            };
            LeftG.FormatValue += delegate (float value)
            {
                return value.ToString();
            };

            //Left Blue
            var LeftB = subMenuUser.AddList("User Left Blue", ColorRange);

            LeftB.GetValue += delegate
            {
                return ModPrefs.GetFloat(Plugin.Name, "LeftBlue", 255, true);
            };

            LeftB.SetValue += delegate (float value)
            {
                ModPrefs.SetFloat(Plugin.Name, "LeftBlue", value);

            };

            LeftB.FormatValue += delegate (float value)
            {
                return value.ToString();
            };

            //Right Red
            var RightR = subMenuUser.AddList("User Right Red", ColorRange);

            RightR.GetValue += delegate
            {
                return ModPrefs.GetFloat(Plugin.Name, "RightRed", 255, true);
            };

            RightR.SetValue += delegate (float value)
            {
                ModPrefs.SetFloat(Plugin.Name, "RightRed", value);

            };

            RightR.FormatValue += delegate (float value)
            {
                return value.ToString();
            };

            //Right Green
            var RightG = subMenuUser.AddList("User Right Green", ColorRange);

            RightG.GetValue += delegate
            {
                return ModPrefs.GetFloat(Plugin.Name, "RightGreen", 255, true);
            };

            RightG.SetValue += delegate (float value)
            {
                ModPrefs.SetFloat(Plugin.Name, "RightGreen", value);

            };
            RightG.FormatValue += delegate (float value)
            {
                return value.ToString();
            };

            //Right Blue
            var RightB = subMenuUser.AddList("User Right Blue", ColorRange);

            RightB.GetValue += delegate
            {
                return ModPrefs.GetFloat(Plugin.Name, "RightBlue", 255, true);
            };

            RightB.SetValue += delegate (float value)
            {
                ModPrefs.SetFloat(Plugin.Name, "RightBlue", value);

            };

            RightB.FormatValue += delegate (float value)
            {
                return value.ToString();
            };

        }

        public static void fillArray(float[] a, int min, int max)
        {
            for (int i = min; i < max; i++)
            {
                a[i] = i;
            }
        }
        public static void fillArrayIncremented(float[] a, int min, int max, int increment)
        {
            max = max / increment;
            for (int i =min; i <= max; i++)
            {
                a[i] = i * increment;
            }
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

        public static float forceColorValid(int userIncrement, float color) 
        {
            while(color > 3000)
                color -= userIncrement;

            color += userIncrement;
            color -= color % userIncrement;



            return color;
        }

        public static void feexColors(int increment)
        {
            float leftRed = forceColorValid(increment, ModPrefs.GetFloat(Plugin.Name, "LeftRed", 255, true));
            float leftGreen = forceColorValid(increment, ModPrefs.GetFloat(Plugin.Name, "LeftGreen", 4, true));
            float leftBlue = forceColorValid(increment, ModPrefs.GetFloat(Plugin.Name, "LeftBlue", 4, true));
            float rightRed = forceColorValid(increment, ModPrefs.GetFloat(Plugin.Name, "RightRed", 0, true));
            float rightGreen = forceColorValid(increment, ModPrefs.GetFloat(Plugin.Name, "RightGreen", 192, true));
            float rightBlue = forceColorValid(increment, ModPrefs.GetFloat(Plugin.Name, "RightBlue", 255, true));
            ModPrefs.SetFloat(Plugin.Name, "LeftRed", leftRed);
            ModPrefs.SetFloat(Plugin.Name, "LeftGreen", leftGreen);
            ModPrefs.SetFloat(Plugin.Name, "LeftBlue", leftBlue);
            ModPrefs.SetFloat(Plugin.Name, "RightRed", rightRed);
            ModPrefs.SetFloat(Plugin.Name, "RightGreen", rightGreen);
            ModPrefs.SetFloat(Plugin.Name, "RightBlue", rightBlue);
        }
        


}
}
