using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponSkill.Weapons.Bows;

namespace WeaponSkill.Weapons.Crossbow.Skills
{
    public class CrossbowNotUse : BasicCrossbowSkill
    {
        public CrossbowNotUse(ModProjectile modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -Projectile.width * 0.25f, 0);
            Projectile.rotation = MathHelper.PiOver2 * player.direction;
            CrossProj.globalItem.CosumeAmmo = false;
            //Projectile.velocity = Vector2.UnitX * player.direction;
            Projectile.spriteDirection = player.direction;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.GetItemDrawFrame(CrossProj.SpawnItem.type, out var tex, out var rect);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            return false;
        }
        public override bool SwitchCondition()
        {
            return player.controlUseTile || player.controlUseItem;
        }
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.rotation = 0;
        }
    }
}
