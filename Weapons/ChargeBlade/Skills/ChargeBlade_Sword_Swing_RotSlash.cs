using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    /// <summary>
    /// 各种回旋斩
    /// </summary>
    public class ChargeBlade_Sword_Swing_RotSlash : ChargeBlade_Sword_Swing
    {
        public ChargeBlade_Sword_Swing_RotSlash(ChargeBladeProj chargeBlade, Func<bool> activationConditionFunc) : base(chargeBlade, activationConditionFunc)
        {
            SwingRot = MathHelper.TwoPi * 0.75f;
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 1)
            {
                Projectile.extraUpdates = 3;
                Projectile.ai[1] -= 0.5f;
                player.velocity.X = player.direction * 6;
            }
            else if (Projectile.ai[0] == 2)
            {
                player.velocity.X *= 0.9f;
                if ((int)Projectile.ai[1] == 0)
                {
                    ChargeBladeProj.shield.Fixed = false;
                }
                ChargeBladeProj.shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * player.width * 0.9f, 0), player.direction);
                ChargeBladeProj.shield.GP = true;
                ChargeBladeProj.shield.InDef = true;
            }
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            ChargeBladeProj.shield.GP = false;
            ChargeBladeProj.shield.InDef = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            ChargeBladeProj.chargeBladeGlobal.StatCharge += 1.3f; // 回旋斩增加
        }
    }
}
