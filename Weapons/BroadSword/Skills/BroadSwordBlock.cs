using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.BroadSword.Skills
{
    public class BroadSwordBlock : BasicBroadSwordSkill
    {
        public BroadSwordBlock(BroadSwordProj broadSwordProj) : base(broadSwordProj)
        {
        }
        public override void AI()
        {
            player.GetModPlayer<WeaponSkillPlayer>().InBlocking = true;
            Projectile.Center = Vector2.Lerp(player.Center + new Vector2(player.direction * 20, 0), Projectile.Center,0.3f);
            Projectile.rotation = MathHelper.Pi * 1.25f * player.direction;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.2f * player.direction);
            Projectile.ai[0]++;
            swingHelper.Change(Vector2.One, Vector2.One, 0.3f);
            if (Projectile.ai[0] > 90)
            {
                SkillTimeOut = true;
            }
            Projectile.spriteDirection = -player.direction;
        }
        public override bool SwitchCondition()
        {
            return player.GetModPlayer<WeaponSkillPlayer>().IsBlockAttack;
        }
        public override bool ActivationCondition()
        {
            return player.controlUseTile;
        }
        public override void OnSkillActive()
        {
            Projectile.ai[0] = 0;
        }
        public override void OnSkillDeactivate()
        {
            if (player.GetModPlayer<WeaponSkillPlayer>().InBlocking)
            {
                player.GetModPlayer<WeaponSkillPlayer>().InBlocking = false;
                player.GetModPlayer<WeaponSkillPlayer>().IsBlockAttack = false;
                Projectile.damage = (int)(Projectile.damage * 1.25f);
            }
            Projectile.ai[0] = 0;
            Projectile.rotation = 0;
            SkillTimeOut = false;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            var tex = TextureAssets.Item[broadSword.SpawnItem.type].Value;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, new Vector2(Projectile.scale * 0.7f,Projectile.scale), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            return false;
        }
    }
}
