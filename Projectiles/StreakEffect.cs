using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using System;

namespace LizSoundPack.Projectiles
{
    public class StreakEffect : ModProjectile
    {
        private int duration;
        Color col = Color.White;

        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {

            duration = 3+Main.rand.Next(2);
            int dir = 1;
            if (Main.rand.NextBool())
            {
                dir = -1;
            }
            Projectile.scale = (1.2f + Main.rand.NextFloat(0.3f));
            Projectile.rotation += Main.rand.NextFloat((float)Math.PI / 16) * dir;
            Projectile.alpha = 64;
            if (Main.rand.NextBool())
            {
                col = Color.Yellow;
            }
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= duration)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame > 3)
                    duration++;
                Projectile.scale += (0.03f * Math.Sign(Projectile.scale));
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.active = false;
            }
            Projectile.alpha += 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var sb = Main.spriteBatch;
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);

            Texture2D tex = (Texture2D)TextureAssets.Projectile[Projectile.type];
            Projectile.Size = new Vector2(8, 8);

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            float offsetY = 16f;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);
            origin.X = -4f;
            float alphaPercent = (255 - Projectile.alpha) / 255f;
            //sb.Draw(tex, Projectile.Center - Main.screenPosition, sourceRectangle, lightColor * alphaPercent, Projectile.rotation, origin, Projectile.scale*1f, SpriteEffects.None, 0);
            var newcol = new Color(50, 45, 35) * alphaPercent * 1.5f;
            var fx = SpriteEffects.None;
            if (col == Color.Yellow)
            {
                fx = SpriteEffects.FlipVertically;
            }
            for (var j = 0; j < 7; j++)
            {
                Vector2 rand = Main.rand.NextVector2Square(-j/4, j/4);
                sb.Draw(tex, Projectile.Center - Main.screenPosition+rand, sourceRectangle, Projectile.GetAlpha(newcol), Projectile.rotation, origin, Projectile.scale * (1.2f+0.06f*j), fx, 0);
            }
            return false;
        }
    }
}