using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Utilities;
using LizOverhaul.Content.Items.Armor;

namespace LizOverhaul
{
    public partial class LizOverhaulPlayer : ModPlayer
    {
        public static LizOverhaulPlayer ModPlayer(Player Player)
        {
            return Player.GetModPlayer<LizOverhaulPlayer>();
        }

        private SoundStyle hitCritical = new SoundStyle("LizOverhaul/sounds/hitCritical") { Volume = 0.6f, MaxInstances = 1, PitchVariance = 0.25f };
        private SoundStyle hitPlayer = new SoundStyle("LizOverhaul/sounds/hitPlayer") { Volume = 0.7f, MaxInstances = 1, PitchVariance = 0.25f };
        public int swingTotalDuration;
        public float swingProgress;
        public int swingTime;
        public static bool MeleeCheck(DamageClass damageClass) => damageClass == DamageClass.Melee
            || damageClass.GetEffectInheritance(DamageClass.Melee) || !damageClass.GetModifierInheritance(DamageClass.Melee).Equals(StatInheritanceData.None);

        public bool IsMeleeBroadSword => MeleeCheck(Player.HeldItem.DamageType);

        public bool CanSlash => Player.HeldItem.damage > 0 && Player.HeldItem.useStyle == ItemUseStyleID.Swing && IsMeleeBroadSword && !Player.HeldItem.noUseGraphic && SoundPackConfig.Instance.MeleeStyle > 0;

        public override void PreUpdate()
        {
            var item = Player.HeldItem;
            var config = SoundPackConfig.Instance;
            if (Player.itemAnimation == Player.itemAnimationMax)
            {
                swingTime = 0;
                swingProgress = 0;
            }
            if (CanSlash && config.MeleeStyle == 1 && Player.itemAnimation > 0) //basic swing with startup
            {
                var swingingThreshHold = 0.5f;
                swingTotalDuration = Player.itemAnimationMax / 2;
                if (swingTime < Player.itemAnimationMax-swingTotalDuration) //startup itself
                {
                    Player.itemAnimation = (int)(Player.itemAnimationMax * swingingThreshHold);
                    Player.itemAnimation += (int)((float)Math.Pow((float)swingTime / (Player.itemAnimationMax - swingTotalDuration), 2) / ((Player.itemAnimationMax - swingTotalDuration)));
                } else
                {
                    
                    var trueswingtime = swingTime-(Player.itemAnimationMax - swingTotalDuration);
                    Main.NewText(trueswingtime + ", " + swingTotalDuration);
                    swingProgress = (float)trueswingtime / swingTotalDuration;
                    Main.NewText(swingProgress);
                    Player.itemAnimation = Player.itemAnimationMax - (int)(Player.itemAnimationMax * swingProgress);
                    
                }
                if (Player.itemAnimation <= 0 && swingTime < Player.itemAnimationMax)
                {
                    Player.itemAnimation = 1;
                }
                swingTime++;
            }
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit,
            ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            //SoundEngine.PlaySound(hitPlayer);
            return true;
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                SoundEngine.PlaySound(hitCritical);
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            foreach(Item item in Player.armor)
            if (item.type == ModContent.ItemType<TallLeggings>())
                drawInfo.Position.Y -= 2;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                SoundEngine.PlaySound(hitCritical);
            }
        }

        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            if (crit)
            {
                SoundEngine.PlaySound(hitCritical);
            }
        }

        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if (crit)
            {
                SoundEngine.PlaySound(hitCritical);
            }
        }

    }
}