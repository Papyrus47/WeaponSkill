using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.SwordShield.Skills
{
    /// <summary>
    /// 盾击
    /// </summary>
    public class SwordShield_ShieldHit : BasicSwordShieldSkill
    {
        /// <summary>
        /// 包括长度在内的速度
        /// </summary>
        public Vector2 Vel;
        public SwordShield_ShieldHit(SwordShieldProj proj) : base(proj)
        {
        }
        
    }
}
