using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Helper.SkillNPC;

namespace WeaponSkill.NPCs.Bosses.StarBreaker.FrostFist
{
    public class FrostFist_Boss : ModNPC, ISkillNPC
    {
        public List<NPCSkill_Instantiation> OldSkills { get; set; }
        public NPCSkill_Instantiation CurrentSkill { get; set; }
        public NPCSkillMode CurrentMode { get; set; }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new()
            {
                Hide = true
            });
        }
        public override void OnSpawn(IEntitySource source)
        {
            Init();
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 50000;
            NPC.defense = 10;
            NPC.damage = 20;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.Size = new(32, 54);
        }
        public void Init()
        {
        }
        public override void AI()
        {
            CurrentSkill.AI();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return CurrentSkill.PreDraw(spriteBatch,screenPos,drawColor);
        }
        public override bool IsLoadingEnabled(Mod mod) => false;
    }
}
