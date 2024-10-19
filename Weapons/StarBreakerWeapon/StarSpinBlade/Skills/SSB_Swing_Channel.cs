using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade.Skills
{
    public class SSB_Swing_Channel : SSB_Swing
    {
        public SSB_Swing_Channel(StarSpinBladeProj modProjectile, Func<bool> changeCondition, Func<float, float> timeChange, Func<bool> canChannel) : base(modProjectile, changeCondition, timeChange)
        {
            CanChannel = canChannel;
        }
        public Action<SSB_Swing_Channel> OnChannel;
        /// <summary>
        /// 可持续蓄力条件
        /// </summary>
        public Func<bool> CanChannel;
        public override void AI()
        {
            if ((int)Projectile.ai[0] == 0) // 代替原本的挥舞
            {
                Projectile.spriteDirection = Player.direction * IsTrueSlash.ToDirectionInt() * SwingDirectionChange.ToDirectionInt();
                SwingHelper.SetRotVel(0);
                PreAtk = true;
                if(!CanChannel.Invoke())
                    Projectile.ai[1]++;
                float time = Projectile.ai[1];
                SwingHelper.Change_Lerp(StartVel, 0.5f, VelScale, 0.5f * 2, VisualRotation, 0.5f);
                SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                SwingHelper.SwingAI(StarSpinBladeProj.SwingLenght, Player.direction, 0);
                OnChannel?.Invoke(this);
                if (time > 1)
                {
                    Projectile.ai[1] = 0;
                    Projectile.ai[0]++;
                    GetStarSpinBladeItem().SpinValue += SpinValue * IsTrueSlash.ToDirectionInt();
                }
            }
            else
                base.AI();
        }
    }
}
