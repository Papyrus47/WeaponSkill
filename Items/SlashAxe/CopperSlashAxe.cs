using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.WorldGen;

namespace WeaponSkill.Items.SlashAxe
{
    public class CopperSlashAxe : BasicSlashAxe
    {
        public override Asset<Texture2D> SwordTex => GetSwordTex;

        public override Asset<Texture2D> AxeTex => GetAxeTex;

        public override Asset<Texture2D> DefTex => GetDefTex;
        public override void InitDefaults()
        {
            Item.damage = 8;
            Item.knockBack = 3.5f;
            Item.Size = new(64, 76);

            SwordSize = new(42, 48);
            AxeSize = new(38);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar, 12).AddTile(TileID.Anvils).Register();
        }
    }
}
