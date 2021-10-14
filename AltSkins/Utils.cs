using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AltSkins.Data;
using UnityEngine;
using System.IO.Compression;
using System.IO;

namespace AltSkins
{
    public static class Utils
    {
        public static T GetFieldValue<T>(this object obj, string name)
        {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }
        public static void SetFieldValue<T>(this T @this, string fieldName, object value)
        {
            Type type = @this.GetType();
            FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            field.SetValue(@this, value);
        }

        public static (CustomSkinTexture2D[] textures, CustomSkinTexture2D[] portraits, SkinFormat json) TexturesAndJSONFromPackage(string path)
        {
            SkinFormat json = null;
            List<CustomSkinTexture2D> textures = new List<CustomSkinTexture2D>();
            List<CustomSkinTexture2D> portraits = new List<CustomSkinTexture2D>();

            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                var jsonEntry = archive.Entries.First(i => i.Name == "package.json");
                if (jsonEntry != null)
                {
                    var stream = new StreamReader(jsonEntry.Open(), Encoding.Default);
                    string jsonString = stream.ReadToEnd();
                    json = JsonConvert.DeserializeObject<SkinFormat>(jsonString);
                }
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (json != null)
                    {
                        foreach (var textureReplacement in json.textureReplacements)
                        {
                            if (entry.Name == textureReplacement.Value)
                            {
                                //here the file
                                var SeekableStream = new MemoryStream();
                                entry.Open().CopyTo(SeekableStream);
                                SeekableStream.Position = 0;

                                Texture2D Texture = new Texture2D(2048, 2048);
                                Texture.LoadImage(SeekableStream.ToArray());
                                textures.Add(new CustomSkinTexture2D(Texture, textureReplacement.Key));
                            }
                        }
                        foreach (var textureReplacement in json.portraitReplacements)
                        {
                            // using functions to get rid of repetitive code? never heard of her
                            if (entry.Name == textureReplacement.Value)
                            {
                                //here the file
                                var SeekableStream = new MemoryStream();
                                entry.Open().CopyTo(SeekableStream);
                                SeekableStream.Position = 0;

                                Texture2D Texture = new Texture2D(2048, 2048);
                                Texture.LoadImage(SeekableStream.ToArray());
                                portraits.Add(new CustomSkinTexture2D(Texture, textureReplacement.Key));
                            }
                        }
                    }
                }
            }
            return (textures.ToArray(), portraits.ToArray(), json);
        }
    }
}
