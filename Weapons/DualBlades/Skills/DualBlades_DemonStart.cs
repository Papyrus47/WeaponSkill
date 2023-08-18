using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    public class DualBlades_DemonStart : BasicDualBladesSkill
    {
        public DualBlades_DemonStart(DualBladesProj dualBladesProj) : base(dualBladesProj)
        {
            ID = "DemonStart";
        }
        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(-player.direction * Projectile.width * 0.3f, 0);
            Projectile.spriteDirection = player.direction;
            Projectile.ai[0]++;
            if (bladesProj.InDemonMode)
            {
                Projectile.rotation = (-MathHelper.PiOver2 * 0.6f) * Projectile.spriteDirection;
                player.heldProj = Projectile.whoAmI;
                Projectile.position.Y -= player.height * 0.5f + (20 * Math.Clamp((Projectile.ai[0] / 4) - 1, 0, 1));
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi * 1.25f * Projectile.spriteDirection);
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi * 1.25f * Projectile.spriteDirection);
            }
            else
            {
                Projectile.rotation = (MathHelper.PiOver4 + 1.2f) * Projectile.spriteDirection;
            }
            if (Projectile.ai[0] > 15)
            {
                SkillTimeOut = true;
            }
            AIAction?.Invoke(this);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            bladesGlobalItem.DemonMode = !bladesGlobalItem.DemonMode;
            SkillTimeOut = false;
        }
        public override bool ActivationCondition() => WeaponSkill.RangeChange.JustReleased;
        public override bool SwitchCondition() => false;
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = 0;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Texture2D tex = bladesProj.DrawProjTex.Value;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, bladesProj.DrawProjTex.Frame(1, Main.projFrames[bladesProj.Type]), lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override void BackDraw(SpriteBatch spriteBatch, Color color)
        {
            Texture2D tex = bladesProj.DrawProjTex.Value;
            if (bladesProj.InDemonMode)
            {
                spriteBatch.Draw(tex, Projectile.Center - new Vector2(Projectile.width * -0.2f * Projectile.spriteDirection, -Projectile.height * 0.1f) - Main.screenPosition, bladesProj.DrawProjTex.Frame(1, Main.projFrames[bladesProj.Type]), color, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                return;
            }
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, bladesProj.DrawProjTex.Frame(1, Main.projFrames[bladesProj.Type]), color, Projectile.rotation + (0.5f * Projectile.spriteDirection), tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
