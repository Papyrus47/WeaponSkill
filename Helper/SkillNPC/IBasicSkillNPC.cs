using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillNPC
{
    public interface IBasicSkillNPC
    {
        public BasicNPCSkill CurrentSkill { get; set; }
        public NPC_ModeProcessClass ModeProcess { get; set; }
        /// <summary>
        /// 老一套切换技能类了
        /// </summary>
        public void ChangeSkill()
        {
            if (ModeProcess == null) return;
            if (!ModeProcess.ChangeSkill(CurrentSkill))
            {
                ChangeSkillFail();
            }
            else
            {
            }
        }
        public void Init();
        /// <summary>
        /// <para>技能切换失败函数(理论不成功会一直运行)</para>
        /// 使用<see cref="BasicNPCSkill.SwitchCondition"/>函数判断是否处于可以技能切换,一般用于回归某个AI使用
        /// </summary>
        public void ChangeSkillFail() { }
    }
}
