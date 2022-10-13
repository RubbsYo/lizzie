using Microsoft.Xna.Framework;
using Terraria;
using LizSoundPack.Core.Effects;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LizSoundPack.Content.Effects
{
	public abstract class DustParticle : ParticleEntity
	{
		public const float MaxParticleDistance = 3000f;
		public const float MaxParticleDistanceSqr = MaxParticleDistance * MaxParticleDistance;

		public float alpha = 1f;
		public float rotation;
		public Vector2 position;
		public Vector2 velocity;
		public Vector2 velocityScale = Vector2.One;
		public Vector2 scale = Vector2.One;
		public Vector2 gravity = new(0f, 0f);
		public Vector2 friction;
		public Color color = Color.White;
		public Texture2D? texture;
		public Rectangle frame;
		public int frameCount;
		public int frameDuration;
		public int frameIndex;
		public int frameMax;
		public Vector2 origin;
		public float shrinkspd = 1;
		public float rotspd;
		public float rotspdDecay = 0.95f;
		public float alphaFadeSpeed;
		public int alphaFadeTimer;

		public int LifeTime { get; private set; }

		public override void Update()
		{
			velocity += gravity;
			velocity.X = Utilities.approachF(velocity.X, 0, Math.Abs(friction.X));
			if (gravity.Y == 0f)
				velocity.Y = Utilities.approachF(velocity.Y, 0, Math.Abs(friction.Y));
			scale -= new Vector2(shrinkspd,shrinkspd);
			rotation += rotspd;
			if (scale.X >= 1.5f)
            {
				scale *= 0.95f;
            }
			rotspd *= rotspdDecay;
			if (scale.X <= 0.08f || scale.Y <= 0.08f || alpha <= 0)
            {
				Destroy();
            }
			if (scale.X <= 0.8f)
            {
				if (additive)
					alpha *= 0.95f;
				velocity *= 0.9f;
            }
			position += velocity * velocityScale;
			LifeTime++;
			if (LifeTime > alphaFadeTimer)
            {
				scale.X -= alphaFadeSpeed;
				scale.Y -= alphaFadeSpeed;
			}
		}

		public Vector2 origin_center(Texture2D tex)
        {
			return new Vector2(tex.Width / 2, (tex.Height/frameMax) / 2);
        }

		public void Draw_Sprite(SpriteBatch sb, Texture2D sprite, Rectangle image_rectangle, Vector2 pos, Vector2 orig, Vector2 image_scale, float rot, Color col)
        {
			sb.Draw(sprite, pos - Main.screenPosition, image_rectangle, col, rot, orig, image_scale, SpriteEffects.None, 0);
        }
		public void Draw_Sprite(SpriteBatch sb, Texture2D sprite, Rectangle image_rectangle, Vector2 pos, Vector2 orig, Vector2 image_scale, float rot, Color col, float alpha)
		{
			var tempcol = new Color(col.R, col.G, col.B, 1 * alpha);
			sb.Draw(sprite, pos - Main.screenPosition, image_rectangle, tempcol, rot, orig, image_scale, SpriteEffects.None, 0);
		}

		public override void Draw(SpriteBatch sb)
        {
			if (texture != null)
			{
				//Color col = Lighting.GetColor(position.ToTileCoordinates(),color);
				Draw_Sprite(sb, texture, frame, position, origin, scale, rotation, color, alpha);
			}
        }
    }
}
