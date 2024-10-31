using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Items.ChargeBlade;
using WeaponSkill.Items.Lances;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Weapons.Lances
{
    public class LancesGlobalItem : BasicWeaponItem<LancesGlobalItem>
    {
        public Asset<Texture2D> ShieldTex;
        public Asset<Texture2D> ProjTex;
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.MeleeNoSpeed;
            entity.useTime = entity.useAnimation = 60;
            entity.noMelee = true;
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheStamina = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LancesProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<LancesProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                if (item.ModItem is BasicLancesItem modProj)
                {
                    ShieldTex = modProj.ShieldTex;
                    ProjTex = modProj.ProjTex;
                    _ = ProjTex;
                    _ = ShieldTex;
                }
                //DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                //TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type]);
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
    }
}
