using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Items.SwordShield;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Weapons.SwordShield
{
    public class SwordShieldGlobalItem : BasicWeaponItem<SwordShieldGlobalItem>
    {
        /// <summary>
        /// 盾的贴图
        /// </summary>
        public Asset<Texture2D> ShieldTex;
        public override void SetStaticDefaults()
        {
            WeaponID ??= new();
        }
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.MeleeNoSpeed;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.useTurn = false;
            entity.useAnimation = entity.useTime = 10;
            entity.noUseGraphic = true;
            entity.noMelee = true;
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SwordShieldProj>()] <= 0)
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<SwordShieldProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage; 
                if (item.ModItem is BasicSwordShield swordShield)
                {
                    ShieldTex = swordShield.ShieldTex;
                }
            }
        }
    }
}
