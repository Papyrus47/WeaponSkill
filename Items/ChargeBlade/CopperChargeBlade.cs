using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.ChargeBlade
{
    public class CopperChargeBlade : BasicChargeBlade
    {
        public override Asset<Texture2D> ShieldTex => ModContent.Request<Texture2D>(Texture + "_Shield");
        public override void InitDefaults()
        {
            Item.Size = new(84,90);
            Item.damage = 25;
            Item.knockBack = 3.3f;
            Item.rare = ItemRarityID.Green;
            Item.crit = 10;
            Item.defense = 40;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar,30).AddTile(TileID.Anvils).Register();
        }
    }
}
