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
using ReLogic.Content;

namespace LizOverhaul.Content.Effects
{
    [Autoload(Side = ModSide.Client)]
    public class HitParticleHeavy : Particle
    {
        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/HitEffectHeavy", AssetRequestMode.ImmediateLoad);

        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 4;
            frameDuration = 3 + Main.rand.Next(2);
            frame = new Rectangle(0, 0, texture.Width, texture.Height/frameMax);
            origin = origin_center(texture);
            rotation = Main.rand.NextFloat((float)Math.PI);
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