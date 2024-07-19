using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.NPCs;
using static WeaponSkill.Items.LongSword.CutTheFire;
using Terraria.ID;

namespace WeaponSkill.Items.DualBlades
{
    public class VolcanoKinef : BasicDualBlades
    {
        public class VolcanoKinef_OnHitNPC : WeaponSkillGlobalNPCComponent
        {
            public int Time;
            public float DamageCount;
            public Player player;
            public NPC.HitInfo hitInfo;
            public VolcanoKinef_OnHitNPC(int time, Player player, NPC.HitInfo hitInfo)
            {
                Time = time;
                DamageCount = 1;
                this.player = player;
                this.hitInfo = hitInfo;
            }
            public override void OnCover(WeaponSkillGlobalNPCComponent weaponSkillGlobalNPCComponent)
            {
                Time += (weaponSkillGlobalNPCComponent as VolcanoKinef_OnHitNPC).Time / 5;
                DamageCount += 0.01f;
            }
            public override void AI(NPC npc)
            {
                if (Time-- < 0)
                {
                    Remove = true;
                    Projectile.NewProjectile(player.GetSource_OnHit(npc), npc.Center, new Vector2(0, -player.gravity), 978, (int)(hitInfo.SourceDamage * DamageCount), hitInfo.Knockback, player.whoAmI, 0f, 2);
                }
            }
        }
        public override void InitDefault()
        {
            Item.Size = new(46, 44);
            Item.damage = 53;
            Item.knockBack = 1.8f;
            Item.crit = 15;
            Item.rare = ItemRarityID.Orange;
            Item.scale = 0.7f;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.HellstoneBar, 15).AddTile(TileID.Anvils).Register();
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            WeaponSkillGlobalNPC.AddComponent(target, new VolcanoKinef_OnHitNPC(30, player, hit), true);
        }
    }
}
