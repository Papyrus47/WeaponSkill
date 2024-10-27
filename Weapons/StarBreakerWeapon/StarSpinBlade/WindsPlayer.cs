using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade
{
    /// <summary>
    /// 风缠状态玩家(太多了)
    /// </summary>
    public class WindsPlayer : ModPlayer
    {
        /// <summary>
        /// 风缠状态启动
        /// </summary>
        public bool UseWindsState;
        public int Time;
        public override void ResetEffects()
        {
            base.ResetEffects();
            if (Time++ > 5)
            {
                Time = 0;
                UseWindsState = false;
            }
        }
    }
}
