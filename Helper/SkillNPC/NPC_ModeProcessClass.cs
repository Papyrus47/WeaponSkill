using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillNPC
{
    public class NPC_ModeProcessClass
    {
        /// <summary>
        /// 状态储存类
        /// </summary>
        public List<NPC_ModeClass> ModeList;
        public int CurrentModeIndex;
        public NPC_ModeClass CurrentMode
        {
            get => ModeList[CurrentModeIndex];
            set => CurrentModeIndex = ModeList.FindIndex(x => x == value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSkill"></param>
        /// <returns>返回false则说明技能切换失败</returns>
        public bool ChangeSkill(BasicNPCSkill currentSkill)
        {
            return CurrentMode.SkillChange(currentSkill) != null;
        }
    }
}
