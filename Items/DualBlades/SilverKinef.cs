using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.DualBlades
{
    public class SilverKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(46, 44);
            Item.damage = 13;
            Item.knockBack = 0.8f;
            Item.crit = 6;
            Item.scale = 0.7f;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.SilverBar, 15).AddTile(TileID.Anvils).Register();
    }
}
