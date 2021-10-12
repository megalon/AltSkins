using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltSkins
{
    public static class PlayerSkinController
    {
        public static PlayerSkin[] players = new PlayerSkin[4] { new PlayerSkin(), new PlayerSkin(), new PlayerSkin(), new PlayerSkin() };
    }

    public class PlayerSkin
    {
        public int skinIndex = 0;
        public string characterName = "";
    }
}
