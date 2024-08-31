using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Staffs.Skills
{
    /// <summary>
    /// 法杖基础技能类
    /// </summary>
    public abstract class BasicMagicStaffsSkill : ProjSkill_Instantiation
    {
        public Player Player;
        public SwingHelper SwingHelper;
        public MagicStaffsProj staffsProj => modProjectile as MagicStaffsProj;
        public BasicMagicStaffsSkill(MagicStaffsProj modProjectile) : base(modProjectile)
        {
            Player = modProjectile.Player;
            SwingHelper = modProjectile.SwingHelper;
        }
        public virtual string Name => "Skill";
        
    }
}
