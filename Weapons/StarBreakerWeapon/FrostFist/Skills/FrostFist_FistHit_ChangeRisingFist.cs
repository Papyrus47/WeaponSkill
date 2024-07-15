using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.NPCs;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    /// <summary>
    /// 蓄力升龙拳
    /// </summary>
    public class FrostFist_FistHit_ChangeRisingFist : FrostFist_FistHit
    {
        public bool NoMana;
        public FrostFist_FistHit_ChangeRisingFist(FrostFistProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile, activationConditionFunc)
        {
            ActionDamage = 1f;
        }

        public override void AI()
        {
            Projectile.Center = Player.Center;
            Vector2 vel = (Main.MouseWorld - Projectile.Center);
            Player.velocity.X *= 0.2f;
            Player.velocity.Y = 0;
            Player.ChangeDir((vel.X > 0).ToDirectionInt());
            Projectile.direction = Player.direction;
            Player.itemTime = Player.itemAnimation = 2;
            Player.itemRotation = MathF.Atan2(vel.Y * Projectile.direction, vel.X * Projectile.direction);
            float addDmg = Projectile.ai[2] * 0.5f;
            Projectile.ai[0]++;
            #region 产生弹幕
            if ((!Player.controlUseItem && Projectile.ai[0] < 6) || NoMana) // 取消松手的时候
            {
                if (Projectile.ai[0] == 2)
                {
                    Vector2 shootVel = vel.SafeNormalize(default) * 6;
                    Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj_MoveFist>(), (int)(Projectile.damage * (ActionDamage + addDmg)), Projectile.knockBack, Player.whoAmI);
                    proj.alpha = 0;
                    proj.timeLeft = 120;
                    proj.extraUpdates += (int)(Projectile.ai[2] - 1);
                    OnHit = OnHitNPC;
                    (proj.ModProjectile as FrostFist_Proj_FistHitProj_MoveFist).MoveFistAI = (proj) =>
                    {
                        Vector2 StartVel = Vector2.UnitY * 60;
                        Vector2 vel = StartVel.RotatedBy(-Player.direction * MathF.Sqrt(proj.ai[0] / 120f) * MathHelper.Pi * 1.4f);
                        Vector2 center = Player.Center + vel;
                        proj.velocity = center - proj.Center;
                        if (proj.ai[0] > 59)
                        {
                            proj.ai[0] = 60;
                            proj.extraUpdates = 0;
                            proj.timeLeft = 5;
                            Projectile.ai[0] = 14;
                            if (proj.ai[1]++ > 3)
                            {
                                proj.Kill();
                            }
                        }
                    };
                    //SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Projectile.Center, shootVel, Projectile.damage, Projectile.knockBack, Player.whoAmI, 60, 200);

                }
            }
            else if (Player.controlUseItem && Projectile.ai[0] < 6)
            {
                Player.manaRegenDelay += 3;
                Projectile.ai[0]--;
                Projectile.ai[1]++;
                switch ((int)Projectile.ai[2])
                {
                    case 0: // 蓄力0
                    case 1: // 蓄力1
                        {
                            if (Projectile.ai[1] > 15)
                            {
                                Projectile.ai[1] = 0;
                                Projectile.ai[2]++;
                            }
                            break;
                        }
                    case 2: // 蓄力2
                        {
                            if (Projectile.ai[1] > 30)
                            {
                                Projectile.ai[1] = 0;
                                Projectile.ai[2]++;
                            }

                            break;
                        }
                }
                switch (Projectile.ai[2])
                {
                    case 1:
                        {
                            NoMana = !Player.CheckMana(1, true);
                            for (int i = 0; i < 32; i++)
                            {
                                Dust dust = FrostFistDust();
                                dust.velocity = -vel.SafeNormalize(default).RotatedByRandom(0.2) * i * 0.3f;
                                dust.position = Player.HandPosition.Value + vel.SafeNormalize(default) * Player.width;
                            }

                            break;
                        }

                    case 2:
                        {
                            NoMana = !Player.CheckMana(2, true);
                            for (int i = 0; i < 32; i++)
                            {
                                Dust dust = FrostFistDust();
                                dust.velocity = -vel.SafeNormalize(default).RotatedByRandom(0.2).RotatedBy(MathF.Sin(i / 3f * MathHelper.TwoPi) * 0.5f) * i * 0.3f;
                                dust.position = Player.HandPosition.Value + vel.SafeNormalize(default) * Player.width;
                            }

                            break;
                        }

                    case 3:
                        {
                            NoMana = !Player.CheckMana(3, true);
                            for (int i = 0; i < 70; i++)
                            {
                                Dust dust = FrostFistDust();
                                dust.velocity = -vel.SafeNormalize(default).RotatedByRandom(0.8) * i * 0.3f;
                                dust.position = Player.HandPosition.Value + vel.SafeNormalize(default) * Player.width;
                            }

                            break;
                        }
                }
            }
            #endregion
            if (Projectile.ai[0] < 7)
            {
                PreAtk = true;
            }
            else
            {
                PreAtk = false;
            }
            if (Projectile.ai[0] > 30)
            {
                SkillTimeOut = true;
            }
            else if (Projectile.ai[0] > 10)
            {
                CanChangeToStopActionSkill = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (target.knockBackResist != 0)
            {
                target.velocity.X = 0;
                target.velocity.Y = -6 * Projectile.ai[2];
            }
            if (Projectile.ai[2] > 2)
            {
                float addDmg = Projectile.ai[2] * 0.5f;
                FrostFist_FistBoom frostFist_FistBoom = new(10, Player, new Vector2(0, 10), (int)(Projectile.damage * (ActionDamage + addDmg)));
                FrostFist_FistBoom frostFist_FistBoom1 = new(35, Player, new Vector2(0, 20), target.life / 10)
                {
                    ExtraAI = (NPC npc) =>
                    {
                        npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime += 120;
                    }
                };
                WeaponSkillGlobalNPC.AddComponent(target, frostFist_FistBoom);
                if (target.knockBackResist != 0)
                    WeaponSkillGlobalNPC.AddComponent(target, frostFist_FistBoom1);
            }
        }
        //public override bool ActivationCondition()
        //{
        //    return Player.controlUseTile;
        //}
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[1] = Projectile.ai[2] = 0;
            NoMana = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[1] = Projectile.ai[2] = 0;
        }
    }
}
