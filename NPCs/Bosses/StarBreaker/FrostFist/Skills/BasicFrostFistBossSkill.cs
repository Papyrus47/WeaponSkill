using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command.SkillNPC;

namespace WeaponSkill.NPCs.Bosses.StarBreaker.FrostFist.Skills
{
    public class BasicFrostFistBossSkill : ModNPCSkill_Instantiation
    {
        public FrostFist_Boss frostFist => modNPC as FrostFist_Boss;

        public BasicFrostFistBossSkill(FrostFist_Boss modNPC) : base(modNPC)
        {
        }
        
    }
}
