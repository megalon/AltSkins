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

        public Texture2D Texture;

        public CustomSkin(string file)
        {
            if (file != null && file.ToLower().Contains("char"))
            {
                Texture = new Texture2D(2048, 2048);
                Texture.LoadImage(File.ReadAllBytes(file));

                Name = Path.GetFileNameWithoutExtension(file);

                CharacterName = new DirectoryInfo(file).Name;
                //skinMaps.Add(new DirectoryInfo(skinFolder).Name, tex);
            }
            else Name = "Default";
            //skinMaps.Add()
        }
    }
}
