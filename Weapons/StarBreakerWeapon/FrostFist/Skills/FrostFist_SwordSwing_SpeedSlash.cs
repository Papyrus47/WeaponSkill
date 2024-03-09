using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_SwordSwing_SpeedSlash : FrostFist_SwordSwing
    {
        public class SpeedSlashProj: ModProjectile
        {
            public FrostFistProj fistProj;
            public Player player;
            public SwingHelper swing;
            public float Rot;
            public bool SwingDirectionChange;
            public Func<float, float> TimeChange;
            public float SwingTime;
            public float VelRot;
            public Action<NPC, NPC.HitInfo, int> OnHit;
            public override string Texture => "Terraria/Images/Item_0";
            public override void SetDefaults()
            {
                Projectile.penetrate = -1;
                Projectile.tileCollide = false;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = -1;
                Projectile.friendly = true;
                Projectile.aiStyle = -1;
                swing = new(Projectile, 15);
                Projectile.extraUpdates = 2;
            }
            public override bool ShouldUpdatePosition() => false;
            public override void AI()
            {
                Projectile.ai[1]++;
                swing.ProjFixedPlayerCenter(player, 0, true);
                swing.SetRotVel(VelRot);
                swing.SwingAI(fistProj.SwordLength, player.direction, Rot * SwingDirectionChange.ToDirectionInt() * TimeChange.Invoke(Projectile.ai[1] / SwingTime));
                if(Projectile.ai[1] / SwingTime > 1)
                {
                    Projectile.Kill();
                    SoundEngine.PlaySound(SoundID.Item19 with { Pitch = 0.5f,MaxInstances = 3 }, Projectile.Center);
                }
                if (Projectile.ai[2] > 30)
                {
                    Projectile.hide = true;
                }
            }
            public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(default), 0.2f, 0.1f, 1, -1));
                OnHit?.Invoke(target, hit, damageDone);
            }
            public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swing.GetColliding(targetHitbox);
            public override bool PreDraw(ref Color lightColor)
            {
                swing.Swing_TrailingDraw(WeaponSkill.SwingTex.Value, (_) => new Color(0.2f, 0.4f, 0.8f, 0f) * 1.4f, null);
                return false;
            }
        }
        public FrostFist_SwordSwing_SpeedSlash(FrostFistProj modProjectile, Func<bool> changeCondition) : base(modProjectile, changeCondition)
        {
            SlashCounts = 30;
        }
        public int SlashCounts;
        public override void AI()
        {
            SwingAI?.Invoke();
            Projectile.spriteDirection = Player.direction * SwingDirectionChange.ToDirectionInt();
            for (int i = 0; i < 4; i++)
            {
                Dust dust = FrostFistDust();
                dust.velocity = -Projectile.velocity.SafeNormalize(default) * i * 0.6f;
                dust.position = Player.HandPosition.Value + Projectile.velocity.SafeNormalize(default) * Player.width;
            }
            switch ((int)Projectile.ai[0])
            {
                case 0: // 攻击的前摇
                    {
                        PreAtk = true;
                        float time = 3f / PreAtkTime;
                        swingHelper.Change_Lerp(StartVel, time, VelScale, time, VisualRotation, time);
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, 0);
                        if (Projectile.ai[1]++ > PreAtkTime) // 大于前摇时间
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 2;
                            TheUtility.ResetProjHit(Projectile);
                            for (int i = 0; i < SlashCounts; i++)
                            {
                                SpeedSlashProj proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<SpeedSlashProj>(),
                                     (int)(Projectile.damage + AddDmg), 0f, Player.whoAmI).ModProjectile as SpeedSlashProj;
                                proj.player = Player;
                                float rand = Main.rand.NextFloat(0.2f, 1f);
                                proj.swing.Change(StartVel, new Vector2(1 + rand * 0.6f, rand), 1f - rand);
                                proj.VelRot = Main.rand.NextFloatDirection() * MathHelper.PiOver4;
                                proj.Rot = SwingRot;
                                proj.TimeChange = TimeChange;
                                proj.SwingTime = AtkTime * Main.rand.NextFloat(0.7f, 1.2f);
                                proj.fistProj = FrostFist;
                                proj.SwingDirectionChange = Main.rand.NextBool();
                                proj.Projectile.ai[2] = i;
                            }
                        }
                        break;
                    }
                case 1: // 挥舞
                    {
                        PreAtk = false;
                        Projectile.ai[1]++;
                        Player.heldProj = Projectile.whoAmI;
                        float Time = TimeChange.Invoke(Projectile.ai[1] / AtkTime);
                        if (Time > 1)
                        {
                            //if (Projectile.ai[2] < 2)
                            //{
                            //    Projectile.ai[0] = 0;
                            //    Projectile.ai[2]++;
                            //}
                            //else
                            //{
                                Projectile.ai[2] = 0;
                                Projectile.ai[0]++;
                            //}
                        }
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        break;
                    }
                case 2: // 后摇
                    {
                        Projectile.extraUpdates = 0;
                        float Time = TimeChange.Invoke(Projectile.ai[1] / AtkTime);
                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        Projectile.ai[2]++;
                        if (Projectile.ai[2] > PostAtkTime)
                        {
                            SkillTimeOut = true;
                        }
                        else if (Projectile.ai[2] > PostAtkTime / 3)
                        {
                            CanChangeToStopActionSkill = true;
                        }
                        break;
                    }
            }
        }
        public override bool ActivationCondition()
        {
            return base.ActivationCondition() && FrostFist.frostFistItem.ChangeLevel >= 5;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            FrostFist.frostFistItem.ChangeLevel -= 5;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            for(int i = 0;i< 29; i++)
            {
                Player.ApplyDamageToNPC(target, damageDone, 0f, Player.direction, false);
            }
        }
    }
}
