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
using LizOverhaul.Core.Effects;
using ReLogic.Content;
using LizOverhaul.Common.TextureColors;

namespace LizOverhaul.Content.Effects
{
    [Autoload(Side = ModSide.Client)]
    public class CometTrailParticle : DustParticle
    {

        private static Asset<Texture2D> tex = ModContent.Request<Texture2D>("LizOverhaul/Particles/Textures/CometTrailEffect", AssetRequestMode.ImmediateLoad);
        public Projectile owner;
        public override void Init()
        {
            texture = (Texture2D)tex;
            frameMax = 1;
            int index = Main.rand.Next(frameMax);
            frame = new Rectangle(0, 0, texture.Width, texture.Height / frameMax);
            frame.Y = frame.Height * index;
            origin = origin_center(texture);
            additive = true;
            rotspd *= 4;
            alphaFadeTimer = 1;
            alphaFadeSpeed = 0.03f;
            alpha *= 0.5f;
            color = new Color(1, 1, 0);
        }

        public override void Update()
        {
            base.Update();
            //rotation = position.AngleTo(owner.position);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            if (texture != null)
            {
                for (var j = 0; j < 1; j++)
                {
                    Vector2 rand = Main.rand.NextVector2Square(-j / 2, j / 2);
                    Draw_Sprite(sb, texture, frame, position + rand, origin, scale * (1f + 0.04f * j), rotation + Main.rand.NextFloat((float)Math.PI / 16), color, 0.4f);
                }
            }
        }
    }
}