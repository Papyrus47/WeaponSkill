using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.Lances
{
    public class WoodLances : BasicLancesItem
    {
        public override Asset<Texture2D> ShieldTex => ModContent.Request<Texture2D>(Texture + "_Shield");
        public override Asset<Texture2D> ProjTex => ModContent.Request<Texture2D>(Texture + "_Proj");
        public override void InitDefaults()
        {
            Item.Size = new(64);
            Item.damage = 12;
            Item.knockBack = 3.3f;
            Item.rare = ItemRarityID.White;
            Item.crit = 2;
            Item.defense = 40;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddRecipeGroup(RecipeGroupID.Wood,100).AddTile(TileID.WorkBenches).Register();
        }
    }
}
