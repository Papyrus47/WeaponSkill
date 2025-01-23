using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.SkillNPC
{
    /// <summary>
    /// NPC的行动Mode
    /// </summary>
    public abstract class NPCSkillMode
    {
        /// <summary>
        /// 子状态
        /// </summary>
        public List<NPCSkillMode> SonModes = new();
        /// <summary>
        /// 父状态
        /// </summary>
        public NPCSkillMode FatherMode;
        /// <summary>
        /// 启动状态
        /// </summary>
        /// <returns></returns>
        public virtual bool ActiveMode() => false;
        public virtual void OnActive() { }
        /// <summary>
        /// 卸载状态
        /// </summary>
        /// <returns></returns>
        public virtual bool DeleteMode() => false;
        public virtual void OnDelete() { }
        /// <summary>
        /// 特别切换状态
        /// </summary>
        public virtual NPCSkillMode ChangeMode() => null;
    }
}
