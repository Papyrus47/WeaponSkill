using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.General
{
    public class HitDamage : BasicDamageType<HitDamage>
    {
        public static bool IsHitDamage;
        public static bool IsHitDamage_Element;
        /// <summary>
        /// 当打击伤害命中时候调用
        /// </summary>
        public static void HitDamageHit(bool isElement = false)
        {
            IsHitDamage = true;
            IsHitDamage_Element = isElement;
        }
        public static float GetHitDamageMultiple_Physics()
        {
            if (!IsHitDamage)
                return 1;
            IsHitDamage = false;
            return PhysicsDamageScale;
        }
        public static float GetHitDamageMultiple_Element()
        {
            if (!IsHitDamage_Element)
                return 1;
            IsHitDamage_Element = false;
            return ElementDamageScale;
        }
        public static void AddDamage(Projectile Projectile,NPC target,NPC.HitInfo hit)
        {
            #region 打击伤害追加
            int hitDamageType_Damage = (int)(hit.SourceDamage * 0.4f);
            Main.player[Projectile.owner].statMana = Math.Min(Main.player[Projectile.owner].statMana + 8, Main.player[Projectile.owner].statManaMax2);
            Main.player[Projectile.owner].ManaEffect(8);
            if (target.CanBeChasedBy())
            {
                target.life -= hitDamageType_Damage;
                target.checkDead();
            }
            Main.player[Projectile.owner].addDPS(hitDamageType_Damage);
            CombatText.NewText(target.Hitbox, hit.Crit ? CombatText.DamagedFriendly * 0.5f : CombatText.DamagedFriendlyCrit * 0.5f, hitDamageType_Damage);
            #endregion
        }
        public static new void Load()
        {
            PhysicsDamageScale = 1.2f;
            ElementDamageScale = 0.8f;
        }
    }
}
