using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.DualBlades
{
    public class DualBladesGlobalItem : BasicWeaponItem<DualBladesGlobalItem>
    {
        /// <summary>
        /// 鬼人化
        /// </summary>
        public bool DemonMode;
        /// <summary>
        /// 鬼人强化条
        /// </summary>
        public float DemonGauge;
        /// <summary>
        /// 鬼人强化条
        /// </summary>
        public float DemonGaugeMax;
        /// <summary>
        /// 鬼人强化模式
        /// </summary>
        public bool ArchdemonMode;
        /// <summary>
        /// 鬼人值加成修正
        /// </summary>
        public float AddCorrection;
        public static bool ShowTheDualUI;
        public string ProjTex;
        public override void Load()
        {
            WeaponID = new();
        }
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.MeleeNoSpeed;
            entity.useTime = entity.useAnimation = 10;
            entity.noMelee = true;
            entity.noUseGraphic = true;
            DemonGaugeMax = 200;
        }
        public override void HoldItem(Item item, Player player)
        {
            ArchdemonMode = false;
            DemonMode = false;
            ShowTheDualUI = true;
            if(DemonGauge >= DemonGaugeMax)
            {
                ArchdemonMode = true;
            }
            if(DemonGauge <= 0)
            {
                DemonGauge = 0;
                ArchdemonMode = false;
            }
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheStamina = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DualBladesProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<DualBladesProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                (Main.projectile[proj].ModProjectile as DualBladesProj).DrawProjTex = ModContent.Request<Texture2D>(ProjTex);
            }
        }
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(DemonGauge > DemonGaugeMax)DemonGauge = DemonGaugeMax;
            if (DemonMode && DemonGauge < DemonGaugeMax)
            {
                DemonGauge += 20 * AddCorrection;
            }
            if(ArchdemonMode && !DemonMode)
            {
                DemonGauge -= 20;
            }
        }
    }
}
