using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist
{
    public class FrostFistItem : ModItem,SPHealMana,StarBreakerMoreItemPart
    {
        public override string Texture => (GetType().Namespace + "." + "FrostFist").Replace('.', '/');

        public LocalizedText PartText => Language.GetText("Mods.WeaponSkill.Items.FrostFistItem.SPText");

        /// <summary>
        /// 霜拳的蓄力等级
        /// </summary>
        public int ChangeLevel;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 0;
            Item.useTime = Item.useAnimation = 1;
            Item.Size = new(32,54);
            Item.rare = ModContent.RarityType<FrostFistRarity>();
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTurn = false;
            Item.shoot = ModContent.ProjectileType<FrostFistProj>();
            Item.knockBack = 0.5f;
            Item.noMelee = true;
            Item.value = 15234960;
        }
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage = new();
        }
        public override bool RangedPrefix() => false;
        public override bool AllowPrefix(int pre) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void HoldItem(Player player)
        {
            if (ChangeLevel > 20) ChangeLevel = 20;
            else if(ChangeLevel < 0) ChangeLevel = 0;
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.position, Vector2.Zero,Item.shoot, player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }

            if (player.statMana >= player.statManaMax2 && player.statLife < player.statLifeMax2)
            {
                player.statLife += player.statLifeMax2 / 50;
                if (player.statLife > player.statLifeMax2) 
                    player.statLife = player.statLifeMax2;
                player.Heal(player.statLifeMax2 / 50);
                player.statMana -= 1000;
            }
        }
    }
}
