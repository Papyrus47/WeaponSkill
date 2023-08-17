using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;
using WeaponSkill.Particles;
using Terraria.ID;

namespace WeaponSkill.Weapons.Spears.Skills
{
    public class SpearsSpurts_Channel : BasicSpearsSkill
    {
        public SpearsSpurts_Channel(SpearsProj SpearsProj, Func<int> channelMaxTime, Func<int> spurtsUseTime, Func<int> addDamage) : base(SpearsProj)
        {
            ChannelMaxTime = channelMaxTime;
            SpurtsUseTime = spurtsUseTime;
            AddDamage = addDamage;
            if(ChannelMaxTime() == 0)
            {
                Projectile.Kill();
            }
            InChannelSound = new("WeaponSkill/Sounds/Spears/Spears_InChannel");
            ChannelEndSound = new("WeaponSkill/Sounds/Spears/Spears_ChannelEnd_Small");
            NoChannelMaxSound = new("WeaponSkill/Sounds/Spears/SpearsSpurts_NoChannel");
        }
        public Func<int> ChannelMaxTime;
        public Func<int> SpurtsUseTime;
        public Func<int> AddDamage;
        public const int SKILLTIMEOUT_TIME = 15;
        public const int SKILLCANCHANGE_TIME = 3;
        public SoundStyle InChannelSound,ChannelEndSound,NoChannelMaxSound;
        public override void AI()
        {
            player.heldProj = Projectile.whoAmI;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction);
            player.itemTime = player.itemAnimation = 2;
            switch ((int)Projectile.ai[0])
            {
                case 0: // 蓄力
                    {
                        Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
                        Projectile.spriteDirection = player.direction;
                        Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(default);
                        Projectile.rotation += MathHelper.WrapAngle(Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2 - Projectile.rotation - (player.direction == 1 ? 0 : MathHelper.PiOver2)) * 0.1f;
                        Projectile.ai[1]++;
                        if ((int)Projectile.ai[1] == ChannelMaxTime()) // 生成粒子
                        {
                            SpearsStar spearsStar = new(player, new Vector2(0.6f,1f) * 3f)
                            {
                                ScaleVelocity = new Vector2(0.3f,1) * -0.25f,
                                TimeLeft = 12
                            };
                            Main.ParticleSystem_World_OverPlayers.Add(spearsStar);
                        }
                        else if ((int)Projectile.ai[1] == 2)
                        {
                            SoundEngine.PlaySound(InChannelSound, Projectile.Center);
                        }
                        if (Projectile.ai[1] > ChannelMaxTime() + 14|| !player.controlUseItem)
                        {
                            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(default);
                            if ((int)Projectile.ai[1] >= ChannelMaxTime() && Projectile.ai[1] < ChannelMaxTime() + 12)
                            {
                                Projectile.damage += AddDamage();
                                SoundEngine.PlaySound(ChannelEndSound, Projectile.Center);
                            }
                            else
                            {
                                SoundEngine.PlaySound(NoChannelMaxSound, Projectile.Center);
                            }
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                            TheUtility.ResetProjHit(Projectile);
                        }
                        break;
                    }
                case 1: // 突刺
                    {
                        player.ChangeDir(Projectile.direction);
                        Projectile.rotation += MathHelper.WrapAngle(Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2 - Projectile.rotation - (player.direction == 1 ? 0 : MathHelper.PiOver2)) * 0.5f;
                        Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * SpearsProj.WeaponLength * ((Projectile.ai[1] / SpurtsUseTime()) - 0.5f);
                        if (Projectile.ai[1] > SpurtsUseTime())
                        {
                            Projectile.ai[2]++;
                            if (Projectile.ai[2] > SKILLTIMEOUT_TIME)
                            {
                                SkillTimeOut = true;
                            }
                            else if (Projectile.ai[2] == 1)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * SpearsProj.WeaponLength * 0.05f, SpearsProj.SpawnItem_OriginShootProj, Projectile.damage, Projectile.knockBack, player.whoAmI);
                            }
                        }
                        else
                        {
                            Projectile.ai[1]++;
                        }
                        break;
                    }
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            if ((int)Projectile.ai[0] == 0)
            {
                Texture2D tex2 = TextureAssets.Extra[174].Value;
                sb.Draw(tex2, player.Center - Main.screenPosition, null, new Color(150, 150, 255, 0) * (Projectile.ai[1] / ChannelMaxTime()), 0, tex2.Size() * 0.5f, Math.Max(0,1 - (Projectile.ai[1] / ChannelMaxTime())) * 2f, SpriteEffects.None, 0f);
            }
            Main.instance.LoadProjectile(SpearsProj.SpawnItem_OriginShootProj);
            var tex = TextureAssets.Projectile[SpearsProj.SpawnItem_OriginShootProj].Value;
            Rectangle rect = tex.Frame(SpearsProj.Projectile.frame + 1, Main.projFrames[SpearsProj.Type]);
            float rot = Projectile.rotation;
            if (SpearsProj.SpawnItem.type == ItemID.MonkStaffT2) rot -= MathHelper.PiOver2 * Projectile.spriteDirection;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor,rot , tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            if (Projectile.ai[2] > 0)
            {
                SpurtsDraw spurtsDraw = new()
                {
                    Direction = Projectile.velocity,
                    Width = SpearsProj.WeaponLength * 2,
                    DrawPos = Projectile.Center,
                    Height = Projectile.height * Math.Max(0, (1 - (Projectile.ai[2] / SKILLCANCHANGE_TIME / 3))),
                    DrawColor = new Color(255, 255, 255, 255),
                    ScreenCorrect = true
                };
                Main.graphics.GraphicsDevice.Textures[2] = SpearsProj.DrawColorTex;
                spurtsDraw.Draw(Main.spriteBatch, WeaponSkill.SpurtsShader.Value);
            }
            return false;
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 4, 5, 3,-1,"SSC_OnHit"));
        }
        public override bool ActivationCondition()
        {
            return player.controlUseItem;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[2] > SKILLCANCHANGE_TIME;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projHitbox.Center(), projHitbox.Center() + Projectile.velocity * SpearsProj.WeaponLength * 2,
                Projectile.height / 2f, ref r);
        }
        public override void OnSkillActive()
        {
            SkillTimeOut = false;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            Projectile.rotation = 0;
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            Projectile.damage = Projectile.originalDamage;
            Projectile.rotation = 0;
        }
    }
}
