using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Crossbow.Skills
{
    public class CrossbowHeld : BasicCrossbowSkill
    {
        public CrossbowHeld(ModProjectile modProjectile) : base(modProjectile)
        {
        }
        public const int HELDTIME = 20;
        public override void AI()
        {
            if (Projectile.ai[0]++ < HELDTIME)
            {
                Projectile.velocity = (Projectile.velocity * 10f + (Main.MouseWorld - player.Center).SafeNormalize(default) * 10f) / 11f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.SafeNormalize(default) * Projectile.width * 0.5f, Projectile.ai[0] / HELDTIME);
            }
            else
            {
                Projectile.velocity = (Main.MouseWorld - player.Center).SafeNormalize(default);
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * Projectile.width * 0.5f;
                if (Projectile.ai[0] > 90)
                {
                    SkillTimeOut = true;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);
            CrossProj.globalItem.CosumeAmmo = false;
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            player.ChangeDir(Projectile.spriteDirection);
            player.heldProj = Projectile.whoAmI;
            player.itemAnimation = player.itemTime = 2;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.spriteDirection, Projectile.velocity.X * Projectile.spriteDirection);
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.GetItemDrawFrame(CrossProj.SpawnItem.type, out var tex, out var rect);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => true;
        public override bool SwitchCondition()
        {
            return (player.controlUseTile || player.controlUseItem) && Projectile.ai[0] > HELDTIME;
        }
        public override void OnSkillActive()
        {
            if (CrossProj.OldSkills.Count == 1) Projectile.ai[0] = 0;
            else Projectile.ai[0] = HELDTIME / 2.5f;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = 0;
        }
    }
}
