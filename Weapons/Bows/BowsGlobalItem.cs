using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.BroadSword;

namespace WeaponSkill.Weapons.Bows
{
    public class BowsGlobalItem : BasicWeaponItem<BowsGlobalItem>
    {
        public override void SetStaticDefaults()
        {
            WeaponID = new() { 3859, 5282, 4381, 4381, 3504, 3029, 44, 655, 4953, 3516, 3516, 3516, 99, 3492, 682, 120, 2515, 661, 661, 3540, 3854, 3854, 2223, 658, 923, 3052, 3510, 796, 2888, 3498, 2624, 3486, 39, 3480 };
        }
        public override void SetDefaults(Item entity)
        {
            entity.autoReuse = false;
            entity.noUseGraphic = true;
            entity.noMelee = true;
            entity.useStyle = ItemUseStyleID.Shoot;
            entity.UseSound = null;
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheRangeChangeUI = true;
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheStamina = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BowsProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<BowsProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
    }
}
