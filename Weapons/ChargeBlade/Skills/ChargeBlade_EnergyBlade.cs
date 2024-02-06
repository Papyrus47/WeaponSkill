using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_EnergyBlade : BasicChargeBladeSkill
    {
        public ChargeBlade_EnergyBlade(ChargeBladeProj chargeBlade) : base(chargeBlade)
        {
        }
        public int UseBottle;
        public override void AI()
        {
            base.AI();
            player.velocity.X *= 0f;
            if (Projectile.ai[0] <= 60)
            {
                Vector2 rotVector = (-Vector2.UnitX).RotatedBy(0.2f).RotatedBy(Projectile.ai[0] * 0.001);

                //bool flag = false;
                swingHelper.Change(rotVector, Vector2.One, 0f);
                if ((int)Projectile.ai[0]++ > 90) // 收入完毕
                {
                    Projectile.ai[0] = 0;
                    //SkillTimeOut = true;
                }

                //if ((int)Projectile.ai[0] > 14) // 可以切换技能
                //{
                //    PreAttack = false;
                //}
                if (Projectile.numHits > 0 && Projectile.ai[2] < 30)
                {
                    Projectile.ai[0] -= 0.9f;
                    Projectile.numHits--;
                    Projectile.ai[2]++;
                }
                //SkillTimeOut = false;
            }
            else
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 30)
                {
                    SkillTimeOut = true;
                }
                else if ((int)Projectile.ai[1] % 2 == 0)
                {
                    Vector2 center = Projectile.Center;
                    for (int i = 0; i < 3; i++)
                    {
                        Dust dust = Dust.NewDustDirect(center, 3, 3, DustID.Smoke);
                        dust.velocity = Projectile.velocity.RotatedByRandom(0.2) * -0.05f * Main.rand.NextFloat();
                        dust.color = Color.White;
                        dust.alpha = 150;
                        dust.scale = 1.5f;

                        //dust = Dust.NewDustDirect(center, 3, 3, DustID.Smoke);
                        //dust.velocity = Projectile.velocity.RotatedByRandom(0.2) * -0.05f * Main.rand.NextFloat();
                        //dust.color = Color.White;
                        //dust.alpha = 150;
                        //dust.scale = 1.5f;

                        //dust = Dust.NewDustDirect(Projectile.Center, 5, 5, DustID.Smoke);
                        //dust.velocity = Projectile.velocity.RotatedByRandom(0.1) * -0.05f;
                        //dust.color = Color.Red;
                    }
                }
            }
            Projectile.spriteDirection = player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.SafeNormalize(default) * (player.height * -0.1f + Math.Min(Projectile.ai[0] - 4, 15) * 2), -ChargeBladeProj.SwingLength * 0.56f, true);
            float rot = Math.Max(Projectile.ai[0] - 30, 0) / 30;
            rot = MathHelper.SmoothStep(0, 2f, rot) * -MathHelper.Pi * 0.74f;
            //rot = ChargeBladeProj.TimeChange(rot) * -MathHelper.TwoPi * 0.74f;
            swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, rot);
            //if (flag)
            //{
            //    Projectile.Center -= Projectile.velocity * 0.45f;
            //}
            #region 盾的更新
            ChargeBladeShield shield = ChargeBladeProj.shield;
            shield.Update(player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.SafeNormalize(default) * (player.height * -0.1f + Math.Min(Projectile.ai[0] - 4, 15) * 2), -player.direction);
            shield.VisualRotation = 0.1f;
            shield.AxeRot = -1.57f - rot;
            ChargeBladeProj.shieldCanDraw = false;
            #endregion
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage += UseBottle;
            Projectile.numHits += 2;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
            swingHelper.DrawSwingItem(lightColor);
            ChargeBladeProj.shield.Draw(sb, lightColor);

            #region 绘制光剑
            int drawConst = 20 * UseBottle;
            if (drawConst < 20) return false;
            List<CustomVertexInfo> vertexInfos = new();
            Vector2 vel = Projectile.velocity.SafeNormalize(default);
            Vector2 center = Projectile.Center + vel;
            Color color = Color.Lerp(Color.Orange,Color.White,0.5f);
            float width = ChargeBladeProj.shield.width / 2;
            color.A = 0;
            if (Projectile.ai[0] <= 30) drawConst -= (int)((30 - Projectile.ai[0]) / 3) * 20;
            else if (Projectile.ai[0] > 60) drawConst -= (int)(Projectile.ai[1] / 3) * 20;
            Vector2 oldCenter = center;
            for (int i = 0; i < drawConst; i++)
            {
                center += vel * 2;
                float factor = (float)i / drawConst;
                float drawWidth = width;
                if(i > drawConst - 20)
                {
                    drawWidth *= (drawConst - i) / 20f;
                }
                vertexInfos.Add(new(center + (new Vector2(-vel.Y, vel.X) * drawWidth) - Main.screenPosition, color, new Vector3(factor, 0.2f, 0)));
                vertexInfos.Add(new(center - (new Vector2(-vel.Y, vel.X) * drawWidth) - Main.screenPosition, color, new Vector3(factor, 0.8f, 0)));

                //vertexInfos.Add(new(center + (new Vector2(-vel.Y, vel.X) * drawWidth) - Main.screenPosition, color, new Vector3(factor, 0.39f, 0)));
                //vertexInfos.Add(new(center - (new Vector2(-vel.Y, vel.X) * drawWidth) - Main.screenPosition, color, new Vector3(factor, 0.61f, 0)));
            }
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            gd.Textures[0] = TextureAssets.Extra[197].Value;
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertexInfos.ToArray(), 0, vertexInfos.Count - 2);

            center = oldCenter;
            for (int i = 0; i < drawConst; i++)
            {
                center += vel * 2;
                float factor = (float)i / drawConst;
                float drawWidth = width;
                if (i > drawConst - 20)
                {
                    drawWidth *= (drawConst - i) / 20f;
                }
                //vertexInfos.Add(new(center + (new Vector2(-vel.Y, vel.X) * drawWidth) - Main.screenPosition, color, new Vector3(factor, 0.2f, 0)));
                //vertexInfos.Add(new(center - (new Vector2(-vel.Y, vel.X) * drawWidth) - Main.screenPosition, color, new Vector3(factor, 0.8f, 0)));

                vertexInfos.Add(new(center + (new Vector2(-vel.Y, vel.X) * drawWidth) - Main.screenPosition, color, new Vector3(factor, 0.3f, 0)));
                vertexInfos.Add(new(center - (new Vector2(-vel.Y, vel.X) * drawWidth) - Main.screenPosition, color, new Vector3(factor, 0.7f, 0)));
            }
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertexInfos.ToArray(), 0, vertexInfos.Count - 2);
            #region 光剑拖尾
            if (Projectile.ai[0] <= 60)
            {
                Vector2[] oldVels = swingHelper.oldVels;
                center = Projectile.Center;
                List<CustomVertexInfo> TrailVertex = new();
                for (int i = 0; i < oldVels.Length; i++)
                {
                    float factor = (float)i / oldVels.Length;
                    TrailVertex.Add(new(center - Main.screenPosition, color, new Vector3(factor, 0, 0)));
                    TrailVertex.Add(new(center + oldVels[i].SafeNormalize(default) * drawConst * 2 - Main.screenPosition, color, new Vector3(factor, 1, 0)));
                }
                List<CustomVertexInfo> DrawTrail = TheUtility.GenerateTriangle(TrailVertex);
                gd.Textures[0] = TextureAssets.Extra[201].Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, DrawTrail.ToArray(), 0, DrawTrail.Count / 3);
            }
            #endregion
            #endregion

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            Texture2D tex = TextureAssets.Extra[89].Value;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null,color, Math.Max(Projectile.ai[0] - 30, 0) / 80 * MathHelper.TwoPi * -Projectile.direction, tex.Size() * 0.5f, Projectile.scale * 1.4f, SpriteEffects.None, 0f);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null,color, Math.Max(Projectile.ai[0] - 30, 0) / 80 * MathHelper.TwoPi * -Projectile.direction + MathHelper.PiOver2, tex.Size() * 0.5f, Projectile.scale * 1.4f, SpriteEffects.None, 0f);
            return false;
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (ChargeBladeProj.chargeBladeGlobal.InShieldStreng_InAxe && ChargeBladeProj.chargeBladeGlobal.BottleLimitRemovalTime > 0) // 盾强化下
            {
                return (nowSkill as BasicChargeBladeSkill).PreAttack;
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override bool ActivationCondition() => false;
        public override bool? CanDamage() => Projectile.ai[0] > 30;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = 0;
            SkillTimeOut = false;
            Projectile.rotation = 0;
            UseBottle = ChargeBladeProj.chargeBladeGlobal.StatChargeBottle;
            //UseBottle = 10;
            ChargeBladeProj.chargeBladeGlobal.StatChargeBottle = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;
            UseBottle = 0;
            //Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
        }
        public override bool? Colliding(Rectangle projhitbox, Rectangle targethitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targethitbox.TopLeft(),targethitbox.Size(),Projectile.Center,Projectile.Center + Projectile.velocity.SafeNormalize(default) * UseBottle * 40,
                ChargeBladeProj.shield.width,ref r);
        }
    }
}
