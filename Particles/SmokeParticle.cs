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
    public class SmokeParticle : DustParticle
    {
        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/SmokeEffect", AssetRequestMode.ImmediateLoad);
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 5;
            int index = Main.rand.Next(frameMax);
            frame = new Rectangle(0, 0, texture.Width, texture.Height / frameMax);
            frame.Y = frame.Height * index;
            origin = origin_center(texture);
            additive = false;
        }
    }
}