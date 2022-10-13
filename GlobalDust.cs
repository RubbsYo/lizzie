using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using LizSoundPack.Core.Effects;
using LizSoundPack.Content.Effects;

namespace LizSoundPack.Dusts;

public class DustOverwrites : ModSystem
{
    public override void PreUpdateDusts()
    {
        for (int i = 0; i < 6000; i++)
        {
            Dust dust = Main.dust[i];
            if (i < Main.maxDustToDraw)
            {
                if (!dust.active)
                    continue;
                if (dust.type == 6)
                {
                    ParticleEntity.Instantiate<FireParticle>(p =>
                    {
                        p.position = dust.position;
                        p.velocity = dust.velocity;
                        p.rotation = dust.rotation;
                        p.scale *= dust.scale;
                        p.scale = new Vector2(Math.Max(p.scale.X, 1), Math.Max(p.scale.Y, 1));
                        if (!dust.noGravity)
                        {
                            p.gravity.Y = 0.05f;
                            p.shrinkspd = 0.06f;
                        } else
                        {
                            p.shrinkspd = 0.06f;
                        }
                        p.rotspd = dust.velocity.X * 0.2f;
                        p.alpha = 0.05f;
                        p.color = new Color(255, 0, 0);
                    }
                    );
                    dust.active = false;
                }
                if (dust.type == 135 || dust.type == 80 || dust.type == 92)
                {
                    ParticleEntity.Instantiate<IceParticle>(p =>
                    {
                        p.position = dust.position;
                        p.velocity = dust.velocity;
                        p.rotation = dust.rotation;
                        p.friction = p.velocity * 0.02f;
                        p.scale *= dust.scale*0.5f;
                        p.scale = new Vector2(Math.Max(p.scale.X, 1), Math.Max(p.scale.Y, 1));
                        if (!dust.noGravity || Main.rand.NextBool() || p.velocity.Y != 0)
                        {
                            p.gravity.Y = 0.1f;
                        }
                        p.alpha = 0.5f;
                        p.alphaFadeSpeed = 0.1f;
                        p.alphaFadeTimer = 20+Main.rand.Next(6);
                    }
                    );
                    dust.active = false;
                }
            }
        }
    }
}