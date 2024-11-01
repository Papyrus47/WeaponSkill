using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.Lances
{
    public class PlatinumLances : BasicLancesItem
    {
        public override Asset<Texture2D> ShieldTex => ModContent.Request<Texture2D>(Texture + "_Shield");
        public override Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture + "_Proj");
        public override void InitDefaults()
        {
            Item.Size = new(64);
            Item.damage = 24;
            Item.knockBack = 5.3f;
            Item.rare = ItemRarityID.Green;
            Item.crit = 2;
            Item.defense = 55;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.PlatinumBar, 30).AddTile(TileID.Anvils).Register();
        }
    }
}
