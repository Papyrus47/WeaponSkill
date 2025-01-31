using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.InsectStaff.Skills
{
    /// <summary>
    /// 操虫斩,点灯技
    /// </summary>
    public class InsectStaff_ControlInsectDash : InsectStaff_Swing
    {
        public InsectStaff_ControlInsectDash(InsectStaffProj proj, Func<bool> changeCondition) : base(proj, changeCondition)
        {
            PreSwingTimeMax = 30;
            SwingRot = MathHelper.Pi + 0.5f;
            VelScale = new Vector2(1, 0.6f);
            VisualRotation = 0.4f;
            IsSkyAtk = true;
        }
        public override void AI()
        {
            SwingAI?.Invoke();
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            switch ((int)Projectile.ai[0])
            {
                case 0: // 准备挥舞
                    PreAtk = true;
                    if ((int)Projectile.ai[1] == 0)
                    {
                        StartVel = (Main.MouseWorld - player.Center).SafeNormalize(default);
                        StartVel.X *= player.direction;
                    }
                    swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                    swingHelper.ProjFixedPlayerCenter(player, -InsectStaffProj.SwingLength * 0.2f, true);
                    swingHelper.SwingAI(InsectStaffProj.SwingLength, player.direction, 0);
                    player.velocity.X = StartVel.X * 20 * player.direction;
                    player.velocity.Y = StartVel.Y * 20;
                    if (Projectile.ai[1]++ > PreSwingTimeMax || Projectile.numHits > 0)
                    {
                        SoundEngine.PlaySound(
                           SoundID.Item1.WithPitchOffset(0.3f),
                           player.Center
                        );
                        if (Projectile.numHits > 0)
                        {
                            InsectStaffProj.SpawnItem.GetGlobalItem<InsectStaffGlobalItem>().StrongTime = 1800;
                        }
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        Projectile.extraUpdates = 1;
                        StartVel = StartVel.RotatedBy(Projectile.spriteDirection * 0.5f);
                        TheUtility.Player_ItemCheck_Shoot(player, InsectStaffProj.SpawnItem, Projectile.damage);
                        TheUtility.ResetProjHit(Projectile);
                    }
                    break;
                case 1: // 挥舞
                    Projectile.extraUpdates = 2;
                    Projectile.ai[1]++;
                    float Time = InsectStaffProj.TimeChange(Projectile.ai[1] / SwingTimeMax);
                    if (Time > 1)
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                    }
                    swingHelper.ProjFixedPlayerCenter(player, -InsectStaffProj.SwingLength * 0.2f, true);
                    swingHelper.SwingAI(InsectStaffProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                    break;
                case 2: // 跳出
                    if (Projectile.ai[1]++ > TimeoutTimeMax)
                    {
                        SkillTimeOut = true;
                    }
                    swingHelper.ProjFixedPlayerCenter(player, -InsectStaffProj.SwingLength * 0.2f, true);
                    swingHelper.SwingAI(InsectStaffProj.SwingLength, player.direction, SwingRot * SwingDirectionChange.ToDirectionInt());
                    break;
            }
        }
        public override bool SwitchCondition()
        {
            return base.SwitchCondition() && Projectile.numHits > 0;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.numHits = 0;
        }
    }
}
