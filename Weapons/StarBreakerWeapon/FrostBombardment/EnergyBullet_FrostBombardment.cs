using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment
{
    public class EnergyBullet_FrostBombardment : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.Size = new(25);
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.ai[2] > 0) // 激光炮模式
            {
                Projectile.tileCollide = false;
                Projectile.localNPCHitCooldown = 0;
                Projectile.penetrate = -1;
                Projectile.timeLeft -= 20;
                Projectile.DamageType = DamageClass.Magic;
                Projectile.position -= Projectile.velocity;
                ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000000;
                Projectile.damage = (int)(Projectile.damage * 0.89f);
                for(float i = 0; i < 4000; i += 3)
                {
                    if (Collision.CanHit(Projectile.Center, 1, 1, Projectile.Center + Projectile.velocity.SafeNormalize(default) * i, 1, 1))
                    {
                        Projectile.ai[1] = i;
                    }
                    else
                        break;
                }
                return;
            }

            if (Projectile.ai[0] >= 10 && Projectile.ai[1] >= 0) // 蓄力等级
            {
                ProjectileID.Sets.DrawScreenCheckFluff[Type] = 480;
                Projectile.DamageType = DamageClass.Ranged;
                NPC npc = Main.npc[(int)Projectile.ai[1]];
                if (npc.active && npc.life > 0)
                    Projectile.timeLeft = 50;
                Projectile.velocity -= (Projectile.Center - (npc.Center + npc.velocity * 0.5f)) * 0.001f;
                if (Projectile.velocity.LengthSquared() > 900) Projectile.velocity *= 0.99f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[2] > 0) // 激光炮模式
            {
                return;
            }
            if (Projectile.ai[0] >= 10) // 蓄力等级
            {
                for (int i = -5; i <= 5; i++)
                {
                    Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity.RotatedBy(i * 0.1f), 961, Projectile.damage, 0f, Projectile.owner, 0f, 0.05f * (i + 6));
                    projectile.friendly = true;
                    projectile.hostile = false;
                    projectile.penetrate = -1;
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = -1;
                }
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[2] > 0) // 激光炮模式
            {
                float r = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(default) * Projectile.ai[1],20,ref r);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[2] > 0) // 激光炮模式
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
                Vector2 vel = Projectile.velocity.SafeNormalize(default);
                List<CustomVertexInfo> list = new();
                for(int i = 0; i < Projectile.ai[1];i += 16)
                {
                    float factor = MathHelper.SmoothStep(0, 1, i / 20f) * Projectile.scale;
                    if(Projectile.timeLeft < 200)
                    {
                        factor *= (Projectile.timeLeft) / 200f;
                    }
                    else if(Projectile.timeLeft > 400)
                    {
                        factor *= (600 - Projectile.timeLeft) / 200f;
                    }
                    float width = 30f * factor * Projectile.scale;

                    Vector2 rotVel = vel.RotatedBy(MathHelper.PiOver2) * width;
                    list.Add(new(Projectile.Center + vel * i + rotVel - Main.screenPosition, new Color(0.2f, 0.3f, 0.7f, 0) * 3f, new Vector3(i / Projectile.ai[1] + Projectile.timeLeft / 300f,0, 0)));
                    list.Add(new(Projectile.Center + vel * i - rotVel - Main.screenPosition, new Color(0.2f, 0.3f, 0.7f, 0) * 3f, new Vector3(i / Projectile.ai[1] + Projectile.timeLeft / 300f, 1, 0)));

                    //list.Add(new(Projectile.Center + vel * i + rotVel - Main.screenPosition, new Color(0.2f, 0.3f, 0.7f, 0) * 4f, new Vector3(0, 0, 0)));
                    //list.Add(new(Projectile.Center + vel * i - rotVel - Main.screenPosition, new Color(0.2f, 0.3f, 0.7f, 0) * 4f, new Vector3(0, 1, 0)));
                }
                GraphicsDevice gd = Main.instance.GraphicsDevice;
                gd.Textures[0] = TextureAssets.Extra[193].Value;
                gd.BlendState = BlendState.AlphaBlend;
                gd.SamplerStates[0] = SamplerState.LinearWrap;
                //var origin = gd.RasterizerState;
                //RasterizerState rasterizerState = new()
                //{
                //    CullMode = CullMode.None,
                //    FillMode = FillMode.WireFrame
                //};
                //gd.RasterizerState = rasterizerState;
                //List<CustomVertexInfo> drawList = TheUtility.GenerateTriangle(list);
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
                //gd.RasterizerState = origin;

                Texture2D light = TextureAssets.Extra[89].Value;
                Rectangle rect = new(0, 0, light.Width, light.Height);
                Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, rect, new Color(0.2f, 0.3f, 0.7f, 0) * 4f, Projectile.rotation + Projectile.timeLeft / 300f, rect.Size() * 0.5f, Projectile.scale * new Vector2(2f, 3.2f), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, rect, new Color(0.2f, 0.3f, 0.7f, 0) * 4f, Projectile.rotation + MathHelper.PiOver2 + Projectile.timeLeft / 300f, rect.Size() * 0.5f, Projectile.scale * new Vector2(2f, 3.2f), SpriteEffects.None, 0f);
                return false;
            }
            Texture2D tex = TextureAssets.Extra[197].Value;
            if (Projectile.ai[0] >= 10) // 冰柱
            {
                tex = TextureAssets.Extra[35].Value;
                Rectangle rect = new(0, 0, tex.Width, tex.Height / 3);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation - MathHelper.PiOver2, new Vector2(rect.Width * 0.5f, rect.Height - Projectile.width), Projectile.scale, SpriteEffects.None, 0f);
            }
            else // 光束
            {
                Rectangle rect = new(0, 0, tex.Width, tex.Height);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect, new Color(0.2f, 0.3f, 0.7f, 0) * 4f, Projectile.rotation, new Vector2(rect.Width - Projectile.width, rect.Height * 0.5f), Projectile.scale * new Vector2(0.5f, 0.2f), SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
