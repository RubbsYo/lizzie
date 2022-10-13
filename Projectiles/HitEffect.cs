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
    public class HitEffect : ModProjectile
    {
        private int duration;

        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {

            duration = 3;
            int dir = 1;
            if (Main.rand.NextBool())
            {
                dir = -1;
            }
            Projectile.rotation = Main.rand.NextFloat(MathHelper.Pi);
            Projectile.scale = (1f + Main.rand.NextFloat(0.3f)) * dir;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= duration)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame > 3)
                    duration++;
                Projectile.scale += (0.1f * Math.Sign(Projectile.scale));
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.active = false;
            }
            Projectile.alpha += 8;
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
            float offsetY = 32f;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);
            float alphaPercent = (255 - Projectile.alpha) / 255f;
            //sb.Draw(tex, Projectile.Center - Main.screenPosition, sourceRectangle, lightColor * alphaPercent, Projectile.rotation, origin, Projectile.scale*1f, SpriteEffects.None, 0);
            var col = new Color(30, 25, 25) * alphaPercent * 1.5f;
            for (var j = 0; j < 12; j++)
            {
                Vector2 rand = Main.rand.NextVector2Square(-j / 4, j / 4);
                sb.Draw(tex, Projectile.Center - Main.screenPosition + rand, sourceRectangle, Projectile.GetAlpha(col), Projectile.rotation, origin, Projectile.scale * (0.6f + 0.1f * j), SpriteEffects.None, 0);
            }
            return false;
        }
    }
}