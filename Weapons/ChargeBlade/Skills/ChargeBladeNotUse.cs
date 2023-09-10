using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBladeNotUse : BasicChargeBladeSkill
    {
        public ChargeBladeNotUse(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            base.AI();
            Vector2 rotVector = Vector2.UnitY.RotatedBy(0.225f);

            PreAttack = true;
            bool flag = false;
            if ((int)Projectile.ai[0]++ > 16) // 收入完毕
            {
                PreAttack = false;
                swingHelper.Change(rotVector, Vector2.One,0.45f);
            }
            else // 渐变收刀
            {
                swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f,0.45f,0.2f);
                flag = true;
            }
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -ChargeBladeProj.SwingLength * 0.6f, true);
            swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
            if (flag)
            {
                Projectile.Center -= Projectile.velocity * 0.45f;
            }
            #region 盾的更新
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -player.width * 0.8f, 0), Projectile.spriteDirection);
            shield.VisualRotation = 0.2f;
            shield.AxeRot = 0.2f;
            ChargeBladeProj.shieldCanDraw = false;
            #endregion
            Projectile.numHits = 0;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
            if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 23)
            {
                var effect = WeaponSkill.SwordHot.Value;
                effect.CurrentTechnique.Passes[0].Apply();
                effect.Parameters["tex"].SetValue(WeaponSkill.HotTex.Value);
                effect.Parameters["uTime"].SetValue(10f);
            }
            swingHelper.DrawSwingItem(lightColor);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
            ChargeBladeProj.shield.Draw(sb, lightColor);
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
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
        }
    }
}
