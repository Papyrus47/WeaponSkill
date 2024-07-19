using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.SwordShield
{
    public class SwordShieldProj : ModProjectile,IBasicSkillProj
    {
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public void Init()
        {

        }
        public override bool IsLoadingEnabled(Mod mod) => false;
    }
}
