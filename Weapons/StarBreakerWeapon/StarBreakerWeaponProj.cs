using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon
{
    public abstract class StarBreakerWeaponProj : ModProjectile
    {
        public Player Player;
        public int GetPlayerDoubleTapDir(int Dir)
        {
            if (Dir == 1)
                return 2; // 朝向为正-右边
            else
                return 3;
        }
        public bool GetPlayerDoubleTap(int Dir) => Player.GetModPlayer<WeaponSkillPlayer>().DashDir == Dir;
        public bool LeftControl => Player.controlUseItem;
        public bool RightControl => Player.controlUseTile;
        public bool LeftChick;
        public bool RightChick;
        public int LeftTime;
        public bool LeftChannel;
        public bool RightChannel;
        public int RightTime;
        public bool CanChangeToStopActionSkill;
        /// <summary>
        /// 预操作-左键
        /// </summary>
        public bool PreLeftChick;
        /// <summary>
        /// 预操作-右键
        /// </summary>
        public bool PreRightChick;
        /// <summary>
        /// 预操作-重置
        /// </summary>
        public bool ResetPreAtk;
        public override void AI()
        {
            LeftChannel = LeftChick = RightChannel = RightChick = false;
            if (LeftControl)
            {
                LeftTime++;
                if (LeftTime >= 30)
                {
                    LeftChannel = true;
                }
            }
            else
            {
                if (LeftTime > 0 && LeftTime < 30)
                    LeftChick = true;
                LeftTime = 0;
            }

            if (RightControl)
            {
                RightTime++;
                if (RightTime >= 30)
                {
                    RightChannel = true;
                }
            }
            else
            {
                if (RightTime > 0 && RightTime < 30)
                    RightChick = true;
                RightTime = 0;
            }
            #region 预操作系统
            if (PreLeftChick)
            {
                LeftChick = true;
            }
            if (PreRightChick)
            {
                RightChick = true;
            }

            if (ResetPreAtk)
            {
                PreRightChick = PreLeftChick = false;
                ResetPreAtk = false;
            }
            #endregion
        }
        /// <summary>
        /// 调用这个处理预操作
        /// </summary>
        /// <returns></returns>
        public virtual void PreAtk()
        {
            if(LeftChick)
                PreLeftChick = true;
            if(RightChick)
                PreRightChick = true;
        }
    }
}
