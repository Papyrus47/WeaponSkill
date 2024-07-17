using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace WeaponSkill.Items.DualBlades
{
    public class WoodKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(46, 44);
            Item.damage = 6;
            Item.knockBack = 0.2f;
            Item.crit = 2;
            Item.scale = 0.7f;
        }
        public override void AddRecipes() => CreateRecipe().AddRecipeGroup(RecipeGroupID.Wood, 20).AddTile(TileID.WorkBenches).Register();
    }
}
