using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Spears
{
    public class SpearsGlobalProj : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            if (SpearsGlobalItem.WeaponID == null) return false;
            return SpearsGlobalItem.WeaponID.Any(x => ContentSamples.ItemsByType[x].shoot == entity.type) && lateInstantiation;
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor) => false;
        public override bool? CanDamage(Projectile projectile) => false;
    }
}
