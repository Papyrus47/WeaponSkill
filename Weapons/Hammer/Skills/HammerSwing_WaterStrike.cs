using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Hammer.Skills
{
    /// <summary>
    /// 水面击
    /// </summary>
    public class HammerSwing_WaterStrike : HammerSwing
    {
        public HammerSwing_WaterStrike(HammerProj hammerProj) : base(hammerProj,null)
        {
            StartVel = (-Vector2.UnitY).RotatedBy(-0.3);
            VelScale = Vector2.One;
            SwingRot = MathHelper.PiOver2;
            VisualRotation = 0f;
            TimeChangeMax = 30;
            TimeChangeFunc = hammerProj.HammerSwingTimeChange;
        }
        public override void AI()
        {
            base.AI();
            if (Projectile.ai[1] > TimeChangeMax * 0.5f) // 大于计时器变化0.5的时候
            {
                player.GetModPlayer<WeaponSkillPlayer>().WaterStrike = true;
            }
        }
        public override bool ActivationCondition() => player.controlUseTile;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            player.GetModPlayer<WeaponSkillPlayer>().WaterStrike = false;
            player.GetModPlayer<WeaponSkillPlayer>().WaterStrike_OnHit = false;
        }
    }
}
