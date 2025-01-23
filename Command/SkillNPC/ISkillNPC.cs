using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.SkillNPC
{
    /// <summary>
    /// 官方NPC和ModNPC都通用
    /// </summary>
    public interface ISkillNPC
    {
        public List<NPCSkill_Instantiation> OldSkills { get; set; }
        public NPCSkill_Instantiation CurrentSkill { get; set; }
        public NPCSkillMode CurrentMode { get; set; }
        public void Init();
        public void SwitchSkill()
        {
            if (CurrentSkill.SkillTimeOut)
            {
                if (PreSkillTimeOut())
                {
                    var targetSkill = OldSkills[0];
                    CurrentSkill.OnSkillDeactivate();
                    targetSkill.OnSkillActive();
                    CurrentSkill = targetSkill;
                    OldSkills.Clear();
                }
                return;
            }
            foreach (var targetSkill in CurrentSkill.switchToSkill)
            {
                if ((CurrentSkill.SwitchCondition() && targetSkill.ActivationCondition()) || targetSkill.CompulsionSwitchSkill(CurrentSkill))
                {
                    CurrentSkill.OnSkillDeactivate();
                    targetSkill.OnSkillActive();
                    OldSkills.Add(CurrentSkill);
                    CurrentSkill = targetSkill;
                    return;
                }
            }
        }
        /// <summary>
        /// 切换模式
        /// </summary>
        public void SwitchMode()
        {
            // 首先读取子状态
            // 如果子状态读取失败
            // 读取父状态直到最高级

            do
            {
                bool change = false;
                foreach(var targetMode in CurrentMode.SonModes)
                {
                    if(targetMode.ActiveMode() && CurrentMode.DeleteMode())
                    {
                        targetMode.OnActive();
                        CurrentMode.OnDelete();
                        CurrentMode = targetMode;
                        change = true;
                        break;
                    }
                }

                if(!change)
                    CurrentMode = CurrentMode.FatherMode;
            }
            while (CurrentMode.FatherMode == null);

            // 特别切换状态
            var mode = CurrentMode.ChangeMode();
            if(mode != null) 
                CurrentMode = mode;
        }
        /// <summary>
        /// 返回false阻止直接调用回起点
        /// </summary>
        /// <returns></returns>
        public bool PreSkillTimeOut() => true;
    }
}
