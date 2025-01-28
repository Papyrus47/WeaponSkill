using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Hammer;

namespace WeaponSkill.Weapons.HuntingHorn.Skills
{
    public class HuntingHorn_GroundHit : HuntingHorn_TwoControlSwing
    {
        public HuntingHorn_GroundHit(HuntingHornProj proj, Func<bool> changeCondition) : base(proj, changeCondition)
        {
        }
        public override void AI()
        {
            SwingAI?.Invoke();
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            switch ((int)Projectile.ai[0])
            {
                case 0: // 准备挥舞
                    PreAtk = true;
                    swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
                    if (Projectile.ai[1]++ > PreSwingTimeMax)
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        Projectile.extraUpdates = 1;
                        TheUtility.Player_ItemCheck_Shoot(player, HuntingHornProj.SpawnItem, Projectile.damage);
                        TheUtility.ResetProjHit(Projectile);
                        if (melodyType != default)
                        {
                            HuntingHornGlobalItem huntingHornGlobalItem = HuntingHornProj.SpawnItem.GetGlobalItem<HuntingHornGlobalItem>();
                            HuntingHornMelody hornMelody = huntingHornGlobalItem.hornMelody;
                            hornMelody.melodies.Enqueue(melodyType);
                            HuntingHornBuff item = hornMelody.FindMelody(player, Projectile);
                            if (item != null)
                                huntingHornGlobalItem.huntingHornBuffs.Enqueue(item);

                        }
                    }
                    break;
                case 1: // 挥舞
                    Projectile.extraUpdates = 2;
                    Projectile.ai[1]++;
                    float Time = HuntingHornProj.TimeChange(Projectile.ai[1] / SwingTimeMax);
                    if (Time > 1)
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        if ((int)Projectile.ai[2] == 0)
                        {
                            Projectile.ai[2]++;
                            Projectile.ai[0] = 0;
                            StartVel = -Vector2.UnitY;
                            SwingRot = MathHelper.PiOver2 * 1.3f;
                            VelScale = Vector2.One;
                            VisualRotation = 0;
                            PreSwingTimeMax = 5;
                            SwingTimeMax = 40;
                            ActionDmg = 3f;
                            SwingDirectionChange = true;
                            break;
                        }
                    }
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                    HammerProj.DrawHammerSwingShader_Index.Add(Projectile.whoAmI);
                    break;
                case 2: // 跳出
                    if (Projectile.ai[1]++ > TimeoutTimeMax)
                    {
                        SkillTimeOut = true;
                    }
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, SwingRot * SwingDirectionChange.ToDirectionInt());
                    break;
            }
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            StartVel = Vector2.UnitY;
            SwingRot = MathHelper.Pi + 0.9f;
            VelScale = new Vector2(1,0.4f);
            VisualRotation = 0;
            SwingDirectionChange = false;
            PreSwingTimeMax = 15;
            SwingTimeMax = 60;
            ActionDmg = 2.5f;
        }
    }
}
