using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Nick;
using AltSkins.Data;
using UnityEngine.UI;
using UnityEngine;

namespace AltSkins.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(RenderImage), "Update")]
    class IngameUIPatch
    {
        static void Prefix(ref ResourceTextureMB ___resourceTexture, ref RawImage ___rawImage, ref bool ___seekingTexture)
        {
            if (___resourceTexture.ResourcePath == "")
            {
                ___seekingTexture = false;
                ___rawImage.enabled = true;
            }
        }
    }
}
