using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Nick;
using BepInEx;
using UnityEngine;

namespace AltSkins.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(GameInstance), "PrepareInstance")]
    class GameInstancePatch
    {
        static void Postfix()
        {
            AltSkinsPlugin.WaitingForUpdate = true;
        }
    }

    [HarmonyPatch(typeof(OnlineWaitMatchScreen), "GameStarted")]
    class OnlineGameInstancePatch
    {
        static void Postfix()
        {
            AltSkinsPlugin.WaitingForUpdate = true;
        }
    }
}
