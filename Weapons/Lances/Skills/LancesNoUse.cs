using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Weapons.Lances.Skills
{
    /// <summary>
    /// 长枪未使用状态
    /// </summary>
    public class LancesNoUse : BasicLancesSkills
    {
        public LancesNoUse(LancesProj lancesProj) : base(lancesProj)
        {
        }
        public override void AI()
        {
            Vector2 rotVector = -Vector2.UnitY;
            swingHelper.ProjFixedPlayerCenter(player, 0);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f, 0f, 0.2f);
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -lancesProj.SwingLength * 0.6f,true);
            swingHelper.SwingAI(lancesProj.SwingLength, player.direction, 0);
            #region 盾的更新

            LancesShield lancesShield = lancesProj.shield;
            lancesShield.Update(Projectile.Center, -player.direction);
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
    }
}
