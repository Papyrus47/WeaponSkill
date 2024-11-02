using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Items.Talisman
{
    public abstract class BaiscAttackTalisman : BasicDefenseTalisman
    {
        public override string Texture => (GetType().Namespace + ".AttackTalisman").Replace('.', '/');
        public override void SetDefaults()
        {
            Item.Size = new(30, 26);
            Item.accessory = true;
            Item.rare = ItemRarityID.Quest;
        }
    }
    public class AttackTalisman1 : BaiscAttackTalisman
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 1);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic).Base += 3;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteOre, 30).AddTile(TileID.Anvils).Register();
            CreateRecipe().AddIngredient(ItemID.CrimtaneOre, 30).AddTile(TileID.Anvils).Register();
        }
    }
    public class AttackTalisman2 : BaiscAttackTalisman
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic).Base += 6;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(1, 1);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AttackTalisman2>().AddIngredient(ItemID.CrystalShard, 30).AddTile(TileID.Anvils).Register();
        }
    }
    public class AttackTalisman3 : BaiscAttackTalisman
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic).Base += 9;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 10000);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AttackTalisman2>().AddIngredient(ItemID.HallowedBar, 30).AddTile(TileID.Anvils).Register();
        }
    }
    public class AttackTalisman4 : BaiscAttackTalisman
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 50000);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic).Base += 12;
            player.GetCritChance(DamageClass.Generic) += 5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AttackTalisman3>().AddIngredient(ItemID.ChlorophyteBar, 30).AddTile(TileID.Anvils).Register();
        }
    }
    public class AttackTalisman5 : BaiscAttackTalisman
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.value = Item.sellPrice(gold: 50000);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic).Base += 15;
            player.GetCritChance(DamageClass.Generic) += 5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<AttackTalisman4>().AddIngredient(ItemID.FragmentSolar, 12).AddIngredient(ItemID.FragmentNebula,12).AddIngredient(ItemID.FragmentStardust).AddIngredient(ItemID.FragmentVortex,12).AddTile(TileID.Anvils).Register();
        }
    }
}
