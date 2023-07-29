using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Shortsword;

namespace WeaponSkill.Weapons.Spears
{
    public class SpearsGlobalItem : BasicWeaponItem<SpearsGlobalItem>
    {
        public Texture2D DrawColorTex;
        public override bool InstancePerEntity => true;
        public bool CanShootProj;
        public override void SetStaticDefaults()
        {
            WeaponID = new()
            {
                280,277,4061,802,2332,274,537,1186,390,1193,406,1200,550,3836,1228,5011,756,2331,1947
            };
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
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SpearsProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<SpearsProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type], 0.9f, 2.2f);
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
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (CanShootProj)
            {
                CanShootProj = false;
                return true;
            }
            return false;
        }
    }
}
