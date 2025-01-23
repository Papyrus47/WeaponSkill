using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public abstract class ChargeBlade_Sword_Basic : BasicChargeBladeSkill
    {
        protected ChargeBlade_Sword_Basic(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            #region 盾的更新
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * player.width * 0.6f, 0), player.direction);
            shield.VisualRotation = 0.3f;
            shield.AxeRot = 0.05f;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.direction * -MathHelper.PiOver2);
            #endregion

            #region 玩家更新
            if (Math.Abs(player.velocity.X) > 4) player.velocity.X = 4 * (player.velocity.X > 0).ToDirectionInt();
            #endregion
        }
    }
}
