using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Items.DualBlades.VolcanoKinef;
using WeaponSkill.NPCs;

namespace WeaponSkill.Items.LongSword
{
    public class VolcanoLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(46, 92);
            Item.damage = 23;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 8;
            Item.rare = ItemRarityID.Orange;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.HellstoneBar, 10).AddTile(TileID.Anvils).Register();
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            WeaponSkillGlobalNPC.AddComponent(target, new VolcanoKinef_OnHitNPC(30, player, hit), true);
        }
    }
}
