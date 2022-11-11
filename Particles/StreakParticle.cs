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
    public class StreakParticle : Particle
    {
        public bool alt;

        private static Asset<Texture2D> tex1 = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/StreakEffect", AssetRequestMode.ImmediateLoad);
        private static Asset<Texture2D> tex2 = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/StreakEffect2", AssetRequestMode.ImmediateLoad);

        public override void Init()
        {
            if (!alt)
            {
                texture = (Texture2D)tex1;
                frameMax = 7;
                frameDuration = 2 + Main.rand.Next(2);
                frame = new Rectangle(0, 0, texture.Width - 1, texture.Height / frameMax);
                origin = new Vector2(-4, 16);
                scale.X *= 2 + Main.rand.NextFloat(0.3f);
                scale.Y *= 2 + Main.rand.NextFloat(0.3f);
            } else
            {
                texture = (Texture2D)tex2;
                frameMax = 5;
                frameDuration = 2 + Main.rand.Next(2);
                frame = new Rectangle(0, 0, texture.Width - 1, texture.Height / frameMax);
                origin = new Vector2(-4, 16);
                scale.X *= 2 + Main.rand.NextFloat(0.5f);
                scale.Y *= 2 + Main.rand.NextFloat(0.7f);
                velocity = new Vector2(3 + Main.rand.NextFloat(3f), 0);
                velocity = velocity.RotatedBy(rotation);
                friction = velocity;
                friction.Normalize();
                friction *= 0.2f;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (texture != null && additive)
            {
                for (var j = 0; j < 7; j++)
                {
                    Vector2 rand = Main.rand.NextVector2Square(-j / 4, j / 4);
                    Draw_Sprite(sb, texture, frame, position+rand, origin, scale * (1f + 0.03f * j), rotation, color, 0.08f);
                }
            }
        }
    }
}