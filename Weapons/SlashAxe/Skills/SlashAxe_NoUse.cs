using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Items.SlashAxe;

namespace WeaponSkill.Weapons.SlashAxe.Skills
{
    public class SlashAxe_NoUse : BasicSlashAxeSkill
    {
        public SlashAxe_NoUse(SlashAxeProj proj) : base(proj)
        {
        }
        public override void AI()
        {

            Projectile.spriteDirection = -player.direction;
            swingHelper.Change_Lerp(Vector2.UnitY.RotatedBy(0.4), 1, Vector2.One, 1f, 0f);
            swingHelper.ProjFixedPlayerCenter(player, -SlashAxeProj.SwingLength * 0.5f);
            swingHelper.SetSwingActive();
            swingHelper.SwingAI(SlashAxeProj.SwingLength, player.direction, 0);

            #region 剑控制
            if (swingHelper.Parts.TryGetValue("Sword", out var sword))
            {
                sword.SPDir = 1;
                sword.Update();
                sword.Rot = MathHelper.Pi * player.direction;
                sword.OffestCenter = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * player.direction) * 0.1f + sword.velocity.SafeNormalize(default) * SlashAxeProj.SwingLength * 0.4f;
            }
            #endregion
            #region 斧控制
            if (swingHelper.Parts.TryGetValue("Axe", out var axe))
            {
                axe.Update();
                axe.OffestCenter = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * player.direction) * -0.1f + axe.velocity.SafeNormalize(default) * SlashAxeProj.SwingLength * 0.1f;
            }
            #endregion
        }
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => true;
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
