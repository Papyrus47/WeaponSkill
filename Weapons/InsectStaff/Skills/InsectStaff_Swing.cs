using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.SlashAxe;

namespace WeaponSkill.Weapons.InsectStaff.Skills
{
    public class InsectStaff_Swing : BasicInsectStaffSkill
    {
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float VisualRotation;
        /// <summary>
        /// 切换技能的条件
        /// </summary>
        public Func<bool> ChangeCondition;
        public float SwingRot;
        /// <summary>
        /// 为true默认正方向 false则为反
        /// </summary>
        public bool SwingDirectionChange = true;
        public Action SwingAI;
        public int SwingTimeMax = 30;
        public int PreSwingTimeMax = 12;
        public int TimeoutTimeMax = 15;
        public bool IsSkyAtk;
        public const float CHANGE_LERP_SPEED = 0.35f;
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDmg = 1f;

        public InsectStaff_Swing(InsectStaffProj proj, Func<bool> changeCondition) : base(proj)
        {
            ChangeCondition = changeCondition;
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
                    swingHelper.ProjFixedPlayerCenter(player, -InsectStaffProj.SwingLength * 0.2f, true);
                    swingHelper.SwingAI(InsectStaffProj.SwingLength, player.direction, 0);
                    if (Projectile.ai[1]++ > PreSwingTimeMax)
                    {
                        SoundEngine.PlaySound(
                           SoundID.Item1.WithPitchOffset(0.3f),
                           player.Center
                        );
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        Projectile.extraUpdates = 1;
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
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => Projectile.ai[0] >= 2;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) => new Color(255, 255, 255, 0), null);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += ActionDmg - 1;
            player.SetImmuneTimeForAllTypes(8);
        }
    }
}
