using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillsNPC.ChangeRule
{
    public class CommandChange_UnCompletelyOldSkill : CommandChange
    {
        public CommandChange_UnCompletelyOldSkill(NPCSkills_Instantiation targetSkill, List<NPCSkills_Instantiation> oldSkill, NPCSkillsManager skillsManager)
        {
            TargetSkill = targetSkill;
            OldSkill = oldSkill;
            this.skillsManager = skillsManager;
        }

        public override NPCSkills_Instantiation ChangeSkill()
        {
            if (CurrentSkill == null || skillsManager == null) return TargetSkill;

            if (skillsManager.OldSkills.Count >= OldSkill.Count) // 如果当前使用过的技能大于应该有过的旧技能
            {
                for (int i = 0; i < OldSkill.Count; i++)
                {
                    if (OldSkill[i] != skillsManager.OldSkills[^(i + 1)]) // 向后读取,如果有一处对不上,则返回null
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
