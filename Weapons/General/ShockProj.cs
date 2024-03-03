using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.General
{
    public class ShockProj : ModProjectile
    {
        public static List<int> ShockProjIndex = new List<int>();
        public override void Load()
        {
            ShockProjIndex = new();
        }
        public override void Unload()
        {
            ShockProjIndex = null;
        }
        public override string Texture => "Terraria/Images/Extra_193";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Size = new(1);
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0) Projectile.ai[0] = Projectile.timeLeft;
            if (!ShockProjIndex.Contains(Projectile.whoAmI)) ShockProjIndex.Add(Projectile.whoAmI);
            Projectile.velocity = Projectile.velocity.SafeNormalize(default) * (Projectile.velocity.Length() + 2f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!WeaponSkill.RenderTargetShaderSystem.RenderDraw.Any(x => x is ShockProj_RenderDraw))
            {
                WeaponSkill.RenderTargetShaderSystem.RenderDraw.Add(new ShockProj_RenderDraw());
            }
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Projectile.velocity * 1.5f, Projectile.Center + Projectile.velocity * 1.5f, 10, ref r);
        }
        public virtual void DrawShock()
        {
            List<CustomVertexInfo> customVertexInfos = new();

            for(int i = 0; i < 40; i++)
            {
                float factor = i / 40f;
                Vector2 vel = Vector2.One.RotatedBy(factor * MathHelper.TwoPi) * Projectile.velocity.Length() * 1f;
                vel.Y *= 0.1f;
                Vector2 vel2 = vel * (2f - (1f - Projectile.timeLeft / Projectile.ai[0]));

                customVertexInfos.Add(new(Projectile.Center + vel - Main.screenPosition, ShockProj_RenderDraw.GetDrawOffsetColor(vel, 0.4f), new(factor, 0, 0)));
                customVertexInfos.Add(new(Projectile.Center + vel2 - Main.screenPosition, ShockProj_RenderDraw.GetDrawOffsetColor(vel, 0.4f), new(factor, 1, 0)));
            }
            List<CustomVertexInfo> drawVertex = TheUtility.GenerateTriangle(customVertexInfos);
            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            graphicsDevice.Textures[0] = TextureAssets.Projectile[Type].Value;
            //graphicsDevice.Textures[0] = TextureAssets.Extra[193].Value;
            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, drawVertex.ToArray(), 0, drawVertex.Count / 3);
        }
    }
}
