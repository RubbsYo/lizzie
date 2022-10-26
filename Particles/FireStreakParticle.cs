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
using ReLogic.Content;

namespace LizSoundPack.Content.Effects
{
    [Autoload(Side = ModSide.Client)]
    public class FireStreakParticle : Particle
    {
        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizSoundPack/Particles/Textures/FireStreakEffect");

        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 6;
            frameDuration = 3 + Main.rand.Next(2);
            frame = new Rectangle(0, 0, texture.Width - 1, texture.Height / frameMax);
            origin = new Vector2(-4, 16);
            scale.X *= 2 + Main.rand.NextFloat(0.3f);
            scale.Y *= 2 + Main.rand.NextFloat(0.8f);
        }

        public override void Draw(SpriteBatch sb)
        {
            //base.Draw(sb);
            if (texture != null)
            {
                for (var j = 0; j < 12; j++)
                {
                    Vector2 rand = Main.rand.NextVector2Square(-j / 2, j / 2);
                    Draw_Sprite(sb, texture, frame, position+rand, origin, scale * (1f + 0.03f * j), rotation, color, 0.4f);
                }
            }
        }
    }
}