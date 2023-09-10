using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Axe_Swing_Liberate_SP_PreAttack : BasicChargeBladeSkill
    {
        public ChargeBlade_Axe_Swing_Liberate_SP_PreAttack(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public Dictionary<ProjSkill_Instantiation, Func<bool>> ChangeSkillCondition = new();
        public override void AI()
        {
            base.AI();
            Vector2 rotVector = (-Vector2.UnitY).RotatedBy(0.225f).RotatedBy(Projectile.ai[0] * 0.001);
            bool flag = false;
            
            //if ((int)Projectile.ai[0]++ > 8) // 收入完毕
            //{
            //    if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 10)
            //    {
            //        HasBottles = true;
            //        ChargeBladeProj.chargeBladeGlobal.StatChargeBottle += 3;
            //        if (ChargeBladeProj.chargeBladeGlobal.StatCharge >= 16)
            //        {
            //            ChargeBladeProj.chargeBladeGlobal.StatChargeBottle = ChargeBladeProj.chargeBladeGlobal.StatChargeBottleMax;
            //        }
            //        ChargeBladeProj.chargeBladeGlobal.StatCharge = 0;
            //    }
            //    if (!HasBottles)
            //    {
            //        Projectile.ai[0] -= 0.5f;
            //    }
            //    if (Projectile.ai[0] > 30)
            //    {
            //        SkillTimeOut = true;
            //    }
            //}
            if ((int)Projectile.ai[0]++ > 50) // 不允许切换技能
            {
                PreAttack = false;
            }
            else PreAttack = true;
            if (Projectile.ai[0] > 25)
            {
                swingHelper.Change(rotVector, Vector2.One, 0.45f);
                swingHelper.SetSwingActive();
                swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0, -player.height * 0.1f - MathHelper.SmoothStep(0, 8, (Projectile.ai[0] - 25) / 25) * 3.5f), -ChargeBladeProj.SwingLength * 0.6f, true);
                swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
            }
            else
            {
                swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0, -player.height * 0.1f), -ChargeBladeProj.SwingLength * 0.6f, true);
            }
            ChargeBladeProj.chargeBladeGlobal.InAxe = false;
            Projectile.spriteDirection = player.direction;
            if (flag)
            {
                Projectile.Center -= Projectile.velocity * 0.45f;
            }
            #region 盾的更新
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(0, -player.height * 0.7f), -player.direction);
            shield.VisualRotation = 0.1f;
            shield.AxeRot = -0.28f - Projectile.ai[0] * 0.001f + MathHelper.Pi;
            if((shield.swingHelper.center - player.Center).Length() > 30)
            {
                shield.AxeRot += 0.015f * (shield.swingHelper.center - player.Center).Length();
            }
            ChargeBladeProj.shieldCanDraw = false;
            #endregion
            Projectile.numHits = 0;
            ChargeBladeProj.chargeBladeGlobal.InAxe = true;
            player.velocity.X *= 0;
            player.ChangeDir((int)Projectile.ai[2]);
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
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => ActivationCondition();
        public override bool ActivationCondition()
        {
            if (ChangeSkillCondition.TryGetValue(ChargeBladeProj.CurrentSkill, out var func))
            {
                return func.Invoke();
            }
            return player.controlUseItem && player.controlUseTile;
        }

        public override bool SwitchCondition() => !PreAttack;
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            Projectile.rotation = 0;
            Projectile.ai[2] = player.direction;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
            Projectile.ai[2] = 0;
        }
        public virtual void AddChangeSkill(BasicChargeBladeSkill basicChargeBladeSkill,Func<bool> ChangeSkill)
        {
            ChangeSkillCondition.Add(basicChargeBladeSkill, ChangeSkill);
        }
    }
}
