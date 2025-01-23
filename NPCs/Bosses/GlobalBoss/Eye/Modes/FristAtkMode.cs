using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command.SkillNPC;

namespace WeaponSkill.NPCs.Bosses.GlobalBoss.Eye.Modes
{
    /// <summary>
    /// 一阶段
    /// </summary>
    public class FristAtkMode : NPCSkillMode
    {
        public NPC NPC;
        /// <summary>
        /// 二阶段AI
        /// </summary>
        public NPCSkillMode NextMode;
        public override bool DeleteMode() => true;
        public override NPCSkillMode ChangeMode()
        {
            if(NPC.life < NPC.lifeMax * 0.5f)
            {
                return NextMode;
            }
            return base.ChangeMode();
        }
    }
}
