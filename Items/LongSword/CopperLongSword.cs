using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace WeaponSkill.Items.LongSword
{
    public class CopperLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(52, 100);
            Item.damage = 7;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 4;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar, 8).AddTile(TileID.Anvils).Register();
        }
    }
}
