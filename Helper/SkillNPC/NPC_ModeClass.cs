using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillNPC
{
    /// <summary>
    /// NPC的状态类
    /// </summary>
    public class NPC_ModeClass
    {
        /// <summary>
        /// 技能的切换字典,通过当前传入的技能来读取技能的切换
        /// </summary>
        public Dictionary<BasicNPCSkill, List<BasicNPCSkill>> SkillDict;
        /// <summary>
        /// 技能的切换函数
        /// </summary>
        /// <param name="currentSkill"></param>
        /// <returns>当返回 null 的时候,则此技能切换作废</returns>
        public virtual BasicNPCSkill SkillChange(BasicNPCSkill currentSkill)
        {
            if (currentSkill != null && SkillDict.TryGetValue(currentSkill, out var SkillList))
            {
                foreach (BasicNPCSkill targetSkill in SkillList)
                {
                    if (currentSkill.SwitchCondition() && targetSkill.ActivationCondition() || targetSkill.CompulsionSwitchSkill(currentSkill))
                    {
                        return targetSkill;
                    }
                }
            }
            return null;
        }
    }
}
