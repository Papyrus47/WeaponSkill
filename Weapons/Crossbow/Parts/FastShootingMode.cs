using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Crossbow.Parts
{
    public class FastShootingMode : ModItem, IPartItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(32, 22);
        }
        public override void AddRecipes() => CreateRecipe().AddCondition(new Condition("", () => CrossbowGlobalItem.WeaponID.Contains(Main.LocalPlayer.HeldItem.type))).Register();
    }
}
