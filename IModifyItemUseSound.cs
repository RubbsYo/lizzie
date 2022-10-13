using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ID;
using Hook = LizSoundPack.Common.Hooks.Items.IModifyItemUseSound;
using LizSoundPack.Projectiles;

namespace LizSoundPack.Common.Hooks.Items
{
	public interface IModifyItemUseSound
	{
		public static readonly HookList<GlobalItem> Hook = ItemLoader.AddModHook(new HookList<GlobalItem>(typeof(Hook).GetMethod(nameof(ModifyItemUseSound))));

		void ModifyItemUseSound(Item item, Player player, ref SoundStyle? useSound);

		public static void Invoke(Item item, Player player, ref SoundStyle? useSound)
		{
			(item.ModItem as Hook)?.ModifyItemUseSound(item, player, ref useSound);

			foreach (Hook g in Hook.Enumerate(item))
			{
				g.ModifyItemUseSound(item, player, ref useSound);
			}
		}
	}

	internal sealed class ModifyItemUseSoundImplementation : GlobalItem
	{
		public static readonly SoundStyle swingSharpS = new SoundStyle("LizSoundPack/sounds/swingSharpS") { Volume = 0.19f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingSharpM = new SoundStyle("LizSoundPack/sounds/swingSharpM") { Volume = 0.19f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingSharpL = new SoundStyle("LizSoundPack/sounds/swingSharpL") { Volume = 0.19f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingThrowS = new SoundStyle("LizSoundPack/sounds/swingThrowS") { Volume = 0.35f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingThrowM = new SoundStyle("LizSoundPack/sounds/swingThrowM") { Volume = 0.25f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingThrowL = new SoundStyle("LizSoundPack/sounds/swingThrowL") { Volume = 0.35f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingPoleS = new SoundStyle("LizSoundPack/sounds/swingPoleS") { Volume = 0.2f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingPoleM = new SoundStyle("LizSoundPack/sounds/swingPoleM") { Volume = 0.2f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingPoleL = new SoundStyle("LizSoundPack/sounds/swingPoleL") { Volume = 0.2f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingFireS = new SoundStyle("LizSoundPack/sounds/swingFireLight") { Volume = 0.4f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingFireL = new SoundStyle("LizSoundPack/sounds/swingFire") { Volume = 0.3f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingIce = new SoundStyle("LizSoundPack/sounds/swingIce") { Volume = 0.6f, PitchVariance = 0.25f, };
		public static readonly SoundStyle firePistol = new SoundStyle("LizSoundPack/sounds/firePistol") { Volume = 0.2f, PitchVariance = 0.25f, };
		public static readonly SoundStyle fireShotgun = new SoundStyle("LizSoundPack/sounds/fireShotgun") { Volume = 0.4f, PitchVariance = 0.25f, };
		public static readonly SoundStyle fireMachinegun = new SoundStyle("LizSoundPack/sounds/fireMachinegun") { Volume = 0.3f, PitchVariance = 0.25f, };
		public static readonly SoundStyle fireRifle = new SoundStyle("LizSoundPack/sounds/fireRifle") { Volume = 0.3f, PitchVariance = 0.25f, };
		public override void Load()
		{
			On.Terraria.Player.ItemCheck_StartActualUse += (orig, player, item) =>
			{
				var heldItem = player.HeldItem;

				if (heldItem == null || heldItem.IsAir)
				{
					orig(player, item);
					return;
				}
				var config = SoundPackConfig.Instance;
				var useSoundBackup = heldItem.UseSound;
				if (heldItem.UseSound == SoundID.Item1)
				{
					if (heldItem.useStyle == ItemUseStyleID.Swing)
					{
						if (heldItem.noUseGraphic)
						{
							if (config.enableThrowSounds)
							{
								if (heldItem.useTime < 15)
									heldItem.UseSound = swingThrowS;
								else if (heldItem.useTime < 30)
									heldItem.UseSound = swingThrowM;
								else
									heldItem.UseSound = swingThrowL;
							}
						}
						else if (config.enableMeleeSounds)
						{
							if (heldItem.useTime < 15)
								heldItem.UseSound = swingSharpS;
							else if (heldItem.useTime < 30)
								heldItem.UseSound = swingSharpM;
							else
								heldItem.UseSound = swingSharpL;
						}

					}
					
					if (heldItem.useStyle == ItemUseStyleID.Shoot && heldItem.DamageType.Equals(DamageClass.Melee) && config.enableSpearSounds)
					{
						if (heldItem.useTime < 26)
							heldItem.UseSound = swingPoleL;
						else if (heldItem.useTime < 30)
							heldItem.UseSound = swingPoleM;
						else
							heldItem.UseSound = swingPoleS;
					}
				}
				if (heldItem.Name.ToLower().Contains("fire") || heldItem.Name.ToLower().Contains("fiery") || heldItem.Name.ToLower().Contains("flam") || heldItem.Name.ToLower().Contains("sun"))
				{
					if (heldItem.DamageType.Name.ToLower().Contains("melee") || heldItem.DamageType.Equals(DamageClass.SummonMeleeSpeed))
					{
						if (heldItem.noUseGraphic)
							heldItem.UseSound = swingFireS;
						else
							heldItem.UseSound = swingFireL;
					}
				}
				if (heldItem.Name.ToLower().Contains("ice") || heldItem.Name.ToLower().Contains("frost"))
				{
					heldItem.UseSound = swingIce;
				}
				if (config.enableGunSounds)
				{
					if (heldItem.UseSound == SoundID.Item41)
						heldItem.UseSound = firePistol;
					if (heldItem.UseSound == SoundID.Item11 || heldItem.UseSound == SoundID.Item31)
						heldItem.UseSound = fireMachinegun;
					if (heldItem.UseSound == SoundID.Item36)
						heldItem.UseSound = fireShotgun;
					if (heldItem.UseSound == SoundID.Item40)
						heldItem.UseSound = fireRifle;
				}
				Hook.Invoke(heldItem, player, ref heldItem.UseSound);
				bool soundSwapped = heldItem.UseSound != useSoundBackup;



				orig(player, item);

			};
		}
	}
}
