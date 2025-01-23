using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    public class DualBladesSwing_InArchdemon : DualBladesSwing
    {
        public float DemonMode_AddCorrection;
        /// <summary>
        /// 鬼人条缩减
        /// </summary>
        public float DelDemonGauge;
        public bool IsDemonDance;

        public DualBladesSwing_InArchdemon(DualBladesProj dualBladesProj, SwingSet swingSet, DoubleSwingSpeed swingSetSpeed, Func<bool> changeCondition) : base(dualBladesProj, swingSet, swingSetSpeed, changeCondition)
        {
        }

        public override void AI()
        {
            Projectile.rotation = MathHelper.PiOver4 * 0.45f;
            base.AI();
            bladesGlobalItem.AddCorrection += DemonMode_AddCorrection;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            bladesGlobalItem.DemonGauge -= DelDemonGauge;
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (IsDemonDance)
            {
                return Projectile.ai[0] <= 0.1f && ActivationCondition();
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override bool ActivationCondition()
        {
            return base.ActivationCondition() && bladesProj.InArchdemonMode && !bladesProj.InDemonMode;
        }
    }
}
