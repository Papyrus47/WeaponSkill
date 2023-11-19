using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Bows.Skills
{
    public class BowSlidingStep : BasicBowsSkill
    {
        public BowSlidingStep(ModProjectile modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * Projectile.width * 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation() - (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            player.ChangeDir((int)Projectile.ai[1]);
            player.heldProj = Projectile.whoAmI;
            player.itemAnimation = player.itemTime = 2;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.spriteDirection, Projectile.velocity.X * Projectile.spriteDirection);
            player.fullRotation = MathHelper.Lerp(player.fullRotation,-MathHelper.PiOver2 * 0.9f * player.direction,0.4f);
            player.fullRotationOrigin = player.Size * 0.5f;
            if (Projectile.ai[0]++ > 18)
            {
                if (Projectile.ai[0] > 25)
                {
                    SkillTimeOut = true;
                }
            }
            else
            {
                if ((int)Projectile.ai[0] == 1)
                {
                    Projectile.ai[1] = (player.velocity.X > 0).ToDirectionInt();
                }
                player.velocity.X = Projectile.ai[1] * 9;
                player.SetImmuneTimeForAllTypes(20);
                player.immuneAlpha = 0;
                
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.GetItemDrawFrame(BowsProj.SpawnItem.type, out var tex, out var rect);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override bool ActivationCondition()
        {
            return player.GetModPlayer<WeaponSkillPlayer>().Player_BowSidingStep && player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 40;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[0] > 18;
        }
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            if(BowsProj.ChannelLevel < 3)BowsProj.ChannelLevel++;
            player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 40;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            player.fullRotation = 0f;
            BowsProj.NoUse = true;
        }
    }
}
