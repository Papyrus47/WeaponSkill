using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.General
{
    public class SpurtsDamage : BasicDamageType<SpurtsDamage>
    {
        public static bool IsSpurtsDamage;
        public static bool IsSpurtsDamage_Element;
        /// <summary>
        /// 刺击命中判定
        /// </summary>
        public static bool SpurtsHit;
        public static new void Load()
        {
            PhysicsDamageScale = 1f;
            ElementDamageScale = 1.5f;
        }
        /// <summary>
        /// 当刺击伤害命中时候调用
        /// </summary>
        public static void SpurtsDamageHit(bool isElement = false)
        {
            IsSpurtsDamage = true;
            IsSpurtsDamage_Element = isElement;
            SpurtsHit = true;
        }
        public static float GetHitDamageMultiple_Physics()
        {
            if (!IsSpurtsDamage)
                return 1;
            IsSpurtsDamage = false;
            return PhysicsDamageScale;
        }
        /// <summary>
        /// 突刺伤害命中前
        /// </summary>
        public static void ModifySpurtsHit(ref NPC.HitModifiers self)
        {
            if(SpurtsHit)
            {
                SpurtsHit = false;
                self.ScalingArmorPenetration += 0.4f;
            }
        }
        public static float GetHitDamageMultiple_Element()
        {
            if (!IsSpurtsDamage_Element)
                return 1;
            IsSpurtsDamage_Element = false;
            return ElementDamageScale;
        }
    }
}
