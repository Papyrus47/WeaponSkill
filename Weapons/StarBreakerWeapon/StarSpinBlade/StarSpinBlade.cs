using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade
{
    public class StarSpinBlade : ModItem, StarBreakerMoreItemPart
    {
        public LocalizedText PartText => Language.GetOrRegister("Mods.WeaponSkill.Items.StarSpinBlade.SPText");
        /// <summary>
        /// 正负回旋积累
        /// </summary>
        public int SpinValue;
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Melee;
            Item.crit = 0;
            Item.useTime = Item.useAnimation = 1;
            Item.Size = new(112,114);
            Item.rare = ModContent.RarityType<SSB_Rarity>();
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTurn = false;
            Item.shoot = ModContent.ProjectileType<StarSpinBladeProj>();
            Item.knockBack = 9999999999;
            Item.noMelee = true;
            Item.value = 15234960;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void HoldItem(Player player)
        {
            player.manaCost += 0.95f;
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.position, Vector2.Zero, Item.shoot, player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; ++i)
            {
                if (tooltips[i].Name == "Speed")
                {
                    tooltips[i].Text = "???速度";
                    tooltips[i].OverrideColor = Color.Purple;
                }
            }
        }
    }
}
