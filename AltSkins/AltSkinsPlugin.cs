using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using BepInEx;
using UnityEngine.SceneManagement;
using UnityEngine;
using AltSkins.HarmonyPatches;
using System.Collections;
using Nick;

namespace AltSkins
{
    [BepInPlugin("legoandmars-altskins", "AltSkins", "1.6.0")]
    public class AltSkinsPlugin : BaseUnityPlugin
    {
        internal static AltSkinsPlugin Instance;
        public static bool WaitingForUpdate = false;
        private void Awake()
        {
            if (Instance) DestroyImmediate(this);
            Instance = this;

            Logger.LogInfo($"AltSkins is loaded!");
            SkinLoader.LoadSkins();
            AltSkinsPatches.ApplyHarmonyPatches();
        }

        #region logging
        internal static void LogDebug(string message) => Instance.Log(message, LogLevel.Debug);
        internal static void LogInfo(string message) => Instance.Log(message, LogLevel.Info);
        internal static void LogWarning(string message) => Instance.Log(message, LogLevel.Warning);
        internal static void LogError(string message) => Instance.Log(message, LogLevel.Error);
        private void Log(string message, LogLevel logLevel) => Logger.Log(logLevel, message);
        #endregion
    }
}
