using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using static WeaponSkill.Weapons.ChargeBlade.Skills.ChargeBlade_Axe_Swing_Liberate;
using WeaponSkill.Items.ChargeBlade;
using WeaponSkill.NPCs;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Sword_ShieldSpurts : ChargeBlade_Sword_Basic
    {
        public ChargeBlade_Sword_ShieldSpurts(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public override void AI()
        {
            Vector2 rotVector = Vector2.UnitX.RotatedBy(0.225f);
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 120) // 用于跳出技能
            {
                SkillTimeOut = true;
                return;
            }
            #region 剑更新
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f);
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -ChargeBladeProj.SwingLength * 0.45f, true);
            swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
            #endregion
            #region 盾更新
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * player.width * 0.6f, 0), player.direction);
            shield.VisualRotation = 0.3f;
            Projectile.extraUpdates = 2;
            float factor = Projectile.ai[0] - 15;
            if(factor > 15)
            {
                factor = 50 - Projectile.ai[0];
            }
            if (factor < 0) factor = 0;
            shield.swingHelper.center += new Vector2(1 * player.direction, -0.7f) * factor * 1.5f;
            shield.AxeRot = MathHelper.Lerp(shield.AxeRot, -MathHelper.PiOver2 * 1.35f,0.3f);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.direction * -MathHelper.PiOver2);
            #endregion
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
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ChargeBladeProj.shield.swingHelper.GetColliding(targetHitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            TheUtility.SetPlayerImmune(player, 35);
            (ChargeBladeProj.SpawnItem.ModItem as BasicChargeBlade).LiberateHit(target, player);
            WeaponSkillGlobalNPC.AddComponent(target, new LiberateOnHit(15, ChargeBladeProj.SpawnItem.ModItem as BasicChargeBlade, player));
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 15 && Projectile.ai[0] < 30;
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if(nowSkill is ChargeBlade_Sword_Swing_Dash dash)
            {
                return player.controlUseTile && player.controlUseItem && dash.PreAttack;
            }
            return player.controlUseTile && player.controlUseItem;
        }

        public override bool ActivationCondition() => player.controlUseTile && player.controlUseItem;
        public override bool SwitchCondition() => Projectile.ai[0] > 90;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            Projectile.ai[1] = 0;
            Projectile.rotation = 0;
            TheUtility.ResetProjHit(Projectile);
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;
            Projectile.extraUpdates = 0;
        }
    }
}
