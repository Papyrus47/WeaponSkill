using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace WeaponSkill.Items.Talisman
{
    public abstract class BasicDefenseTalisman : BasicTalisman
    {
        public override string Texture => (GetType().Namespace + ".DefenseTalisman").Replace('.','/');
        public override void SetDefaults()
        {
            Item.Size = new(30, 26);
            Item.accessory = true;
            Item.rare = ItemRarityID.Quest;
        }
    }
    public class DefenseTalisman1 : BasicDefenseTalisman
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 5;
            Item.value = Item.sellPrice(gold: 1);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteOre, 30).AddTile(TileID.Anvils).Register();
            CreateRecipe().AddIngredient(ItemID.CrimtaneOre, 30).AddTile(TileID.Anvils).Register();
        }
    }
    public class DefenseTalisman2 : BasicDefenseTalisman
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 15;
            Item.value = Item.sellPrice(1,1);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<DefenseTalisman1>().AddIngredient(ItemID.CrystalShard, 30).AddTile(TileID.Anvils).Register();
        }
    }
    public class DefenseTalisman3 : BasicDefenseTalisman
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 25;
            Item.value = Item.sellPrice(gold: 10000);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<DefenseTalisman2>().AddIngredient(ItemID.HallowedBar, 30).AddTile(TileID.Anvils).Register();
        }
    }
    public class DefenseTalisman4 : BasicDefenseTalisman
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 25;
            Item.value = Item.sellPrice(gold: 50000);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense *= 1.05f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<DefenseTalisman3>().AddIngredient(ItemID.ChlorophyteBar, 30).AddTile(TileID.Anvils).Register();
        }
    }
    public class DefenseTalisman5 : BasicDefenseTalisman
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 25;
            Item.value = Item.sellPrice(gold: 50000);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense *= 1.1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<DefenseTalisman3>().AddIngredient(ItemID.FragmentSolar, 30).AddTile(TileID.Anvils).Register();
        }
    }
}
