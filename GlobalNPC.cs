using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using LizOverhaul.Common.Hooks.Items;
using LizOverhaul.Common.ModEntities.NPCs;

namespace LizOverhaul.NPCs
{
	public class NPCHitState : GlobalNPC
	{
		public static readonly SoundStyle hitWallBounce = new SoundStyle("LizOverhaul/sounds/hitWallBounce") { Volume = 0.4f, PitchVariance = 0.25f, };
		public static readonly SoundStyle ukemiLand = new SoundStyle("LizOverhaul/sounds/ukemiLand") { Volume = 0.4f, PitchVariance = 0.25f, };
		public static readonly SoundStyle groundSlide = new SoundStyle("LizOverhaul/sounds/groundSlide") { Volume = 0.2f, PitchVariance = 0.25f, };
		public override bool InstancePerEntity => true;

		public int FreezeFrames;
		public int Hitstun;
		public int StunValue;
		public int StunThreshhold;
		public Vector2 KnockbackVelocity;
		public float KnockbackFriction;
		public float KnockbackGravity;
		public int HitPropertyID;
		public bool Untechable;

		public override bool PreAI(NPC npc)
		{
			if (FreezeFrames > 0)
			{
				FreezeFrames--;
				npc.velocity.X = 0;
				npc.velocity.Y = 0;
				return false;
			}

			if (Hitstun > 0 && FreezeFrames == 0)
			{

				Hitstun--;
				if (npc.velocity.Y != 0 && Untechable)
				{
					Hitstun++;
				}
				//this is the wall detection
				float num = npc.position.X;
				if (npc.velocity.X < 0)
					num -= 1f;

				if (npc.velocity.X > 0)
					num += 1 + (float)npc.width;

				float num2 = npc.Center.Y;

				//convert npc coords into tile grid
				num /= 16f;
				num2 /= 16f;

				if ((WorldGen.SolidTile((int)num, (int)num2)))
				{
					if (HitPropertyID == 1 && npc.velocity.Y != 0) //wallbounce hahaha
					{
						KnockbackVelocity.X *= -0.3f;
						KnockbackVelocity.Y = -2f;
						HitPropertyID = 0;
						FreezeFrames = 4;
						SoundEngine.PlaySound(npc.HitSound, npc.Center);
						SoundEngine.PlaySound(hitWallBounce, npc.Center);
						SoundEngine.PlaySound(NPCDamageAudio.MediumGeneric, npc.Center);
						Collision.HitTiles(npc.position, npc.velocity, npc.width, npc.height);
					}
				}
				if (Collision.SolidCollision(new Vector2(npc.position.X+Math.Sign(npc.velocity.X), npc.position.Y + 1), npc.width, npc.height))
				{
					if (HitPropertyID == 1 && npc.velocity.Y != 0) //groundslide
					{
						KnockbackVelocity.Y = 0;
						HitPropertyID = 2;
						Untechable = false;
						Hitstun *= 2;
						SoundEngine.PlaySound(npc.HitSound, npc.Center);
						//SoundEngine.PlaySound(hitWallBounce, npc.Center);
						SoundEngine.PlaySound(NPCDamageAudio.MediumGeneric, npc.Center);
						Collision.HitTiles(npc.position, npc.velocity, npc.width, npc.height);
					}

					if (Untechable && npc.velocity.Y > 1 && HitPropertyID == 0) //ukemi! i see right through you
					{
						Hitstun = 0;
						Untechable = false;
						SoundEngine.PlaySound(ukemiLand, npc.position);
						npc.GetGlobalNPC<NPCHitEffects>().drawRotation = (float)Math.PI * Math.Sign(npc.GetGlobalNPC<NPCHitEffects>().drawRotation);
					}
				}
				if (Hitstun % 9 == 0 && HitPropertyID == 2)
                {
					SoundEngine.PlaySound(groundSlide, npc.position);
					Collision.HitTiles(npc.position, npc.velocity, npc.width, npc.height);
                }

				npc.velocity = KnockbackVelocity;
				Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
				KnockbackVelocity.X = Utilities.approachF(KnockbackVelocity.X, 0, KnockbackFriction);
				KnockbackVelocity.Y += KnockbackGravity;
				return false;
            }
			return true;
		}

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
			if (knockback > 6)
            {
				Untechable = true;
				HitPropertyID = 1;
            } else
            {
				Untechable = false;
				HitPropertyID = 0;
            }

            if (npc.velocity.Y != 0 || Untechable) //air knockback
            {
				KnockbackVelocity.X = knockback;
				KnockbackVelocity.Y = -4;
				KnockbackGravity = 0.2f;
            }
			else if (npc.velocity.Y == 0)
            {
				KnockbackVelocity.X = knockback;
				KnockbackVelocity.Y = 0;
				KnockbackFriction = 0.2f;
				KnockbackGravity = 0f;
            }
			KnockbackVelocity.X *= player.direction;
			//Main.NewText(knockback);
			FreezeFrames = (int)Math.Abs(knockback);
			Hitstun = FreezeFrames * 2;
			knockback = 0;
        }
        public void SetFreezeFrames(NPC npc, int ticks)
		{
			FreezeFrames = Math.Max(ticks, FreezeFrames);
		}
	}
}