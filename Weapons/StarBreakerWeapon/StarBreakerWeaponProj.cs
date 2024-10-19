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
        public override void AI()
        {
            LeftChannel = LeftChick = RightChannel = RightChick = false;
            if (LeftControl)
            {
                LeftTime++;
                if(LeftTime >= 15)
                {
                    LeftChannel = true;
                }
            }
            else
            {
                if (LeftTime > 0 && LeftTime < 15)
                    LeftChick = true;
                LeftTime = 0;
            }

            if (RightControl)
            {
                RightTime++;
                if (RightTime >= 15)
                {
                    RightChannel = true;
                }
            }
            else
            {
                if (RightTime > 0 && RightTime < 15)
                    RightChick = true;
                RightTime = 0;
            }
        }
    }
}
