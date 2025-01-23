using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Weapons.Lances.Skills
{
    public class LancesDef : BasicLancesSkills
    {
        public LancesDef(LancesProj lancesProj) : base(lancesProj)
        {
        }

        public override void AI()
        {
            Vector2 rotVector = Vector2.UnitX.RotatedBy(-0.5);
            swingHelper.ProjFixedPlayerCenter(player, 0);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f, 0f, 0.2f);
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SwingAI(lancesProj.SwingLength, player.direction, 0);
            PreAttack = true;
            //if (Projectile.ai[1]++ <= 0)
            //{
            //    player.ChangeDir((player.velocity.X >= 0).ToDirectionInt());
            //}
            #region 盾的更新

            LancesShield lancesShield = lancesProj.shield;
            lancesShield.Update(Projectile.Center - new Vector2(-5 * player.direction, 5), player.direction);
            lancesShield.InDef = true;
            if (Projectile.ai[0]++ < 20)
            {
                PreAttack = true;
                lancesShield.GP = true;
            }
            #endregion
            if (!WeaponSkill.BowSlidingStep.Current || player.GetModPlayer<WeaponSkillPlayer>().StatStamina < 0)
            {
                SkillTimeOut = true;
            }
            if (Math.Abs(player.velocity.X) > 1) player.velocity.X = 1 * (player.velocity.X > 0).ToDirectionInt();
            #region 防御成功击退代码
            player.GetModPlayer<WeaponSkillPlayer>().StatStaminaAddTime = 0;
            if (lancesShield.DefSucceeded)
            {
                player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 50 * ((int)lancesShield.KNLevel);
                player.velocity.X -= 50 * ((int)lancesShield.KNLevel) * player.direction;
            }
            #endregion
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
        public override bool ActivationCondition() => WeaponSkill.BowSlidingStep.Current && player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 50;
        public override bool SwitchCondition() => Projectile.ai[0] >= 30 && Projectile.ai[1] <= 0;
    }
}
