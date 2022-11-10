using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace LizSoundPack
{
    [Label("Client Config")]
    public class SoundPackConfig : ModConfig
    {
        //This is here for the Config to work at all.
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static SoundPackConfig Instance;

        [Header("Toggles")]

        [Label("Enable Melee Swing Sounds")]
        [Tooltip("Changes melee swing sounds to add variety.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableMeleeSounds { get; set; }

        [Label("Enable Spear Swing Sounds")]
        [Tooltip("Changes spear use sounds to add variety.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableSpearSounds { get; set; }

        [Label("Enable Gun Sounds")]
        [Tooltip("Changes gun sounds to feel 'better'.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableGunSounds { get; set; }

        [Label("Enable Generic Swing Sounds")]
        [Tooltip("Changes generic swing sounds (i.e throwing weapons) to add variety.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableThrowSounds { get; set; }

        [Label("Enable Hit Sounds")]
        [Tooltip("Changes weapon hit sounds to add variety and feel better.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableHitSounds { get; set; }

        [Label("Enable Bow Sounds")]
        [Tooltip("Changes bow use sounds to add variety.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableBowSounds { get; set; }

        [Label("Enable Elemental Sounds")]
        [Tooltip("Changes elemental sounds (i.e fire/ice) to add variety.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableElementalSounds { get; set; }

        [Label("Enable Shotgun Rework")]
        [Tooltip("Adjusts shotgun mechanics to make bullets shot from shotguns act uniquely.\nPellets will be doubled, have friction, and bounce off of walls.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableShotgunRework { get; set; }

        [Label("Enable High Velocity Bullet Rework")]
        [Tooltip("Makes High Velocity Bullets true hitscan.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableHighVelocityRework { get; set; }

        [Label("Enable Spear Rework")]
        [Tooltip("Adjusts the spear animation to function much more like thrusting.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableSpearRework { get; set; }

        [Label("Enable Hit Visual Effects")]
        [Tooltip("Adds unique visual effects to various types of hits.\nRequires the sound for a given visual effect.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableVisualEffects { get; set; }

        [Label("Enable Dust Overrides")]
        [Tooltip("Completely reworks existing particles (e.g fire) to look subjectively cooler.\n[Default: On]")]
        [DefaultValue(true)]
        public bool enableParticleReplacements { get; set; }
    }
}