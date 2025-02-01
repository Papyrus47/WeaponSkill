using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.SlashAxe
{
    public class GoldGreatSlashAxe : BasicSlashAxe
    {
        public override Asset<Texture2D> SwordTex => GetSwordTex;

        public override Asset<Texture2D> AxeTex => GetAxeTex;

        public override Asset<Texture2D> DefTex => GetDefTex;
        public override void InitDefaults()
        {
            Item.damage = 50;
            Item.knockBack = 5.5f;
            Item.Size = new(62, 72);

            SwordSize = new(46, 48);
            AxeSize = new(50);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar, 12).AddTile(TileID.Anvils).Register();
        }
    }
}
