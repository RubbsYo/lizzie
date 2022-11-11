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
    public class GuardParticle : Particle
    {
        private static Texture2D tex = (Texture2D)ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/GuardEffect", AssetRequestMode.ImmediateLoad);
        public override void Init()
        {
            texture = tex;
            frameMax = 10;
            frameDuration = 3 + Main.rand.Next(2);
            frame = new Rectangle(0, 0, texture.Width-1, texture.Height/frameMax);
            origin = origin_center(texture);
            scale.X *= 2;
            scale.Y *= 2;
            rotation = Main.rand.NextFloat((float)Math.PI);
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