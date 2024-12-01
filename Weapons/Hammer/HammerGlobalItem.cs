using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Weapons.Shortsword;

namespace WeaponSkill.Weapons.Hammer
{
    public class HammerGlobalItem : BasicWeaponItem<HammerGlobalItem>, IVanillaWeapon
    {
        public override void SetStaticDefaults()
        {
            WeaponID ??= new()
            {
                196,//木锤
                657,//homo锤
                2516,//棕榈木锤
                2746,//针叶木锤
                3505,//铜锤
                3499,//锡锤
                7,//铁锤
                654,//乌木锤
                922,//暗影木锤
                3493,//铅锤
                5283,//灰烬木锤
                3511,//银锤
                3487,//钨锤
                3517,//金锤
                104,//魔锤
                660,//珍珠木锤
                797,//血肉锤
                3481,//铂金锤
                2320,//岩鱼锤
                367,//神锤
                787,//蘑菇锤
                1234,//叶绿战锤
            };
        }
        public bool InAttackMode;
        public override bool InstancePerEntity => true;
        //public override void SetDefaults(Item entity)
        //{
        //    entity.autoReuse = false;
        //    entity.noUseGraphic = true;
        //    entity.noMelee = true;
        //    entity.useStyle = ItemUseStyleID.Rapier;
        //    entity.UseSound = null;
        //}
        public override void HoldItem(Item item, Player player)
        {
            if(WeaponSkill.RangeChange.JustPressed)
            {
                InAttackMode = !InAttackMode;
                if (InAttackMode)
                {
                    item.autoReuse = false;
                    item.noUseGraphic = true;
                    item.noMelee = true;
                    item.useStyle = ItemUseStyleID.Rapier;
                    item.UseSound = null;
                }
                else
                {
                    item.SetDefaults(item.type);
                }
            }
            if (InAttackMode && player.ownedProjectileCounts[ModContent.ProjectileType<HammerProj>()] <= 0)
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<HammerProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void UpdateInventory(Item item, Player player)
        {
            base.UpdateInventory(item, player);
        }
    }
}
