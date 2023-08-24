using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillsNPC
{
    public class NPCSkillsManager
    {
        public NPCSkills_Instantiation CurrentSkill;
        public List<NPCSkillsPool> SkillPool;
        public List<NPCSkills_Instantiation> OldSkills;
        /// <summary>
        /// 是否改变了技能
        /// </summary>
        /// <returns>技能切换成功返回true,否则为false</returns>
        public virtual bool ChangeSkill()
        {
            foreach (NPCSkillsPool skillPool in SkillPool)
            {
                var getSkill = skillPool.ChagneSkill(CurrentSkill);
                if (getSkill != null)
                {
                    OldSkills.Add(CurrentSkill);
                    CurrentSkill = getSkill;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 切换到过去的技能,这个建议技能用,而且是挂在技能里面使用
        /// </summary>
        /// <param name="index">回滚索引数量,注意,是从索引 - 1开始的</param>
        /// <returns></returns>
        public virtual bool ChangeToOldSkill(int index)
        {
            var getSkill = OldSkills[^(index + 1)];
            if (getSkill != null)
            {
                getSkill.OnSkillActive();
                CurrentSkill.OnSkillDeactivate(getSkill);
                CurrentSkill = getSkill;
                OldSkills.RemoveAll(x => OldSkills.IndexOf(x) > OldSkills.Count - index - 1);
                return true;
            }
            return false;
        }
    }
}
