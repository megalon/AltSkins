using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx;
using UnityEngine;
using AltSkins.Data;
using Nick;

namespace AltSkins
{
    public static class SkinLoader
    {
        public static string folder = Path.Combine(Paths.BepInExRootPath, "Skins");

        public static Dictionary<string, List<CustomSkin>> skinMaps = new Dictionary<string, List<CustomSkin>>();

        public static void LoadSkins()
        {
            var gameMeta = Resources.FindObjectsOfTypeAll<GameMetaData>().FirstOrDefault();

            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string[] files = Directory.GetFiles(folder, "*.nasbskin", SearchOption.AllDirectories);

            int loadedSkinCount = 0;
            foreach (var file in files)
            {
                string loggedName = file.Substring(file.IndexOf("Skins"));
                try
                {
                    AltSkinsPlugin.LogInfo($"Loading skin: {loggedName}");
                    CustomSkin customSkin = new CustomSkin(file);

                    CharacterMetaData charMeta = gameMeta.characterMetas.FirstOrDefault(x => x.id.Equals(customSkin.CharacterName));

                    if (charMeta == null)
                    {
                        AltSkinsPlugin.LogError($"Could not find character \"{customSkin.CharacterName}\" for skin \"{customSkin.Name}\"");
                        continue;
                    }

                    if (!skinMaps.ContainsKey(customSkin.CharacterName)) skinMaps.Add(customSkin.CharacterName, new List<CustomSkin>() { new CustomSkin("") });
                    skinMaps[customSkin.CharacterName].Add(customSkin);

                    string skinId = $"CUSTOM_{customSkin.CharacterName}_{customSkin.Name}";

                    #region CharMetaData
                    CharacterMetaData.CharacterSkinMetaData skinMetaData = new CharacterMetaData.CharacterSkinMetaData()
                    {
                        id = skinId,
                        locNames = new string[]
                        {
                            customSkin.CharacterName
                        },
                        resPortraits = new string[]
                        {
                            // Maybe use some custom identifier then intercept that when loading the image?
                            string.Empty
                        },
                        resMediumPortraits = new string[]
                        {
                            string.Empty
                        },
                        resMiniPortraits = new string[]
                        {
                            string.Empty
                        },
                        unlockSkin = string.Empty,
                        unlockedByUnlockIds = new string[]
                        {
                            string.Empty
                        }
                    };

                    CharacterMetaData.CharacterSkinMetaData[] skins = new CharacterMetaData.CharacterSkinMetaData[charMeta.skins.Length + 1];
                    charMeta.skins.CopyTo(skins, 0);
                    skins[skins.Length - 1] = skinMetaData;
                    charMeta.skins = skins;
                    #endregion CharMetaData


                    #region LoadedSkin
                    LoadedSkin loadedSkin = new LoadedSkin();

                    ScriptableObject skinDataSO = ScriptableObject.CreateInstance("SkinData");
                    SkinData skinData = (SkinData)skinDataSO;
                    skinData.skinid = skinId;

                    // Populate the SkinData here I guess

                    loadedSkin.skinId = skinId;
                    loadedSkin.skin = skinData;
                    #endregion LoadedSkin


                    AltSkinsPlugin.LogInfo($"Succesfully loaded {customSkin.Name} skin for {customSkin.CharacterName}.");
                    loadedSkinCount++;
                }
                catch (Exception e)
                {
                    AltSkinsPlugin.LogWarning($"Failed to load skin: {loggedName}");
                    AltSkinsPlugin.LogWarning(e.ToString());
                }
                //List<CustomSkin> skins = new List<CustomSkin>();
                //skins.Add(new CustomSkin(""));
            }
            AltSkinsPlugin.LogInfo($"Loaded {loadedSkinCount} skin{(loadedSkinCount == 1 ? "" : "s")} for {skinMaps.Count} character{(skinMaps.Count == 1 ? "" : "s")}");


            // Test loaded skins
            /*
            LoadedSkin tempLoadedSkin = new LoadedSkin();
            Dictionary<string, LoadedSkin> loadedSkins = tempLoadedSkin.GetFieldValue<Dictionary<string, LoadedSkin>>("loadedSkins");

            foreach (string key in loadedSkins.Keys)
            {
                AltSkinsPlugin.LogInfo(key);
            }
            */
        }
    }
}
