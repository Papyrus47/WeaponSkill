using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    /// <summary>
    /// 突进斩
    /// </summary>
    public class ChargeBlade_Sword_Swing_Dash : ChargeBlade_Sword_Swing
    {
        public ChargeBlade_Sword_Swing_Dash(ChargeBladeProj chargeBlade, Func<bool> activationConditionFunc) : base(chargeBlade, activationConditionFunc)
        {
            StartVel = (-Vector2.UnitY).RotatedBy(-0.3f);
            VelScale = new Vector2(1, 0.8f);
            VisualRotation = 0.2f;
            SwingRot = MathHelper.Pi + MathHelper.PiOver2;
            SwingDirectionChange = true;
        }
        public override void AI()
        {
            base.AI();
            PreAttack = false;
            if ((int)Projectile.ai[0] == 1)
            {
                TheUtility.SetPlayerImmune(player);
                player.velocity.X = player.direction * ((Projectile.ai[1] * 3) - 1.5f) * 0.4f;
            }
            else if ((int)Projectile.ai[0] == 2)
            {
                player.velocity.X *= 0;
                PreAttack = true;
            }
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => ActivationCondition();
    }
}
