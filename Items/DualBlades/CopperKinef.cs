using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.DualBlades
{
    public class CopperKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(46, 44);
            Item.damage = 9;
            Item.knockBack = 0.4f;
            Item.crit = 2;
            Item.scale = 0.7f;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.CopperBar,15).AddTile(TileID.Anvils).Register();
    }
}
