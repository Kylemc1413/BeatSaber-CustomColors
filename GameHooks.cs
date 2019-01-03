using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using System.Diagnostics;
using Ryder.Lightweight;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using Object = UnityEngine.Object;
using IllusionPlugin;

namespace CustomColors {
    class GameHooks {
        private static Redirection _stretchableCube_Awake;

        public static void Initialize() {
            _stretchableCube_Awake = new Redirection(typeof(StretchableCube).GetMethod("Awake"), typeof(GameHooks).GetMethod("StretchableCube_Awake"), true);
        }

        public static void Shutdown() {
            _stretchableCube_Awake.Stop();
        }

        public static void StretchableCube_Awake(StretchableCube t)
        {
            _stretchableCube_Awake.InvokeOriginal(t);
            Color col;
            if(!Plugin.rainbowWall)
            {
            if (Plugin.wallColorPreset == 0) return;
            if (Plugin.wallColorPreset == 1)
                col = Plugin.ColorLeft;
            else if (Plugin.wallColorPreset == 2)
                col = Plugin.ColorRight;
                else if (Plugin.wallColorPreset == 3)
                    col = new Color(
                    ModPrefs.GetInt(Plugin.Name, "LeftRed", 255, true) / 255f,
                    ModPrefs.GetInt(Plugin.Name, "LeftGreen", 4, true) / 255f,
                    ModPrefs.GetInt(Plugin.Name, "LeftBlue", 4, true) / 255f);
                else if (Plugin.wallColorPreset == 4)
                    col = new Color(
                    ModPrefs.GetInt(Plugin.Name, "RightRed", 255, true) / 255f,
                    ModPrefs.GetInt(Plugin.Name, "RightGreen", 4, true) / 255f,
                    ModPrefs.GetInt(Plugin.Name, "RightBlue", 4, true) / 255f);
                else
                    col = ColorsUI.OtherPresets[Plugin.wallColorPreset].Item1;
            }
            else
            {
                col = new Color(UnityEngine.Random.Range(0f, 1.5f), UnityEngine.Random.Range(0f, 1.5f), UnityEngine.Random.Range(0f, 1.5f));
            }


            foreach (Transform component in t.transform.parent.parent)
            {
                if(component != null)
                foreach (Transform child in component.transform)
                {
                        if(child != null)
                    child.GetComponent<MeshRenderer>().material.color = col;
                }
            }

            MeshRenderer r = t.GetComponent<MeshRenderer>();
            if (r != null) 
            r.material.SetColor("_AddColor", (col/4f).ColorWithAlpha(0f));
        }
    }
}
