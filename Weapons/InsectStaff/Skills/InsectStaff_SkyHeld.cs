using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.InsectStaff.Skills
{
    public class InsectStaff_SkyHeld : InsectStaff_Held
    {
        public InsectStaff_SkyHeld(InsectStaffProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            if (player.velocity.Y == 0)
            {
                SkillTimeOut = true;
            }
            Projectile.localAI[0] += player.velocity.X * 0.02f;
            Projectile.spriteDirection = player.direction;
            swingHelper.Change_Lerp(Vector2.UnitX.RotatedBy(0.4 + MathF.Sin(Projectile.localAI[0]) * 0.2f), 1, Vector2.One, 1f, 0f);
            swingHelper.ProjFixedPlayerCenter(player, -InsectStaffProj.SwingLength * 0.2f, true);
            swingHelper.SwingAI(InsectStaffProj.SwingLength, player.direction, 0);

        }
    }
}
