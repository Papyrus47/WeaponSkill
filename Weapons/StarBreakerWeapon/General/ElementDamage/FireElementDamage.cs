using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.NPCs;
using static WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage.IceElementDamage;

namespace WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage
{
    public class FireElementDamage : ElementDamageType
    {
        public override string DamageName => nameof(FireElementDamage);

        public class FireNPC : WeaponSkillGlobalNPCComponent
        {
            /// <summary>
            /// 火属性的积累
            /// </summary>
            public float accumulate;
            public byte timer;
            public override void AI(NPC npc)
            {
                Dictionary<string, int> elementDef = npc.GetGlobalNPC<ElementDamage_GlobalNPC>().ElementDef;
                timer++;
                if(timer > 20)
                {
                    timer = 0;
                    if(npc.CanBeChasedBy())
                        npc.life -= (int)accumulate;
                    CombatText.NewText(npc.Hitbox, Color.OrangeRed, (int)accumulate, true, true);
                    npc.checkDead();

                    accumulate -= elementDef.TryGetValue(nameof(FireElementDamage), out int def) ? def : 0;
                    if(accumulate <= 0)
                    {
                        Remove = true;
                    }
                }
                else if(timer % 4 == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Torch);
                        dust.velocity.Y -= 4 * Main.rand.NextFloat(0.5f, 1.1f);
                        dust.velocity.X -= npc.velocity.X;
                        dust.color = Color.OrangeRed;
                        dust.scale *= 1.5f;
                    }
                }
            }
        }
        public override Color HitColor => Color.OrangeRed;
        public override void OnApply(NPC npc, int dmgDone)
        {
            if (npc.TryGetGlobalNPC<WeaponSkillGlobalNPC>(out var result) && result.weaponSkillGlobalNPCComponents.Find(x => x is FireNPC) == null)
            {
                FireNPC weaponSkillGlobalNPCComponent = new FireNPC();
                weaponSkillGlobalNPCComponent.accumulate += dmgDone;
                WeaponSkillGlobalNPC.AddComponent(npc, weaponSkillGlobalNPCComponent);
            }
            else
            {
                FireNPC fireNPC = result.weaponSkillGlobalNPCComponents.Find(x => x is FireNPC) as FireNPC;
                fireNPC.accumulate += dmgDone;
            }
        }
    }
}
