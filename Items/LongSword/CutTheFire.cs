using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;
using Terraria.ID;
using WeaponSkill.NPCs;

namespace WeaponSkill.Items.LongSword
{
    public class CutTheFire : BasicLongSwordItem
    {
        public class CutTheFire_OnHitNPC : WeaponSkillGlobalNPCComponent
        {
            public int Time;
            public float DamageCount;
            public Player player;
            public NPC.HitInfo hitInfo;
            public CutTheFire_OnHitNPC(int time, Player player, NPC.HitInfo hitInfo)
            {
                Time = time;
                DamageCount = 1;
                this.player = player;
                this.hitInfo = hitInfo;
            }
            public override void OnCover(WeaponSkillGlobalNPCComponent weaponSkillGlobalNPCComponent)
            {
                Time += (weaponSkillGlobalNPCComponent as CutTheFire_OnHitNPC).Time / 5;
                DamageCount += 0.2f;
            }
            public override void AI(NPC npc)
            {
                if(Time-- < 0)
                {
                    Remove = true;
                    Projectile.NewProjectile(player.GetSource_OnHit(npc),npc.Center,new Vector2(0,-player.gravity), 978,(int)(hitInfo.SourceDamage * DamageCount), hitInfo.Knockback, player.whoAmI, 0f, 2);
                }
            }
        }
        public override void SetDefaults()
        {
            Item.Size = new(48,98);
            Item.damage = 25;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 13;
            Item.scale = 0.8f;
            Item.rare = ItemRarityID.Green;
        }
        public override void HoldItem(Player player)
        {
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/CutTheFireScabbard");
            }
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            WeaponSkillGlobalNPC.AddComponent(target, new CutTheFire_OnHitNPC(90, player, hit),true);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.AntlionMandible, 20).AddRecipeGroup(RecipeGroupID.IronBar).AddTile(TileID.Anvils).Register();
        }
    }
}
