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
            if (Plugin.customWallColor == 0) return;
            if (Plugin.customWallColor == 1)
                col = Plugin.ColorLeft;
            else if (Plugin.customWallColor == 2)
                col = Plugin.ColorRight;
            else
                col = ColorsUI.WallPresets[Plugin.customWallColor].Item1;
            col.r = col.r / 4f;
            col.g = col.g / 4f;
            col.b = col.b / 4f;
            foreach (Transform component in t.transform.parent.parent)
            {
                foreach (Transform child in component.transform)
                {
                    child.GetComponent<MeshRenderer>().material.color = col;
                }
            }

            MeshRenderer r = t.GetComponent<MeshRenderer>();
            r.material.SetColor("_AddColor", col.ColorWithAlpha(0f));

            Mesh mesh = t.GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Color[] colors = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) colors[i] = colors[i].ColorWithAlpha(1f);
            mesh.colors = colors;
        }
    }
}
