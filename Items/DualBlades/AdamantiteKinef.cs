using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.DualBlades
{
    public class AdamantiteKinef : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(40);
            Item.damage = 49;
            Item.knockBack = 2;
            Item.crit = 10;
            Item.rare = ItemRarityID.LightRed;
            Item.scale = 1.3f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.AdamantiteBar, 15).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
