using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Hammer.Skills
{
    public class BasicHammerSkill : ProjSkill_Instantiation
    {
        public BasicHammerSkill(HammerProj hammerProj) : base(hammerProj)
        {
            player = hammerProj.Player;
            swingHelper = hammerProj.SwingHelper;
        }
        public Player player;
        public SwingHelper swingHelper;
        /// <summary>
        /// 攻击的前摇,用于见切等需要双键判断的技能使用
        /// </summary>
        public bool PreAttack;
        public HammerProj hammerProj => modProjectile as HammerProj;
        public override void OnSkillActive()
        {
            PreAttack = false;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            PreAttack = false;
            SkillTimeOut = false;
        }
    }
}
