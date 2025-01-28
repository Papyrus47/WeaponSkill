using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.HuntingHorn
{
    public class HuntingHorn_AddBuffToMinion : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => lateInstantiation && entity.minion;
        public override bool InstancePerEntity => true;
        public float HuntingHorn_AttackUp;
        public float HuntingHorn_AttackAdd;
        public override void AI(Projectile projectile)
        {
            base.AI(projectile);
            if (HuntingHorn_AttackAdd > 0)
                HuntingHorn_AttackUp -= 1f/(projectile.extraUpdates + 1);
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (HuntingHorn_AttackAdd > 0)
            {
                modifiers.SourceDamage += HuntingHorn_AttackAdd;
            }
        }
    }
}
