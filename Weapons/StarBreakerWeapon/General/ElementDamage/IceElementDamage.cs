using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.NPCs;
using static WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage.FireElementDamage;

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
            /// <summary>
            /// 冰冻次数
            /// </summary>
            public int FrozenCount;
            public override void AI(NPC npc)
            {
                Dictionary<string, int> elementDef = npc.GetGlobalNPC<ElementDamage_GlobalNPC>().ElementDef;
                if (accumulate > npc.life + (elementDef.TryGetValue(nameof(IceElementDamage), out int value) ? value : 0) * (FrozenCount + 1))
                {
                    npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrozenNPCTime = 120;
                    accumulate = 0;
                    if(FrozenCount < 4)
                        FrozenCount++;
                }
            }
        }
        public override string DamageName => nameof(IceElementDamage);
        public override Color HitColor => Color.LightSkyBlue;
        public override void OnApply(NPC npc, int dmgDone)
        {
            if(npc.TryGetGlobalNPC<WeaponSkillGlobalNPC>(out var result) && result.weaponSkillGlobalNPCComponents.Find(x => x is FrozenNPC) == null)
            {
                FrozenNPC weaponSkillGlobalNPCComponent = new FrozenNPC();
                weaponSkillGlobalNPCComponent.accumulate += dmgDone;
                WeaponSkillGlobalNPC.AddComponent(npc, weaponSkillGlobalNPCComponent);
            }
            else
            {
                FrozenNPC frozenNPC = result.weaponSkillGlobalNPCComponents.Find(x => x is FrozenNPC) as FrozenNPC;
                frozenNPC.accumulate += dmgDone;
            }
        }
    }
}
