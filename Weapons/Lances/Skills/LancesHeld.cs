using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Lances.Skills
{
    public class LancesHeld : BasicLancesSkills
    {
        public LancesHeld(LancesProj lancesProj) : base(lancesProj)
        {
        }

        public override void AI()
        {
            Vector2 rotVector = Vector2.UnitX.RotatedBy(-0.5);
            swingHelper.ProjFixedPlayerCenter(player, 0);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f, 0f, 0.2f);
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SwingAI(lancesProj.SwingLength, player.direction, 0);
            if (Projectile.ai[1] <= 0)
            {
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
            }
            #region 盾的更新

            LancesShield lancesShield = lancesProj.shield;
            lancesShield.Update(Projectile.Center, player.direction);
            #endregion
            Projectile.ai[0]++;

            if (Projectile.ai[0] > 120)
            {
                Projectile.ai[0] = 0;
                SkillTimeOut = true;
            }
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1]--;
                player.velocity.X = 10 * -player.direction;
            }
            if (WeaponSkill.SpKeyBind.JustPressed && Projectile.ai[1] <= 0)
            {
                player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 50;
                Projectile.ai[0] = 0;
                Projectile.ai[1] = 15;
                player.velocity.Y = -5;
            }
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
        public override bool ActivationCondition() => player.controlUseItem;
        public override bool SwitchCondition() => Projectile.ai[0] >= 30 && Projectile.ai[1] <= 0;
    }
}
