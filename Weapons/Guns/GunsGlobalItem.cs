using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Guns.GunsType;

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
        public override void SetStaticDefaults()
        {
            int[] types =
            {
                ItemID.Handgun
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
                case ItemID.Handgun:
                    GunType = new Handgun();
                    break;
                default:
                    break;
            }
        }
        public override bool CanShoot(Item item, Player player)
        {
            if (ResetBullet)
                return false;
            GunType.HasBullet--;
            return base.CanShoot(item, player);
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
        public override bool? CanChooseAmmo(Item weapon, Item ammo, Player player) // 武器上调用
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
        public override bool CanConsumeAmmo(Item weapon, Item ammo, Player player) // 武器上调用
        {
            return base.CanConsumeAmmo(weapon, ammo, player);
        }
        public override void PickAmmo(Item weapon, Item ammo, Player player, ref int type, ref float speed, ref StatModifier damage, ref float knockback)
        {
            if (ResetBullet)
                return;
            GunType.HasBullet--;
        }
    }
}
