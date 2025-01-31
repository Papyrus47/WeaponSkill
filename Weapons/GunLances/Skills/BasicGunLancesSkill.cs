using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;

namespace WeaponSkill.Weapons.GunLances.Skills
{
    public abstract class BasicGunLancesSkill : ProjSkill_Instantiation
    {
        public Player player;
        public PartSwingHelper swingHelper;
        public GunLancesProj GunLancesProj => modProjectile as GunLancesProj;
        /// <summary>
        /// 攻击的前摇,用于见切等需要双键判断的技能使用
        /// </summary>
        public bool PreAttack;
        public BasicGunLancesSkill(GunLancesProj modProjectile) : base(modProjectile)
        {
            player = modProjectile.Player;
            swingHelper = modProjectile.SwingHelper;
        }
        public override void OnSkillActive()
        {
            PreAttack = false;
            SkillTimeOut = false;
        }
        public virtual bool SwitchCondition(ProjSkill_Instantiation changeToSkill) => SwitchCondition();
        public virtual void OnSkillDeactivate(ProjSkill_Instantiation changeToSkill)
        {
            OnSkillDeactivate();
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
