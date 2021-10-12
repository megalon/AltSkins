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
            Console.WriteLine("Init Prepare instance");
            AltSkinsPlugin.WaitingForUpdate = true;

            //Console.WriteLine(screen);
            /*if (screen == "mainmenu")
            {
                MenuModifier.WaitingForUpdate = true;
                // gaming?
                //UITestingNew.UITesting.AwaitScreenState(___screenStack.Last());
                //UITestingNew.UITesting.modifier.PassScreenState(___screenStack.Last());
                //Console.WriteLine(GameObject.Find(screen).layer);
            }*/
        }
    }

    [HarmonyPatch(typeof(OnlineWaitMatchScreen), "GameStarted")]
    class OnlineGameInstancePatch
    {
        static void Postfix()
        {
            Console.WriteLine("Init Prepare instance");
            AltSkinsPlugin.WaitingForUpdate = true;

            //Console.WriteLine(screen);
            /*if (screen == "mainmenu")
            {
                MenuModifier.WaitingForUpdate = true;
                // gaming?
                //UITestingNew.UITesting.AwaitScreenState(___screenStack.Last());
                //UITestingNew.UITesting.modifier.PassScreenState(___screenStack.Last());
                //Console.WriteLine(GameObject.Find(screen).layer);
            }*/
        }
    }
}
