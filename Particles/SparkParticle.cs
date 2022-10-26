using Microsoft.Xna.Framework;
using Terraria;
using LizSoundPack.Core.Effects;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace LizSoundPack.Content.Effects
{
	[Autoload(Side = ModSide.Client)]
	public class SparkParticle : ParticleEntity
	{
		public const float MaxParticleDistance = 3000f;
		public const float MaxParticleDistanceSqr = MaxParticleDistance * MaxParticleDistance;

		public float alpha = 1f;
		public float rotation;
		public Vector2 position;
		public Vector2 velocity;
		public Vector2 velocityScale = Vector2.One;
		public Vector2 scale = Vector2.One;
		public Vector2 gravity = new(0f, 1f);
		public Vector2 friction;
		public Color color = Color.White;
		public int maxTime = 20;
		public Vector2[] oldPos = new Vector2[10];
		public Vector2[] oldVel = new Vector2[10];
		public int width = 2;

		public int LifeTime { get; private set; }

		public void trail_set_length(int length)
        {
			if (length != oldPos.Length)
			{
				Array.Resize(ref oldPos, length);
			}

			for (int i = 0; i < oldPos.Length; i++)
			{
				oldPos[i].X = position.X;
				oldPos[i].Y = position.Y;
				oldVel[i] = velocity;
			}
		}

		public override void Update()
		{
			for (int i = oldPos.Length - 1; i > 0; i--)
			{
				oldPos[i] = oldPos[i - 1];
			}

			oldPos[0] = position;
			for (int i = oldVel.Length - 1; i > 0; i--)
			{
				oldVel[i] = oldVel[i - 1];
			}

			oldVel[0] = velocity;



			LifeTime++;
			if (LifeTime >= maxTime)
            {
				if (LifeTime >= maxTime + oldPos.Length)
                {
					Destroy();
                }
            } 
			else
            {
				position += velocity * velocityScale;
				velocity += gravity;
				velocity -= friction;
			}
		}

		public override void Draw(SpriteBatch sb)
        {
			for (var j = 1; j < oldPos.Length-1; j++)
            {
				if (oldPos[j-1] != oldPos[j])
				Utils.DrawLine(sb, oldPos[j - 1] , oldPos[j] - oldVel[j], color, color, width);
            }
        }
    }
}
