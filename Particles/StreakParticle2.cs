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

namespace LizSoundPack.Content.Effects
{
    public class StreakParticle2 : Particle
    {
        public override void Init()
        {
            texture = (Texture2D)ModContent.Request<Texture2D>("LizSoundPack/Particles/StreakEffect2");
            frameMax = 5;
            frameDuration = 3 + Main.rand.Next(3);
            frame = new Rectangle(0, 0, texture.Width-1, texture.Height/frameMax);
            origin = new Vector2(-4, 16);
            scale.X *= 2 + Main.rand.NextFloat(0.3f);
            scale.Y *= 2 + Main.rand.NextFloat(0.5f);
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