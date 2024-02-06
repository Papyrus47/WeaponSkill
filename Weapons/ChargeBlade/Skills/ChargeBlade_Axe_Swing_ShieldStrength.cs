using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    /// <summary>
    /// 盾强化
    /// </summary>
    public class ChargeBlade_Axe_Swing_ShieldStrength : ChargeBlade_Sword_Swing_RotSlash
    {
        public ChargeBlade_Axe_Swing_ShieldStrength(ChargeBladeProj chargeBlade) : base(chargeBlade, null)
        {
            StartVel = (-Vector2.UnitX).RotatedBy(0.6);
            VelScale = new Vector2(1, 0.3f);
            VisualRotation = 0.7f;
            SwingDirectionChange = true;
        }
        public bool IsShieldStrength;
        public override void AI()
        {
            if ((int)Projectile.ai[0] == 1 && !IsShieldStrength)
            {
                IsShieldStrength = true;
                ChargeBladeProj.chargeBladeGlobal.ShieldStrengthening += ChargeBladeProj.chargeBladeGlobal.StatChargeBottle * 1800;
                ChargeBladeProj.chargeBladeGlobal.StatChargeBottle = 0;
            }
            base.AI();
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => ActivationCondition() && Projectile.ai[0] > 25;
        public override bool ActivationCondition() => WeaponSkill.BowSlidingStep.Current;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            IsShieldStrength = false;
        }
    }
}
