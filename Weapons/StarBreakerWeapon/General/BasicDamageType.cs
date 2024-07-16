using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.General
{
    public class BasicDamageType<T> where T : BasicDamageType<T>,new()
    {
        private BasicDamageType<T> init;

        public BasicDamageType()
        {
        }

        /// <summary>
        /// 物理伤害倍率
        /// </summary>
        public static float PhysicsDamageScale = 1f;
        /// <summary>
        /// 属性伤害倍率
        /// </summary>
        public static float ElementDamageScale = 1f;

        public BasicDamageType<T> Init
        {
            get
            {
                init ??= new T();
                return init;
            }
        }
        public static void Load() { }
    }
}
