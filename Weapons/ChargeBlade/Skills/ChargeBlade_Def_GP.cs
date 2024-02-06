using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Def_GP : ChargeBlade_Sword_Held
    {
        public BasicChargeBladeSkill ChangeSkill;
        public int Time;
        public float kn;
        public ChargeBlade_Def_GP(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            base.AI();
            PreAttack = false;
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * player.width, 0), player.direction);
            shield.VisualRotation = 0.3f;
            shield.AxeRot = -0.2f;
            shield.InDef = true;

            Vector2 rotVector = Vector2.UnitX.RotatedBy(0.225f + Math.Sin(Projectile.ai[1]) * 0.05f);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f);
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -ChargeBladeProj.SwingLength * 0.45f, true);
            swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
            Time--;
            if(Time > 0)
            {
                player.velocity.X = Time * -player.direction * kn;
            }
            else
            {
                SkillTimeOut = true;
            }
        }
        //public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        //{
        //    return ChangeSkill.PreDraw(sb,ref lightColor);
        //}
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (ChargeBladeProj.shield.DefSucceeded_GP) // 成功GP
            {
                return true;
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override bool ActivationCondition() => false; // 不能通过正常方法激活
        public override bool SwitchCondition()
        {
            return Time <= 9 && kn > 0.2f;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            SkillTimeOut = false;
            Time = ChargeBladeProj.shield.KNLevel switch
            {
                ChargeBladeShield.KNLevelEnum.Small => 12,
                ChargeBladeShield.KNLevelEnum.Medium => 25,
                ChargeBladeShield.KNLevelEnum.Big => 40,
                _ => 1
            };
            ChargeBladeProj.DefSucceededTime = 60;
            player.immuneTime += 15;
            kn = ChargeBladeProj.shield.KNLevel switch
            {
                ChargeBladeShield.KNLevelEnum.Small => 0.4f,
                ChargeBladeShield.KNLevelEnum.Medium => 0.4f,
                ChargeBladeShield.KNLevelEnum.Big => 0.2f,
                _ => 1f
            };
        }
    }
}
