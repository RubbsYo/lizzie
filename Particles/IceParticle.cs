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
    public class IceParticle : DustParticle
    {
        public override void Init()
        {
            texture = (Texture2D)ModContent.Request<Texture2D>("LizSoundPack/Particles/IceEffect");
            frameMax = 4;
            int index = Main.rand.Next(frameMax);
            frame = new Rectangle(0, 0, texture.Width, texture.Height / frameMax);
            frame.Y = frame.Height * index;
            origin = origin_center(texture);
            additive = false;
            color = new Color(16, 64, 255);
            shrinkspd = 0;
        }
    }
}