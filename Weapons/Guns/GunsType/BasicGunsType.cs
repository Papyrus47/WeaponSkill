using System;
using System.Collections.Generic;
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
    }
}
