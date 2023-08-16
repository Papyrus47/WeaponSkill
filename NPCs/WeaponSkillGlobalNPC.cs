using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.NPCs
{
    public class WeaponSkillGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public List<WeaponSkillGlobalNPCComponent> weaponSkillGlobalNPCComponents = new();
        public override void ResetEffects(NPC npc)
        {
            weaponSkillGlobalNPCComponents.RemoveAll(x => x.Remove);
        }
        public override void AI(NPC npc)
        {
            weaponSkillGlobalNPCComponents.ForEach(x => x.AI(npc));
        }
        /// <summary>
        /// 添加npc组件
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="weaponSkillGlobalNPCComponent"></param>
        /// <param name="cover">覆盖</param>
        public static void AddComponent(NPC npc, WeaponSkillGlobalNPCComponent weaponSkillGlobalNPCComponent,bool cover = false)
        {
            if (!cover)
            {
                npc.GetGlobalNPC<WeaponSkillGlobalNPC>().weaponSkillGlobalNPCComponents.Add(weaponSkillGlobalNPCComponent);
            }
            else
            {
                var find = npc.GetGlobalNPC<WeaponSkillGlobalNPC>().weaponSkillGlobalNPCComponents.Find(x => x.GetType().Equals(weaponSkillGlobalNPCComponent.GetType()));
                if(find != null)
                {
                    find.OnCover(weaponSkillGlobalNPCComponent);
                }
                else
                {
                    npc.GetGlobalNPC<WeaponSkillGlobalNPC>().weaponSkillGlobalNPCComponents.Add(weaponSkillGlobalNPCComponent);
                }
            }
        }
    }
}
