using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Hook = LizSoundPack.Common.Hooks.Items.IModifyItemNPCHitSound;
using LizSoundPack.DamageSources;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;
using LizSoundPack.Content.Effects;
using LizSoundPack.Core.Effects;
using LizSoundPack.Common.TextureColors;

namespace LizSoundPack.Projectiles;

public class ProjectileSounds : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    public int juiceStrength = 1;
    public Item? item;
    public Vector2 startpos;
    public int spawnTime;
    public int timer;
    public int? ammoType;
    public Item? ammoItem;
    public static List<int> shotguns = new();
    public bool isPellet;
    public float friction;
    public Color texColor;
    public Projectile? inst;
    public IEntitySource sourceToInherit;
    public float knockbackToInherit;
    private bool soundIsPlayed;

    public override void SetStaticDefaults()
    {
        shotguns.Add(ItemID.Boomstick);
        shotguns.Add(ItemID.Shotgun);
        shotguns.Add(ItemID.QuadBarrelShotgun);
        shotguns.Add(ItemID.OnyxBlaster);
        shotguns.Add(ItemID.TacticalShotgun);
    }
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        var config = SoundPackConfig.Instance;
        if (source is EntitySource_ItemUse itemSource && isPellet == false && projectile.position != new Vector2(-2845, -12491) && inst == null && sourceToInherit == null)
        {
            if (itemSource.Item.useTime < 18) 
                juiceStrength = 0;
            if (itemSource.Item.useTime > 28)
                juiceStrength = 2;
            item = itemSource.Item;
            if (projectile.aiStyle == ProjAIStyleID.Spear && config.enableSpearRework)
            {
                if (item != null)
                {
                    timer++;
                    Player player = Main.player[projectile.owner];
                    ItemID.Sets.SkipsInitialUseSound[item.type] = true;
                    var threshhold = item.useTime / 2;
                    projectile.friendly = false;
                    projectile.damage *= 2;
                    if (timer <= player.itemAnimationMax)
                    {
                        if (timer < threshhold)
                        {
                            player.itemAnimation = player.itemAnimationMax - threshhold + timer;
                        }
                        else
                        {
                            player.itemAnimation--;
                            if (player.itemAnimation < (player.itemAnimationMax / 2) - 4)
                            {
                                player.itemAnimation = (player.itemAnimationMax / 2) - 4;
                            }
                        }

                    }
                }
            }
            if (shotguns.Contains(item.type) && projectile.Name.ToLower().Contains("bullet") && projectile.knockBack != -8 && config.enableShotgunRework)
            {
                friction = (projectile.velocity.Length() * Main.rand.NextFloat(0.01f, 0.03f)) / projectile.extraUpdates;
                isPellet = true;
                projectile.damage /= 2;
                projectile.velocity *= 0.8f;
                inst = Projectile.NewProjectileDirect(source,new Vector2(-2845,-12491),projectile.velocity.RotatedByRandom(Math.PI/8)*(Main.rand.NextFloat(0.4f)+0.8f),projectile.type,projectile.damage,-8,projectile.owner);
                inst.GetGlobalProjectile<ProjectileSounds>().isPellet = true;
                inst.GetGlobalProjectile<ProjectileSounds>().sourceToInherit = source;
                inst.GetGlobalProjectile<ProjectileSounds>().knockbackToInherit = projectile.knockBack;
                inst.position = projectile.position;
                inst.GetGlobalProjectile<ProjectileSounds>().item = item;
            }

        }
        
        if (projectile.type == ProjectileID.BulletHighVelocity && config.enableHighVelocityRework)
        {
            projectile.extraUpdates = 256;
        }
        startpos = projectile.Center;
    }

    public override void Kill(Projectile projectile, int timeLeft)
    {
        var config = SoundPackConfig.Instance;
        if (projectile.type == ProjectileID.BulletHighVelocity && config.enableHighVelocityRework)
        {
            float dist = startpos.Distance(projectile.Center);
            ParticleEntity.Instantiate<TracerParticle>(h =>
            {
                h.position = startpos;
                h.scale.X *= dist;
                h.rotation = startpos.DirectionTo(projectile.Center).ToRotation();
            });
            ParticleEntity.Instantiate<BulletDestroyParticle>(h =>
            {
                h.position = projectile.Center;
            });
        }
    }

    public override void AI(Projectile projectile)
    {
        var config = SoundPackConfig.Instance;
        spawnTime++;
        if (sourceToInherit != null && projectile.knockBack == -8 && config.enableShotgunRework)
        {
            
            friction = (projectile.velocity.Length()*Main.rand.NextFloat(0.01f,0.03f))/projectile.extraUpdates;
            isPellet = true;
            startpos = projectile.Center;
            projectile.knockBack = knockbackToInherit;
        }
        if (projectile.type == 933) //Zenith addendums.
        {

        }
        if (projectile.aiStyle == ProjAIStyleID.Spear && config.enableSpearRework)
        {
            if (item != null)
            {
                timer++;
                Player player = Main.player[projectile.owner];
                var threshhold = item.useTime / 2;
                if (timer <= player.itemAnimationMax)
                {
                    if (timer < threshhold)
                    {
                        player.itemAnimation = player.itemAnimationMax - threshhold + timer;
                        projectile.velocity = new Vector2(projectile.velocity.Length(),0).RotatedBy(player.MountedCenter.AngleTo(Main.MouseWorld));
                        player.direction = 1;
                        if (Main.MouseWorld.X < player.MountedCenter.X)
                            player.direction = -1;
                        projectile.direction = player.direction;
                    }
                    else
                    {
                        projectile.friendly = true;
                        if (!soundIsPlayed)
                        {
                            SoundEngine.PlaySound(item.UseSound);
                            soundIsPlayed = true;
                        }
                        player.itemAnimation--;
                        if (player.itemAnimation < (player.itemAnimationMax / 2)-5)
                        {
                            player.itemAnimation = (player.itemAnimationMax / 2)-5;
                        }
                    }
                } else
                {
                    player.itemAnimation--;
                }
            }
        }
        if (isPellet && config.enableShotgunRework)
        {
            var frictionVector = new Vector2(friction, 0);
            projectile.velocity -= frictionVector.RotatedBy(projectile.velocity.ToRotation());

            if (projectile.velocity.Length() <= 1)
            {
                Texture2D tex = (Texture2D)TextureAssets.Projectile[projectile.type];
                var col = TextureColorSystem.GetBrightestColor(TextureAssets.Projectile[projectile.type]);
                ParticleEntity.Instantiate<PelletDestroyParticle>(p =>
                {
                    p.position = projectile.Center;
                    p.rotation = projectile.velocity.ToRotation();
                    p.color = col;
                    p.velocity = projectile.velocity;
                    p.scale = new Vector2((float)tex.Height / 20, 1);
                    p.scale = new Vector2(1, 1);
                    //p.friction = frictionVector;
                });
                projectile.Kill();
            }
        }
        if (projectile.type == ProjectileID.BulletHighVelocity && config.enableHighVelocityRework)
        {
            if (spawnTime >= 192 || projectile.position.Y+projectile.velocity.Y <= 0)
            {
                projectile.Kill();
                float dist = startpos.Distance(projectile.Center);
                ParticleEntity.Instantiate<TracerParticle>(h =>
                {
                    h.position = startpos;
                    h.scale.X *= dist;
                    h.rotation = startpos.DirectionTo(projectile.Center).ToRotation();
                });
                ParticleEntity.Instantiate<BulletDestroyParticle>(h =>
                {
                    h.position = projectile.Center;
                });
            }
        }
    }
}

public class ProjectileRicochet : GlobalProjectile
{
    public static readonly SoundStyle hitWall = new SoundStyle("LizSoundPack/sounds/bulletHitWall") { Volume = 0.2f, PitchVariance = 0.25f, };
    public static readonly SoundStyle iceHitWall = new SoundStyle("LizSoundPack/sounds/iceHitWall") { Volume = 0.5f, PitchVariance = 0.25f, };
    public static readonly SoundStyle swingZenith = new SoundStyle("LizSoundPack/sounds/swingZenith") { Volume = 0.2f, PitchVariance = 0.4f, MaxInstances = 3,};
    public static Asset<Texture2D> Bullet1Flash = ModContent.Request<Texture2D>("LizSoundPack/Projectiles/BulletFlash");
    public static Asset<Texture2D> Pellet = ModContent.Request<Texture2D>("LizSoundPack/Projectiles/Pellet");
    public static Asset<Texture2D> CometCore = ModContent.Request<Texture2D>("LizSoundPack/Particles/Textures/CometBaseEffect");
    public static Asset<Texture2D> CometTail = ModContent.Request<Texture2D>("LizSoundPack/Particles/Textures/CometTrailTail");
    public static Asset<Texture2D> PelletBright = ModContent.Request<Texture2D>("LizSoundPack/Projectiles/PelletBright");

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        var config = SoundPackConfig.Instance;
        if (projectile.type == 933 && config.enableMeleeSounds)
            SoundEngine.PlaySound(swingZenith, projectile.Center);
    }
    public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
    {
        var config = SoundPackConfig.Instance;
        if (projectile.Name.ToLower().Contains("bullet") && projectile.DamageType.Equals(DamageClass.Ranged) && config.enableGunSounds)
            SoundEngine.PlaySound(hitWall, projectile.Center);
        if (projectile.Name.ToLower().Contains("ice") || projectile.Name.ToLower().Contains("frost") && config.enableElementalSounds)
        {
            SoundEngine.PlaySound(iceHitWall, projectile.Center);
        }
        if (projectile.GetGlobalProjectile<ProjectileSounds>().isPellet && config.enableShotgunRework)
        {
            var inst = projectile.GetGlobalProjectile<ProjectileSounds>();
            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                projectile.velocity.X = -oldVelocity.X;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }

            projectile.velocity *= 0.8f;
            for (int i = 0; i < 3; i++)
            {
                ParticleEntity.Instantiate<DustCloudParticle>(p =>
                {
                    p.position = projectile.position;
                    p.velocity = new Vector2(projectile.velocity.X * Main.rand.NextFloat(0.3f, 0.7f), projectile.velocity.Y * Main.rand.NextFloat(0.3f, 0.7f));
                    p.rotation = Main.rand.NextFloat((float)Math.PI);
                    p.scale *= Main.rand.NextFloat(1, 1.3f);
                    p.rotspd = 0.1f;
                    p.rotspdDecay = 1.01f;
                    p.shrinkspd = 0.04f;

                });
            }
            if (projectile.type == ProjectileID.BulletHighVelocity && config.enableHighVelocityRework)
            {
                float dist = inst.startpos.Distance(projectile.Center);
                ParticleEntity.Instantiate<TracerParticle>(h =>
                {
                    h.position = inst.startpos;
                    h.scale.X *= dist;
                    h.rotation = inst.startpos.DirectionTo(projectile.Center).ToRotation();
                });
                inst.startpos = projectile.position;
                return false;
            }
            return false;
        }

        return true;
    }

    public override void Load()
    {
        TextureAssets.Projectile[ProjectileID.Bullet] = ModContent.Request<Texture2D>("LizSoundPack/Projectiles/Bullet");
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        var config = SoundPackConfig.Instance;
        if (projectile.type == ProjectileID.Bullet && projectile.GetGlobalProjectile<ProjectileSounds>().isPellet)
        { 
            var inst = projectile.GetGlobalProjectile<ProjectileSounds>();
            var sb = Main.spriteBatch;
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
            Texture2D tex = (Texture2D)TextureAssets.Projectile[projectile.type];
            projectile.Size = new Vector2(8, 8);
            var origin = new Vector2(6, 24);
            if (inst.spawnTime > 4)
            {
                sb.Draw(tex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, tex.Size() / 2, 1, SpriteEffects.None, 0);
                sb.Draw(tex, projectile.Center - Main.screenPosition, null, new Color(25, 25, 25), projectile.rotation, tex.Size() / 2, 2, SpriteEffects.None, 0);
            }
            else
            {
                var item = projectile.GetGlobalProjectile<ProjectileSounds>().item;
                if (item != null)
                {
                    var guntip = Utilities.GetGunBarrelEndPosition(item.type, (Texture2D)TextureAssets.Item[item.type]);
                    var player = projectile.GetOwner();
                    if (player != null)
                    {
                        var pos = projectile.GetGlobalProjectile<ProjectileSounds>().startpos;
                        var vec = projectile.velocity;
                        vec.Normalize();
                        pos += (vec * guntip.Length());
                        pos.X += 2 * player.direction;
                        pos.Y += 2;
                        origin = new Vector2(10, 10);
                        sb.Draw((Texture2D)Bullet1Flash, pos - Main.screenPosition, null, lightColor, projectile.rotation, Bullet1Flash.Size() / 2, 1, SpriteEffects.None, 0);
                        sb.Draw((Texture2D)Bullet1Flash, pos - Main.screenPosition, null, new Color(25, 25, 25), projectile.rotation, Bullet1Flash.Size() / 2, 2, SpriteEffects.None, 0);
                    }
                }
            }
        }

        if (projectile.type == ProjectileID.BulletHighVelocity && config.enableHighVelocityRework)
            return false;

        if (projectile.GetGlobalProjectile<ProjectileSounds>().isPellet && config.enableShotgunRework)
        {
            var inst = projectile.GetGlobalProjectile<ProjectileSounds>();
            var sb = Main.spriteBatch;
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
            Texture2D tex = (Texture2D)TextureAssets.Projectile[projectile.type];
            Texture2D bulletTex = (Texture2D)Pellet;
            Texture2D bulletBrightTex = (Texture2D)PelletBright;
            var col = TextureColorSystem.GetBrightestColor(TextureAssets.Projectile[projectile.type]);
            projectile.Size = new Vector2(8, 8);
            var origin = new Vector2(6, 24);
            var scale = new Vector2(1, 1);
            if (tex.Width < 8 || tex.Height < 8)
            { 
                sb.Draw(bulletTex, projectile.Center - Main.screenPosition, null, col, projectile.rotation, bulletTex.Size() / 2, scale, SpriteEffects.None, 0);
                sb.Draw(bulletBrightTex, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, bulletTex.Size() / 2, scale, SpriteEffects.None, 0);
                sb.Draw(bulletTex, projectile.Center - Main.screenPosition, null, new Color(15, 15, 15).MultiplyRGB(col), projectile.rotation, bulletTex.Size() / 2, scale * 1.5f, SpriteEffects.None, 0);
                sb.Draw(bulletBrightTex, projectile.Center - Main.screenPosition, null, new Color(15, 15, 15), projectile.rotation, bulletTex.Size() / 2, scale * 1.5f, SpriteEffects.None, 0);
            }
        }

        if (projectile.type == 16)
        {
            for (int i = 0; i < 6; i++)
            {
                ParticleEntity.Instantiate<CometNebulaParticle>(p =>
                {
                    p.position = projectile.Center + new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 8));
                    p.velocity = new Vector2(1, 0).RotatedBy(Main.rand.NextFloat((float)(Math.PI * 2)));
                    p.rotation = Main.rand.NextFloat((float)Math.PI);
                    p.scale *= Main.rand.NextFloat(0.8f, 1.6f);
                    p.rotspd = 0.02f;
                    p.shrinkspd = 0.02f;
                    p.velocityDecay = 0.9f;
                    p.back = true;
                    if (Main.rand.NextBool())
                    {
                        p.additive = false;
                    } else
                    {
                        p.additive = true;
                    }
                });
            }
            if (Main.rand.NextBool(2))
            {
                ParticleEntity.Instantiate<CometStarParticle>(p =>
                {
                    p.position = projectile.Center + new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 8));
                    p.velocity = new Vector2(1, 0).RotatedBy(Main.rand.NextFloat((float)(Math.PI * 2)));
                    p.velocityDecay = 0.98f;
                    p.scale *= Main.rand.NextFloat(1, 1.3f);
                    p.shrinkspd = 0.01f;
                    p.scale *= 0.4f;
                });
            }
            var sb = Main.spriteBatch;
            var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
            Texture2D tex = (Texture2D)CometCore;
            Texture2D tailtex = (Texture2D)CometTail;
            projectile.Size = new Vector2(20, 20);
            var origin = new Vector2(6, 24);
            var dist = projectile.Center - projectile.position;
            sb.Draw(tex, projectile.Center - Main.screenPosition, null, lightColor, projectile.timeLeft/4, tex.Size() / 2, 1.2f, SpriteEffects.None, 0);
            for (var i = 1; i < projectile.oldPos.Length; i++)
            {
                if (projectile.oldPos[i] != new Vector2(0, 0))
                {
                    var scale = new Vector2(projectile.oldPos[i].Distance(projectile.oldPos[i - 1]) / 1.5f, 0.6f - 0.05f * i);
                    var pos = projectile.oldPos[i] + dist;
                    var angle = projectile.oldPos[i].AngleTo(projectile.oldPos[i - 1]);
                    if (scale.Y > 0)
                    {
                        var colvalue = 0.5f * (1f - (i * 0.05f));
                        sb.Draw(tailtex, pos - Main.screenPosition, null, lightColor, angle, new Vector2(0, tailtex.Height / 2), scale, SpriteEffects.None, 0);
                    }
                }
            }
            /*for (int i = 0; i < 3; i++)
            {
                ParticleEntity.Instantiate<CometTrailParticle>(p =>
                {
                    p.position = projectile.Center;
                    p.velocity = new Vector2(-Main.rand.NextFloat(1,2f), 0).RotatedBy(projectile.rotation);
                    p.rotation = Main.rand.NextFloat((float)Math.PI);
                    p.scale *= Main.rand.NextFloat(1, 1.3f);
                    p.scale *= 0.8f;
                    p.shrinkspd = 0.02f;
                    p.owner = projectile;
                    p.alpha = 0.001f;
                });
            }*/
            
            return false;
        }
        return true;
    }
}

public class PopBullet : ModProjectile
{
    public override void SetStaticDefaults()
    {
        DisplayName.SetDefault("Pop Bullet"); // The English name of the projectile
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
    }
    public static Asset<Texture2D> PopBulletFlash = ModContent.Request<Texture2D>("LizSoundPack/Projectiles/PopBulletFlash");
    public static bool canDupe = true;
    public static Projectile? child;

    public override void SetDefaults()
    {
        Projectile.width = 8; // The width of projectile hitbox
        Projectile.height = 8; // The height of projectile hitbox
        Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
        Projectile.friendly = true; // Can the projectile deal damage to enemies?
        Projectile.hostile = false; // Can the projectile deal damage to the player?
        Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
        Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
        Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
        Projectile.light = 0.5f; // How much light emit around the projectile
        Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
        Projectile.tileCollide = true; // Can the projectile collide with tiles?
        Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame

        AIType = ProjectileID.Bullet; // Act exactly like default Bullet
    }

    public override void OnSpawn(IEntitySource source)
    {
        // Rotate the velocity randomly by 30 degrees at max.
        Vector2 newVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15));

        // Decrease velocity randomly for nicer visuals.
        newVelocity *= 1f - Main.rand.NextFloat(0.3f);
        if (source is EntitySource_ItemUse_WithAmmo && canDupe)
        {
            Main.player[Projectile.owner].itemTime += 1;
            child = Projectile.NewProjectileDirect(source, Projectile.position, newVelocity, ModContent.ProjectileType<PopBulletSplit>(), Projectile.damage, Projectile.knockBack);
            newVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15));

            // Decrease velocity randomly for nicer visuals.
            newVelocity *= 1f - Main.rand.NextFloat(0.3f);
            Projectile.velocity = newVelocity;

        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

        // If the projectile hits the left or right side of the tile, reverse the X velocity
        if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
        {
            Projectile.velocity.X = -oldVelocity.X;
        }

        // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
        if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
        {
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        Projectile.velocity *= 0.8f;

        return false;
    }

    public override void AI()
    {
        var friction = Projectile.velocity;
        friction.Normalize();
        friction *= 0.2f;
        if (child != null)
            child.damage = Projectile.damage;
        Projectile.velocity -= friction;
        if (Projectile.velocity.Length() < 2)
        {
            Projectile.Kill();
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var inst = Projectile.GetGlobalProjectile<ProjectileSounds>();
        var sb = Main.spriteBatch;
        var trans = Main.GameViewMatrix != null ? Main.GameViewMatrix.TransformationMatrix : Matrix.Identity;
        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.Identity);
        Texture2D tex = (Texture2D)TextureAssets.Projectile[Projectile.type];
        var origin = new Vector2(8, 24);
        if (inst.spawnTime < 4)
        {
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, 1, SpriteEffects.None, 0);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(25, 25, 25), Projectile.rotation, tex.Size() / 2, 2, SpriteEffects.None, 0);
        }
        else
        {
            var pos = Projectile.GetGlobalProjectile<ProjectileSounds>().startpos;
            var vec = Projectile.velocity;
            pos += (vec * 4);
            pos.X += 2;
            pos.Y += 2;
            origin = new Vector2(10, 10);
            sb.Draw((Texture2D)PopBulletFlash, pos - Main.screenPosition, null, lightColor, Projectile.rotation, PopBulletFlash.Size() / 2, 1, SpriteEffects.None, 0);
            sb.Draw((Texture2D)PopBulletFlash, pos - Main.screenPosition, null, new Color(25, 25, 25), Projectile.rotation, PopBulletFlash.Size() / 2, 2, SpriteEffects.None, 0);
        }

        return true;
    }

    public override void Kill(int timeLeft)
    {
        // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
        Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
    }

    public class PopBulletSplit : PopBullet
    {
        //public static bool canDupe = false;
        public override void OnSpawn(IEntitySource source)
        {
            //Nothing.
        }
    }
}
