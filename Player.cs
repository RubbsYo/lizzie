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
    public partial class UltrakillJuicePlayer : ModPlayer
    {
        public static UltrakillJuicePlayer ModPlayer(Player Player)
        {
            return Player.GetModPlayer<UltrakillJuicePlayer>();
        }

        private SoundStyle hitCritical = new SoundStyle("LizOverhaul/sounds/hitCritical") { Volume = 0.6f, MaxInstances = 1, PitchVariance = 0.25f };
        private SoundStyle hitPlayer = new SoundStyle("LizOverhaul/sounds/hitPlayer") { Volume = 0.7f, MaxInstances = 1, PitchVariance = 0.25f };

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