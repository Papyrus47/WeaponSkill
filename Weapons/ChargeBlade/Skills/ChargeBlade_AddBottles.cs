using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_AddBottles : BasicChargeBladeSkill
    {
        public ChargeBlade_AddBottles(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        /// <summary>
        /// 是否装填了瓶子
        /// </summary>
        public bool HasBottles;
        public override void AI()
        {
            base.AI();
            Vector2 rotVector = (-Vector2.UnitY).RotatedBy(0.225f).RotatedBy(Projectile.ai[0] * 0.001);

            bool flag = false;
            swingHelper.Change(rotVector, new Vector2(-1,1), 0.45f);
            if ((int)Projectile.ai[0]++ > 8) // 收入完毕
            {
                if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 10)
                {
                    HasBottles = true;
                    ChargeBladeProj.chargeBladeGlobal.StatChargeBottle += 3;
                    if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 16)
                    {
                        ChargeBladeProj.chargeBladeGlobal.StatChargeBottle += 2;
                    }
                    ChargeBladeProj.chargeBladeGlobal.StatCharge = 0;
                }
                if (!HasBottles)
                {
                    Projectile.ai[0] -= 0.5f;
                }
                if (Projectile.ai[0] > 30)
                {
                    SkillTimeOut = true;
                }
            }
            if ((int)Projectile.ai[0] > 14) // 可以切换技能
            {
                PreAttack = false;
            }
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0, -player.height * 0.1f - Math.Min(Projectile.ai[0],8) * 3.5f), -ChargeBladeProj.SwingLength * 0.6f, true);
            swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
            if (flag)
            {
                Projectile.Center -= Projectile.velocity * 0.45f;
            }
            #region 盾的更新
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0, -player.height * 0.5f), -player.direction);
            shield.VisualRotation = 0.1f;
            shield.AxeRot = 0.28f + Projectile.ai[0] * 0.001f + MathHelper.Pi;
            ChargeBladeProj.shieldCanDraw = false;
            #endregion
            Projectile.numHits = 0;
            //SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
            swingHelper.DrawSwingItem(lightColor);
            ChargeBladeProj.shield.Draw(sb, lightColor);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);

            return false;
        }
        public override bool ActivationCondition() => player.controlUseTile && WeaponSkill.BowSlidingStep.Current;
        public override bool SwitchCondition() => !PreAttack;
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            Projectile.rotation = 0;
            HasBottles = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
        }
    }
}
