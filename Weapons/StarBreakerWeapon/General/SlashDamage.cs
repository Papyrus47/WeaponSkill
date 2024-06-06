using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.General
{
    public static class SlashDamage
    {
        public static int SlashCount; // 斩击伤害连击次数
        public static int SlashCountRemoveTime; // 斩击伤害连击段数移除时间
        public static bool IsSlashDamage;

        public static float GetSlashDamageMultiple()
        {
            if (!IsSlashDamage) 
                return 1;
            IsSlashDamage = false;
            if (SlashCount < 20) 
                return 1;
            return MathF.Log10(SlashCount - 20);
        }
        /// <summary>
        /// 当斩击伤害命中时候调用
        /// </summary>
        public static void SlashDamageHit()
        {
            SlashCount++;
            SlashCountRemoveTime = 0;
            IsSlashDamage = true;
        }
        /// <summary>
        /// 更新斩击伤害命中
        /// </summary>
        public static void UpdateSlashDamageCount()
        {
            if(SlashCount > 0 && SlashCountRemoveTime++ > 180)
            {
                SlashCount = 0;
                SlashCountRemoveTime = 0;
            }
        }
    }
}
