using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.InsectStaff.Skills
{
    public class InsectStaff_Held : BasicInsectStaffSkill
    {
        public InsectStaff_Held(InsectStaffProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            if (InsectStaffProj.insectProj.Projectile.ai[0] <= 0 && Projectile.ai[0]++ > 120)
            {
                SkillTimeOut = true;
            }
            Projectile.localAI[0] += player.velocity.X * 0.01f;
            Projectile.spriteDirection = player.direction;
            swingHelper.Change_Lerp(Vector2.UnitX.RotatedBy(0.4 + MathF.Sin(Projectile.localAI[0]) * 0.1f), 1, Vector2.One, 1f, 0f);
            swingHelper.ProjFixedPlayerCenter(player, -InsectStaffProj.SwingLength * 0.2f,true);
            swingHelper.SwingAI(InsectStaffProj.SwingLength, player.direction, 0);

        }
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => player.controlUseItem;
        public override bool SwitchCondition() => true;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, null, null);
            return false;
        }
    }
}
