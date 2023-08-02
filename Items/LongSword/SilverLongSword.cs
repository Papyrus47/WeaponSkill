using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.LongSword
{
    public class SilverLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(52, 100);
            Item.damage = 19;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 7;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SilverBar, 8).AddTile(TileID.WorkBenches).Register();
        }
    }
}
