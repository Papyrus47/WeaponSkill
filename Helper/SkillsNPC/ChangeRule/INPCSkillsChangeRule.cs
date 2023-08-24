using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillsNPC.ChangeRule
{
    public interface INPCSkillsChangeRule
    {
        public NPCSkills_Instantiation CurrentSkill { get; set; }
        public NPCSkills_Instantiation ChangeSkill();
        /// <summary>
        /// 当技能发生改变时候调用
        /// </summary>
        public void OnChange(NPCSkills_Instantiation target);
    }
}
