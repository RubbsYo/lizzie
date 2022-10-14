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
    public class BulletDestroyParticle : Particle
    {
        public override void Init()
        {
            texture = (Texture2D)ModContent.Request<Texture2D>("LizSoundPack/Particles/BulletDestroyEffect");
            frameMax = 4;
            frameDuration = 3 + Main.rand.Next(2);
            frame = new Rectangle(0, 0, texture.Width / frameMax, texture.Height);
            origin = new(16,16);
            rotation = Main.rand.NextFloat((float)Math.PI);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (texture != null)
            {
                Draw_Sprite(sb, texture, frame, position, origin, scale * 2f, rotation, color, 0.1f);
            }
        }
    }
}