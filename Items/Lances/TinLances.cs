using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.Lances
{
    public class TinLances : BasicLancesItem
    {
        public override Asset<Texture2D> ShieldTex => ModContent.Request<Texture2D>(Texture + "S");
        public override Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture + "P");
        public override void InitDefaults()
        {
            Item.Size = new(64);
            Item.damage = 15;
            Item.knockBack = 5.3f;
            Item.rare = ItemRarityID.Green;
            Item.crit = 2;
            Item.defense = 38;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.TinBar, 30).AddTile(TileID.Anvils).Register();
        }
    }
}
