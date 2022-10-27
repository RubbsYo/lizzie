using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ID;
using Hook = LizSoundPack.Common.Hooks.Items.IModifyItemUseSound;
using LizSoundPack.Projectiles;
using LizSoundPack;
using Microsoft.Xna.Framework;

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
		public static SoundStyle[] loadSoundStrengthSet(string filename, float volume)
		{
			SoundStyle[] array = new SoundStyle[3];
			string[] suffixes = { "S", "M", "L" };
			for(var i = 0; i <= 2; i++)
            {
				array[i] = new SoundStyle("LizSoundPack/sounds/" + filename + suffixes[i]);
				array[i].Volume = volume;
				array[i].PitchVariance = 0.25f;
			}
			return array;
		}

		public static SoundStyle SetSoundStrength(float indexer, float mediumPoint, float heavyPoint, SoundStyle[] strengthSet)
        {
			var index = 0;
			if (indexer > mediumPoint)
				index = 1;
			if (indexer > heavyPoint)
				index = 2;
			return strengthSet[index];
        }
		public static readonly SoundStyle[] swingSharp = loadSoundStrengthSet("swingSharp", 0.19f);
		public static readonly SoundStyle[] swingSharp2 = loadSoundStrengthSet("swingSharp2", 0.19f);
		public static readonly SoundStyle[] swingThrow = loadSoundStrengthSet("swingThrow", 0.35f);
		public static readonly SoundStyle[] swingPole = loadSoundStrengthSet("swingPole", 0.35f);
		public static readonly SoundStyle[] swingBow = loadSoundStrengthSet("swingBow", 0.4f);
		public static readonly SoundStyle swingFireS = new SoundStyle("LizSoundPack/sounds/swingFireLight") { Volume = 0.4f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingFireL = new SoundStyle("LizSoundPack/sounds/swingFire") { Volume = 0.3f, PitchVariance = 0.25f, };
		public static readonly SoundStyle swingIce = new SoundStyle("LizSoundPack/sounds/swingIce") { Volume = 0.6f, PitchVariance = 0.25f, };
		public static readonly SoundStyle firePistol = new SoundStyle("LizSoundPack/sounds/firePistol") { Volume = 0.2f, PitchVariance = 0.25f, };
		public static readonly SoundStyle fireShotgun = new SoundStyle("LizSoundPack/sounds/fireShotgun") { Volume = 0.4f, PitchVariance = 0.25f, };
		public static readonly SoundStyle fireMachinegun = new SoundStyle("LizSoundPack/sounds/fireMachinegun") { Volume = 0.3f, PitchVariance = 0.25f, };
		public static readonly SoundStyle fireRifle = new SoundStyle("LizSoundPack/sounds/fireRifle") { Volume = 0.3f, PitchVariance = 0.25f, };
		public static readonly SoundStyle fireVeryStrong = new SoundStyle("LizSoundPack/sounds/fireVeryStrong") { Volume = 0.4f, PitchVariance = 0.25f, };

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
				bool soundIsModded = false;
				bool resetSound = false;
				for (int i = 0; i <= 2; i++)
                {
					if (useSoundBackup == swingSharp[i]
					|| useSoundBackup == swingSharp2[i]
					|| useSoundBackup == swingThrow[i] 
					|| useSoundBackup == swingPole[i] 
					|| useSoundBackup == swingBow[i])
						soundIsModded = true;
                }
				if (heldItem.UseSound == SoundID.Item1 || soundIsModded)
				{
					if (heldItem.useStyle == ItemUseStyleID.Swing)
					{
						if (heldItem.noUseGraphic)
						{
							if (config.enableThrowSounds)
								heldItem.UseSound = SetSoundStrength(heldItem.useTime, 15, 30, swingThrow);
						}
						else if (config.enableMeleeSounds)
						{
							float num = heldItem.useTime;
							if (heldItem.useAnimation < heldItem.useTime)
								num = heldItem.useAnimation;
							num /= player.GetWeaponAttackSpeed(heldItem);
							Main.NewText(num);
							heldItem.UseSound = SetSoundStrength(num, 15, 25, Main.rand.NextBool() ? swingSharp : swingSharp2);
						}
					}
					
					if (heldItem.useStyle == ItemUseStyleID.Shoot && heldItem.DamageType.Equals(DamageClass.Melee) && config.enableSpearSounds)
						heldItem.UseSound = SetSoundStrength(heldItem.useTime, 26, 30, swingPole);
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
					heldItem.UseSound = swingIce;
				if (config.enableGunSounds)
				{
					//putting bow stuff here too for now
					if (heldItem.useAmmo == AmmoID.Arrow)
                    {
						var strength = (heldItem.useTime / 7 + heldItem.shootSpeed / 4);
						heldItem.UseSound = SetSoundStrength(strength, 5.1f, 5.5f, swingBow);
					}
					if (heldItem.UseSound == SoundID.Item41)
						heldItem.UseSound = firePistol;
					if (heldItem.UseSound == SoundID.Item11 || heldItem.UseSound == SoundID.Item31)
						heldItem.UseSound = fireMachinegun;
					if (heldItem.UseSound == SoundID.Item36)
						heldItem.UseSound = fireShotgun;
					if (heldItem.UseSound == SoundID.Item40)
						heldItem.UseSound = fireRifle;
					if (heldItem.useAmmo == AmmoID.Bullet && heldItem.useTime > 30 && player.ChooseAmmo(heldItem).type == ItemID.HighVelocityBullet)
					{
						heldItem.UseSound = fireVeryStrong;
						resetSound = true;
					}
				}

				if (heldItem.UseSound == fireVeryStrong)
                {
					var vec = player.DirectionTo(Main.MouseWorld) * 16;
					player.GetModPlayer<ScreenShakePlayer>().SetViewOffset(vec);
					player.GetModPlayer<ScreenShakePlayer>().SetScreenShake(8);
				}
				Hook.Invoke(heldItem, player, ref heldItem.UseSound);
				bool soundSwapped = heldItem.UseSound != useSoundBackup;



				orig(player, item);
				if (resetSound)
                {
					heldItem.UseSound = useSoundBackup;
                }
			};
		}
	}
}
