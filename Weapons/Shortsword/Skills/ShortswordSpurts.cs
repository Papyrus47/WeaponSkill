using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.Shortsword.Skills
{
    public class ShortswordSpurts : BasicShortswordSkill
    {
        public ShortswordSpurts(ShortswordProj broadSwordProj) : base(broadSwordProj)
        {
            Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Spurts");
        }
        public Func<Vector2> GetSpurtDir;
        public Func<float> SpurtTimeMax;
        public Action SpurtAction;
        public SoundStyle Sound;
        public override void AI()
        {
            float Length = Projectile.width;
            float time = (Projectile.ai[0] / SpurtTimeMax.Invoke());
            if (time < 1f)
            {
                Projectile.ai[0]++;
            }
            else Projectile.ai[1]++;
            player.itemAnimation = player.itemTime = 2;
            player.heldProj = Projectile.whoAmI;
            if ((int)Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(Sound, Projectile.Center);
                Projectile.velocity = GetSpurtDir.Invoke();
                SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, Projectile.originalDamage / 2, Projectile.knockBack, Projectile.owner, Projectile.width * 3, Projectile.height * 2,Shortsword.DrawColorTex);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.spriteDirection = 1;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * Length * time * 0.5f;
            SpurtAction?.Invoke();
            if (Projectile.ai[1] > 15f)
            {
                SkillTimeOut = true;
                Projectile.ai[0] = 0;
            }
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public override void OnSkillActive()
        {
            SoundEngine.PlaySound(Shortsword.SpawnItem.UseSound, player.position);
            TheUtility.ResetProjHit(Projectile);
            Projectile.ai[0] = Projectile.ai[1] = 0;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            TheUtility.ResetProjHit(Projectile);
            Projectile.damage = Projectile.originalDamage;
            Projectile.ai[0] = Projectile.ai[1] = 0;
            SkillTimeOut = false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projHitbox.Center(), projHitbox.Center() + Projectile.velocity * Projectile.width * Projectile.scale, Projectile.height, ref r);
        }
        public override bool ActivationCondition()
        {
            return player.controlUseItem;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[1] > 0;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            var tex = TextureAssets.Item[Shortsword.SpawnItem.type].Value;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            return false;
        }
    }
}
