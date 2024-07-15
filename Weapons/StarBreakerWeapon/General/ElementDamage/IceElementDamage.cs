using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.NPCs;

namespace WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage
{
    /// <summary>
    /// 冰属性伤害
    /// </summary>
    public class IceElementDamage : ElementDamageType
    {
        public class FrozenNPC : WeaponSkillGlobalNPCComponent
        {
            /// <summary>
            /// 冰属性的积累
            /// </summary>
            public float accumulate;
            public override void AI(NPC npc)
            {
                Dictionary<string, int> elementDef = npc.GetGlobalNPC<ElementDamage_GlobalNPC>().ElementDef;
                if (accumulate > npc.lifeMax + (elementDef.ContainsKey(nameof(IceElementDamage)) ? elementDef[nameof(IceElementDamage)] : 0))
                {
                    npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime = 120;
                    accumulate = 0;
                }
            }
        }
        public override string DamageName => nameof(IceElementDamage);
        public override Color HitColor => Color.SkyBlue;
        public override void OnApply(NPC npc, int dmgDone)
        {
            if(npc.TryGetGlobalNPC<WeaponSkillGlobalNPC>(out var result) && result.weaponSkillGlobalNPCComponents.Find(x => x is FrozenNPC) == null)
            {
                WeaponSkillGlobalNPC.AddComponent(npc, new FrozenNPC());
            }
            else
            {
                FrozenNPC frozenNPC = result.weaponSkillGlobalNPCComponents.Find(x => x is FrozenNPC) as FrozenNPC;
                frozenNPC.accumulate += dmgDone;
            }
        }
    }
}
