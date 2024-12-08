using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Configs;
using WeaponSkill.Helper.SkillNPC;
using WeaponSkill.NPCs.Bosses.GlobalBoss.Eye.Modes;

namespace WeaponSkill.NPCs.Bosses.GlobalBoss.Eye
{
    public class EyeofCthulhu : GlobalNPC, ISkillNPC
    {
        public List<NPCSkill_Instantiation> OldSkills { get; set; }
        public NPCSkill_Instantiation CurrentSkill { get; set; }
        public NPCSkillMode CurrentMode { get; set; }
        public NPC NPC;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.EyeofCthulhu && lateInstantiation && BossSetting_Config.Init.ResetBossAI;

        public void Init()
        {
            #region 状态切换
            FristAtkMode fristAtkMode = new FristAtkMode();
            ClosePlayerMode closePlayerMode = new ClosePlayerMode(NPC);
            FarPlayerMode farPlayerMode = new FarPlayerMode(NPC);
            fristAtkMode.SonModes.Add(closePlayerMode);
            fristAtkMode.SonModes.Add(farPlayerMode);
            #endregion
            CurrentMode = fristAtkMode;
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
           Init();
        }
        public override bool PreAI(NPC npc)
        {
            CurrentSkill.AI();
            return false;
        }
    }
}