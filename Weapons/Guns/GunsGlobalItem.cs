using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Guns.GunsType;
using Terraria.ID;

namespace WeaponSkill.Weapons.Guns
{
    public class GunsGlobalItem : BasicWeaponItem<GunsGlobalItem>
    {
        public override bool InstancePerEntity => true;
        /// <summary>
        /// 枪的种类
        /// </summary>
        public BasicGunsType GunType;
        /// <summary>
        /// 装填子弹
        /// </summary>
        public bool ResetBullet;
        /// <summary>
        /// 允许展示子弹UI
        /// </summary>
        public static bool ShowUI;
        /// <summary>
        /// 消耗子弹用
        /// </summary>
        public bool CosumeAmmo;

        public override void SetStaticDefaults()
        {
            int[] types =
            {
                ItemID.Handgun,
                95,
                14,
                219,
                1255,
                930,
                587,
                3007,
                800
            };
            WeaponID ??= new();
            for (int i = 0; i < types.Length; i++)
            {
                WeaponID.Add(types[i]);
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            ShowUI = true;
            GunType.OnHold(player,item);
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheRangeChangeUI = true;
            #region 如果进入装填弹药时间
            if (ResetBullet)
            {
                if (player.itemAnimation == 0 || player.itemTime == 0)
                    player.itemAnimation = player.itemTime = GunType.ResetTime;
                else if (player.itemAnimation == 2 || player.itemTime == 2)
                {
                    ResetBullet = false;
                    GunType.HasBullet = GunType.MaxBullet;
                    Item item1 = new Item(item.type);
                    item1.SetDefaults(item.type);
                    item.UseSound = item1.UseSound;
                }
                else if (player.itemAnimation == GunType.ResetTime / 2 || player.itemTime == GunType.ResetTime / 2)
                    SoundEngine.PlaySound(GunType.ResetSound, player.position);
            }
            else
            {
                if(GunType.HasBullet <= 0)
                {
                    item.UseSound = null;
                }
            }
            #endregion
        }
        public override void UpdateInventory(Item item, Player player)
        {
            GunType.UpdateInventory(player, item);
        }
        public override void SetDefaults(Item entity)
        {
            switch (entity.type)
            {
                #region 手枪一类
                case ItemID.Handgun:
                case 95:
                case 587:
                case 930:
                case 800:
                    GunType = new Handgun();
                    break;
                case 14:
                    GunType = new Handgun(6);
                    break;
                case 219:
                case 1255:
                    GunType = new Handgun(17)
                    {
                        ResetTime = 15
                    };
                    break;
                case 3007:
                    GunType = new Handgun(10)
                    {
                        ResetTime = 30
                    };
                    break;
                #endregion
                default:
                    throw new Exception("忘记填入枪的类型了!请修复!");
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CosumeAmmo = false;
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
        public override bool CanShoot(Item item, Player player)
        {
            if (ResetBullet)
                return false;
            GunType.HasBullet--;
            return base.CanShoot(item, player);
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            GunType.ModifyWeaponDamage(item, player, ref damage);
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (ResetBullet)
            {
                item.UseSound = null;
                return false;
            }
            return base.CanUseItem(item, player);
        }
        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            CosumeAmmo = true;
            WeaponSkillPlayer weaponSkillPlayer = player.GetModPlayer<WeaponSkillPlayer>();
            if (weaponSkillPlayer.AmmoItems.Count == 0)
                return;
            Item shootItem = weaponSkillPlayer.AmmoItems[weaponSkillPlayer.UseAmmoIndex];
            if (ItemLoader.ConsumeItem(shootItem, player) && !player.IsAmmoFreeThisShot(player.HeldItem, item, item.shoot))
            {
                CombinedHooks.OnConsumeAmmo(player, player.HeldItem, shootItem);
                if (shootItem.consumable && shootItem.stack-- <= 0)
                {
                    shootItem.active = false;
                    shootItem.TurnToAir();
                }
            }
            type = shootItem.shoot;
            #region 特判区域
            if ((item.type == ItemID.VenusMagnum || item.type == ItemID.SniperRifle) && (type == ProjectileID.Bullet || type == ProjectileID.SilverBullet || type == 14))
            {
                type = ProjectileID.BulletHighVelocity;
            }
            #endregion
        }
        public override bool? CanChooseAmmo(Item weapon, Item ammo, Player player) // 武器上调用,选择子弹
        {
            if(GunType.HasBullet < 0) // 进入装填子弹
            {
                ResetBullet = true;
                player.itemRotation = 0;
            }
            if (ResetBullet)
                return false;
            return base.CanChooseAmmo(weapon, ammo, player);
        }
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player) // 武器上调用,子弹消耗
        {
            return CosumeAmmo;
        }
        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
            if (ResetBullet)
                return;
            GunType.HasBullet--;
        }
    }
}
