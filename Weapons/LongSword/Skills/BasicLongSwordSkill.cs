using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.BroadSword;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    public abstract class BasicLongSwordSkill : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        /// <summary>
        /// 攻击的前摇,用于见切等需要双键判断的技能使用
        /// </summary>
        public bool PreAttack;
        public LongSwordProj LongSword => modProjectile as LongSwordProj;
        public BasicLongSwordSkill(LongSwordProj longSword) : base(longSword)
        {
            player = longSword.Player;
            swingHelper = longSword.SwingHelper;
        }
        public override void OnSkillActive()
        {
            PreAttack = false;
        }
        public override void OnSkillDeactivate()
        {
            PreAttack = false;
        }
    }
}
