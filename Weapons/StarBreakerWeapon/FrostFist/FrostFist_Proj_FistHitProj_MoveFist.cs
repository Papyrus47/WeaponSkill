using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist
{
    public class FrostFist_Proj_FistHitProj_MoveFist : FrostFist_Proj_FistHitProj
    {
        public override string Texture => (GetType().Namespace + "/FrostFist_Proj_FistHitProj").Replace('.','/');
        public Action<Projectile> MoveFistAI;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public Vector2[] oldVels;
        public override void AI()
        {
            if (!FistHitProjs.Contains(Projectile.whoAmI)) FistHitProjs.Add(Projectile.whoAmI);
            Projectile.ai[0]++;
            //Projectile.Center = Main.player[Projectile.owner].Center + Projectile.velocity.SafeNormalize(default) * Projectile.ai[0];
            Projectile.rotation = Projectile.velocity.ToRotation();
            if ((int)Projectile.ai[0] % 3 == 0)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue, 1.6f);
                dust.position = Projectile.Center;
                //dust.velocity *= 0;
                dust.noGravity = true;
                dust.fadeIn = 1;
            }

            oldVels ??= new Vector2[Projectile.oldPos.Length];
            MoveFistAI?.Invoke(Projectile);

            for (int i = oldVels.Length - 1; i > 0; i--)
            {
                oldVels[i] = oldVels[i - 1];
            }
            oldVels[0] = Projectile.velocity;
            //if(Projectile.timeLeft < 2)
            //{
            //    Projectile.ai[0] = 0;
            //    Projectile.timeLeft = 60;
            //}
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 0.4f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!WeaponSkill.RenderTargetShaderSystem.RenderDraw.Any(x => x is FrostFist_Proj_FistHitProj_Render))
            {
                WeaponSkill.RenderTargetShaderSystem.RenderDraw.Add(new FrostFist_Proj_FistHitProj_Render());
            }
            return false;
        }
        public override void FistHitProjDraw_Offset(Color color)
        {
            //Main.instance.DrawProj(Projectile.whoAmI);
            //Texture2D tex = TextureAssets.Projectile[Type].Value;
            //Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null,color, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale + 0.2f, SpriteEffects.None, 0f);
            List<CustomVertexInfo> customVertexInfos = new();
            if (oldVels == null) return;
            int maxLenght = oldVels.Length;
            for (int i = 0; i < maxLenght; i++)
            {
                Vector2 pos = Projectile.oldPos[i];
                Vector2 Vel = new Vector2(oldVels[i].Y, -oldVels[i].X).SafeNormalize(default) * 42f;
                if (pos == default || Vel == default) break;
                float factor = 1f - (float)i / maxLenght;
                color *= factor;
                customVertexInfos.Add(new(pos + Vel - Main.screenPosition, color, new Vector3(factor, 0, 0)));
                customVertexInfos.Add(new(pos - Main.screenPosition, color, new Vector3(factor, 1, 0)));
            }
            if (customVertexInfos.Count > 6)
            {
                List<CustomVertexInfo> drawVertex = TheUtility.GenerateTriangle(customVertexInfos);
                GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
                graphicsDevice.Textures[0] = TextureAssets.Projectile[Type].Value;
                //graphicsDevice.Textures[0] = TextureAssets.Extra[193].Value;
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, drawVertex.ToArray(), 0, drawVertex.Count / 3);
            }
        }
        public override void FistHitProjDraw_Proj()
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            //var origin = gd.RasterizerState;
            //RasterizerState rasterizerState = new()
            //{
            //    CullMode = CullMode.None,
            //    FillMode = FillMode.WireFrame
            //};
            //gd.RasterizerState = rasterizerState;
            List<CustomVertexInfo> customVertexInfos = new();
            if (oldVels == null) return;
            int maxLenght = Projectile.oldPos.Length;
            Color color = Color.SkyBlue * 0.8f;
            color.A = 0;
            color *= Projectile.Opacity;
            for (int i = 0; i < maxLenght; i++)
            {
                Vector2 pos = Projectile.oldPos[i];
                Vector2 Vel = new Vector2(oldVels[i].Y, -oldVels[i].X).SafeNormalize(default) * 30f;
                if (pos == default || Vel == default) break;
                float factor = 1f - (float)i / maxLenght;
                customVertexInfos.Add(new(pos + Vel - Main.screenPosition, color, new Vector3(MathF.Pow(factor, 0.7f), 0, 1)));
                customVertexInfos.Add(new(pos - Vel - Main.screenPosition, color, new Vector3(MathF.Pow(factor, 0.7f), 1, 1)));
            }
            if(customVertexInfos.Count > 20)
            {
                List<CustomVertexInfo> drawVertex = TheUtility.GenerateTriangle(customVertexInfos);

                gd.Textures[0] = TextureAssets.Projectile[Type].Value;
                //gd.Textures[0] = TextureAssets.MagicPixel.Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, drawVertex.ToArray(), 0, drawVertex.Count / 3);
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, drawVertex.ToArray(), 0, drawVertex.Count / 3);
            }

            //gd.RasterizerState = origin;
        }
    }
}
