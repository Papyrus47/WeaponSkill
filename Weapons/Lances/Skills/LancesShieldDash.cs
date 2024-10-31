using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Lances.Skills
{
    public class LancesShieldDash : BasicLancesSkills
    {
        public LancesShieldDash(LancesProj lancesProj) : base(lancesProj)
        {
        }

        public override void AI()
        {
            Vector2 rotVector = -Vector2.UnitX;
            swingHelper.ProjFixedPlayerCenter(player, 0);
            swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f, 0f, 0.2f);
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SwingAI(lancesProj.SwingLength, player.direction, 0);
            if (Projectile.ai[1] <= 0)
            {
                TheUtility.ResetProjHit(Projectile);
                player.ChangeDir((Main.MouseWorld.X - player.Center.X > 0).ToDirectionInt());
            }
            #region 盾的更新

            LancesShield lancesShield = lancesProj.shield;
            lancesShield.Update(Projectile.Center, player.direction);
            if(player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 0)
                lancesShield.InDef = true;
            if (lancesShield.DefSucceeded)
            {
                player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 50 * ((int)lancesShield.KNLevel);
            }
            #endregion
            Projectile.ai[0]++;
            Projectile.ai[1]++;
            if (Projectile.ai[0] > 80)
            {
                SkillTimeOut = true;
            }
            else if (Projectile.ai[0] < 50)
            {
                player.velocity.X = 8 * player.direction;
                lancesShield.GP = true;
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
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => player.Hitbox.Intersects(targetHitbox);
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            TheUtility.ResetProjHit(Projectile);
        }
        public override bool ActivationCondition() => player.controlUseItem;
        public override bool SwitchCondition() => Projectile.ai[0] >= 50;
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill) => ActivationCondition();
    }
}
