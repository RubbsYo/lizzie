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
    public class PelletDestroyParticle : Particle
    {
        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/PelletDestroyEffect", AssetRequestMode.ImmediateLoad);
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 5;
            frameDuration = 3 + Main.rand.Next(2);
            frame = new Rectangle(0, 0, texture.Width / frameMax, texture.Height);
            origin = new(16,16);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (texture != null)
            {
                Draw_Sprite(sb, texture, frame, position, origin, scale * 1.5f, rotation, color, 0.1f);
            }
        }
    }
}