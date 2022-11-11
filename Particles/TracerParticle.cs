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
    public class TracerParticle : Particle
    {
        float shrinkspd = 0.8f;

        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/TracerEffect", AssetRequestMode.ImmediateLoad);
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 1;
            frameDuration = 999;
            frame = new Rectangle(0, 0, texture.Width, texture.Height/frameMax);
            origin = new(0,16);
            scale.X /= texture.Width;
        }

        public override void Update()
        {
            base.Update();
            scale.Y *= shrinkspd;
            if (scale.Y <= 0.01f)
                Destroy();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (texture != null)
            {
                Draw_Sprite(sb, texture, frame, position, origin, new(scale.X, scale.Y * 2f), rotation, color, 0.2f);
            }
        }
    }
}