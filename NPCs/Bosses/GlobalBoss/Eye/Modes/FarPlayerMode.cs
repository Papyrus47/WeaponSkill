using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command.SkillNPC;

namespace WeaponSkill.NPCs.Bosses.GlobalBoss.Eye.Modes
{
    public class FarPlayerMode : NPCSkillMode
    {
        public Player Target => Main.player[NPC.target];
        public NPC NPC;
        public FarPlayerMode(NPC nPC)
        {
            NPC = nPC;
        }
        public override bool ActiveMode() => Target.Distance(NPC.position) > 700;
        public override bool DeleteMode() => !ActiveMode();
    }
}
