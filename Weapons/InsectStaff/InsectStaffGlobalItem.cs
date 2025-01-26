using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Items.InsectStaff.Insects;
using WeaponSkill.Weapons.SlashAxe;

namespace WeaponSkill.Weapons.InsectStaff
{
    public class InsectStaffGlobalItem : BasicWeaponItem<InsectStaffGlobalItem>
    {
        /// <summary>
        /// 这是装备的虫子
        /// </summary>
        public Item Insect;
        /// <summary>
        /// 强化时间
        /// </summary>
        public int StrongTime;
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.Summon;
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.useTurn = false;
            entity.useAnimation = entity.useTime = 10;
            entity.noMelee = true;
            Insect = new(ModContent.ItemType<TestInsect>());
        }
        public override void HoldItem(Item item, Player player)
        {
            if(StrongTime > 0)
            {
                StrongTime--;
                player.GetDamage(item.DamageType) += 0.5f;
                player.statDefense *= 2;
            }
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheStamina = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<InsectStaffProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<InsectStaffProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                if (player.ownedProjectileCounts[Insect.shoot] <= 0) // 召唤虫子
                {
                    InsectStaffProj proj1 = Main.projectile[proj].ModProjectile as InsectStaffProj;
                    proj = Projectile.NewProjectile(new InsectProj.InsectProjSource(proj1),
                        player.position, Vector2.Zero, Insect.shoot, player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                    Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
                    proj1.insectProj = Main.projectile[proj].ModProjectile as InsectProj;
                }
                //DrawColorTex ??= new Texture2D(Main.graphics.GraphicsDevice, 1, TextureAssets.Item[item.type].Height());
                //TheUtility.GetWeaponDrawColor(DrawColorTex, TextureAssets.Item[item.type], 0.9f, 2.5f);
            }
        }
    }
}
