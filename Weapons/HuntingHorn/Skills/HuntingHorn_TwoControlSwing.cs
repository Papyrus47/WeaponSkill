using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.HuntingHorn.Skills
{
    public class HuntingHorn_TwoControlSwing : HuntingHorn_Swing
    {
        public HuntingHorn_TwoControlSwing(HuntingHornProj proj, Func<bool> changeCondition) : base(proj, changeCondition)
        {
        }
        public override void AI()
        {
            base.AI();
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => (nowSkill as BasicHuntingHornSkill).PreAtk && ActivationCondition() && nowSkill is not HuntingHorn_TwoControlSwing;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            melodyType = HuntingHornMelody.MelodyType.LeftAndRight;
        }
    }
}
