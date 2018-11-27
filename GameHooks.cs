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
            if (Plugin.wallColorPreset == 0) return;
            if (Plugin.wallColorPreset == 1)
                col = Plugin.ColorLeft;
            else if (Plugin.wallColorPreset == 2)
                col = Plugin.ColorRight;
            else
                col = ColorsUI.OtherPresets[Plugin.wallColorPreset].Item1;

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
