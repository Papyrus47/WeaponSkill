using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Weapons.StarBreakerWeapon.DamageTypes.MoreDamageType;

namespace WeaponSkill.Weapons.StarBreakerWeapon.DamageTypes
{
    public class EnergyDamage : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if(damageClass == Generic || damageClass == Default)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return false;
        }
    }
}
