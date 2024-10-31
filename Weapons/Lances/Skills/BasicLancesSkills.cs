using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Weapons.Lances.Skills
{
    public class BasicLancesSkills : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        /// <summary>
        /// 攻击的前摇,用于见切等需要双键判断的技能使用
        /// </summary>
        public bool PreAttack;
        public LancesProj lancesProj => modProjectile as LancesProj;
        public BasicLancesSkills(LancesProj lancesProj) : base(lancesProj)
        {
            player = lancesProj.Player;
            swingHelper = lancesProj.SwingHelper;
        }
        public override void OnSkillActive()
        {
            PreAttack = false;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            PreAttack = false;
            Projectile.extraUpdates = 0;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
        }
    }
}
