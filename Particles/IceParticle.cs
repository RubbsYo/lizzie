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
    public class IceParticle : DustParticle
    {

        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/IceEffect", AssetRequestMode.ImmediateLoad);
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 4;
            int index = Main.rand.Next(frameMax);
            frame = new Rectangle(0, 0, texture.Width, texture.Height / frameMax);
            frame.Y = frame.Height * index;
            origin = origin_center(texture);
            additive = false;
            color = new Color(16, 64, 255);
            shrinkspd = 0;
            shatter = true;
            alphaFadeSpeed = 1;
        }
    }
}