using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.DualBlades
{
    public class GoldKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(46, 44);
            Item.damage = 16;
            Item.knockBack = 0.8f;
            Item.crit = 6;
            Item.scale = 0.7f;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.GoldBar, 15).AddTile(TileID.Anvils).Register();
    }
}
