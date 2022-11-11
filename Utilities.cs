using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OnPlayerSittingHelper = On.Terraria.GameContent.PlayerSittingHelper;
using OnPlayerSleepingHelper = On.Terraria.GameContent.PlayerSleepingHelper;
using ReLogic.Content;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Renderers;
namespace LizOverhaul
{
    class Utilities
    {
        public static float approachF(float a, float b, float amount)
        {
			if (a < b)
			{
				a += amount;
				if (a > b)
					return b;
			}
			else
			{
				a -= amount;
				if (a < b)
					return b;
			}
			return a;
		}

		public static float radToDeg(float radians)
        {
			return radians * (180 / (float)Math.PI);
		}

		public static float degToRad(float degrees)
        {
			return degrees * ((float)Math.PI / 180);
		}

		public static float Lerp(float a, float b, float value)
        {
			return a + value * (b - a);
		}

		public static float Random_Range(float num1,float num2)
        {
			var diff = num2 - num1;
			return num1+(Main.rand.Next((int)(diff*100f))/100);
        }

		public static Vector2 GetGunBarrelEndPosition(int type, Texture2D texture)
		{

			var surface = new Surface<Color>(texture.Width, texture.Height);

			texture.GetData(surface.Data);

			var columnPoints = new List<Vector2>();

			for (int x = surface.Width - 1; x >= 0; x--)
			{
				bool columnIsEmpty = true;

				for (int y = 0; y < surface.Height; y++)
				{
					if (surface[x, y].A > 0)
					{
						columnIsEmpty = false;

						columnPoints.Add(new Vector2(x, y));
					}
				}

				if (!columnIsEmpty)
				{
					break;
				}
			}

			Vector2 result = new Vector2(0,0);

			if (columnPoints.Count > 0)
			{
				foreach (var value in columnPoints)
				{
					result += value;
				}

				result /= columnPoints.Count;
			}

			return result;
		}

	}

	
	public class ParticleExtensions : ParticleOrchestrator
    {
		private static PrettySparkleParticle GetNewPrettySparkleParticle() => new PrettySparkleParticle();
		private static ParticlePool<PrettySparkleParticle> _poolPrettySparkle = new ParticlePool<PrettySparkleParticle>(200, GetNewPrettySparkleParticle);

		public static void SpawnLizParticles(int type, ParticleOrchestraSettings settings)
        {

        }
		private static void Spawn_NightsEdge(ParticleOrchestraSettings settings)
		{
			float num = 30f;
			float num2 = 0f;
			for (float num3 = 0f; num3 < 3f; num3 += 1f)
			{
				PrettySparkleParticle prettySparkleParticle = _poolPrettySparkle.RequestParticle();
				Vector2 vector = ((float)Math.PI / 4f + (float)Math.PI / 4f * num3 + num2).ToRotationVector2() * 4f;
				prettySparkleParticle.ColorTint = new Color(0.25f, 0.1f, 0.5f, 0.5f);
				prettySparkleParticle.LocalPosition = settings.PositionInWorld;
				prettySparkleParticle.Rotation = vector.ToRotation();
				prettySparkleParticle.Scale = new Vector2(2f, 1f) * 1.1f;
				prettySparkleParticle.LocalPosition -= vector * num * 0.25f;
				prettySparkleParticle.Velocity = vector;
				if (num3 == 1f)
				{
					prettySparkleParticle.Scale *= 1.5f;
					prettySparkleParticle.Velocity *= 1.5f;
					prettySparkleParticle.LocalPosition -= prettySparkleParticle.Velocity * 4f;
				}

				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle);
			}

			for (float num4 = 0f; num4 < 3f; num4 += 1f)
			{
				PrettySparkleParticle prettySparkleParticle2 = _poolPrettySparkle.RequestParticle();
				Vector2 vector2 = ((float)Math.PI / 4f + (float)Math.PI / 4f * num4 + num2).ToRotationVector2() * 4f;
				prettySparkleParticle2.ColorTint = new Color(0.5f, 0.25f, 1f, 1f);
				prettySparkleParticle2.LocalPosition = settings.PositionInWorld;
				prettySparkleParticle2.Rotation = vector2.ToRotation();
				prettySparkleParticle2.Scale = new Vector2(2f, 1f) * 0.7f;
				prettySparkleParticle2.LocalPosition -= vector2 * num * 0.25f;
				prettySparkleParticle2.Velocity = vector2;
				if (num4 == 1f)
				{
					prettySparkleParticle2.Scale *= 1.5f;
					prettySparkleParticle2.Velocity *= 1.5f;
					prettySparkleParticle2.LocalPosition -= prettySparkleParticle2.Velocity * 4f;
				}

				Main.ParticleSystem_World_OverPlayers.Add(prettySparkleParticle2);
			}
		}
	}
	public static partial class PlayerExtensions
    {
		// Essentials

		public static bool IsLocal(this Player player)
			=> player.whoAmI == Main.myPlayer;

		public static bool OnGround(this Player player)
			=> player.velocity.Y == 0f; //player.GetModPlayer<PlayerMovement>().OnGround;

		public static bool WasOnGround(this Player player)
			=> player.oldVelocity.Y == 0f; //player.GetModPlayer<PlayerMovement>().WasOnGround;

		public static bool IsUnderwater(this Player player)
			=> Collision.DrownCollision(player.position, player.width, player.height, player.gravDir);

		public static int KeyDirection(this Player player)
			=> player.controlLeft ? -1 : player.controlRight ? 1 : 0;


	}
}