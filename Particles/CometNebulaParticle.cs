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
    public class CometNebulaParticle : DustParticle
    {
        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/CometNebulaEffect", AssetRequestMode.ImmediateLoad);
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 1;
            int index = Main.rand.Next(frameMax);
            frame = new Rectangle(0, 0, texture.Width, texture.Height / frameMax);
            frame.Y = frame.Height * index;
            origin = origin_center(texture);
            additive = false;
            if (Main.rand.NextBool())
            {
                color = new Color(0, 1, 1);
                additive = true;
            } else
            {
                //additive = false;
            }
            scale *= 0.7f;
            alpha = 0.2f;
        }

        public override void Update()
        {
            base.Update();
            if (LifeTime < 8)
                alpha = Math.Min(alpha+0.07f,0.25f);
            else
            {
                alpha = Math.Max(alpha - 0.02f, 0);
            }
        }
    }
}