using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.BroadSword;

namespace WeaponSkill.Weapons.Shortsword
{
    public class ShortswordGlobalItem : BasicWeaponItem<ShortswordGlobalItem>, IVanillaWeapon
    {
        public Texture2D DrawColorTex;
        public override void SetStaticDefaults()
        {
            WeaponID = new()
            {
                3507,3351,3483,3519,3489,3513,3495,6,3501,4463,3106
            };
        }
        public override void SetDefaults(Item entity)
        {
            entity.autoReuse = false;
            entity.noUseGraphic = true;
            entity.noMelee = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.UseSound = null;
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ShortswordProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<ShortswordProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type],0.9f,2.5f);
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            if (player.HeldItem != item && DrawColorTex != null)
            {
                DrawColorTex.Dispose();
                DrawColorTex = null;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
    }
}
