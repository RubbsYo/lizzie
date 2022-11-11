using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using System;
using LizOverhaul.Content.Effects;
using LizOverhaul.Core.Effects;
using ReLogic.Content;
using LizOverhaul.Common.TextureColors;

namespace LizOverhaul.Content.Effects
{
    [Autoload(Side = ModSide.Client)]
    public class WaterMagicParticle : DustParticle
    {

        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/WaterMagicEffect", AssetRequestMode.ImmediateLoad);
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 1;
            int index = Main.rand.Next(frameMax);
            frame = new Rectangle(0, 0, texture.Width, texture.Height / frameMax);
            frame.Y = frame.Height * index;
            origin = origin_center(texture);
            additive = true;
            color = Color.Blue;
            if (Main.rand.NextBool(3))
                color = new Color(0.005f, 0.005f, 1);
            scale *= Main.rand.NextFloat(0.4f,0.6f);
            rotspd *= 4;
            shrinkspd = 0;
            alphaFadeTimer = 1;
            alphaFadeSpeed = 0.03f;
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (texture != null)
            {
                for (var j = 0; j < 1; j++)
                {
                    Vector2 rand = Main.rand.NextVector2Square(-j / 2, j / 2);
                    Draw_Sprite(sb, texture, frame, position + rand, origin, scale * (1f + 0.04f * j), rotation + Main.rand.NextFloat((float)Math.PI / 16), color, 0.4f);
                }
            }
        }
    }
}