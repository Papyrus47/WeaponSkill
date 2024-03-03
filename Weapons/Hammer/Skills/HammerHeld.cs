using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Weapons.Hammer.Skills
{
    public class HammerHeld : BasicHammerSkill
    {
        public HammerHeld(HammerProj hammerProj) : base(hammerProj)
        {
        }
        public override void AI()
        {
            base.AI();
            Vector2 rotVector = Vector2.UnitX.RotatedBy(0.225f + Math.Sin(Projectile.ai[1]) * 0.05f);

            player.heldProj = Projectile.whoAmI;
            Projectile.ai[0]++;
            Projectile.ai[1] += player.velocity.X * 0.03f + 0.03f;
            if (Projectile.ai[0] > 120) // 用于跳出技能
            {
                Projectile.ai[1] = 0;
                SkillTimeOut = true;
                return;
            }
            swingHelper.Change_Lerp(rotVector, 0.1f, Vector2.One, 1f);
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0), -hammerProj.SwingLength * 0.1f);
            swingHelper.SwingAI(hammerProj.SwingLength, player.direction, 0);
            Projectile.numHits = 0;
            hammerProj.ChannelLevel = 0;
            SkillTimeOut = false;
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
        public override bool SwitchCondition() => Projectile.ai[0] > 15;
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            Projectile.ai[1] = 0;
            Projectile.rotation = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;
        }
    }
}
