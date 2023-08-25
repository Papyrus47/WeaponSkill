using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.ChargeBlade
{
    public class ChargeBladeProj : ModProjectile, IBasicSkillProj
    {
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }

        public void Init()
        {
            OldSkills = new List<ProjSkill_Instantiation>();
        }
    }
}
