using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public abstract class BasicChargeBladeSkill : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        /// <summary>
        /// 攻击的前摇,用于见切等需要双键判断的技能使用
        /// </summary>
        public bool PreAttack;
        public ChargeBladeProj ChargeBladeProj => modProjectile as ChargeBladeProj;
        public BasicChargeBladeSkill(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
            player = chargeBlade.Player;
            swingHelper = chargeBlade.SwingHelper;
        }
        public override void OnSkillActive()
        {
            PreAttack = false;
        }
        public override void OnSkillDeactivate()
        {
            PreAttack = false;
            ChargeBladeProj.shield.Fixed = false;
        }
    }
}
