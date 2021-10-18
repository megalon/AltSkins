using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Nick;
using AltSkins.Data;
using UnityEngine;
using UnityEngine.UI;

namespace AltSkins.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(MenuSystem), "SwitchToScreen")]
    class MenuUIPatch
    {
        public static bool WaitingForUpdate = false;
        static void Postfix(string screen, ref List<MenuSystem.ScreenState> ___screenStack)
        {
            if (screen == "loading" || screen == "onlinematchwait")
            {
                WaitingForUpdate = true;
            }
        }
    }

    [HarmonyPatch(typeof(MenuSystem), "Update")]
    class MenuUIPatchUpdate
    {
        static void Postfix(ref List<MenuSystem.ScreenState> ___screenStack)
        {
            if (MenuUIPatch.WaitingForUpdate)
            {
                var lastStack = ___screenStack.Last();
                if ((lastStack.id == "loading" || lastStack.id == "onlinematchwait") && lastStack.instance != null)
                {
                    MenuUIPatch.WaitingForUpdate = false;
                    var MenuInstance = lastStack.instance;
                    var allVisualizers = MenuInstance.gameObject.GetComponentsInChildren<RenderVisualizer>(false);
                    foreach (var visualizer in allVisualizers) {
                        if (visualizer.transform.parent.name.Contains("VrsPlayer"))
                        {
                            int playerNumber = Int32.Parse(visualizer.transform.parent.name.Last().ToString()) - 1;
                            var skinIndex = PlayerSkinController.players[playerNumber].skinIndex;

                            if(skinIndex > 0)
                            {
                                KeyValuePair<string, List<CustomSkin>> skinMap = SkinLoader.skinMaps.Where(e => e.Key.Trim() == PlayerSkinController.players[playerNumber].characterName).FirstOrDefault();

                                if (skinMap.Key != null && skinMap.Value.Count > 1)
                                {
                                    var skin = skinMap.Value.ToArray()[skinIndex];
                                    if (skin != null && skin.Portraits.Any(e => e.Name == "portrait"))
                                    {
                                        RenderImage renderImage = visualizer.GetComponentInChildren<RenderImage>(false);
                                        Image image = renderImage.GetComponentsInChildren<Image>(true).Where(e => e.name == "Image").First();
                                        Image newImage = SlotStatePatchSelectCharacter.GetCustomImage(renderImage);

                                        newImage.gameObject.GetComponent<Image>().sprite = Sprite.Create(skin.Portraits.Where(e => e.Name == "portrait").First().Texture2D, image.sprite.rect, image.sprite.pivot);
                                        image.gameObject.SetActive(false);
                                        newImage.gameObject.SetActive(true);
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
