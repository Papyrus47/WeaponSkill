using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Axe_Basic : BasicChargeBladeSkill
    {
        public ChargeBlade_Axe_Basic(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            ChargeBladeProj.chargeBladeGlobal.InAxe = true;
            player.heldProj = Projectile.whoAmI;
            #region 盾的更新
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(Projectile.Center + Projectile.velocity * 0.95f, 1);
            shield.VisualRotation = 0;
            shield.AxeRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2 * 1.02f;
            shield.Fixed = true;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter,MathF.Atan2(Projectile.velocity.Y * player.direction,Projectile.velocity.X * player.direction) - MathHelper.PiOver2 * 1.05f * player.direction);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction) - MathHelper.PiOver2 * 0.46f * player.direction);
            #endregion

            #region 玩家更新
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            #endregion
        }
    }
}
