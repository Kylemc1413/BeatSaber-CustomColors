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
    class ColorNoteVisualsPatch
    {
        public static void Postfix(NoteController noteController, ref MeshRenderer ____arrowMeshRenderer, ref SpriteRenderer ____arrowGlowSpriteRenderer, ref SpriteRenderer ____circleGlowSpriteRenderer, ref float ____arrowGlowIntensity, ref MaterialPropertyBlockController[] ____materialPropertyBlockControllers, ref int ____colorID)
        {
            if (Plugin.disablePlugin)
                return;
            if (noteController.noteData.noteType == NoteType.NoteA)
            {
                if (Plugin.leftArrowGlowPreset != 0)
                {
                    ____arrowGlowSpriteRenderer.color = Plugin.LeftArrowGlowColor;//.ColorWithAlpha(____arrowGlowIntensity);
                    ____circleGlowSpriteRenderer.color = Plugin.LeftArrowGlowColor;//.ColorWithAlpha(____arrowGlowIntensity);
                }

                if (Plugin.leftArrowPreset != 0)
                {
                    ____arrowMeshRenderer.material.color = Plugin.LeftArrowColor;//.ColorWithAlpha(____arrowGlowIntensity);

                    if (noteController.noteData.cutDirection == NoteCutDirection.Any)
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
                    ____arrowGlowSpriteRenderer.color = Plugin.RightArrowGlowColor;//.ColorWithAlpha(____arrowGlowIntensity);
                    ____circleGlowSpriteRenderer.color = Plugin.RightArrowGlowColor;//.ColorWithAlpha(____arrowGlowIntensity);
                }
                if (Plugin.rightArrowPreset != 0)
                {
                    ____arrowMeshRenderer.material.color = Plugin.RightArrowColor;//.ColorWithAlpha(____arrowGlowIntensity);

                    if (noteController.noteData.cutDirection == NoteCutDirection.Any)
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
}
