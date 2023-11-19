using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper.SkillsNPC.ChangeRule;

namespace WeaponSkill.Helper.SkillsNPC
{
    public class NPCSkillsPool
    {
        /// <summary>
        /// 每个技能的技能链储存
        /// </summary>
        public Dictionary<NPCSkills_Instantiation, List<INPCSkillsChangeRule>> ChangeRulePart;
        public NPCSkillsManager skillManager;
        public virtual NPCSkillsPool AddSkill(NPCSkills_Instantiation skill, INPCSkillsChangeRule rule)
        {
            if (ChangeRulePart.TryGetValue(skill, out _)) // 如果可以添加技能
            {
                ChangeRulePart[skill].Add(rule);
            }
            else
            {
                ChangeRulePart.Add(skill, new() { rule });
            }
            return this;
        }
        // 技能登记在前的技能会优先执行切换
        // 反之,会越后
        // 所以我们要把优先级最高的技能放到最前面
        // 这是靠NPC配合的
        // 虽然可以往技能储存列表分配技能的优先级,但综合来看,直接排序是一种更好的选择

        public virtual NPCSkills_Instantiation ChagneSkill(NPCSkills_Instantiation currentSkill)
        {
            if (ChangeRulePart.TryGetValue(currentSkill, out var skillChangeRules))
            {
                foreach (var skillChangeRule in skillChangeRules)
                {
                    skillChangeRule.CurrentSkill = currentSkill;
                    var getSkill = skillChangeRule.ChangeSkill();
                    if (getSkill != null)
                    {
                        skillChangeRule.OnChange(getSkill);
                        return getSkill;
                    }
                }
            }
            return null;
        }
    }
}
