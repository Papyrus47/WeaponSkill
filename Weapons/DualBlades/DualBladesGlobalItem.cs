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
        public float AddCorrection = 1;
        public static bool ShowTheDualUI;
        public string ProjTex;
        public byte StaminaTime; 
        public Texture2D DrawColorTex;
        public override void Load()
        {
            WeaponID ??= new();
        }
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.MeleeNoSpeed;
            entity.useTime = entity.useAnimation = 10;
            entity.noMelee = true;
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            DemonGaugeMax = 1200;
        }
        public override void HoldItem(Item item, Player player)
        {
            //DemonMode = true;
            ShowTheDualUI = true;
            //DemonGauge = DemonGaugeMax;
            if (DemonGauge >= DemonGaugeMax)
            {
                ArchdemonMode = true;
            }
            if(DemonGauge <= 0)
            {
                DemonGauge = 0;
                ArchdemonMode = false;
            }
            if (DemonMode)
            {
                if (player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 0 && StaminaTime-- <= 0)
                {
                    StaminaTime = 3;
                    player.GetModPlayer<WeaponSkillPlayer>().StatStamina--;
                    player.GetModPlayer<WeaponSkillPlayer>().StatStaminaAddTime = 0;
                    Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.FireworkFountain_Red);
                    dust.velocity = Main.rand.NextVector2Unit() * 2f;
                    dust.noGravity = true;
                }
            }
            else if (ArchdemonMode)
            {
                DemonGauge--;
            }
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheStamina = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DualBladesProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<DualBladesProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                Asset<Texture2D> drawTex = ModContent.Request<Texture2D>(ProjTex);
                if (drawTex.IsLoaded && drawTex.Value != null)
                {
                    (Main.projectile[proj].ModProjectile as DualBladesProj).DrawProjTex = drawTex;
                    DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, drawTex.Height());
                    TheUtility.GetWeaponDrawColor(DrawColorTex, drawTex);
                }
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
        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(DemonGauge > DemonGaugeMax)DemonGauge = DemonGaugeMax;
            if (DemonMode && DemonGauge < DemonGaugeMax)
            {
                DemonGauge += 5 * AddCorrection;
                AddCorrection = 1;
            }
        }
    }
}
