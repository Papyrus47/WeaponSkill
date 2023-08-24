using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillsNPC
{
    public abstract class NPCSkills_Instantiation
    {
        /// <summary>
        /// 技能切换后调用
        /// </summary>
        public virtual void OnSkillDeactivate(NPCSkills_Instantiation targetSkill) { }
        /// <summary>
        /// 技能激活时调用
        /// </summary>
        public virtual void OnSkillActive() { }
    }
}
