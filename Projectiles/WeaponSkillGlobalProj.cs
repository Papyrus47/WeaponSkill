using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade;

namespace WeaponSkill.Projectiles
{
    public class WeaponSkillGlobalProj : GlobalProjectile
    {
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.hostile && Main.LocalPlayer.GetModPlayer<WindsPlayer>().UseWindsState && projectile.Distance(Main.LocalPlayer.Center) < projectile.Size.Length() * 3.5f)
                projectile.velocity = -projectile.velocity;
            return base.PreAI(projectile);
        }
    }
}
