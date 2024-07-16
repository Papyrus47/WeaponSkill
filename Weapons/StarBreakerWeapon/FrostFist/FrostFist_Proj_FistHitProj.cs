using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.LongSword;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills;
using WeaponSkill.Weapons.StarBreakerWeapon.General;
using WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist
{
    public class FrostFist_Proj_FistHitProj : ModProjectile
    {
        public static List<int> FistHitProjs = new List<int>();
        public FrostFist_FistHit frostFist_FistHit;
        public override void Load()
        {
            FistHitProjs = new();
        }
        public override void Unload()
        {
            FistHitProjs = null;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 6;
            Projectile.aiStyle = -1;
            Projectile.Size = new(40);
            Projectile.ownerHitCheck = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is FrostFist_FistHit.FrostFistHit_ProjEntitySource proj)
            {
                frostFist_FistHit = proj.frostFist_FistHit;
            }
            SoundEngine.PlaySound(SoundID.Item19 with { Pitch = -0.5f, PitchVariance = 0.3f }, Projectile.Center);
        }
        public override void AI()
        {
            if (!FistHitProjs.Contains(Projectile.whoAmI)) FistHitProjs.Add(Projectile.whoAmI);
            Projectile.ai[0] += MathF.Pow(Projectile.timeLeft / 30f, 2);
            Projectile.Center = Main.player[Projectile.owner].Center + Projectile.velocity.SafeNormalize(default) * Projectile.ai[0];
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue, 1.6f);
                dust.position = Projectile.Center;
                //dust.velocity *= 0;
                dust.noGravity = true;
                dust.fadeIn = 1;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            #region 属性伤害
            IceElementDamage iceElementDamage = new();
            iceElementDamage.baseDamage = Projectile.damage;
            iceElementDamage.statModifier += 4;
            Main.player[Projectile.owner].addDPS((int)ElementDamageSystem.Instance.ElementDamageApply(iceElementDamage, target));
            #endregion
            HitDamage.AddDamage(Projectile, target, hit);
            TheUtility.SetPlayerImmune(Main.player[Projectile.owner], 8);
            if (frostFist_FistHit is not FrostFist_SpeedAtk_FastetHit) Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.velocity.SafeNormalize(default), 3, 0.1f, 1, -1));
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(target.Center, 1, 1, DustID.FrostStaff, 0, 0, 200, Color.AliceBlue);
                dust.position = target.Center;
                dust.scale = 0.3f;
                Vector2 vel = Projectile.velocity.RotatedBy(i / 80f * MathHelper.TwoPi).SafeNormalize(default) * 3.5f;
                vel.X *= 0.2f;
                vel = vel.RotatedBy(Projectile.velocity.ToRotation());
                dust.velocity = vel;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                //dust.velocity += Projectile.velocity.RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f) * 1.2f;
            }
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(target.Center, 1, 1, DustID.FrostStaff,0, 0, 200, Color.AliceBlue, 1.3f);
                dust.position = target.Center;
                dust.velocity *= 0;
                dust.noGravity = true;
                dust.fadeIn = 1;
                dust.velocity += Projectile.velocity.RotatedByRandom(0.4).SafeNormalize(default) * Main.rand.NextFloat(0.2f,1f) * 10f;
            }
            frostFist_FistHit?.OnHit?.Invoke(target,hit,damageDone);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            modifiers.FinalDamage *= 1.2f;
            HitDamage.HitDamageHit(true);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!WeaponSkill.RenderTargetShaderSystem.RenderDraw.Any(x => x is FrostFist_Proj_FistHitProj_Render))
            {
                WeaponSkill.RenderTargetShaderSystem.RenderDraw.Add(new FrostFist_Proj_FistHitProj_Render());
            }
            return false;
        }
        public virtual void FistHitProjDraw_Offset(Color color)
        {
            //Main.instance.DrawProj(Projectile.whoAmI);
            //Texture2D tex = TextureAssets.Projectile[Type].Value;
            //Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null,color, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale + 0.2f, SpriteEffects.None, 0f);
            List<CustomVertexInfo> customVertexInfos = new();
            Vector2 Vel = new Vector2(Projectile.velocity.Y, -Projectile.velocity.X).SafeNormalize(default) * 32f;
            int maxLenght = Projectile.oldPos.Length;
            for (int i = 0; i < maxLenght; i++)
            {
                Vector2 pos = Projectile.Center - Projectile.velocity * i * 0.7f;
                float factor = 1f - (float)i / maxLenght;
                color *= factor;
                customVertexInfos.Add(new(pos + Vel + Projectile.velocity * 0.3f - Main.screenPosition, color, new Vector3(factor, 0, 0)));
                customVertexInfos.Add(new(pos - Vel + Projectile.velocity * 0.3f - Main.screenPosition, color, new Vector3(factor, 1, 0)));
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
        public virtual void FistHitProjDraw_Proj()
        {
            //Main.instance.DrawProj(Projectile.whoAmI);
            //Texture2D tex = TextureAssets.Extra[91].Value;
            ////Texture2D tex = TextureAssets.Projectile[Type].Value;
            //Color color = Color.SkyBlue;
            //color.A = 0;
            //color *= 0.6f;
            //for (int i = 0; i < 2; i++)
            //{
            //    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation + MathHelper.PiOver2, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            //}
            List<CustomVertexInfo> customVertexInfos = new();
            Vector2 Vel = new Vector2(Projectile.velocity.Y, -Projectile.velocity.X).SafeNormalize(default) * 20f;
            int maxLenght = Projectile.oldPos.Length;
            Color color = Color.SkyBlue * 0.8f;
            color.A = 0;
            color *= Projectile.Opacity;
            for (int i = 0; i < maxLenght; i++)
            {
                Vector2 pos = Projectile.Center - Projectile.velocity * i * 1.4f;

                //if (pos == default) break;
                float factor = 1f - (float)i / maxLenght;
                customVertexInfos.Add(new(pos + Vel - Main.screenPosition, color, new Vector3(MathF.Pow(factor, 1.5f), 0, 1)));
                customVertexInfos.Add(new(pos - Vel - Main.screenPosition, color, new Vector3(MathF.Pow(factor, 1.5f), 1, 1)));
            }
            if(customVertexInfos.Count > 6)
            {
                List<CustomVertexInfo> drawVertex = TheUtility.GenerateTriangle(customVertexInfos);
                GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
                graphicsDevice.Textures[0] = TextureAssets.Projectile[Type].Value;
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, drawVertex.ToArray(), 0, drawVertex.Count / 3);
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, drawVertex.ToArray(), 0, drawVertex.Count / 3);
            }
        }
    }
}
