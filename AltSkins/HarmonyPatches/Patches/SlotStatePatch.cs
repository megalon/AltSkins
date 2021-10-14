using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Nick;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using BepInEx;
using System.Reflection;
using TMPro;
using AltSkins.Data;

namespace AltSkins.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(PlayerSlotContainer), "Clear")]
    class SlotStatePatch
    {
        /*static bool Prefix(PlayerSlotContainer.State state, CursorMovement clicker, ref PlayerSlotContainer ___playerSlot)
        {
            Console.WriteLine(state);
            Console.WriteLine(___playerSlot.state);
            if(___playerSlot.state == PlayerSlotContainer.State.PlayerSlot && state == PlayerSlotContainer.State.NoneSlot)
            {
                // change the funny skin
                ___playerSlot.SetSkin(3);
                return false;
            }
            return true;
        }*/
        static void Postfix(ref ButtonImage ___skinsButtonImage, ref PlayerSlotContainer __instance)
        {
            //Console.WriteLine(mode);
            if (___skinsButtonImage.transform.parent.gameObject.activeSelf) ___skinsButtonImage.transform.parent.gameObject.SetActive(false);
            SlotStatePatchSelectCharacter.GetSkinName(__instance).gameObject.SetActive(false);

            //if (___skinsButtonImage.transform.parent.gameObject.activeSelf) ___skinsButtonImage.transform.parent.gameObject.SetActive(true);
        }
    }

    [HarmonyPatch(typeof(PlayerSlotContainer), "SelectCharacter")]
    class SlotStatePatchSelectCharacter
    {
        public static GameObject dupeText(GameObject baseText, Vector2 position)
        {
            // literally horrible what the fuck find a better solution
            var newText = UnityEngine.Object.Instantiate(baseText, baseText.transform.parent);
            newText.transform.SetSiblingIndex(0);
            newText.GetComponent<TextMeshProUGUI>().color = Color.black;
            newText.GetComponent<RectTransform>().offsetMin = new Vector2(newText.GetComponent<RectTransform>().offsetMin.x + position.x, newText.GetComponent<RectTransform>().offsetMin.y + position.y);

            return newText;
        }

        public static Transform GetSkinName(PlayerSlotContainer __instance)
        {
            var slotName = __instance.gameObject.transform.Find("SlotName");
            var skinname = __instance.gameObject.transform.Find("SkinName");
            if (skinname == null)
            {
                __instance.gameObject.AddComponent<PlayerSlotSkinData>();
                skinname = UnityEngine.Object.Instantiate(slotName, slotName.parent);
                //skinname.parent = __instance.gameObject.transform.Find("SlotName").parent;
                skinname.name = "SkinName";
                var rect = skinname.GetComponent<RectTransform>();
                rect.offsetMin = new Vector2(rect.offsetMin.x, 0);

                skinname.transform.Find("ReadyImage").gameObject.SetActive(false);

                //skinname.GetChild(0).GetComponent<TextMeshProUGUI>().SetText("pog");
                float thickness = 5f;
                dupeText(skinname.GetChild(0).gameObject, new Vector2(0, thickness));
                dupeText(skinname.GetChild(0).gameObject, new Vector2(0, -thickness));
                dupeText(skinname.GetChild(0).gameObject, new Vector2(thickness, 0));
                dupeText(skinname.GetChild(0).gameObject, new Vector2(-thickness, 0));
            }

            foreach (var text in skinname.GetComponentsInChildren<TextMeshProUGUI>()) text.SetText("Default");

            return skinname;
        }

        public static Image GetCustomImage(RenderImage renderImage)
        {
            Image image = renderImage.GetComponentsInChildren<Image>(true).Where(e => e.name == "Image").First();
            Image newImage;

            if (renderImage.transform.childCount < 2)
            {
                var tempNewImage = UnityEngine.Object.Instantiate(image, renderImage.transform).transform;
                tempNewImage.name = "CustomImage";
                tempNewImage.gameObject.SetActive(false);
                newImage = tempNewImage.GetComponent<Image>();
            }
            else newImage = renderImage.GetComponentsInChildren<Image>(true).Where(e => e.name == "CustomImage").First();

            return newImage;
        }


        static void Postfix(ref ButtonImage ___skinsButtonImage, ref PlayerSlotContainer __instance, ref RenderVisualizer ___characterRenderVisualizer)
        {
            Console.WriteLine(___skinsButtonImage.transform.parent.gameObject.name);
            //Console.WriteLine(mode);
            if (!___skinsButtonImage.transform.parent.gameObject.activeSelf) ___skinsButtonImage.transform.parent.gameObject.SetActive(true);
            GetSkinName(__instance).gameObject.SetActive(true);
            __instance.GetComponent<PlayerSlotSkinData>().skinIndex = 0;

            PlayerSkinController.players[__instance.playerSlotIndex].characterName = __instance.Character.id;
            PlayerSkinController.players[__instance.playerSlotIndex].skinIndex = 0;
        }
    }

    [HarmonyPatch(typeof(PlayerSlotContainer), "Update")]
    class SlotStatePatch2
    {
        static void Postfix(ref PlayerSlotContainer __instance, ref RenderVisualizer ___characterRenderVisualizer)
        {

            if (__instance.playerCursor.menuInput.IsButtonPress(MenuAction.ActionButton.Opt3))
            {
                string characterName = __instance.Character.id;

                KeyValuePair<string, List<CustomSkin>> skinMap = SkinLoader.skinMaps.Where(e => e.Key.Trim() == characterName).FirstOrDefault();

                if(skinMap.Key != null && skinMap.Value.Count > 1)
                {
                    // do the funny thing
                    var skinName = SlotStatePatchSelectCharacter.GetSkinName(__instance);

                    var skinIndex = PlayerSkinController.players[__instance.playerSlotIndex].skinIndex;

                    if (skinIndex == -1 || skinIndex + 1 >= skinMap.Value.Count) skinIndex = 0;
                    else skinIndex++;

                    PlayerSkinController.players[__instance.playerSlotIndex].skinIndex = skinIndex;

                    var skin = skinMap.Value.ToArray()[skinIndex];
                    foreach (var text in skinName.GetComponentsInChildren<TextMeshProUGUI>()) text.SetText(skin.Name);
                    try
                    {
                        // bad
                        RenderImage renderImage = ___characterRenderVisualizer.GetComponentInChildren<RenderImage>(false);
                        Image image = renderImage.GetComponentsInChildren<Image>(true).Where(e => e.name == "Image").First();
                        Image newImage = SlotStatePatchSelectCharacter.GetCustomImage(renderImage);

                        if (skinIndex == 0)
                        {
                            image.gameObject.SetActive(true);
                            newImage.gameObject.SetActive(false);
                        }
                        else
                        {
                            if (skin.Portraits.Any(e => e.Name == "portrait_medium"))
                            {
                                Console.WriteLine("portrait");
                                newImage.gameObject.GetComponent<Image>().sprite = Sprite.Create(skin.Portraits.Where(e => e.Name == "portrait_medium").First().Texture2D, image.sprite.rect, image.sprite.pivot);
                                image.gameObject.SetActive(false);
                                newImage.gameObject.SetActive(true);
                            }
                        }
                    }catch(Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine(e.StackTrace);
                    }
                    //___characterRenderVisualizer.transform.Find()

                    }
                    /*string[] listOfNames = new string[] { "Default", "Blue", "Cool Alt Skin", "Red", "MLG Gamer Edition" };

                    var skinName = SlotStatePatchSelectCharacter.GetSkinName(__instance);

                    var skinIndex = __instance.GetComponent<PlayerSlotSkinData>().skinIndex;

                    if (skinIndex == -1 || skinIndex + 1 == listOfNames.Length) skinIndex = 0;
                    else skinIndex = skinIndex + 1;

                    Console.WriteLine("OH NO");

                    foreach (var text in skinName.GetComponentsInChildren<TextMeshProUGUI>()) text.SetText(listOfNames[skinIndex]);

                    __instance.GetComponent<PlayerSlotSkinData>().skinIndex = skinIndex;*/

                    // skin press
                }
            /*if (__instance.playerCursor.menuInput.IsButtonPress(MenuAction.ActionButton.Opt0)) Console.WriteLine("opt0");
            if (__instance.playerCursor.menuInput.IsButtonPress(MenuAction.ActionButton.Opt1)) Console.WriteLine("opt1");
            if (__instance.playerCursor.menuInput.IsButtonPress(MenuAction.ActionButton.Opt2)) Console.WriteLine("opt2");
            if () Console.WriteLine("opt3");*/
        }
    }
}
