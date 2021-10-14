using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace AltSkins.Data
{
    public class CustomSkin
    {
        public string Name;

        public string CharacterName;

        public CustomSkinTexture2D[] Textures;

        public CustomSkinTexture2D[] Portraits;

        public SkinFormat Json;

        public CustomSkin(string file)
        {
            if (!String.IsNullOrEmpty(file))
            {
                var texturesAndJson = Utils.TexturesAndJSONFromPackage(file);

                Json = texturesAndJson.json;
                Textures = texturesAndJson.textures;
                Portraits = texturesAndJson.portraits;

                Name = Json.skinName;

                CharacterName = Json.characterName;
                //skinMaps.Add(new DirectoryInfo(skinFolder).Name, tex);
            }
            else Name = "Default";
            //skinMaps.Add()
        }
    }

    public class CustomSkinTexture2D
    {
        public Texture2D Texture2D;
        public string Name;

        public CustomSkinTexture2D(Texture2D texture, string name)
        {
            Texture2D = texture;
            Name = name;
        }
    }
}
