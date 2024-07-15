using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.StarBreakerWeapon.DamageTypes;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment
{
    public class FrostBombardment : ModItem
    {
        /// <summary>
        /// 霜星的蓄力等级
        /// </summary>
        public int ChangeLevel;
        /// <summary>
        /// 处于激光炮模式
        /// </summary>
        public bool InBomMode;
        public static Asset<Texture2D> ChangeTex;
        public override void Load()
        {
            ChangeTex = ModContent.Request<Texture2D>(Texture + "_BomMode");
        }
        public override void Unload()
        {
            ChangeTex = null;
        }
        public override void SetDefaults()
        {
            Item.damage = 100;
            MoreDamageType moreDamageType = ModContent.GetInstance<MoreDamageType>();
            moreDamageType.AddDamageType(new()
            {
                damageClass = DamageClass.Magic,
                ActiveDamageClass = (item) =>
                {
                    return (item.ModItem as FrostBombardment).InBomMode;
                }
            });
            moreDamageType.AddDamageType(new()
            {
                damageClass = ModContent.GetInstance<EnergyDamage>(),
                ActiveDamageClass = (item) =>
                {
                    return (item.ModItem as FrostBombardment).ChangeLevel < 10;
                }
            });
            moreDamageType.AddDamageType(new()
            {
                damageClass = DamageClass.Ranged,
                ActiveDamageClass = (item) =>
                {
                    return (item.ModItem as FrostBombardment).ChangeLevel >= 10;
                }
            });
            Item.DamageType = moreDamageType;
            Item.crit = 0;
            Item.useTime = Item.useAnimation = 60;
            Item.Size = new(32, 54);
            Item.rare = ModContent.RarityType<FrostFistRarity>();
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTurn = false;
            Item.shoot = ModContent.ProjectileType<FrostBombardment_Proj>();
            Item.knockBack = 0.5f;
            Item.noMelee = true;
            Item.value = 15234960;
        }
        public override bool RangedPrefix() => false;
        public override bool AllowPrefix(int pre) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //ChangeLevel = 0;
            return false;
        }
        public override void HoldItem(Player player)
        {
            if(player.CheckMana(100) && player.manaRegenDelay <= 10 && ChangeLevel < 10)
            {
                player.manaRegenDelay = 60;
                if (player.CheckMana(100, true))
                {
                    SoundEngine.PlaySound(SoundID.Item25 with { Pitch = 0.2f,Volume = 0.2f }, player.position);
                    ChangeLevel++;
                }
            }
            if (ChangeLevel > 10) ChangeLevel = 10;
            else if (ChangeLevel < 0) ChangeLevel = 0;
            if (player.ownedProjectileCounts[Item.shoot] <= 0)
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.position, Vector2.Zero, Item.shoot, player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }
        }
    }
}
