using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.NPCs
{
    public abstract class WeaponSkillGlobalNPCComponent
    {
        public bool Remove;
        public virtual void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { }
        public virtual void OnCover(WeaponSkillGlobalNPCComponent weaponSkillGlobalNPCComponent) { }
        public virtual void AI(NPC npc) { }
    }
}
