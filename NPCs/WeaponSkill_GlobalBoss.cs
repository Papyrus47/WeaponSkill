using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Configs;

namespace WeaponSkill.NPCs
{
    public class WeaponSkill_GlobalBoss : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.boss && entity.ModNPC is null && BossSetting_Config.Init.ResetBossAI;
        public override bool PreAI(NPC npc)
        {
            return base.PreAI(npc);
        }
    }
}
