using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponSkill.NPCs
{
    public class WeaponSkillGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public List<WeaponSkillGlobalNPCComponent> weaponSkillGlobalNPCComponents = new();
        public bool CanUpdate = true;
        /// <summary>
        /// 霜拳特殊的冻结NPC
        /// </summary>
        public int FrostFist_FrozenNPCTime = 0;
        public int FrostFist_Seal = 0; // 霜拳封印术
        public override void Load()
        {
            On_NPC.UpdateNPC += On_NPC_UpdateNPC;
        }
        public override void Unload()
        {
            On_NPC.UpdateNPC -= On_NPC_UpdateNPC;
        }
        private static void On_NPC_UpdateNPC(On_NPC.orig_UpdateNPC orig, NPC self, int i)
        {
            if (self.TryGetGlobalNPC<WeaponSkillGlobalNPC>(out var skill) && (!skill.CanUpdate || (skill.FrostFist_FrozenNPCTime > 0 && self.collideY)))
            {
                if (skill.FrostFist_FrozenNPCTime > 0)
                {
                    if (--skill.FrostFist_FrozenNPCTime <= 0)
                    {
                        for (int j = 0; j < 30; j++)
                        {
                            Dust dust = Dust.NewDustDirect(self.position, self.width, self.height, DustID.FrostStaff, 0, 0, 150, default, 1.3f);
                            dust.noGravity = true;
                        }
                        NPC n = new NPC();
                        n.SetDefaults(self.type);
                        self.noTileCollide = n.noTileCollide;
                        self.noGravity = n.noGravity;
                    }
                }
                skill.CanUpdate = true;
                return;
            }
            orig.Invoke(self, i);
        }
        public override bool PreAI(NPC npc)
        {
            if (FrostFist_FrozenNPCTime > 0)
            {
                npc.velocity.X *= 0.5f;
                npc.noTileCollide = false;
                npc.noGravity = false;
                return false;
            }
            return base.PreAI(npc);
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (!CanUpdate || FrostFist_FrozenNPCTime > 0)
            {
                return false;
            }
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        public override void ResetEffects(NPC npc)
        {
            weaponSkillGlobalNPCComponents.RemoveAll(x => x.Remove);
        }
        public override void AI(NPC npc)
        {
            weaponSkillGlobalNPCComponents.ForEach(x => x.AI(npc));

            #region 霜拳冻结特殊判定
            if(npc.realLife != -1)
            {
                Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime = FrostFist_FrozenNPCTime;
            }
            if (npc.realLife != -1 && Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime > 0) FrostFist_FrozenNPCTime = Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime;
            #endregion
            #region 霜拳封印术
            if (npc.realLife != -1)
            {
                Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_Seal = FrostFist_Seal;
            }
            if (npc.realLife != -1 && Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_Seal > 0) FrostFist_Seal = Main.npc[npc.realLife].GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_Seal;
            if (FrostFist_Seal-- >= 0)
            {
                if (npc.velocity.LengthSquared() > 100) npc.velocity = npc.velocity.SafeNormalize(default) * 10;
                npc.damage = npc.defDamage / 5;
                if(FrostFist_Seal == 0)
                {
                    npc.damage = npc.defDamage;
                }
            }
            #endregion
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            weaponSkillGlobalNPCComponents.ForEach(x => x.PostDraw(npc, spriteBatch, screenPos, drawColor));

            #region 霜拳的冻结
            if (FrostFist_FrozenNPCTime > 0)
            {
                Color color = drawColor;
                color.A = 0;
                Texture2D tex = TextureAssets.Frozen.Value;
                Vector2 origin = tex.Size() * 0.5f;
                for (int i = 0; i < 2; i++)
                {
                    spriteBatch.Draw(tex, npc.Center - screenPos, null, color, 0, origin, new Vector2((float)npc.width / tex.Width,(float)npc.height / tex.Height) * 1.5f, SpriteEffects.None, 0f);
                }
            }
            #endregion
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
