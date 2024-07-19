using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.NPCs;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_SwordSwing_CutItInTwo : FrostFist_SwordSwing
    {
        public FrostFist_SwordSwing_CutItInTwo(FrostFistProj modProjectile, Func<bool> changeCondition) : base(modProjectile, changeCondition)
        {
            VelScale = new Vector2(1, 0.4f);
            VisualRotation = 0.6f;
            StartVel = -Vector2.UnitX;
            SwingRot = MathHelper.TwoPi * 0.75f;
            SwingDirectionChange = false;
            AddDmg = -0.9f;
            PreAtkTime = 90;
            AtkTime = 35;
            PostAtkTime = 20;
            HitNPCs = new();
        }
        public List<NPC> HitNPCs;
        public override void AI()
        {
            SwingAI?.Invoke();
            Projectile.spriteDirection = Player.direction * SwingDirectionChange.ToDirectionInt();
            HitNPCs.ForEach(n => n.GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime = 2);
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
                            SoundEngine.PlaySound(SoundID.Item19 with { Pitch = 0.5f, MaxInstances = 3 }, Projectile.Center);
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
                            Projectile.ai[0]++;
                        }
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());

                        //for (int i = 0; i < 30; i++)
                        //{
                        //    Vector2 center = Player.HandPosition.Value - Player.velocity;
                        //    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.Frost, Player.direction * 2, 0, 150, default, 1.3f);
                        //    dust.position = center;
                        //    dust.velocity *= 0;
                        //    dust.noGravity = true;
                        //    dust.fadeIn = 1;
                        //    dust.velocity = new Vector2(Projectile.velocity.Y,-Projectile.velocity.X).SafeNormalize(default).RotatedBy(i / 30f * MathHelper.Pi) * 6;
                        //    dust.velocity += Player.velocity;
                        //    dust.position = Projectile.Center + Projectile.velocity * i * 0.035f;
                        //}
                        break;
                    }
                case 2: // 后摇
                case 6:
                    {
                        if (Projectile.ai[0] < 3 && HitNPCs.Count > 0)
                        {
                            swingHelper.Change_Lerp(-Vector2.UnitY, 0.6f, Vector2.One, 0.6f);
                            if (Projectile.ai[2]++ > PostAtkTime / 3)
                            {
                                Projectile.ai[0]++;
                                Projectile.ai[1] = -40;
                                Projectile.extraUpdates = 2;
                                break;
                            }
                        }
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
                case 3: // 转刀
                    {
                        Projectile.spriteDirection = Player.direction * -SwingDirectionChange.ToDirectionInt();
                        Player.velocity.X *= 0;
                        Projectile.ai[1]++;
                        Player.heldProj = Projectile.whoAmI;
                        Projectile.extraUpdates = 3;
                        float Time = Math.Max(Projectile.ai[1],0)/ 40f;
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * MathHelper.TwoPi * -SwingDirectionChange.ToDirectionInt());
                        if ((int)Projectile.ai[1] == 40 || (int)Projectile.ai[1] == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item19 with { Pitch = 0.5f, MaxInstances = 3 }, Projectile.Center);
                        }
                        if(Time > 2)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 1;
                        }
                        break;
                    }
                case 4: // 举起刀
                    {
                        Player.velocity.X *= 0;
                        float time = 2f / PreAtkTime;
                        Projectile.spriteDirection = Player.direction * -SwingDirectionChange.ToDirectionInt();
                        swingHelper.Change_Lerp(-Vector2.UnitY, time, Vector2.One, time);
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, 0);
                        if (Projectile.ai[1]++ > PreAtkTime) // 大于前摇时间
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 4;
                            TheUtility.ResetProjHit(Projectile);
                            SoundEngine.PlaySound(SoundID.Item19 with { Pitch = 0.5f, MaxInstances = 3 }, Projectile.Center);
                        }
                        break;
                    }
                case 5: // 下砍
                    {
                        Player.velocity.X *= 0;
                        Projectile.ai[1]++;
                        Player.heldProj = Projectile.whoAmI;
                        Projectile.spriteDirection = Player.direction * -SwingDirectionChange.ToDirectionInt();
                        float Time = TimeChange.Invoke(Projectile.ai[1] / AtkTime);
                        if (Time > 1)
                        {
                            Time = 1 + Time * 0.03f;
                            if (Time > 1.05)
                            {
                                Projectile.ai[0]++;
                                HitNPCs.ForEach(n =>
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        FrostFist_FistBoom frostFist_FistBoom = new(10 + i * 5, Player, Main.rand.NextVector2Unit() * 5, (int)(Projectile.damage));
                                        if (i == 4)
                                        {
                                            frostFist_FistBoom.ExtraAI = (NPC npc) =>
                                            {
                                                npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime = 60;
                                            };
                                        }
                                        WeaponSkillGlobalNPC.AddComponent(n, frostFist_FistBoom);
                                    }
                                });
                            }
                        }
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * MathHelper.Pi * 1.25f * -SwingDirectionChange.ToDirectionInt());
                        break;
                    }
            }
        }
        public override bool SwitchCondition()
        {
            if(HitNPCs.Count <= 0) return base.SwitchCondition();
            return (int)Projectile.ai[0] >=6;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            HitNPCs.Add(target);
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            HitNPCs.Clear();
        }
    }
}
