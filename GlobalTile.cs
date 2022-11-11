using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Drawing;
using Terraria.GameContent;
using Terraria.DataStructures;

namespace LizOverhaul
{
    public class GlobalTiles : GlobalTile
    {

        public byte[,] additionalInfo = new byte[Main.tile.Width,Main.tile.Height];

        public override void RightClick(int i, int j, int type)
        {
            additionalInfo[i, j] ^= 1;
        }

        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (additionalInfo[i,j] == 1)
            {
                drawData.finalColor = Color.Multiply(drawData.finalColor, 0.5f);
            }
        }
    }
}
