using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace WeaponSkill.Items.DualBlades
{
    public class StrongAle : BasicDualBlades
    {
        public override void InitDefault()
        {
            Item.Size = new(64, 48);
            Item.damage = 24;
            Item.knockBack = 2;
            Item.crit = 7;
            Item.rare = ItemRarityID.LightRed;
            Item.scale = 0.7f;
            Item.value = Item.sellPrice(0, 40);
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.Ale,4).Register();
    }
}
