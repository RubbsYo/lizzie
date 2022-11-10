using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Hook = LizSoundPack.Common.Hooks.Items.IModifyItemNPCHitSound;
using LizSoundPack.DamageSources;
using LizSoundPack.Projectiles;
using Microsoft.Xna.Framework;
using LizSoundPack.Common.ModEntities.NPCs;
using LizSoundPack.Content.Effects;
using LizSoundPack.Core.Effects;

namespace LizSoundPack.Common.Hooks.Items;

public interface IModifyItemNPCHitSound
{
	public static readonly HookList<GlobalItem> Hook = ItemLoader.AddModHook(new HookList<GlobalItem>(typeof(Hook).GetMethod(nameof(ModifyItemNPCHitSound))));

	void ModifyItemNPCHitSound(Item item, Player player, NPC target, ref SoundStyle? customHitSound, ref bool playNPCHitSound);

	public static void Invoke(Item item, Player player, NPC target, ref SoundStyle? customHitSound, ref bool playNPCHitSound)
	{
		(item.ModItem as Hook)?.ModifyItemNPCHitSound(item, player, target, ref customHitSound, ref playNPCHitSound);

		foreach (Hook g in Hook.Enumerate(item)) {
			g.ModifyItemNPCHitSound(item, player, target, ref customHitSound, ref playNPCHitSound);
		}
	}
}

public class NPCDamageAudio : GlobalNPC
{
	public static readonly SoundStyle LightSlash = new SoundStyle("LizSoundPack/sounds/lightSlash2") { Volume = 0.4f, PitchVariance = 0.25f, };
	public static readonly SoundStyle MediumSlash = new SoundStyle("LizSoundPack/sounds/mediumSlash_4") { Volume = 0.3f, PitchVariance = 0.25f, MaxInstances = 3, };
	public static readonly SoundStyle HeavySlash = new SoundStyle("LizSoundPack/sounds/heavySlash") { Volume = 0.24f, PitchVariance = 0.4f, MaxInstances = 3, };
	public static readonly SoundStyle MediumStrike = new SoundStyle("LizSoundPack/sounds/mediumStrike") { Volume = 0.5f, PitchVariance = 0.25f, };
	public static readonly SoundStyle LightBullet = new SoundStyle("LizSoundPack/sounds/mediumBullet") { Volume = 0.6f, PitchVariance = 0.25f, };
	public static readonly SoundStyle HeavyBullet = new SoundStyle("LizSoundPack/sounds/heavyBullet") { Volume = 0.6f, PitchVariance = 0.25f, };
	public static readonly SoundStyle LightMagic = new SoundStyle("LizSoundPack/sounds/lightMagic") { Volume = 0.5f, PitchVariance = 0.25f, };
	public static readonly SoundStyle MediumMagic = new SoundStyle("LizSoundPack/sounds/mediumMagic") { Volume = 0.5f, PitchVariance = 0.25f, };
	public static readonly SoundStyle HeavyMagic = new SoundStyle("LizSoundPack/sounds/heavyMagic") { Volume = 0.5f, PitchVariance = 0.25f, };
	public static readonly SoundStyle LightGeneric = new("LizSoundPack/sounds/lightGeneric") { Volume = 0.6f, PitchVariance = 0.25f, };
	public static readonly SoundStyle MediumGeneric = new("LizSoundPack/sounds/mediumGeneric") { Volume = 0.6f, PitchVariance = 0.25f, };
	public static readonly SoundStyle hitWhip = new("LizSoundPack/sounds/hitWhip_2") { Volume = 0.3f, PitchVariance = 0.25f, };
	public static readonly SoundStyle hitPhase = new("LizSoundPack/sounds/hitPhase") { Volume = 0.6f, PitchVariance = 0.25f, MaxInstances = 5, };
	public static readonly SoundStyle hitFire = new("LizSoundPack/sounds/hitFire") { Volume = 0.55f, PitchVariance = 0.25f, MaxInstances = 5,};
	public static readonly SoundStyle hitIce = new("LizSoundPack/sounds/hitIce") { Volume = 0.55f, PitchVariance = 0.25f, MaxInstances = 5, };
	public static readonly SoundStyle hitGuard = new("LizSoundPack/sounds/hitGuard") { Volume = 0.5f, PitchVariance = 0.25f, };

	public override void Load()
	{
		// Hook for making the PlayHitSound method control whether or not to play the original hitsound.
		IL.Terraria.NPC.StrikeNPC += context => {
			var cursor = new ILCursor(context);

			// Match 'if (HitSound != null)'
			ILLabel? onCheckFailureLabel = null;

			cursor.GotoNext(
				MoveType.After,
				i => i.Match(OpCodes.Ldarg_0),
				i => i.MatchLdflda(typeof(NPC), nameof(NPC.HitSound))
			);
			cursor.GotoNext(
				MoveType.After,
				i => i.MatchBrfalse(out onCheckFailureLabel)
			);

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate<Func<NPC, bool>>(npc => !npc.TryGetGlobalNPC(out NPCDamageAudio npcDamageAudio) || PlayHitSound(npc));
			cursor.Emit(OpCodes.Brfalse, onCheckFailureLabel);

		};

		// Hook for making the PlayDeathSound method control whether or not to play the original death sound.
		/*
		IL.Terraria.NPC.checkDead += context => {
			var cursor = new ILCursor(context);

			// Match 'if (DeathSound != null)'
			ILLabel? onCheckFailureLabel = null;

			cursor.GotoNext(
				MoveType.After,
				i => i.Match(OpCodes.Ldarg_0),
				i => i.MatchLdflda(typeof(NPC), nameof(NPC.DeathSound))
			);
			cursor.GotoNext(
				MoveType.After,
				i => i.MatchBrfalse(out onCheckFailureLabel)
			);

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate<Func<NPC, bool>>(npc => !npc.TryGetGlobalNPC(out NPCDamageAudio npcDamageAudio) || PlayDeathSound(npc));
			cursor.Emit(OpCodes.Brfalse, onCheckFailureLabel);
		};*/
	}

	private static bool PlayHitSound(NPC npc)
	{

		bool playOriginalSound = true;
		SoundStyle? customSoundStyle = null;

		
		

		var damageSource = DamageSourceSystem.CurrentDamageSource;
		var config = SoundPackConfig.Instance;
		var hiteffects = npc.GetGlobalNPC<NPCHitEffects>();

		// Call item hit sound modification hooks.
		if (damageSource != null && damageSource.Source is Item item && damageSource.Parent?.Source is Player player) {
			if (npc.GetGlobalNPC<NPCHitEffects>().highDefense)
				customSoundStyle = hitGuard;
			else
			{
				//playOriginalSound = npc.HitSound != SoundID.NPCHit1;
				customSoundStyle = MediumGeneric;
				if (item.DamageType.Name.ToLower().Contains("melee"))
					customSoundStyle = MediumSlash;
				if (item.hammer > 0 && item.axe == 0)
					customSoundStyle = MediumStrike;
			}
			if (config.enableVisualEffects)
			{
				if (customSoundStyle == hitGuard) //guard hitspark
				{
					Vector2 positionInWorld = new Vector2((npc.Hitbox.Height) / 2, 0);
					positionInWorld = positionInWorld.RotatedBy(npc.Center.AngleTo(player.Center));
					positionInWorld = npc.Center + positionInWorld;
					ParticleEntity.Instantiate<GuardParticle>(p =>
					{
						p.position = positionInWorld;
					}
					);
				}
				else
				{
					if (customSoundStyle == MediumSlash || customSoundStyle == HeavySlash)
					{
						Vector2 positionInWorld = new Vector2((npc.Hitbox.Height) / 2, 0);
						positionInWorld = positionInWorld.RotatedBy(npc.Center.AngleTo(player.Center));
						positionInWorld = npc.Center + positionInWorld;

						var strength = ((item.knockBack / 9) + (item.width / 18) + (item.damage / (item.rare + 1) / 32)) / 3;
						if (strength < 0.12f)
							strength = 0.12f;
						if (strength > 1)
							strength = 1 + (strength - 1) * 0.6f;
						if (strength > 1.25f)
							strength = 1.25f + (strength - 1.25f) * 0.4f;
						if (strength > 1.5f)
							strength = 1.5f;

						if (strength >= 1f)
							customSoundStyle = HeavySlash;
						if (strength <= 0.5f)
							customSoundStyle = LightSlash;
						ParticleEntity.Instantiate<HitParticle>(h =>
						{
							h.position = positionInWorld;
							h.scale *= strength * 2;
						}
						);
						for (var i = 0; i < strength; i++)
						{
							ParticleEntity.Instantiate<SlashParticle>(p =>
							{
								p.position = positionInWorld;
								p.rotation += (float)Math.PI / 2;
								if (strength > 0.5f)
									p.rotation += (float)(Math.PI) / 6;
								else
									p.rotation -= (float)Math.PI / 4;
								p.scale *= strength * 0.7f;
							}
							);
						}
						for (var i = 0; i < 6 * strength; i++)
						{
							ParticleEntity.Instantiate<SparkParticle>(p =>
							{
								p.position = positionInWorld;
								p.velocity = new Vector2(-player.direction * (4 + Main.rand.NextFloat(4f)), -4 - Main.rand.NextFloat(4f)) * strength * 2;
								p.color = new Color(255, 221, 0);
								p.trail_set_length(7 + Main.rand.Next(4));
								p.maxTime = (int)((10 + Main.rand.Next(8)));
							}
							);
						}
						for (var i = 0; i < 6 * strength; i++)
						{
							int dir = 1;
							if (Main.rand.NextBool())
							{
								dir = -1;
							}
							var vec = new Vector2(0, 16);
							ParticleEntity.Instantiate<StreakParticle>(p =>
							{
								p.position = positionInWorld;
								p.scale.Y = dir;
								p.rotation = (player.Center + vec).AngleTo(npc.Center) - Main.rand.NextFloat((float)Math.PI / 8 * strength) * dir;
								p.scale *= strength;
								p.alt = Main.rand.NextBool();
							}
							);
						}
						if (strength > 0.5f)
						{
							for (var i = 0; i < 5 * strength; i++)
							{
								int dir = 1;
								if (Main.rand.NextBool())
								{
									dir = -1;
								}
								var vec = new Vector2(0, 16);
								ParticleEntity.Instantiate<StreakParticle>(p =>
								{
									p.position = positionInWorld;
									p.scale.Y = dir;
									p.rotation = (npc.Center).AngleTo(player.Center + vec) - Main.rand.NextFloat((float)Math.PI / 8 * strength) * dir;
									p.scale *= strength;
									p.scale *= 0.7f;
									p.alt = Main.rand.NextBool();
								}
								);
							}
						}
					}
				}
			}
			if (item.Name.ToLower().Contains("phase"))
				customSoundStyle = hitPhase;
			IModifyItemNPCHitSound.Invoke(item, player, npc, ref customSoundStyle, ref playOriginalSound);
		}
		
		// Call projectile hit sound modification hooks.
		if (damageSource != null && damageSource.Source is Projectile projectile)
		{
			ProjectileSounds globalProjectile = projectile.GetGlobalProjectile<ProjectileSounds>();
			//playOriginalSound = npc.HitSound != SoundID.NPCHit1;
			customSoundStyle = LightGeneric;
			if (globalProjectile.item != null) //checks that involve the projectile's original item
			{
				if (globalProjectile.item.useAmmo == AmmoID.Bullet)
					customSoundStyle = MediumGeneric;
				if (projectile.type == ProjectileID.BulletHighVelocity)
					customSoundStyle = HeavyBullet;
				if (globalProjectile.item.useAmmo == AmmoID.Gel)
					customSoundStyle = hitFire;
				if (projectile.DamageType.Equals(DamageClass.Magic))
				{
					customSoundStyle = MediumMagic;
					if (globalProjectile.juiceStrength == 0)
						customSoundStyle = LightMagic;
					if (globalProjectile.juiceStrength == 2)
						customSoundStyle = HeavyMagic;
				}
			}
			if (projectile.Name.ToLower().Contains("flam") || projectile.Name.ToLower().Contains("fire"))
				customSoundStyle = hitFire;
			if (projectile.Name.ToLower().Contains("ice") || projectile.Name.ToLower().Contains("frost"))
				customSoundStyle = hitIce;
			if (projectile.type == 933)
			{
				customSoundStyle = HeavySlash;
				projectile.friendly = false;
			}
			if (projectile.arrow || projectile.aiStyle == 161)
				customSoundStyle = LightSlash;
			if (projectile.aiStyle == ProjAIStyleID.Spear)
				customSoundStyle = MediumSlash;
			if (projectile.DamageType.Equals(DamageClass.SummonMeleeSpeed))
				customSoundStyle = hitWhip;
			if (npc.GetGlobalNPC<NPCHitEffects>().highDefense)
				customSoundStyle = hitGuard;
			if (config.enableVisualEffects)
			{
				if (customSoundStyle == hitGuard)
				{
					Vector2 positionInWorld = new Vector2((npc.Hitbox.Height) / 2, 0);
					positionInWorld = positionInWorld.RotatedBy(npc.Center.AngleTo(projectile.Center));
					positionInWorld = npc.Center + positionInWorld;
					ParticleEntity.Instantiate<GuardParticle>(p =>
					{
						p.position = positionInWorld;
					}
					);
				}
				else if (customSoundStyle == LightMagic || customSoundStyle == MediumMagic || customSoundStyle == HeavyMagic)
				{
					var strength = 0.6f;
					var amount = 1;
					if (customSoundStyle == MediumMagic)
						strength = 0.8f; amount = 5;
					if (customSoundStyle == HeavyMagic)
						strength = 1f; amount = 8;
					Vector2 positionInWorld = new Vector2((npc.Hitbox.Height) / 2, 0);
					positionInWorld = positionInWorld.RotatedBy(npc.Center.AngleTo(projectile.Center));
					positionInWorld = npc.Center + positionInWorld;
					for (var i = 0; i < amount; i++)
					{
						ParticleEntity.Instantiate<MagicParticle>(h =>
						{
							h.position = positionInWorld;
							h.scale *= strength;
							h.alt = Main.rand.NextBool();

						});
					}
				}
				else if (customSoundStyle == hitFire)
				{
					Vector2 positionInWorld = new Vector2((npc.Hitbox.Height) / 2, 0);
					positionInWorld = positionInWorld.RotatedBy(npc.Center.AngleTo(projectile.Center));
					positionInWorld = npc.Center + positionInWorld;
					var strength = 0.6f;
					var amount = 6;
					int dir1 = 1;
					if (Main.rand.NextBool())
					{
						dir1 = -1;
					}
					ParticleEntity.Instantiate<FireHitParticle>(p =>
					{
						p.position = positionInWorld;
						p.rotation = (Vector2.Zero).AngleTo(projectile.oldVelocity) - Main.rand.NextFloat((float)Math.PI / 8 * strength) * dir1;
						p.scale *= strength * 1.5f;
					});
					for (var i = 0; i < amount; i++)
					{
						int dir = 1;
						if (Main.rand.NextBool())
						{
							dir = -1;
						}
						ParticleEntity.Instantiate<FireStreakParticle>(h =>
						{
							h.position = positionInWorld;
							h.scale *= strength;
							h.rotation = (Vector2.Zero).AngleTo(projectile.oldVelocity) - Main.rand.NextFloat((float)Math.PI / 8 * strength) * dir;

						});
					}
				}
				else
				{
					if (customSoundStyle == MediumGeneric)
					{
						Vector2 positionInWorld = new Vector2((npc.Hitbox.Height) / 2, 0);
						positionInWorld = positionInWorld.RotatedBy(npc.Center.AngleTo(projectile.Center));
						positionInWorld = npc.Center + positionInWorld;
						ParticleEntity.Instantiate<HitParticle>(p =>
						{
							p.position = positionInWorld;
						}
					);
					}
					if (customSoundStyle == hitWhip || customSoundStyle == HeavyBullet)
					{
						Vector2 positionInWorld = new Vector2((npc.Hitbox.Height) / 2, 0);
						positionInWorld = positionInWorld.RotatedBy(npc.Center.AngleTo(projectile.Center));
						positionInWorld = npc.Center + positionInWorld;
						ParticleEntity.Instantiate<HitParticleHeavy>(p =>
						{
							p.position = positionInWorld;
						}
					);
					}
					if ((customSoundStyle == MediumSlash || customSoundStyle == HeavySlash) && projectile.GetGlobalProjectile<ProjectileSounds>().item != null)
					{
						Player myplayer = Main.player[projectile.owner];
						Vector2 positionInWorld = new Vector2((npc.Hitbox.Height) / 2, 0);
						positionInWorld = positionInWorld.RotatedBy(npc.Center.AngleTo(myplayer.Center));
						positionInWorld = npc.Center + positionInWorld;

						var strength = ((projectile.knockBack / 7) + (projectile.damage / 2 / 12)) / 4;
						if (strength < 0.5f)
							strength = 0.5f;
						if (strength > 1)
							strength = 1;

						if (strength >= 1f)
							customSoundStyle = HeavySlash;
						if (strength <= 0.5f)
							customSoundStyle = LightSlash;
						ParticleEntity.Instantiate<HitParticle>(h =>
						{
							h.position = positionInWorld;
							h.scale *= strength * 2;
						}
						);
						for (var i = 0; i < strength; i++)
						{
							ParticleEntity.Instantiate<SlashParticle>(p =>
							{
								p.position = positionInWorld;
								p.rotation += (float)Math.PI / 2;
								p.rotation -= (float)Math.PI / 4;
								p.scale *= strength * 0.7f;
							}
							);
						}
						for (var i = 0; i < 8 * strength; i++)
						{
							int dir = 1;
							if (Main.rand.NextBool())
							{
								dir = -1;
							}
							var vec = new Vector2(0, 0);
							ParticleEntity.Instantiate<StreakParticle>(p =>
							{
								p.position = positionInWorld;
								p.scale.Y = dir;
								p.rotation = (myplayer.Center + vec).AngleTo(npc.Center) - Main.rand.NextFloat((float)Math.PI / 8 * strength) * dir;
								p.scale *= strength;
								p.alt = Main.rand.NextBool();
								if (Main.rand.NextBool())
								{
									p.color = Color.Red;
									p.additive = false;
								}
							}
							);
						}
					}
				}
			}
		}
		hiteffects.lastLife = npc.life;
		if (customSoundStyle.HasValue && config.enableHitSounds) {
			SoundEngine.PlaySound(customSoundStyle.Value, npc.Center);
		}

		return playOriginalSound;
	}

	/*private static bool PlayDeathSound(NPC npc)
	{
		if (!npc.TryGetGlobalNPC(out NPCBloodAndGore npcBloodAndGore)) {
			return true;
		}

		bool playOriginalSound = true;
		SoundStyle? customSoundStyle = null;

		if (npcBloodAndGore.LastHitBloodAmount > 0) {
			customSoundStyle = GoreSound;
			playOriginalSound = npc.DeathSound != SoundID.NPCDeath1;
		}

		var damageSource = DamageSourceSystem.CurrentDamageSource;

		// Call item death sound modification hooks.
		if (damageSource != null && damageSource.Source is Item item && damageSource.Parent?.Source is Player player) {
			IModifyItemNPCDeathSound.Invoke(item, player, npc, ref customSoundStyle, ref playOriginalSound);
		}

		if (customSoundStyle.HasValue) {
			SoundEngine.PlaySound(customSoundStyle.Value, npc.Center);
		}

		return playOriginalSound;
	}*/
}