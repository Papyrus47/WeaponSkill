using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.BroadSword;

namespace WeaponSkill.Weapons.SwordShield.Skills
{
    public abstract class BasicSwordShieldSkill : ProjSkill_Instantiation
    {
        public Player player;
        public SwingHelper swingHelper;
        public SwordShieldProj SwordShieldProj => modProjectile as SwordShieldProj;
        public BasicSwordShieldSkill(SwordShieldProj proj) : base(proj)
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
            Projectile.damage = player.GetWeaponDamage(SwordShieldProj.SpawnItem);

            #region 屏幕缩放shader调用
            if (Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Deactivate(WeaponSkill.ScreenScaleShader);
            #endregion
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            SwordDraw(sb, ref lightColor);
            ShieldDraw(sb, ref lightColor);
            return false;
        }
        public virtual void SwordDraw(SpriteBatch sb, ref Color lightColor) { }
        public virtual void ShieldDraw(SpriteBatch sb, ref Color lightColor) { }
    }
}
