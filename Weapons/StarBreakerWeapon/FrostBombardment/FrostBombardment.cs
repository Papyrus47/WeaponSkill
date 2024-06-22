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
        public int ChennelLevel;
        public override void SetDefaults()
        {
            Item.damage = 100;
            MoreDamageType moreDamageType = ModContent.GetInstance<MoreDamageType>();
            moreDamageType.AddDamageType(new()
            {
                damageClass = ModContent.GetInstance<EnergyDamage>(),
                ActiveDamageClass = (item) =>
                {
                    return (item.ModItem as FrostBombardment).ChennelLevel < 10;
                }
            });
            moreDamageType.AddDamageType(new()
            {
                damageClass = DamageClass.Ranged,
                ActiveDamageClass = (item) =>
                {
                    return (item.ModItem as FrostBombardment).ChennelLevel >= 10;
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
            Item.shoot = ModContent.ProjectileType<FrostFistProj>();
            Item.knockBack = 0.5f;
            //Item.noMelee = true;
            Item.value = 15234960;
        }
        public override bool RangedPrefix() => false;
        public override bool AllowPrefix(int pre) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            ChennelLevel = 10 - ChennelLevel;
            return false;
        }
    }
}
