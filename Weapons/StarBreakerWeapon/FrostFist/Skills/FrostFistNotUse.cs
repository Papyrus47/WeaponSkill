using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFistNotUse : BasicFrostFistSkill
    {
        public FrostFistNotUse(FrostFistProj modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Projectile.Center = Player.Center;
            Projectile.velocity *= 0;
            Projectile.extraUpdates = 0;
            Vector2 vel = (Main.MouseWorld - Projectile.Center);
            Player.ChangeDir((vel.X > 0).ToDirectionInt());
            for (int i = 0; i < 6; i++)
            {
                FrostFistDust();
            }
        }
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
        }
    }
}
