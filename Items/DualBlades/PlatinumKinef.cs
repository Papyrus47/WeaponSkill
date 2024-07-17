using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.DualBlades
{
    public class PlatinumKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(46, 44);
            Item.damage = 17;
            Item.knockBack = 0.8f;
            Item.crit = 9;
            Item.scale = 0.7f;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.PlatinumBar, 15).AddTile(TileID.Anvils).Register();
    }
}
