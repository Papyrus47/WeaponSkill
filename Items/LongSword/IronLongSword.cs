using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Items.LongSword
{
    public class IronLongSword : BasicLongSwordItem
    {
        public override void SetDefaults()
        {
            Item.Size = new(52, 100);
            Item.damage = 16;
            Item.knockBack = 2;
            Item.useTime = Item.useAnimation = 30;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.crit = 7;
            Item.scale = 0.7f;
        }
        public override void HoldItem(Player player)
        {
            if (Item.TryGetGlobalItem(out LongSwordGlobalItem longSwordGlobalItem))
            {
                longSwordGlobalItem.ScabbardTex = ModContent.Request<Texture2D>("WeaponSkill/Items/LongSword/DefaultLongSwordScabbard");
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.IronBar, 8).AddTile(TileID.Anvils).Register();
        }
    }
}
