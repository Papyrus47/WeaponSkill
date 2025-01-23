using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Configs;
using WeaponSkill.Command.SkillNPC;
using WeaponSkill.NPCs.Bosses.GlobalBoss.Eye.Modes;

namespace WeaponSkill.NPCs.Bosses.GlobalBoss.Eye
{
    public class EyeofCthulhu : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.EyeofCthulhu && lateInstantiation && BossSetting_Config.Init.ResetBossAI;
        public override bool PreAI(NPC npc)
        {
            Player player = Main.player[npc.target];
            if(player == null) 
                return true;
            npc.TargetClosest();
            Vector2 vel = player.Center - npc.Center;
            const int RandomMax = 16;
            const int RandomMax2 = 27;
            switch ((int)npc.ai[0])
            {
                case 0: // 一阶段的ai
                    if (Main.dayTime)
                        return true;

                    if (npc.localAI[0] > 20)
                    {
                        npc.ai[1] = 0;
                        npc.localAI[0] = 0;
                    }
                    if(npc.life < npc.lifeMax * 0.5f)
                    {
                        return true;
                    }
                    switch ((int)npc.ai[1])
                    {
                        case < 5: // 随机游走
                            Move(npc, vel, RandomMax);
                            break;
                        case < 10: // 后退冲刺
                            Rash(npc, vel, RandomMax);
                            break;
                        case < 13: // S冲
                            S_Rash(npc, vel, RandomMax);
                            break;
                        case < 16: // 喷小克眼
                            if (npc.ai[2]++ < 90) // 后退
                            {
                                if (vel.Length() > 300) // 大于距离
                                {
                                    npc.velocity = (npc.velocity * 10 + vel.SafeNormalize(default) * 10) / 11f;
                                }
                                else if (vel.Length() < 290) // 大于距离
                                {
                                    npc.velocity = (npc.velocity * 10 - vel.SafeNormalize(default) * 10) / 11f;
                                }
                                npc.rotation += MathHelper.WrapAngle((vel.ToRotation() - MathHelper.PiOver2) - npc.rotation) * 0.3f;
                            }
                            else
                            {
                                for(int i = -1; i <= 1; i++)
                                {
                                    int n = NPC.NewNPC(npc.GetSource_FromAI(), npc.Center.ToPoint().X, npc.Center.ToPoint().Y, 5, npc.whoAmI);
                                    Main.npc[n].velocity = vel.RotatedBy(0.3 * i) * 0.1f;
                                }
                                npc.ai[1] = Main.rand.Next(RandomMax);
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                npc.localAI[0] += 2;
                            }
                            break;
                            
                    }
                    return false;
                case 3:
                    if (npc.localAI[0] > 50)
                    {
                        npc.ai[1] = 0;
                        npc.localAI[0] = 0;
                    }
                    switch ((int)npc.ai[1])
                    {
                        case < 4: // 随机游走
                            Move(npc, vel, RandomMax2);
                            break;
                        case < 12: // 后退冲刺
                            Rash(npc, vel, RandomMax2, 20);
                            break;
                        case < 16: // S冲
                            S_Rash(npc, vel, RandomMax2, 15);
                            break;
                        case < 25: // 近圆冲
                            if (npc.ai[2]++ < 30) // 后退
                            {
                                if (vel.Length() > 600) // 大于距离
                                {
                                    npc.ai[2] -= 0.9f;
                                    npc.velocity = (npc.velocity * 10 + vel.SafeNormalize(default) * 10) / 11f;
                                }
                                else if (vel.Length() < 590) // 小于距离
                                {
                                    npc.velocity = (npc.velocity * 10 - vel.SafeNormalize(default) * 10) / 11f;
                                }
                                else
                                {
                                    npc.velocity *= 0.9f;
                                }
                                if ((int)npc.ai[2] == 30)
                                {
                                    npc.velocity = vel.SafeNormalize(default) * 20;
                                    npc.ai[3] = vel.ToRotation();
                                    npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                                    PlaySound(npc);
                                }
                                npc.rotation += MathHelper.WrapAngle((npc.velocity.ToRotation() - MathHelper.PiOver2) - npc.rotation) * 0.9f;
                            }
                            else if (npc.ai[2] < 90 && npc.ai[2] > 30) // 转弯
                            {
                                npc.velocity = npc.velocity.RotatedByRandom(0.1);
                                npc.velocity = npc.velocity.RotatedBy(0.03 * (vel.X > 0).ToDirectionInt());
                                npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                            }
                            else if (npc.ai[2] > 90) // 跳出
                            {
                                npc.ai[1] = Main.rand.Next(RandomMax2);
                                //npc.ai[1] = 12;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                npc.localAI[0] += 12;
                            }
                            break;
                        case < 27: // 大 杀 招

                            if (npc.ai[2]++ < 60) // 后退
                            {
                                npc.ai[2] -= 0.9f;
                                if (vel.Length() > 800) // 大于距离
                                {
                                    npc.velocity = (npc.velocity * 10 + vel.SafeNormalize(default) * 10) / 11f;
                                }
                                else if (vel.Length() < 500) // 大于距离
                                {
                                    npc.velocity = (npc.velocity * 10 - vel.SafeNormalize(default) * 10) / 11f;
                                }
                                if ((int)npc.ai[2] == 60)
                                {
                                    npc.velocity = vel.SafeNormalize(default) * 30;
                                    npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                                    npc.localAI[1] = vel.ToRotation();
                                    PlaySound(npc);

                                }
                                npc.rotation += MathHelper.WrapAngle((vel.ToRotation() - MathHelper.PiOver2) - npc.rotation) * 0.9f;
                            }
                            else if (npc.ai[2] < 90 && npc.ai[2] > 60) // 减速
                            {
                                if (npc.ai[3] <= 0)
                                {
                                    npc.velocity = MathF.Cos((npc.ai[2] - 60f) / 30f * MathHelper.TwoPi).ToRotationVector2().RotatedBy(npc.localAI[1]) * 30;
                                }
                                npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                            }
                            else if (npc.ai[2] < 200 && npc.ai[2] > 90) // 高级减速
                            {
                                if (npc.ai[3]++ < 2)
                                {
                                    npc.ai[2] = 60;
                                    PlaySound(npc);
                                    npc.velocity = vel.SafeNormalize(default) * 20;
                                }
                                npc.velocity = (npc.velocity * 50 + vel.SafeNormalize(default)) / 51f;
                                npc.rotation += MathHelper.WrapAngle((npc.velocity.ToRotation() - MathHelper.PiOver2) - npc.rotation) * 0.9f;
                            }
                            else if (npc.ai[2] > 200) // 跳出
                            {
                                npc.ai[1] = Main.rand.Next(RandomMax2);
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                                npc.localAI[1] = 0;
                                npc.localAI[0] += 50;
                            }
                            break;

                    }
                    return false;
            }
            return true;
        }

        public static void S_Rash(NPC npc, Vector2 vel, int RandomMax, float dashSpeed = 10)
        {
            if (npc.ai[2]++ < 60) // 后退
            {
                if (vel.Length() > 400) // 大于距离
                {
                    npc.velocity = (npc.velocity * 10 + vel.SafeNormalize(default) * 10) / 11f;
                }
                else if (vel.Length() < 390) // 大于距离
                {
                    npc.velocity = (npc.velocity * 10 - vel.SafeNormalize(default) * 10) / 11f;
                }
                if (npc.ai[2] == 60)
                {
                    npc.velocity = vel.SafeNormalize(default) * 10;
                    npc.ai[3] = vel.ToRotation();
                    npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                    PlaySound(npc);
                }
                npc.rotation += MathHelper.WrapAngle((npc.velocity.ToRotation() - MathHelper.PiOver2) - npc.rotation) * 0.9f;
            }
            else if (npc.ai[2] < 90 && npc.ai[2] > 60) // 转弯
            {
                npc.velocity = MathF.Cos((npc.ai[2] - 60f) / 30f * MathHelper.Pi).ToRotationVector2().RotatedBy(npc.ai[3]) * dashSpeed;
                npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
            }
            else if (npc.ai[2] < 150 && npc.ai[2] > 90) // 停留
            {
                npc.velocity *= 0.95f;
                npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                if ((int)npc.ai[2] == 140)
                {
                    PlaySound(npc);
                }
            }
            else if (npc.ai[2] < 180 && npc.ai[2] > 150) // 冲刺
            {
                npc.velocity = MathF.Cos((npc.ai[2] - 60f) / 30f * MathHelper.Pi).ToRotationVector2().RotatedBy(npc.ai[3]) * dashSpeed;
                npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
            }
            else if (npc.ai[2] > 180) // 跳出
            {
                npc.ai[1] = Main.rand.Next(RandomMax);
                //npc.ai[1] = 12;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
                npc.localAI[0] += 7;
            }
        }

        public static void PlaySound(NPC npc)
        {
            if (npc.life > npc.lifeMax * 0.5f)
                return;
            SoundStyle roar = SoundID.Roar;
            if (npc.life < npc.lifeMax * 0.35f)
            {
                roar.Pitch = 0.5f;
            }
            SoundEngine.PlaySound(roar);
        }

        public static void Rash(NPC npc, Vector2 vel, int RandomMax,float dashSpeed = 10)
        {
            if(npc.life < npc.lifeMax * 0.5f)
            {
                npc.ai[2]++;
            }
            if (npc.ai[2]++ < 60) // 后退
            {
                if (vel.Length() > 20 * dashSpeed) // 大于距离
                {
                    npc.velocity = (npc.velocity * 10 + vel.SafeNormalize(default) * 10) / 11f;
                }
                else if (vel.Length() < 19 * dashSpeed) // 大于距离
                {
                    npc.velocity = (npc.velocity * 10 - vel.SafeNormalize(default) * 10) / 11f;
                }
                if (npc.ai[2] == 60)
                {
                    npc.velocity = vel.SafeNormalize(default) * dashSpeed;
                    npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                    PlaySound(npc);

                }
                npc.rotation += MathHelper.WrapAngle((vel.ToRotation() - MathHelper.PiOver2) - npc.rotation) * 0.9f;
            }
            else if (npc.ai[2] < 90 && npc.ai[2] > 60) // 减速
            {
                npc.velocity *= 0.99f;
                npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
            }
            else if (npc.ai[2] < 200 && npc.ai[2] > 90) // 高级减速
            {
                npc.velocity = (npc.velocity * 50 + vel.SafeNormalize(default)) / 51f;
                npc.rotation += MathHelper.WrapAngle((npc.velocity.ToRotation() - MathHelper.PiOver2) - npc.rotation) * 0.9f;
            }
            else if (npc.ai[2] > 200) // 跳出
            {
                npc.ai[1] = Main.rand.Next(RandomMax);
                npc.ai[2] = 0;
                npc.localAI[0] += 5;
            }
        }

        public static void Move(NPC npc, Vector2 vel, int RandomMax)
        {
            if (vel.Length() > 400) // 大于距离
            {
                npc.velocity = vel.SafeNormalize(default) * 10;
            }
            else if (vel.Length() > 150) // 中等距离
            {
                npc.velocity = (npc.velocity * 50 + vel.SafeNormalize(default) * 4) / 51f;
            }
            else // 过于靠近
            {
                npc.velocity = (npc.velocity * 50 + vel.SafeNormalize(default)) / 51f;
                npc.ai[2] -= 0.3f;
                //npc.rotation = -npc.rotation;
            }
            npc.rotation += MathHelper.WrapAngle((npc.velocity.ToRotation() - MathHelper.PiOver2) - npc.rotation) * 0.5f;
            npc.ai[2] += Main.rand.NextFloat(0.5f, 1f);
            if (npc.ai[2] >= 120)
            {
                npc.ai[1] = Main.rand.Next(RandomMax);
                npc.ai[2] = 0;
            }
        }
    }
}