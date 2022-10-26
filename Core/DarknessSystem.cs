using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria;

namespace LizSoundPack.Common.Darkness;

[Autoload(Side = ModSide.Client)]

public class DarknessSystem : ModSystem
{
    public static Texture2D darknessTexture = new Texture2D(Main.instance.GraphicsDevice, Main.screenWidth * 2, Main.screenHeight * 2);
    public static Color darknessColor = new Color(0, 0, 0);
    public static Vector2 offsetFromScreen = new Vector2(-darknessTexture.Width / 4, -darknessTexture.Height / 4);

    private static void UpdateDarkness()
    {
        var data = new Color[darknessTexture.Width * darknessTexture.Height];

        darknessTexture.GetData(data);

        for (int i = 0; i < data.Length; i++)
        {
            int currentX = i % darknessTexture.Width;
            int currentY = i / darknessTexture.Height;

            Vector2 positionInTexture = new(currentX, currentY);
            Vector2 positionOnScreen = positionInTexture - offsetFromScreen;

            data[i].R = darknessColor.R;
            data[i].G = darknessColor.G;
            data[i].B = darknessColor.B;
            data[i].A += 8;
            if (positionOnScreen.Distance(Main.MouseScreen) < 64)
            {
                data[i].A = 0;
            }

        }

        darknessTexture.SetData(data);
    }

    public override void PostDrawTiles()
    {
        UpdateDarkness();
        SpriteBatch sb = Main.spriteBatch;
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
        sb.Draw(darknessTexture, offsetFromScreen, Color.White);
        sb.End();
    }
}