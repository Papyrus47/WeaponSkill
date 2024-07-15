using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment.Skills
{
    public class BasicFrostBombardment : ProjSkill_Instantiation
    {
        public FrostBombardment_Proj frostBombardment => modProjectile as FrostBombardment_Proj;
        public Player Player;
        /// <summary>
        /// 可以用于切换停止技能
        /// </summary>
        public bool CanChangeToStopActionSkill
        {
            get
            {
                return frostBombardment.CanChangeToStopActionSkill;
            }
            set
            {
                frostBombardment.CanChangeToStopActionSkill = value;
            }
        }
        /// <summary>
        /// 攻击前摇判定
        /// </summary>
        public bool PreAtk;
        public BasicFrostBombardment(FrostBombardment_Proj modProjectile) : base(modProjectile)
        {
            Player = modProjectile.Player;
        }
        public override void OnSkillActive()
        {
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
        }
        public Texture2D GetTexture()
        {
            if(frostBombardment.SourceItem.InBomMode)
            {
                return FrostBombardment.ChangeTex.Value;
            }
            return TextureAssets.Projectile[Projectile.type].Value;
        }
    }
}
