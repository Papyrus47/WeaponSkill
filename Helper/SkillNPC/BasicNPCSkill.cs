using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper.SkillNPC
{

    public abstract class BasicNPCSkill
    {
        public ModNPC modNPC;
        public NPC NPC;
        public BasicNPCSkill(ModNPC modNPC)
        {
            this.modNPC = modNPC;
            NPC = modNPC.NPC;
        }
        /// <summary>
        /// 激活这个技能的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool ActivationCondition() => true;
        /// <summary>
        /// 技能切换的条件
        /// </summary>
        /// <returns>返回true 则可以切换技能</returns>
        public virtual bool SwitchCondition() => true;
        /// <summary>
        /// 强制切换到这个技能,无视前者的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool CompulsionSwitchSkill(BasicNPCSkill nowSkill) => false;
    }
}
