using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Shortsword;

namespace WeaponSkill.Weapons.SlashAxe
{
    public class SlashAxeGlobalItem : BasicWeaponItem<SlashAxeGlobalItem>
    {
        /// <summary>
        /// 斩斧的觉醒条
        /// </summary>
        public int Power;
        /// <summary>
        /// 斩斧的觉醒条上限
        /// </summary>
        public int PowerMax = 300;
        public int PowerTimeLeft;
        /// <summary>
        /// 斩击条
        /// </summary>
        public int Slash;
        /// <summary>
        /// 斩击条上限
        /// </summary>
        public int SlashMax = 1000;
        /// <summary>
        /// 斧强化
        /// </summary>
        public int AxeStrength;
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.MeleeNoSpeed;
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.useTurn = false;
            entity.useAnimation = entity.useTime = 10;
            entity.noMelee = true;
        }
        public override void HoldItem(Item item, Player player)
        {
            if (AxeStrength > 0)
            {
                player.GetDamage(item.DamageType) += 0.3f;
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SlashAxeProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<SlashAxeProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                //DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                //TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type], 0.9f, 2.5f);
            }
        }
        public override void UpdateInventory(Item item, Player player)
        {
            Power = Math.Clamp(Power, 0, PowerMax);
            if(Power == PowerMax && PowerTimeLeft == -1)
            {
                PowerTimeLeft = 1800;
            }
            if(PowerTimeLeft > 0)
                PowerTimeLeft--;
            else if (PowerTimeLeft == 0)
            {
                PowerTimeLeft = -1;
                Power = 0;
            }
            Slash = Math.Clamp(Slash, 0, SlashMax);
            AxeStrength = Math.Max(0, --AxeStrength);
        }
    }
}
