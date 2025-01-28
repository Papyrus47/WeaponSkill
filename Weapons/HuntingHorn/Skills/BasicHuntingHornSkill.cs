using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.InsectStaff;

namespace WeaponSkill.Weapons.HuntingHorn.Skills
{
    public abstract class BasicHuntingHornSkill : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        public bool PreAtk;
        public HuntingHornProj HuntingHornProj => modProjectile as HuntingHornProj;
        public BasicHuntingHornSkill(HuntingHornProj proj) : base(proj)
        {
            player = proj.Player;
            swingHelper = proj.SwingHelper;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            OnSkillActive();
            Projectile.extraUpdates = 0;
            Projectile.damage = player.GetWeaponDamage(HuntingHornProj.SpawnItem);
            Projectile.localAI[0] = 0;

            #region 屏幕缩放shader取消调用
            if (Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Deactivate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
    }
}
