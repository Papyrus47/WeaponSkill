using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.Lances.Skills
{
    public class LancesSpurts : BasicLancesSkills
    {
        public LancesSpurts(LancesProj lancesProj) : base(lancesProj)
        {
            ActivationConditionFunc = () => player.controlUseItem;
        }
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDmg = 1f;
        public Func<bool> ActivationConditionFunc;
        public override void AI()
        {
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] == 1)
            {
                swingHelper.Change(Vector2.UnitX, new Vector2(1f, 0f), 0);
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
            }
            if (Projectile.ai[0] < 20)
            {
                player.velocity.X = 5 * player.direction;
                PreAttack = true;
            }
            else if (Projectile.ai[0] == 20)
            {
                SpurtsProj proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromThis(), player.Center, Projectile.velocity.SafeNormalize(default), (int)(Projectile.damage * ActionDmg), 0, Projectile.owner, lancesProj.SwingLength * 2.5f, 100, TextureAssets.Heart.Value);
            }
            LancesShield lancesShield = lancesProj.shield;
            lancesShield.Update(player.Center, player.direction);

            swingHelper.ProjFixedPlayerCenter(player, (5 - Projectile.ai[0] * Projectile.ai[0] * Projectile.ai[0] / 250000f) * 2f, true);
            swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            swingHelper.SwingAI(lancesProj.SwingLength, player.direction, 0);
            if (Projectile.ai[0] > 150)
            {
                Projectile.ai[0] = 0;
                Projectile.extraUpdates = 0;
                SkillTimeOut = true;
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
        public override bool ActivationCondition() => ActivationConditionFunc.Invoke();
        public override bool SwitchCondition() => Projectile.ai[0] >= 130;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            Projectile.extraUpdates = 4;
        }
    }
}
