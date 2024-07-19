using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;

namespace WeaponSkill.Items.Books
{
    public abstract class BasicInstructions : ModItem
    {
        public override string Texture => "Terraria/Images/Item_149";
        public override void SetDefaults()
        {
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddCondition(new Condition(Language.GetTextValue("Mods.WeaponSkill.BookCondition"),() => Main.LocalPlayer.HeldItem?.IsAir == true)).Register();
        }
    }
}
