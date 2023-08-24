using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillsNPC.ChangeRule
{
    /// <summary>
    /// 固定切换-技能表完全符合
    /// </summary>
    public class CommandChange_CompletelyOldSkill : CommandChange
    {
        public CommandChange_CompletelyOldSkill(NPCSkills_Instantiation targetSkill, List<NPCSkills_Instantiation> oldSkill, NPCSkillsManager skillsManager)
        {
            TargetSkill = targetSkill;
            OldSkill = oldSkill;
            this.skillsManager = skillsManager;
        }

        public override NPCSkills_Instantiation ChangeSkill()
        {
            if(CurrentSkill == null || skillsManager == null) return TargetSkill;

            if(skillsManager.OldSkills.Count == OldSkill.Count) // 如果当前使用过的技能,与应该有过的旧技能数量一致
            {
                for(int i = 0;i < skillsManager.OldSkills.Count;i++)
                {
                    if (skillsManager.OldSkills[i] != OldSkill[i]) // 如果旧的技能表有一处对不上
                    {
                        return null;
                    }
                }
                return TargetSkill;
            }
            return null;
        }
    }
}
