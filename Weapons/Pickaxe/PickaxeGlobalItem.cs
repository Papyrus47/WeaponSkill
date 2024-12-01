using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Hammer;

namespace WeaponSkill.Weapons.Pickaxe
{
    public class PickaxeGlobalItem : BasicWeaponItem<PickaxeGlobalItem>, IVanillaWeapon
    {
        /// <summary>
        ///  特殊的镐子,斧镐之类
        /// </summary>
        public static List<int> SP_Pickaxe;
        public bool CanShootProj;
        /// <summary>
        /// 攻击状态
        /// </summary>
        public bool InAttackMode;
        /// <summary>
        /// 特殊的状态
        /// </summary>
        public int SP_PickaxeMode;
        public override bool InstancePerEntity => true;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            WeaponID ??= new();
            int[] items = 
            { 
                ItemID.IronPickaxe,
                882,
                3509,
                3503,3521,3491,3515,3497,1917,4059,1320,3485,103,798,122,776,777,1188,1195,1202,778,1506,1230,2786,2776,2781,3466,2341,990
            };
            SP_Pickaxe = new List<int>()
            {
                990
            };
            for (int i = 0; i < items.Length; i++)
            {
                WeaponID.Add(items[i]);
            }
        }
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (WeaponSkill.SpKeyBind.JustPressed && SP_Pickaxe.Contains(item.type))
            {
                SP_PickaxeMode = 1 - SP_PickaxeMode;
                if (SP_PickaxeMode == 0)
                {
                    item.SetDefaults(item.type);
                }
            }
            if (WeaponSkill.RangeChange.JustPressed)
            {
                if(SP_PickaxeMode == 0)
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
            if (InAttackMode && player.ownedProjectileCounts[ModContent.ProjectileType<PickaxeProj>()] <= 0)
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<PickaxeProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => CanShootProj;
    }
}
