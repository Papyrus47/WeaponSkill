using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Guns.GunsType
{
    /// <summary>
    /// 枪的基类
    /// </summary>
    public abstract class BasicGunsType
    {
        public BasicGunsType(int maxBullet = 0)
        {
            MaxBullet = maxBullet;
        }
        /// <summary>
        /// 最大子弹量
        /// </summary>
        public int MaxBullet;
        /// <summary>
        /// 拥有自动量
        /// </summary>
        public int HasBullet;
        /// <summary>
        /// 装子弹的声音
        /// </summary>
        public virtual SoundStyle ResetSound => SoundID.Item149;
        /// <summary>
        /// 装弹速度
        /// </summary>
        public int ResetTime = 30;
        public virtual void OnHold(Player player,Item item) { }
        public virtual void UpdateInventory(Player player, Item item) { }
        public virtual void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage) { }
        public virtual void OnShoot(Player player, Item item) { }
        public virtual void OnResetBullet(Player player, Item item) 
        {
            if (player.itemAnimation == 0 || player.itemTime == 0)
                player.itemAnimation = player.itemTime = ResetTime;
            else if (player.itemAnimation == 2 || player.itemTime == 2)
            {
                item.GetGlobalItem<GunsGlobalItem>().ResetBullet = false;
                HasBullet = MaxBullet;
                Item item1 = new Item(item.type);
                item1.SetDefaults(item.type);
                item.UseSound = item1.UseSound;
            }
            else if (player.itemAnimation == ResetTime / 2 || player.itemTime == ResetTime / 2)
                SoundEngine.PlaySound(ResetSound, player.position);
        }
    }
}
