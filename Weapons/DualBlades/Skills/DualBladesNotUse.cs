using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    public class DualBladesNotUse : BasicDualBladesSkill
    {
        public DualBladesNotUse(DualBladesProj dualBladesProj) : base(dualBladesProj)
        {
            ID = "NotUse";
        }
        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(-player.direction * Projectile.width * 0.3f,0);
            Projectile.spriteDirection = player.direction;
            Projectile.ai[0] = 0;
            if (bladesProj.InDemonMode)
            {
                Projectile.rotation = (-MathHelper.PiOver2 - 1.5f) * Projectile.spriteDirection;
                player.heldProj = Projectile.whoAmI;
                Projectile.position.Y += Projectile.height * 0.3f;
                Projectile.position.X -= Projectile.width * 0.2f * Projectile.spriteDirection;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full,Projectile.rotation + MathHelper.Pi * 1.25f * Projectile.spriteDirection);
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi * 1.25f * Projectile.spriteDirection);
                if(player.GetModPlayer<WeaponSkillPlayer>().StatStamina <= 0)
                {
                    bladesGlobalItem.DemonMode = false;
                }
                if (WeaponSkill.BowSlidingStep.JustPressed && player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 50)
                {
                    player.velocity.X = 30 * player.direction;
                    player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 50;
                    if (player.GetModPlayer<WeaponSkillPlayer>().StatStamina <= 0) player.GetModPlayer<WeaponSkillPlayer>().StatStamina = 0;
                    Projectile.ai[1] = 1;
                }
                else if (Projectile.ai[1] > 0 && Projectile.ai[1]++ > 7)
                {
                    Projectile.ai[1] = 0;
                    player.velocity.X *= 0.1f;
                }
            }
            else
            {
                Projectile.rotation = (MathHelper.PiOver4 + 1.2f) * Projectile.spriteDirection;
            }
            AIAction?.Invoke(this);
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
                spriteBatch.Draw(tex, Projectile.Center - new Vector2(Projectile.width * -0.2f * Projectile.spriteDirection,-Projectile.height * 0.1f) - Main.screenPosition, bladesProj.DrawProjTex.Frame(1, Main.projFrames[bladesProj.Type]), color, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                return;
            }
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, bladesProj.DrawProjTex.Frame(1, Main.projFrames[bladesProj.Type]), color, Projectile.rotation + (0.5f * Projectile.spriteDirection), tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        }
    }
}
