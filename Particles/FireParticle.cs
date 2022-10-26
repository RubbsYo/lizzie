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
using LizSoundPack.Core.Effects;
using ReLogic.Content;

namespace LizSoundPack.Content.Effects
{
    [Autoload(Side = ModSide.Client)]
    public class FireParticle : DustParticle
    {

        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizSoundPack/Particles/Textures/FireEffect");
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 8;
            int index = Main.rand.Next(frameMax);
            frame = new Rectangle(0, 0, texture.Width, texture.Height/frameMax);
            frame.Y = frame.Height * index;
            origin = origin_center(texture);
            additive = true;
        }

        public override void Update()
        {
            base.Update();
            var lightcol = new Color(128, 53, 0, 16);
            float r = (float)(int)lightcol.R / 255f * scale.X;
            float g = (float)(int)lightcol.G / 255f * scale.X;
            float b = (float)(int)lightcol.B / 255f * scale.X;
            Lighting.AddLight(position, r, g, b);
            if (Main.rand.NextBool(100))
            {
                Instantiate<SmokeParticle>(p =>
                {
                    p.position = position;
                    p.velocity = new Vector2(-2 + Main.rand.NextFloat(4f), 0);
                    p.rotation = Main.rand.NextFloat((float)Math.PI);
                    p.scale *= scale*(0.5f + Main.rand.NextFloat(0.2f));
                    p.gravity.Y = -0.1f;
                    p.rotspd = 0.1f;
                    p.rotspdDecay = 1.01f;
                    p.shrinkspd = 0.02f;

                });
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (texture != null)
            {
                for (var j = 0; j < 12; j++)
                {
                    Vector2 rand = Main.rand.NextVector2Square(-j / 2, j / 2);
                    var col = color;
                    if (Main.rand.Next(100) < 80)
                        col.G = 100;
                    if (Main.rand.Next(100) < 40)
                        col.B = 100;
                    Draw_Sprite(sb, texture, frame, position + rand, origin, scale * (1f + 0.04f * j), rotation+Main.rand.NextFloat((float)Math.PI/16), col,0.08f);
                }
            }
        }
    }
}