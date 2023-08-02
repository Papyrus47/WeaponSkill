﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.BroadSword;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    public class LongSwordNotUse : BasicLongSwordSkill
    {
        public LongSwordNotUse(LongSwordProj broadSwordProj) : base(broadSwordProj)
        {
        }
        public override void AI()
        {
            Vector2 rotVector = Vector2.UnitY.RotatedBy(0.225f);

            PreAttack = true;
            bool flag = false;
            if ((int)Projectile.ai[0]++ > 16) // 收入完毕
            {
                PreAttack = false;
                swingHelper.Change(rotVector,Vector2.One);
            }
            else // 渐变收刀
            {
                swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f);
                flag = true;
            }
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -LongSword.SwingLength * 0.45f, true);
            swingHelper.SwingAI(LongSword.SwingLength, player.direction,0);
            LongSword.swordScabbard.DrawPos = Projectile.Center;
            LongSword.swordScabbard.Dir = player.direction;
            if (flag)
            {
                Projectile.Center -= Projectile.velocity * 0.45f;
            }
            Projectile.numHits = 0;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
            swingHelper.DrawSwingItem(lightColor);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);

            return false;
        }
        public override bool SwitchCondition() => !PreAttack;
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            Projectile.rotation = 0;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = 0;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
        }
    }
}
