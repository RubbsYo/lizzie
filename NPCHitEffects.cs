using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using LizOverhaul.Core.Time;
using ReLogic.Content;
using LizOverhaul.NPCs;

namespace LizOverhaul.Common.ModEntities.NPCs
{
	// Offsets, rotates, and scales enemies whenever they're hit, so that they look less static even when they're not moving.
	// Looks like some sort of flinching.
	public class NPCHitEffects : GlobalNPC
	{
		private const int EffectLength = 10;
		private static readonly SoundStyle healSound = new SoundStyle("LizOverhaul/sounds/heal") { Volume = 1f, };

		private ulong lastHitTime;
		private int lastHealth;
		private int lastNPC;
		private float? usedDrawScaleMultiplier;
		private float? usedDrawRotationOffset;
		private Vector2? usedDrawPositionOffset;
		private Color drawColor;
		public bool highDefense;
		public int lastLife;
		public float drawRotation;

		//public static Asset<Effect> spriteEffect = ModContent.Request<Effect>("Shader/fillWhite");

		public override bool InstancePerEntity => true;


		public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
			=> ResetHitTime(npc, damage, knockback, item.DamageType);

		public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
			=> ResetHitTime(npc, damage, knockback, projectile.DamageType);

		// Drawing
		public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			ulong delta = TimeSystem.UpdateCount - lastHitTime;

			const int EffectLength = 15;

			if (delta <= EffectLength || npc.GetGlobalNPC<NPCHitState>().FreezeFrames > 0)
			{
				float intensity = 1f - (delta / (float)EffectLength);
				usedDrawPositionOffset = new Vector2(-8+Main.rand.Next(16),0) * intensity;

				npc.position += usedDrawPositionOffset.Value;
			}
			var hitstate = npc.GetGlobalNPC<NPCHitState>();
			if (hitstate.Hitstun > 0)
            {
				if (hitstate.FreezeFrames == 0)
				{
					if (hitstate.HitPropertyID == 2)
					{
						drawRotation += hitstate.KnockbackVelocity.X / 16;
						usedDrawPositionOffset = new Vector2(0, Main.rand.Next(-4, 4));
						npc.position += usedDrawPositionOffset.Value;
					}
					else
					{
						drawRotation += hitstate.KnockbackVelocity.X / 64;
					}
				}
				drawRotation = Utilities.degToRad(Utilities.radToDeg(drawRotation) % 360);
				npc.rotation += drawRotation;
				usedDrawRotationOffset = drawRotation;
			} else
            {
				drawRotation *= 0.8f;
				npc.rotation += drawRotation;
				usedDrawRotationOffset = drawRotation;
			}

			return true;
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (usedDrawScaleMultiplier.HasValue)
			{
				if (usedDrawScaleMultiplier.Value != 0f)
				{
					npc.scale /= usedDrawScaleMultiplier.Value;
				}

				usedDrawScaleMultiplier = null;
			}

			if (usedDrawRotationOffset.HasValue)
			{
				npc.rotation -= usedDrawRotationOffset.Value;
				usedDrawRotationOffset = null;
			}

			if (usedDrawPositionOffset.HasValue)
			{
				npc.position -= usedDrawPositionOffset.Value;
				usedDrawPositionOffset = null;
			}
		}

		private void ResetHitTime(NPC npc, int damage, float knockback, DamageClass type)
		{
			lastHitTime = TimeSystem.UpdateCount;
			drawRotation = 0;
		}
	}
}
