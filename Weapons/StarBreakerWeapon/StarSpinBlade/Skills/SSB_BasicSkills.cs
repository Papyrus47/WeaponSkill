using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade.Skills
{
    public class SSB_BasicSkills : StarBreakerSkills
    {
        public StarSpinBladeProj StarSpinBladeProj => modProjectile as StarSpinBladeProj;
        public SwingHelper SwingHelper;
        /// <summary>
        /// 攻击前摇判定
        /// </summary>
        public bool PreAtk;
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            CanChangeToStopActionSkill = false;
            Projectile.rotation = 0;
            StarSpinBladeProj.ResetPreAtk = true;
        }

        public StarSpinBlade GetStarSpinBladeItem()
        {
            return (Player.HeldItem.ModItem as StarSpinBlade);
        }
        public SSB_BasicSkills(StarSpinBladeProj modProjectile) : base(modProjectile)
        {
            Player = modProjectile.Player;
            SwingHelper = modProjectile.SwingHelper;
        }
    }
}
