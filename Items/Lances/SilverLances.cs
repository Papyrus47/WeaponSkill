using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.Lances
{
    public class SilverLances : BasicLancesItem
    {
        public override Asset<Texture2D> ShieldTex => ModContent.Request<Texture2D>(Texture + "S");
        public override Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture + "P");
        public override void InitDefaults()
        {
            Item.Size = new(64);
            Item.damage = 19;
            Item.knockBack = 5.3f;
            Item.rare = ItemRarityID.Green;
            Item.crit = 2;
            Item.defense = 52;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SilverBar, 30).AddTile(TileID.Anvils).Register();
        }
    }
}
