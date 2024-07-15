using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.NPCs;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_FistHit_ChangeDashFist : FrostFist_FistHit
    {
        public bool NoMana;
        public FrostFist_FistHit_ChangeDashFist(FrostFistProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile, activationConditionFunc)
        {
            ActionDamage = 1.2f;
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
            if ((int)Projectile.ai[0] == -1)
            {
                Player.velocity.X = -Player.direction * (Projectile.ai[1] * 0.2f + 3);
                if(Projectile.ai[1]++ > 30)
                {
                    Projectile.ai[1] = 0;
                    Projectile.ai[0]++;
                }
                return;
            }
            Projectile.ai[0]++;
            #region 蓄力
            if ((!Player.controlUseTile && Projectile.ai[0] < 6) || NoMana) // 取消松手的时候
            {
                if (Projectile.ai[0] == 5)
                {
                    Vector2 shootVel = vel.SafeNormalize(default) * 6;
                    float addDmg = (int)Projectile.ai[2] switch
                    {
                        1 => 0.5f,
                        2 => 0.8f,
                        3 => 1.7f,
                        _ => 0
                    };
                    Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj>(), (int)(Projectile.damage * (ActionDamage + addDmg)), Projectile.knockBack, Player.whoAmI);
                    proj.alpha = 0;
                    OnHit = OnHitNPC;
                    //SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Projectile.Center, shootVel, Projectile.damage, Projectile.knockBack, Player.whoAmI, 60, 200);

                }
            }
            else if (Player.controlUseTile && Projectile.ai[0] < 6)
            {
                Player.manaRegenDelay += 6;
                Projectile.ai[0]--;
                Projectile.ai[1]++;
                switch ((int)Projectile.ai[2])
                {
                    case 0: // 蓄力0
                    case 1: // 蓄力1
                        {
                            if (Projectile.ai[1] > 30)
                            {
                                Projectile.ai[1] = 0;
                                Projectile.ai[2]++;
                            }
                            break;
                        }
                    case 2: // 蓄力2
                        {
                            if (Projectile.ai[1] > 60)
                            {
                                Projectile.ai[1] = 0;
                                Projectile.ai[2]++;
                            }

                            break;
                        }
                }
                switch ((int)Projectile.ai[2])
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
                            NoMana = !Player.CheckMana(3, true);
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
                            NoMana = !Player.CheckMana(5, true);
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
            if (Projectile.ai[0] < 5)
            {
                PreAtk = true;
            }
            else
            {
                PreAtk = false;
            }
            if (Projectile.ai[0] > 40)
            {
                SkillTimeOut = true;
            }
            else if (Projectile.ai[0] > 20)
            {
                CanChangeToStopActionSkill = true;
            }
            #region 原本额外动作的位置
            if (Projectile.ai[0] < 16 && Projectile.ai[0] > 6)
            {
                Player.SetImmuneTimeForAllTypes(3);
                Player.velocity.X = Player.direction * 50;
                for (int i = 0; i < 80; i++)
                {
                    Dust dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue);
                    dust.position = Player.Center;
                    dust.scale = 0.3f;
                    vel = Player.velocity.RotatedBy(i / 80f * MathHelper.TwoPi).SafeNormalize(default) * 3.5f;
                    vel.X *= 0.2f;
                    vel = vel.RotatedBy(Player.velocity.ToRotation());
                    dust.velocity = vel;
                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    //dust.velocity += Projectile.velocity.RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f) * 1.2f;
                }
            }
            else
            {
                Player.velocity.X *= 0.1f;
            }
            #endregion
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            switch ((int)Projectile.ai[2])
            {
                case 2: // 二连爆
                    {
                        FrostFist_FistBoom frostFist_FistBoom = new(10, Player, new Vector2(hit.HitDirection,0), (int)(Projectile.damage * 2));
                        FrostFist_FistBoom frostFist_FistBoom1 = new(20, Player, new Vector2(hit.HitDirection,0), (int)(Projectile.damage * 2));
                        WeaponSkillGlobalNPC.AddComponent(target, frostFist_FistBoom);
                        WeaponSkillGlobalNPC.AddComponent(target, frostFist_FistBoom1);
                        break;
                    }
                case 3: // 冰冻爆
                    {
                        FrostFist_FistBoom frostFist_FistBoom = new(10, Player, new Vector2(hit.HitDirection, 0), (int)(Projectile.damage * 2));
                        FrostFist_FistBoom frostFist_FistBoom1 = new(20, Player, new Vector2(hit.HitDirection, 0), (int)(Projectile.damage * 2))
                        {
                            ExtraAI = (NPC npc) =>
                            {
                                npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime += 120;
                            }
                        };
                        WeaponSkillGlobalNPC.AddComponent(target, frostFist_FistBoom);
                        WeaponSkillGlobalNPC.AddComponent(target, frostFist_FistBoom1);
                        break;
                    }
            }
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = -1;
            NoMana = false;
        }
    }
}
