using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Axe_Swing_AxeStrength : ChargeBlade_Axe_Swing
    {
        public ChargeBlade_Axe_Swing_AxeStrength(ChargeBladeProj chargeBlade) : base(chargeBlade, null)
        {
            StartVel = Vector2.UnitY.RotatedBy(0.4);
            VelScale = new Vector2(1, 0.725f);
            VisualRotation = 0.3f;
            SwingRot = MathHelper.Pi + MathHelper.PiOver4 * 1.25f;
            SwingDirectionChange = false;
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 0)
            {
                ChargeBladeProj.chargeBladeGlobal.AxeStrengthening = true;
            }
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => ActivationCondition();
        public override bool ActivationCondition() => WeaponSkill.RangeChange.Current;
    }
}
