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
namespace CustomColors
{
    class ColorsUI : MonoBehaviour
    {

        public static void CreateSettingsUI()
        {
            float[] ColorRange = new float[1000];
            fillArray(ColorRange, 0, 1000);

            //Left Color Menu
            var subMenuL = SettingsUI.CreateSubMenu("Left Color");

            //Left Red
            var LeftR = subMenuL.AddList("RGB Red", ColorRange);

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
            var LeftG = subMenuL.AddList("RGB Green", ColorRange);
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
            var LeftB = subMenuL.AddList("RGB Blue", ColorRange);
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

            //Right Color Menu
            var subMenuR = SettingsUI.CreateSubMenu("Right Color");

            //Right Red
            var RightR = subMenuR.AddList("RGB Red", ColorRange);

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
            var RightG = subMenuR.AddList("RGB Green", ColorRange);
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
            var RightB = subMenuR.AddList("RGB Blue", ColorRange);
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

            
            //Saber Override Setting for Left menu
            var saberOverrideL = subMenuL.AddBool("Override Custom Saber Color");

            saberOverrideL.GetValue += delegate
            {
                return ModPrefs.GetBool(Plugin.Name, "OverrideCustomSabers", true, true);
            };

            saberOverrideL.SetValue += delegate (bool value)
            {
                ModPrefs.SetBool(Plugin.Name, "OverrideCustomSabers", value);
            };
    




        }

        public static void fillArray(float[] a, int min, int max)
        {
            for (int i = min; i < max; i++)
            {
                a[i] = i;
            }
        }

    }
}




