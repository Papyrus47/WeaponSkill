using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillNPC
{
    public abstract class ModNPCSkill_Instantiation : NPCSkill_Instantiation
    {
        public ModNPC modNPC;
        public ModNPCSkill_Instantiation this[ModNPCSkill_Instantiation projSkill] => switchToSkill.Find(x => x == projSkill) as ModNPCSkill_Instantiation;
        public ModNPCSkill_Instantiation(ModNPC modNPC)
        {
            this.modNPC = modNPC;
            NPC = modNPC.NPC;
            switchToSkill = new();
        }
    }
}
