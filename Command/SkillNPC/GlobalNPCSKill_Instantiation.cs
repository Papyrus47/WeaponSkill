using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.SkillNPC
{
    public class GlobalNPCSKill_Instantiation : NPCSkill_Instantiation
    {
        public GlobalNPCSKill_Instantiation this[GlobalNPCSKill_Instantiation projSkill] => switchToSkill.Find(x => x == projSkill) as GlobalNPCSKill_Instantiation;
    }
}
