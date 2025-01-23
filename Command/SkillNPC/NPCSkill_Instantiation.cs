using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.SkillNPC
{
    public abstract class NPCSkill_Instantiation
    {
        public NPC NPC;
        public List<NPCSkill_Instantiation> switchToSkill;
        public bool SkillTimeOut;
        /// <summary>
        /// 激活这个技能的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool ActivationCondition() => true;
        public void AddBySkill(params NPCSkill_Instantiation[] skill)
        {
            foreach (var skillItem in skill)
            {
                skillItem.AddSkill(this);
            }
        }
        public NPCSkill_Instantiation AddSkill(NPCSkill_Instantiation projSkill)
        {
            switchToSkill.Add(projSkill);
            return projSkill;
        }
        public List<NPCSkill_Instantiation> AddSkilles(params NPCSkill_Instantiation[] projSkill)
        {
            switchToSkill.AddRange(projSkill);
            return new List<NPCSkill_Instantiation>(projSkill);
        }
        public virtual void AI() { }
        public virtual bool? CanDamage() => null;
        /// <summary>
        /// 强制切换到这个技能,无视前者的条件
        /// </summary>
        /// <returns></returns>
        public virtual bool CompulsionSwitchSkill(NPCSkill_Instantiation nowSkill) => false;
        /// <summary>
        /// 技能激活时调用
        /// </summary>
        public virtual void OnSkillActive() { }
        /// <summary>
        /// 技能切换后调用
        /// </summary>
        public virtual void OnSkillDeactivate() { }
        public virtual bool PreDraw(SpriteBatch spriteBatch,Vector2 scrennPos,Color drawColor) => true;
        /// <summary>
        /// 技能切换的条件
        /// </summary>
        /// <returns>返回true 则可以切换技能</returns>
        public virtual bool SwitchCondition() => true;
    }
}
