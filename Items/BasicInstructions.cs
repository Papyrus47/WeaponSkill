using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace WeaponSkill.Items
{
    public abstract class BasicInstructions : ModItem
    {
        public override string Texture => "Terraria/Images/Item_149";
        public override void SetDefaults()
        {
        }
        public override void AddRecipes()
        {
            CreateRecipe().Register();
        }
    }
}
