using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.SwordShield
{
    public class CopperSwordShield : BasicSwordShield
    {
        public override Asset<Texture2D> ShieldTex => GetShieldTex;
        public override void InitDefaults()
        {
            Item.damage = 6;
            Item.knockBack = 0.5f;
            Item.defense = 10;
            Item.Size = new(38, 42);
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.CopperBar, 20).AddTile(TileID.Anvils).Register();
    }
}
