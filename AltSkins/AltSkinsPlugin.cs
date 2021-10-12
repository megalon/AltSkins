using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

using BepInEx;
using UnityEngine.SceneManagement;
using UnityEngine;
using AltSkins.HarmonyPatches;
using System.Collections;
using Nick;

namespace AltSkins
{
    [BepInPlugin("legoandmars-altskins", "AltSkins", "1.2.0")]
    public class AltSkinsPlugin : BaseUnityPlugin
    {
        public static bool WaitingForUpdate = false;
        private void Awake()
        {
            //modifier = new GameObject("MenuModifier").AddComponent<MenuModifier>();
            //DontDestroyOnLoad(modifier.gameObject);

            Logger.LogInfo($"Plugin is loaded!");
            SkinLoader.LoadSkins();
            AltSkinsPatches.ApplyHarmonyPatches();
            // Plugin startup logic
            //Logger.LogInfo(UnityEngine.GameObject.Find("mainmenu").name);
            //SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }


    }
}
