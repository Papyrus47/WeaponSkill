using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.InsectStaff.Skills
{
    public class InsectStaff_ControlInsect : InsectStaff_Swing
    {
        public InsectStaff_ControlInsect(InsectStaffProj proj, Func<bool> changeCondition) : base(proj, changeCondition)
        {
        }
        public override bool? CanDamage() => false;
    }
}
