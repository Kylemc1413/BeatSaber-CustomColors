using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
namespace CustomColors.HarmonyPatches
{
    
    [HarmonyPatch(typeof(ColorNoteVisuals), "HandleNoteControllerDidInitEvent")]
    internal class ColorNoteVisualsPatch
    {
        public static void Postfix(NoteController noteController, ref MeshRenderer ____arrowMeshRenderer, ref SpriteRenderer ____arrowGlowSpriteRenderer, ref SpriteRenderer ____circleGlowSpriteRenderer, ref float ____arrowGlowIntensity, ref MaterialPropertyBlockController[] ____materialPropertyBlockControllers, ref int ____colorID)
        {
            if (Plugin.disablePlugin || Plugin.disableArrowChanges)
                return;
            if (noteController.noteData.noteType == NoteType.NoteA)
            {
                if (Plugin.leftArrowGlowPreset != 0)
                {
                    ____arrowGlowSpriteRenderer.color = Plugin.LeftArrowGlowColor.ColorWithAlpha(____arrowGlowIntensity);
                    ____circleGlowSpriteRenderer.color = Plugin.LeftArrowGlowColor.ColorWithAlpha(____arrowGlowIntensity);
                }

                if (Plugin.leftArrowPreset != 0)
                {
                    ____arrowMeshRenderer.material.color = Plugin.LeftArrowColor;//.ColorWithAlpha(____arrowGlowIntensity);
                    if (noteController.noteData.cutDirection == NoteCutDirection.Any && Plugin.dotArrowFix)
                    {
                        foreach (MaterialPropertyBlockController materialPropertyBlockController in ____materialPropertyBlockControllers)
                        {
                            MaterialPropertyBlock materialPropertyBlock = materialPropertyBlockController.materialPropertyBlock;
                            materialPropertyBlock.SetColor(____colorID, Plugin.LeftArrowColor.ColorWithAlpha(1f));
                            materialPropertyBlockController.ApplyChanges();
                        }
                    }
                }

            }
            else if (noteController.noteData.noteType == NoteType.NoteB)
            {
                if (Plugin.rightArrowGlowPreset != 0)
                {
                    ____arrowGlowSpriteRenderer.color = Plugin.RightArrowGlowColor.ColorWithAlpha(____arrowGlowIntensity);
                    ____circleGlowSpriteRenderer.color = Plugin.RightArrowGlowColor.ColorWithAlpha(____arrowGlowIntensity);
                }
                if (Plugin.rightArrowPreset != 0)
                {
                    ____arrowMeshRenderer.material.color = Plugin.RightArrowColor;//.ColorWithAlpha(____arrowGlowIntensity);

                    if (noteController.noteData.cutDirection == NoteCutDirection.Any && Plugin.dotArrowFix)
                    {
                        foreach (MaterialPropertyBlockController materialPropertyBlockController in ____materialPropertyBlockControllers)
                        {
                            MaterialPropertyBlock materialPropertyBlock = materialPropertyBlockController.materialPropertyBlock;
                            materialPropertyBlock.SetColor(____colorID, Plugin.RightArrowColor.ColorWithAlpha(1f));
                            materialPropertyBlockController.ApplyChanges();
                        }
                    }
                }
             

            }

        }
    }
    
    [HarmonyPatch(typeof(BombNoteController), "Init")]
    class BombInitPatch
    {
        public static Material noteMat = null;
        public static void Postfix(NoteData noteData,
            Vector3 moveStartPos, Vector3 moveEndPos, Vector3 jumpEndPos, float moveDuration, float jumpDuration, float startTime, float jumpGravity, ref GameObject ____wrapperGO)
        {
            if(Plugin.bombColorPreset != 0)
            {
                if (noteMat == null)
                {
                    noteMat = Resources.FindObjectsOfTypeAll<ColorNoteVisuals>().First().GetPrivateField<MaterialPropertyBlockController[]>("_materialPropertyBlockControllers")[0].GetPrivateField<Renderer[]>("_renderers").First(x => x.sharedMaterial != null).sharedMaterial;
                    // testMat = materialPropertyBlockController.GetPrivateField<Renderer[]>("_renderers").First(x => x.sharedMaterial != null).sharedMaterial;
                }
                if (noteMat != null)
                {
                    ____wrapperGO.GetComponent<MeshRenderer>().sharedMaterial = noteMat;
                    ____wrapperGO.GetComponent<MeshRenderer>().sharedMaterial.color = Plugin.bombColor;
                }
            }
        }
    }
    
}
