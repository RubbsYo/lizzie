using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using System;
using LizSoundPack.Content.Effects;
using LizSoundPack.Core.Effects;
using ReLogic.Content;
using LizSoundPack.Common.TextureColors;

namespace LizSoundPack.Content.Effects
{
    [Autoload(Side = ModSide.Client)]
    public class ShadowMagicParticle : DustParticle
    {

        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizSoundPack/Particles/Textures/ShadowMagicEffect", AssetRequestMode.ImmediateLoad);
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 1;
            int index = Main.rand.Next(frameMax);
            frame = new Rectangle(0, 0, texture.Width, texture.Height/frameMax);
            frame.Y = frame.Height * index;
            origin = origin_center(texture);
            additive = false;
            scale *= 0.5f;
            shrinkspd = 0.02f;
            if (Main.rand.NextBool())
            color = new Color(0, 0, 0f);
        }

        public override void Update()
        {
            base.Update();
            var lightcol = new Color(130, 3, 15, 16);
            float r = (float)(int)lightcol.R / 255f * scale.X;
            float g = (float)(int)lightcol.G / 255f * scale.X;
            float b = (float)(int)lightcol.B / 255f * scale.X;
            Lighting.AddLight(position, r, g, b);
            if (Main.rand.NextBool())
                color = new Color(0, 0, 0f);
            else color = Color.White;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (texture != null)
            {
                for (var j = 0; j < 1; j++)
                {
                    Vector2 rand = Main.rand.NextVector2Square(-j / 2, j / 2);

                    Draw_Sprite(sb, texture, frame, position + rand, origin, scale * (1f + 0.04f * j), rotation+Main.rand.NextFloat((float)Math.PI/16), color ,0.08f);
                }
            }
        }
    }
}