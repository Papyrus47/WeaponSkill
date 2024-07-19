using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace WeaponSkill.Weapons.Crossbow.Parts
{
    public class AddDamageMode : ModItem, IPartItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(32, 22);
        }
        public override void AddRecipes() => CreateRecipe().AddCondition(new Condition(Language.GetTextValue("Mods.WeaponSkill.CrossbowPart"), () => CrossbowGlobalItem.WeaponID.Contains(Main.LocalPlayer.HeldItem.type))).Register();
    }
}
