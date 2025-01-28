using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.WorldGen;

namespace WeaponSkill.Items.InsectStaff.Weapons
{
    public class CopperInsectStaff : BasicInsectStaffItem
    {
        public override void InitDefaults()
        {
            Item.damage = 8;
            Item.knockBack = 5.5f;
            Item.Size = new(76);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar, 12).AddTile(TileID.Anvils).Register();
        }
    }
}
