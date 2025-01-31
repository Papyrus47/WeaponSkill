using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Items.GunLances;
using WeaponSkill.Items.Lances;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Weapons.GunLances
{
    public class GunLancesGlobalItem : BasicWeaponItem<GunLancesGlobalItem>
    {
        public Asset<Texture2D> ShieldTex;
        public Asset<Texture2D> ProjTex1;
        public Asset<Texture2D> ProjTex2;
        /// <summary>
        /// 最大弹药量
        /// </summary>
        public int MaxAmmo = 6;
        /// <summary>
        /// 铳枪的弹药数量
        /// </summary>
        public int Ammo;
        /// <summary>
        /// 拥有龙杭炮
        /// </summary>
        public bool HasLongHang = true;
        /// <summary>
        /// 龙击炮冷却时间
        /// </summary>
        public int DrogueHitTime;
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.MeleeNoSpeed;
            entity.useTime = entity.useAnimation = 60;
            entity.noMelee = true;
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            Ammo = MaxAmmo;
        }
        public override void HoldItem(Item item, Player player)
        {
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheStamina = true;
            if (DrogueHitTime > 0)
                DrogueHitTime--;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GunLancesProj>()] <= 0) // 生成手持弹幕
            {
                if (item.ModItem is BasicGunLancesItem modItem)
                {
                    ShieldTex = modItem.ShieldTex;
                    ProjTex1 = modItem.ProjTex1;
                    ProjTex2 = modItem.ProjTex2;
                    _ = ProjTex1;
                    _ = ShieldTex;
                    _ = ProjTex2;
                }
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<GunLancesProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                //DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                //TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type]);
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
    }
}
