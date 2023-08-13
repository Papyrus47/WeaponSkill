using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Items.LongSword
{
    public class CopperLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(30,70);
            Item.damage = 7;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 4;
        }
        public override void HoldItem(Player player)
        {
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/CopperLongSwordScabbard");
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar, 8).AddTile(TileID.Anvils).Register();
        }
    }
}
