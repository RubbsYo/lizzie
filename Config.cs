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
    }
}