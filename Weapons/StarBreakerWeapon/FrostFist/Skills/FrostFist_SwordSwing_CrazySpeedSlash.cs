using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_SwordSwing_CrazySpeedSlash : FrostFist_SwordSwing_SpeedSlash
    {
        public FrostFist_SwordSwing_CrazySpeedSlash(FrostFistProj modProjectile) : base(modProjectile, null)
        {
        }
        public int ChannelTime;
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
                case 3:
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
                            if ((int)Projectile.ai[0] == 1)
                            {
                                for (int i = 0; i < 1000; i++)
                                {
                                    if (!Player.CheckMana(1))
                                    {
                                        break;
                                    }
                                    Player.statMana--;
                                    SlashCounts++;
                                }
                            }
                            for (int i = 0; i < SlashCounts / 5; i++)
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
                                proj.OnHit = (npc,hit,dmg) =>
                                {
                                    for(int i = 0; i < 5; i++)
                                    {
                                        Player.ApplyDamageToNPC(npc, dmg, 0f, hit.HitDirection, false);
                                        SlashDamage.SlashDamageOnHit();
                                    }
                                };
                            }
                        }
                        break;
                    }
                case 1: // 挥舞
                case 4:
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
                case 5:
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
                            if (Projectile.ai[0] == 2)
                            {
                                Projectile.ai[0]++;
                                TheUtility.ResetProjHit(Projectile);
                            }
                        }
                        break;
                    }
            }
        }
        public override bool SwitchCondition() => Projectile.ai[0] > 4 && Projectile.ai[2] > 10;
        public override bool ActivationCondition()
        {
            if (WeaponSkill.BowSlidingStep.Current && FrostFist.frostFistItem.ChangeLevel == 20)
            {
                ChannelTime++;
                if (ChannelTime > 60)
                {
                    Player.velocity *= 0.2f;
                    Vector2 center = Player.HandPosition.Value;
                    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.FrostStaff, Player.direction * 2, 0, 150, default, 1.3f);
                    dust.position = center + Vector2.One.RotatedBy(ChannelTime % 20f / 20f * MathHelper.TwoPi) * (ChannelTime - 180) * 0.4f;
                    dust.velocity *= 0;
                    dust.noGravity = true;
                    dust.fadeIn = 1;
                    dust.velocity += Player.velocity * 0.1f;

                    dust = Dust.NewDustDirect(center, 1, 1, DustID.FrostStaff, Player.direction * 2, 0, 150, default, 1.3f);
                    dust.position = center + Vector2.One.RotatedBy(ChannelTime % 20f / 20f * MathHelper.TwoPi) * (ChannelTime - 180) * -0.4f;
                    dust.velocity *= 0;
                    dust.noGravity = true;
                    dust.fadeIn = 1;
                    dust.velocity += Player.velocity * 0.1f;
                }
            }
            else ChannelTime = 0;
            return ChannelTime > 180 && Player.CheckMana(100);
        }

        public override void OnSkillActive()
        {
            base.OnSkillActive();
            SlashCounts = 0;
            FrostFist.frostFistItem.ChangeLevel = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            ChannelTime = 0;
        }
    }
}
