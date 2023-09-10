using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Sword_Held : ChargeBlade_Sword_Basic
    {
        public ChargeBlade_Sword_Held(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            base.AI();
            Vector2 rotVector = Vector2.UnitX.RotatedBy(0.225f + Math.Sin(Projectile.ai[1]) * 0.05f);

            player.heldProj = Projectile.whoAmI;
            Projectile.ai[0]++;
            Projectile.ai[1] += player.velocity.X * 0.02f + 0.03f;
            if (Projectile.ai[0] > 120) // 用于跳出技能
            {
                Projectile.ai[1] = 0;
                SkillTimeOut = true;
                return;
            }
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f);
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -ChargeBladeProj.SwingLength * 0.45f, true);
            swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
            ChargeBladeShield shield = ChargeBladeProj.shield;
            if ((shield.swingHelper.center - player.Center).Length() > 30)
            {
                shield.AxeRot += 0.015f * (shield.swingHelper.center - player.Center).Length();
            }
            //if (flag)
            //{
            //    Projectile.Center -= Projectile.velocity * 0.45f;
            //}
            //#region 盾的更新
            //ChargeBladeShield shield = ChargeBladeProj.shield;
            //shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * player.width * 0.45f, 0), Projectile.spriteDirection);
            //shield.VisualRotation = 0.3f;
            //shield.AxeRot = 0.05f;
            //player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.direction * -MathHelper.PiOver2);
            //#endregion
            Projectile.numHits = 0;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
            if(ChargeBladeProj.chargeBladeGlobal.StatCharge >= 23)
            {
                var effect = WeaponSkill.SwordHot.Value;
                effect.CurrentTechnique.Passes[0].Apply();
                effect.Parameters["tex"].SetValue(WeaponSkill.HotTex.Value);
                effect.Parameters["uTime"].SetValue(10f);
            }
            swingHelper.DrawSwingItem(lightColor);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);

            return false;
        }
        public override bool ActivationCondition() => player.controlUseItem;
        public override bool SwitchCondition() => Projectile.ai[0] > 15;
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            Projectile.ai[1] = 0;
            Projectile.rotation = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
        }
    }
}
