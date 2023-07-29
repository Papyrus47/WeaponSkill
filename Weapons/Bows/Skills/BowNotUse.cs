using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponSkill.Weapons.Bows.Skills
{
    public class BowNotUse : BasicBowsSkill
    {
        public BowNotUse(ModProjectile modProjectile) : base(modProjectile)
        {
        }
        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -Projectile.width * 0.75f, 0);
            Projectile.rotation = (MathHelper.Pi * 0.95f) * player.direction;
            Projectile.velocity = Vector2.UnitX * player.direction;
            Projectile.spriteDirection = player.direction;
            BowsProj.NoUse = true;
            BowsProj.ChannelLevel = 0;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            var tex = TextureAssets.Item[BowsProj.SpawnItem.type].Value;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f,new Vector2(0.6f * Projectile.scale,Projectile.scale * 1.2f), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

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
