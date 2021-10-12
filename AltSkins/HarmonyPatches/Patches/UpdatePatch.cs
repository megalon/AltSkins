using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Nick;
using UnityEngine;
using System.IO;
using BepInEx;
using System.Reflection;
using AltSkins.Data;

namespace AltSkins.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(GameInstance), "DoFrame")]
    class UpdatePatch
    {
        public static void UpdateLogic(ref GameAgent[] ___updagents, ref GameInstance __instance, ref int ___agentsAdded, bool isOnline = false)
        {
            if (AltSkinsPlugin.WaitingForUpdate)
            {
                for (int i = 0; i < ___updagents.Length; i++)
                {
                    if (___updagents[i].GameUniqueIdentifier.StartsWith("char"))
                    {

                        AltSkinsPlugin.WaitingForUpdate = false;
                        GameObject updategent = ___updagents[i].gameObject;

                        var player = PlayerSkinController.players[___updagents[i].playerIndex];
                        if(isOnline) player = PlayerSkinController.players[0];

                        var skinIndex = player.skinIndex;
                        if (skinIndex > 0)
                        {
                            KeyValuePair<string, List<CustomSkin>> skinMap = SkinLoader.skinMaps.Where(e => e.Key.Trim() == player.characterName).FirstOrDefault();
                            if (skinMap.Key != null && skinMap.Value.ToArray()[skinIndex] != null)
                            {
                                var customSkin = skinMap.Value.ToArray()[skinIndex];
                                Console.WriteLine($"Applying {customSkin.Name} to player {___updagents[i].playerIndex}'s {customSkin.CharacterName}");

                                Renderer[] rends = updategent.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<Renderer>(true);

                                var skins = updategent.GetComponent<GameAgentSkins>();
                                var skin = UnityEngine.Object.Instantiate(skins.lastSkinRef);

                                foreach (var textureSwitch in skin.GetFieldValue<SkinData.TextureSwitch[]>("textureSwitches"))
                                {
                                    for (int j = 0; j < textureSwitch.textures.Length; j++)
                                    {

                                        var texture = textureSwitch.textures[j];
                                        var rawTextureBit = skinMap.Key.Substring("char_".Length).ToLower();
                                        if (texture.name.ToLower().Contains(rawTextureBit))
                                        {
                                            textureSwitch.textures[j] = customSkin.Texture;
                                        }
                                    }
                                }

                                skins.ApplySkin(skin, skins.lastColorApplied);

                                foreach (Renderer rend in rends)
                                {
                                    // this will probably cause memory issues
                                    // remove it!
                                    Material mat = rend?.material;
                                    //mat = UnityEngine.Object.Instantiate(mat);
                                    if (mat != null)
                                    {
                                        if (mat.shader?.name == "NickCharacters")
                                        {
                                            if (mat.GetTexture("_MainTex") != null)
                                            {
                                                var rawTextureBit = skinMap.Key.Substring("char_".Length).ToLower();

                                                if (mat.GetTexture("_MainTex").name.ToLower().Contains(rawTextureBit))
                                                {
                                                    mat.SetTexture("_MainTex", customSkin.Texture);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        static void Postfix(ref GameAgent[] ___updagents, ref GameInstance __instance, ref int ___agentsAdded) => UpdateLogic(ref ___updagents, ref __instance, ref ___agentsAdded);
    }

    [HarmonyPatch(typeof(GameInstance), "OnlineDoFrameStoreFXDT")]
    class OnlineUpdatePatch
    {
        static void Postfix(ref GameAgent[] ___updagents, ref GameInstance __instance, ref int ___agentsAdded) => UpdatePatch.UpdateLogic(ref ___updagents, ref __instance, ref ___agentsAdded, true);
    }
}
