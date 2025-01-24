using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.SwordShield;

namespace WeaponSkill.Weapons.SlashAxe.Skills
{
    public class BasicSlashAxeSkill : ProjSkill_Instantiation
    {
        public Player player;
        public PartSwingHelper swingHelper;
        /// <summary>
        /// 预备攻击判定
        /// </summary>
        public bool PreAtk;
        public SlashAxeProj SlashAxeProj => modProjectile as SlashAxeProj;
        public BasicSlashAxeSkill(SlashAxeProj proj) : base(proj)
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
            #region 屏幕缩放shader调用
            if (Filters.Scene[WeaponSkill.ScreenScaleShader].IsActive())
                Filters.Scene.Deactivate(WeaponSkill.ScreenScaleShader);
            #endregion
            Projectile.damage = player.GetWeaponDamage(SlashAxeProj.SpawnItem);
        }
    }
}
