using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.SlashAxe.Skills
{
    public class SlashAxe_AxeHeld : BasicSlashAxeSkill
    {
        public SlashAxe_AxeHeld(SlashAxeProj proj) : base(proj)
        {
        }

        public override void AI()
        {
            if(Projectile.ai[0]++ > 120)
            {
                SkillTimeOut = true;
            }
            Projectile.spriteDirection = player.direction;
            player.direction = ((Main.MouseWorld - player.Center).X > 0).ToDirectionInt();
            swingHelper.Change_Lerp(Vector2.UnitX.RotatedBy(-0.4), 0.2f, Vector2.One, 1f, 0f);
            swingHelper.ProjFixedPlayerCenter(player, 0,true);
            swingHelper.SetSwingActive();
            swingHelper.SwingAI(SlashAxeProj.SwingLength, player.direction, 0);

            #region 剑控制
            if (swingHelper.Parts.TryGetValue("Sword", out var sword))
            {
                sword.SPDir = 1;
                sword.Update();
                sword.Rot = MathHelper.Pi * -0.95f * -player.direction;
                sword.OffestCenter = Vector2.Lerp(sword.OffestCenter, Projectile.velocity.RotatedBy(MathHelper.PiOver2 * Projectile.spriteDirection) * -0.1f + sword.velocity.SafeNormalize(default) * SlashAxeProj.SwingLength * 0.4f,0.9f);
            }
            #endregion
            #region 斧控制
            if (swingHelper.Parts.TryGetValue("Axe", out var axe))
            {
                axe.Update();
                axe.OffestCenter = Vector2.Lerp(axe.OffestCenter, Projectile.velocity.RotatedBy(MathHelper.PiOver2 * Projectile.spriteDirection) * 0.1f + axe.velocity.SafeNormalize(default) * SlashAxeProj.SwingLength * 0.6f, 0.9f);
            }
            #endregion
        }
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => player.controlUseItem;
        public override bool SwitchCondition() => true;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            //swingHelper.Swing_Draw_ItemAndTrailling(lightColor, null, null);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            swingHelper.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Sword", out var sword))
                sword.DrawSwingItem(lightColor);
            if (swingHelper.Parts.TryGetValue("Axe", out var axe))
                axe.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
