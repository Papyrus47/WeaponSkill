using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.SwordShield.Skills
{
    public class SwordShield_NoUse : BasicSwordShieldSkill
    {
        public SwordShield_NoUse(SwordShieldProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            swingHelper.Change_Lerp(-Vector2.UnitX, 1, Vector2.One, 0.2f,0.9f);
            swingHelper.ProjFixedPlayerCenter(player, SwordShieldProj.SwingLength * -0.2f);
            swingHelper.SetSwingActive();
            Projectile.spriteDirection = -player.direction;
            swingHelper.SwingAI(SwordShieldProj.SwingLength, player.direction, 0);
            #region 盾的更新
            player.itemRotation = (player.velocity.X * 0.1f);
            player.SetCompositeArmFront(true,Player.CompositeArmStretchAmount.Full, player.itemRotation);
            SwordShieldProj.swordShield_Shield.Update(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full,player.itemRotation), player.direction,0);

            #endregion
        }
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => true;
        public override bool SwitchCondition() => true;
        public override void SwordDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, null,null);
        }
        public override void ShieldDraw(SpriteBatch sb, ref Color lightColor)
        {
            SwordShieldProj.swordShield_Shield.Draw(sb, lightColor);
        }
    }
}
