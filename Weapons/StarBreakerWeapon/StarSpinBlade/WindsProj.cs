using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade
{
    public class WindsProj : ModProjectile
    {
        /// <summary>
        /// 执行的AI
        /// </summary>
        public Action<Projectile> UseAI;
        public delegate void OnHit(NPC target, NPC.HitInfo hitInfo, int dmgDone);
        public OnHit onHit;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 200;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.Size = new(150);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 900;
            ProjectileID.Sets.TrailCacheLength[Type] = 200;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void AI()
        {
            UseAI?.Invoke(Projectile);
            Projectile.rotation = Projectile.velocity.ToRotation();
            #region 保存旧位置
            //if ((int)Projectile.ai[0] != 1) // 通过ai[0] != 1 控制保存位置
            //{
            //    ProjectileID.Sets.TrailingMode[Type] = 0;
            //}
            //else
            //{
            //    ProjectileID.Sets.TrailingMode[Type] = 1;
            //}
            #endregion
        }
        public override bool ShouldUpdatePosition() => (int)Projectile.ai[0] != 1;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projHitbox.Center(), projHitbox.Center() - Projectile.velocity * 10, 75,ref r);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            onHit?.Invoke(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, ModAsset.WindShader.Value, Main.Transform);

            var tex = TextureAssets.Projectile[Type].Value;
            for (int i = 1; i < Projectile.oldPos.Length; i += 2)
            {
                ModAsset.WindShader.Value.Parameters["Rot"].SetValue(i + Main.GlobalTimeWrappedHourly * 2);
                ModAsset.WindShader.Value.CurrentTechnique.Passes[0].Apply();
                sb.Draw(tex, Projectile.oldPos[i] + tex.Size() * 0.5f - Main.screenPosition, null, Color.MediumPurple with { A = 0 } * 0.2f, Projectile.rotation + MathF.Sin(i) * 1.2f, tex.Size() * 0.5f, new Vector2(0.8f, 1f) * Projectile.scale * (1 - (float)i / Projectile.oldPos.Length), SpriteEffects.None, 0f);
            }

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }
    }
}
