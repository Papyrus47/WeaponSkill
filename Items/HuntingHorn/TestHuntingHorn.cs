using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI.Chat;
using WeaponSkill.Weapons.HuntingHorn.Melodies;

namespace WeaponSkill.Items.HuntingHorn
{
    public class TestHuntingHorn : BasicHuntingHornItem
    {
        public override void InitDefaults()
        {
            if(globalItem != null) 
                globalItem.hornMelody = new HealthMelody1();
            Item.damage = 8;
            Item.knockBack = 5.5f;
            Item.Size = new(56);
            Item.scale = 2;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CopperBar, 12).AddTile(TileID.Anvils).Register();
        }
    }
}
