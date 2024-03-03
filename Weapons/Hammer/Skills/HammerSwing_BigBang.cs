using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Hammer.Skills
{
    public class HammerSwing_BigBang : HammerSwing
    {
        public HammerSwing_BigBang(HammerProj hammerProj) : base(hammerProj, null)
        {
            Reset();
        }
        /// <summary>
        /// 挥舞次数
        /// </summary>
        public int SwingNum;
        public bool OnHit;
        public void Reset()
        {
            StartVel = (-Vector2.UnitY).RotatedBy(-0.3);
            VelScale = Vector2.One;
            SwingRot = MathHelper.PiOver2 + 0.3f;
            VisualRotation = 0f;
            TimeChangeMax = 30;
            TimeChangeFunc = hammerProj.HammerSwingTimeChange;
            AddDamage = 0;
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 2 && SwingNum < 4 && ActivationCondition() && OnHit) // 重新敲打
            {
                StartVel = StartVel.RotatedBy(-0.05);
                OnHit = false;
                SwingNum++;
                SwingRot += 0.05f;
                AddDamage += 0.8f;
                Projectile.ai[0] = Projectile.ai[2] = 0;
                Projectile.ai[1] = -20;
                TheUtility.ResetProjHit(Projectile);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            OnHit = true;
        }
        public override bool ActivationCondition() => player.controlUseTile;
        public override bool SwitchCondition() => base.SwitchCondition() && SwingNum >= 4;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            SwingNum = 1;
            Reset();
        }
    }
}
