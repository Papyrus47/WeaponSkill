using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Hammer;

namespace WeaponSkill.Weapons.HuntingHorn.Skills
{
    public class HuntingHorn_ShankStrike : HuntingHorn_Swing
    {
        public HuntingHorn_ShankStrike(HuntingHornProj proj, Func<bool> changeCondition) : base(proj, changeCondition)
        {
            StartVel = -Vector2.UnitX;
            VelScale = Vector2.One;
            SwingRot = 0;
            SwingDirectionChange = true;
            ActionDmg = 0.5f;
            PreSwingTimeMax = 5;
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

                        if (player.controlUseItem && player.controlUseTile)
                        {
                            melodyType = HuntingHornMelody.MelodyType.LeftAndRight;
                        }
                        else if (player.controlUseItem)
                        {
                            melodyType = HuntingHornMelody.MelodyType.Left;
                        }
                        else if (player.controlUseTile)
                        {
                            melodyType = HuntingHornMelody.MelodyType.Right;
                        }

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
                    }
                    swingHelper.ProjFixedPlayerCenter(player, -Time * 60, true);
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
    }
}
