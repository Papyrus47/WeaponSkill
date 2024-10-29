using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade.Skills
{
    /// <summary>
    /// 暴风转刃
    /// </summary>
    public class SSB_SC_WindSpinBlade : SSB_Swing
    {
        public SSB_SC_WindSpinBlade(StarSpinBladeProj modProjectile, Func<bool> changeCondition, Func<float, float> timeChange) : base(modProjectile, changeCondition, timeChange)
        {
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 1) // 代替原本的挥舞
            {
                PreAtk = false;
                SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                Projectile.extraUpdates = 4;
                Projectile.ai[1]++;
                float swingTime = Projectile.ai[1] / (SwingTime * 3);
                if (swingTime > 1)
                {
                    TheUtility.ResetProjHit(Projectile);
                    if (Player.CheckMana(5, true))
                    {
                        (Player.HeldItem.ModItem as StarSpinBlade).SpinValue += 200;
                    }
                    if (!StarSpinBladeProj.RightChannel)
                        Projectile.ai[0]++;
                    Projectile.ai[1] = 0;
                    var pj = StarSpinBladeProj.NewWindProj(Player.Center, Vector2.UnitX * 2 * Player.direction, 10,0, Player.direction, 0, (proj) =>
                    {
                        proj.velocity = proj.velocity.RotatedBy(0.003 * -proj.ai[1]);
                    });
                    (pj.ModProjectile as WindsProj).onHit = (NPC target, NPC.HitInfo _, int _) =>
                    {
                        if(!target.immortal)
                            target.velocity -= pj.velocity;
                    };
                    pj.timeLeft *= 4;
                    return;
                }

                if (swingTime > 0.5f) // 预输入处理
                {
                    StarSpinBladeProj.PreAtk();
                }
                OnUse?.Invoke(this);
                swingTime = TimeChange.Invoke(swingTime);

                SwingHelper.SwingAI(StarSpinBladeProj.SwingLenght, Player.direction, swingTime * SwingRot * SwingDirectionChange.ToDirectionInt());
            }
            else
                base.AI();
        }
    }
}
