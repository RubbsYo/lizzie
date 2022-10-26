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
using LizSoundPack.Common.TextureColors;
using ReLogic.Content;

namespace LizSoundPack.Content.Effects
{
    [Autoload(Side = ModSide.Client)]
    public class SlashParticle : Particle
    {
        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizSoundPack/Particles/Textures/SlashEffect");
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 7;
            frameDuration = 2 + Main.rand.Next(2);
            frame = new Rectangle(0, 0, texture.Width-1, texture.Height/frameMax);
            origin = origin_center(texture);
            int dir = 1;
            if (Main.rand.NextBool())
            {
                dir = -1;
            }
            scale.X *= 2*dir;
            scale.Y *= (2+Main.rand.NextFloat(0.3f))*dir;
            rotation +=  - Main.rand.NextFloat(MathHelper.Pi / 8) + Main.rand.NextFloat(MathHelper.Pi / 16);
        }

        public override void Draw(SpriteBatch sb)
        {
            //base.Draw(sb);
            if (texture != null)
            {
                for (var j = 0; j < 7; j++)
                {
                    Vector2 rand = Main.rand.NextVector2Square(-j / 4, j / 4);
                    Draw_Sprite(sb, texture, frame, position+rand, origin, scale * (1f + 0.02f * j), rotation, color, 0.08f);
                }
            }
        }
    }
}