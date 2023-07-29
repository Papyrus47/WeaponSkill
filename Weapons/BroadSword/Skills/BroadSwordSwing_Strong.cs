using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.BroadSword.Skills
{
    public class BroadSwordSwing_Strong : BroadSwordSwing
    {
        public BroadSwordSwing_Strong(BroadSwordProj broadSwordProj) : base(broadSwordProj)
        {
        }
        public Func<bool> AIAction;
        public override void AI()
        {
            if(AIAction?.Invoke() != false) base.AI();
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.localAI[0] = Projectile.localAI[1] = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.localAI[0] = Projectile.localAI[1] = 0;
        }
    }
}
