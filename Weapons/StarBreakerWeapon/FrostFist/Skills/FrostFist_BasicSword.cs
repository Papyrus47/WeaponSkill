using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_BasicSword : BasicFrostFistSkill
    {
        public FrostFist_BasicSword(FrostFistProj modProjectile) : base(modProjectile)
        {
        }
        public virtual SwingHelper swingHelper => FrostFist.SwordSwingHelper;
    }
}
