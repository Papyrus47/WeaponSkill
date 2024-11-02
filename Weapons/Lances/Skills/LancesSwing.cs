using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.Lances.Skills
{
    public class LancesSwing : BasicLancesSkills
    {
        public LancesSwing(LancesProj lancesProj) : base(lancesProj)
        {
            ActivationConditionFunc = () => player.controlUseTile && player.controlUseItem;
        }
        public Func<bool> ActivationConditionFunc;
        public override void AI()
        {
            if (Math.Abs(player.velocity.X) > 2) player.velocity.X = 2 * (player.velocity.X > 0).ToDirectionInt();
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] == 1)
            {
                swingHelper.Change(-Vector2.UnitX, new Vector2(1.5f, 0.3f), 0);
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
                swingHelper.SetRotVel(player.direction == 1 ? (Main.MouseWorld - player.Center).ToRotation() : -(player.Center - Main.MouseWorld).ToRotation());
            }
            if (Projectile.ai[0] < 20)
            {
                player.velocity.X = 2 * player.direction;
                PreAttack = true;
            }
            LancesShield lancesShield = lancesProj.shield;
            lancesShield.Update(player.Center, player.direction);

            Projectile.spriteDirection = Projectile.direction = player.direction;
            swingHelper.ProjFixedPlayerCenter(player, (5 - Projectile.ai[0] * Projectile.ai[0] * Projectile.ai[0] / 250000f) * 2f, true);
            float factor = Projectile.ai[0] / 150f;
            factor *= factor * factor;
            swingHelper.SwingAI(lancesProj.SwingLength, player.direction, factor * (MathHelper.PiOver2 * 3.75f));
            if (Projectile.ai[0] > 150)
            {
                Projectile.ai[0] = 0;
                Projectile.extraUpdates = 0;
                SkillTimeOut = true;
            }

        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Color color = lightColor;
            swingHelper.Swing_Draw_ItemAndAfterimage(lightColor, (factor) => Draw(color, factor), -1);
            //sb.End();
            //sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);

            //swingHelper.DrawSwingItem(lightColor);

            //sb.End();
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
            //    Main.Rasterizer, null, Main.Transform);
            return false;
        }

        public static Color Draw(Color lightColor, float factor)
        {
            return lightColor * (1 - factor);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool? CanDamage() => true;
        public override bool ActivationCondition() => ActivationConditionFunc.Invoke();
        public override bool SwitchCondition() => Projectile.ai[0] >= 130;
        public override void OnSkillActive()
        {
            TheUtility.ResetProjHit(Projectile);
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            Projectile.extraUpdates = 4;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            swingHelper.SetRotVel(0);
        }
        //public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => (nowSkill as BasicLancesSkills).PreAttack && ActivationCondition();
    }
}
