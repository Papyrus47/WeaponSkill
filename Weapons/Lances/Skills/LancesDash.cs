using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Lances.Skills
{
    public class LancesDash : BasicLancesSkills
    {
        public LancesDash(LancesProj lancesProj) : base(lancesProj)
        {
        }
        public override void AI()
        {
            Vector2 rotVector = Vector2.UnitX;
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
            Projectile.ai[1]++;
            if (Projectile.ai[0] > 5)
            {
                Projectile.ai[0] = 0;
                TheUtility.ResetProjHit(Projectile);
            }
            if (Math.Abs(player.velocity.X) < 12) player.velocity.X = 12 * player.direction;

            if (WeaponSkill.SpKeyBind.JustPressed) // 刹车
                SkillTimeOut = true;
            
        }
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
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
        public override bool ActivationCondition() => player.controlUseItem && player.controlUseTile;
        public override bool SwitchCondition() => (Projectile.ai[1] > 30 && (player.controlUseItem || WeaponSkill.BowSlidingStep.JustPressed)) || player.GetModPlayer<WeaponSkillPlayer>().StatStamina-- <= 0;
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            return ActivationCondition() && nowSkill is BasicLancesSkills skills && skills.PreAttack;
        }
    }
}
