using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Spears
{
    public class SpearsGlobalProj : GlobalProjectile
    {
        public bool Spears_SPProj;
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            if (SpearsGlobalItem.WeaponID == null) return false;
            return SpearsGlobalItem.WeaponID.Any(x => ContentSamples.ItemsByType[x].shoot == entity.type) && lateInstantiation;
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if((source is EntitySource_ItemUse item && item.Item.GetGlobalItem<SpearsGlobalItem>().SPItem) || projectile.type == ProjectileID.Daybreak)
            {
                Spears_SPProj = true;
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (Spears_SPProj) return true;
            return false;
        }

        public override bool? CanDamage(Projectile projectile)
        {
            if (Spears_SPProj) return null;
            return false;
        }
    }
}
