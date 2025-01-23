using WeaponSkill.Command.SkillNPC;

namespace WeaponSkill.NPCs.Bosses.GlobalBoss.Eye.Modes
{
    public class ClosePlayerMode : NPCSkillMode
    {
        public Player Target => Main.player[NPC.target];
        public NPC NPC;
        public ClosePlayerMode(NPC nPC)
        {
            NPC = nPC;
        }
        public override bool ActiveMode() => Target.Distance(NPC.position) <= 700;
        public override bool DeleteMode() => !ActiveMode();
    }
}
