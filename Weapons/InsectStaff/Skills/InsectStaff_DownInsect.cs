using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.InsectStaff.Skills
{
    public class InsectStaff_DownInsect : InsectStaff_Swing
    {
        public InsectStaff_DownInsect(InsectStaffProj proj, Func<bool> changeCondition) : base(proj, changeCondition)
        {
            SwingRot = 0;
            IsSkyAtk = true;
            VelScale = Vector2.One;
            StartVel = new Vector2(0,1);
        }
    }
}
