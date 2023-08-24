using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponSkill.Helper.SkillsNPC.ChangeRule
{
    /// <summary>
    /// 随机切换
    /// </summary>
    public class RandomChange : INPCSkillsChangeRule
    {
        public NPCSkills_Instantiation CurrentSkill { get; set; }
        public Dictionary<NPCSkills_Instantiation,float> randomChangeSkill;
        public RandomChange(params (NPCSkills_Instantiation,float)[] skills)
        {
            randomChangeSkill = new(); // 添加一个new
            for(int i = 0;i< skills.Length; i++) 
            {
                randomChangeSkill.Add(skills[i].Item1, skills[i].Item2);
            }
        }
        public NPCSkills_Instantiation ChangeSkill()
        {
            if (randomChangeSkill.Count == 0) return null;

            float randMax = 0f;
            // 获取随机数的上限
            foreach (var skill in randomChangeSkill)
            {
                randMax += skill.Value;
            }

            float rand = Main.rand.NextFloat(randMax); // 根据上限获取随机数
            float factor = 0;
            foreach (var skill in randomChangeSkill)
            {
                if(skill.Value + factor > rand) // 累计值大于随机数后
                {
                    return skill.Key;
                }
                factor += skill.Value; // 改变factor变量,修正下一个
            }
            return null;
        }

        public virtual void OnChange(NPCSkills_Instantiation target)
        {
            target.OnSkillActive();
            CurrentSkill.OnSkillDeactivate(target);
        }
    }
}
