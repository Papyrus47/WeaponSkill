using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.ChargeBlade.Skills;
using static Humanizer.In;

namespace WeaponSkill.Weapons.ChargeBlade
{
    public class ChargeBladeShield
    {
        public int width;
        public int height;
        public Vector2 Size
        {
            get => new(width, height);
            set
            {
                width = (int)value.X;
                height = (int)value.Y;
            }
        }
        public enum KNLevelEnum : byte
        {
            Small = 1,
            Medium = 2,
            Big = 3
        }
        public ShieldSwingHelper swingHelper;
        public ChargeBladeProj chargeBladeProj;
        public Vector2 StartVel = Vector2.UnitX;
        public bool Fixed;
        public float VisualRotation;
        public float AxeRot;
        /// <summary>
        /// GP格挡攻击
        /// </summary>
        public bool GP;
        /// <summary>
        /// 正在防御
        /// </summary>
        public bool InDef;
        /// <summary>
        /// 防御成功攻击
        /// </summary>
        public bool DefSucceeded;
        /// <summary>
        /// GP成功
        /// </summary>
        public bool DefSucceeded_GP;
        /// <summary>
        /// 计时器
        /// </summary>
        public int Time = 0;
        /// <summary>
        /// 击退级别
        /// </summary>
        public KNLevelEnum KNLevel;
        //public int SPDir;
        public ChargeBladeShield(ChargeBladeProj chargeBladeProj, Asset<Texture2D> DrawShieldTex)
        {
            this.chargeBladeProj = chargeBladeProj;
            swingHelper = new ShieldSwingHelper(this, 13, DrawShieldTex);
            //width = DrawShieldTex.Width() * 1.5f;
            //height = DrawShieldTex.Height() * 1.5f;
            Size = DrawShieldTex.Size() * 0.7f * chargeBladeProj.Projectile.scale;
            swingHelper.center = chargeBladeProj.Projectile.Center;
        }

        public virtual void Update(Vector2 Pos,int Dir)
        {
            //swingHelper.center = Pos;
            swingHelper.Change_Lerp(StartVel,0.8f, Vector2.One,0.8f, VisualRotation,0.3f);
            StartVel = Vector2.UnitX;
            swingHelper.ChangeLerp = false;
            if ((swingHelper.center - Pos).LengthSquared() > 50 && !Fixed) Pos = Vector2.Lerp(swingHelper.center, Pos, 0.1f) + chargeBladeProj.Player.velocity;
            else if ((swingHelper.center - Pos).LengthSquared() <= 50 && !Fixed) Fixed = true;
            swingHelper.ProjFixedPos(Pos, 0, true);
            swingHelper.SetSwingActive();
            swingHelper.SPDir = Dir;
            swingHelper.Rot = MathHelper.Pi;
            float rot = MathHelper.PiOver4;
            if (AxeRotBool)
            {
                swingHelper.AxeRot = AxeRot;
                if(Dir == -1)
                {
                    swingHelper.AxeRot += 0.15f;
                }
                if (chargeBladeProj.chargeBladeGlobal.InAxe && chargeBladeProj.CurrentSkill is ChargeBlade_Axe_Swing swing)
                {
                    rot += swing.VisualRotation;
                    swingHelper.VisualRotation = MathHelper.Lerp(swingHelper.VisualRotation, swing.VisualRotation, 0.3f);
                    swingHelper.AxeRot += swing.VisualRotation * 0.5f;
                }
            }
            else
            {
                swingHelper.AxeRot = AxeRot + Main.GlobalTimeWrappedHourly * 15; 
                if (chargeBladeProj.chargeBladeGlobal.InAxe && chargeBladeProj.CurrentSkill is ChargeBlade_Axe_Swing swing)
                {
                    rot += swing.VisualRotation;
                    swingHelper.VisualRotation = MathHelper.Lerp(swingHelper.VisualRotation, swing.VisualRotation, 0.3f);
                }
            }

            swingHelper.SwingAI(Size.Length(), -Dir, rot);

            //for(int i = 0;i)
        }
        /// <summary>
        /// 防御攻击者的攻击
        /// </summary>
        public virtual void CheckProjHitMe()
        {
            Player player = chargeBladeProj.Player;
            if(player == null || !InDef) return;
            InDef = false;
            if(Time > 0)
            {
                Time--;
            }
            else
            {
                DefSucceeded = false;
                DefSucceeded_GP = false;
            }
            float maxReduction = chargeBladeProj.shieldData.MaxReduction;
            bool flag = false; // 用于GP成功的判定
            if (GP)
            {
                maxReduction *= 0.7f;
                flag = true;
            }
            GP = false;
            if (chargeBladeProj.chargeBladeGlobal.ShieldStrengthening > 0)
            {
                maxReduction *= 0.7f;
            }
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && projectile.hostile && !player.immune && swingHelper.GetColliding(projectile.Hitbox) && projectile.damage > 0 && projectile.ModProjectile is not ChargeBladeProj)
                {
                    double damage = player.Hurt(PlayerDeathReason.ByProjectile(player.whoAmI, projectile.whoAmI),(int)(Main.DamageVar(projectile.damage, 0f - player.luck) * maxReduction), -player.direction,false,false,-1,false, projectile.ArmorPenetration);
                    if(damage < chargeBladeProj.shieldData.MaxDmg * 0.4f)
                    {
                        KNLevel = KNLevelEnum.Small; // 小退
                    }
                    else if(damage < chargeBladeProj.shieldData.MaxDmg)
                    {
                        KNLevel = KNLevelEnum.Medium;
                    }
                    else
                    {
                        KNLevel = KNLevelEnum.Big;
                    }
                    DefSucceeded = true;
                    chargeBladeProj.chargeBladeGlobal.StatCharge += 0.5f;
                    Time = 10;
                    if (flag)
                    {
                        chargeBladeProj.chargeBladeGlobal.StatCharge += 1.5f;
                        DefSucceeded_GP = true;
                    }
                    player.SetItemTime(50);
                    break;
                }
            }
            if (DefSucceeded) return;
            foreach(NPC npc in Main.npc)
            {
                if(npc.active && !npc.friendly && !player.immune && swingHelper.GetColliding(npc.Hitbox) && npc.damage > 0)
                {
                    double damage = player.Hurt(PlayerDeathReason.ByNPC(npc.whoAmI),(int)(Main.DamageVar(npc.damage, 0f - player.luck) * maxReduction), -player.direction, false, false, -1, false);
                    if (damage < chargeBladeProj.shieldData.MaxDmg * 0.4f)
                    {
                        KNLevel = KNLevelEnum.Small; // 小退
                    }
                    else if (damage < chargeBladeProj.shieldData.MaxDmg * 0.8f)
                    {
                        KNLevel = KNLevelEnum.Medium;
                    }
                    else
                    {
                        KNLevel = KNLevelEnum.Big;
                    }
                    DefSucceeded = true;
                    chargeBladeProj.chargeBladeGlobal.StatCharge += 0.5f;
                    Time = 10;
                    if (flag)
                    {
                        chargeBladeProj.chargeBladeGlobal.StatCharge += 1.5f;
                        DefSucceeded_GP = true;
                    }
                    player.SetItemTime(50);
                    break;
                }
            }
        }
        public virtual void Draw(SpriteBatch sb, Color color)
        {

            if (AxeRotBool)
            {
                bool flag = false;
                if (TheUtility.InBegin())
                {
                    flag = true;
                    sb.End();
                }
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);

                swingHelper.DrawSwingItem(color);

                sb.End();
                if (flag) sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.Transform);

            }
            else
            {
                bool flag = false;
                if (TheUtility.InBegin())
                {
                    flag = true;
                    sb.End();
                }
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);

                //swingHelper.Swing_Draw_ItemAndTrailling(color, ModContent.Request<Texture2D>("WeaponSkill/Images/SwingTex_Offset").Value, (_) => new(255, 245, 134, 0));
                swingHelper.DrawSwingItem(color);

                List<CustomVertexInfo> customVertexInfos = new List<CustomVertexInfo>();
                const int Max = 30;
                for (int i = 0; i < Max; i++)
                {
                    float factor = (float)i / Max;
                    color = Color.Lerp(Color.Red, Color.Orange, factor);
                    color.A = 0;
                    Vector2 vector2 = (AxeRot * factor + factor * MathHelper.Pi * 1.5f).ToRotationVector2().RotatedBy(Main.GlobalTimeWrappedHourly * Max) * Size.Length() * 0.5f;
                    vector2.Y *= 1 - swingHelper.VisualRotation;
                    vector2.RotatedBy(chargeBladeProj.Projectile.rotation);
                    customVertexInfos.Add(new(swingHelper.center - Main.screenPosition + vector2.RotatedBy(swingHelper.SPDir * VisualRotation), color, new Vector3(factor, 0, 0)));
                    customVertexInfos.Add(new(swingHelper.center - Main.screenPosition - vector2.RotatedBy(swingHelper.SPDir * VisualRotation * 1.5f), color, new Vector3(factor, 4f, 0)));
                }
                if (customVertexInfos.Count > 0)
                {
                    List<CustomVertexInfo> infos = TheUtility.GenerateTriangle(customVertexInfos);

                    //var origin = Main.graphics.GraphicsDevice.RasterizerState;
                    //RasterizerState rasterizerState = new()
                    //{
                    //    CullMode = CullMode.None,
                    //    FillMode = FillMode.WireFrame
                    //};
                    //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

                    Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("WeaponSkill/Images/SwingTex_Offset").Value;
                    //Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, infos.ToArray(), 0, infos.Count / 3);

                    //Main.graphics.GraphicsDevice.RasterizerState = origin;
                }
                sb.End();
                if (flag) sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.Transform);
            }
        }

        protected virtual bool AxeRotBool => (!chargeBladeProj.chargeBladeGlobal.InAxe || !chargeBladeProj.chargeBladeGlobal.AxeStrengthening || chargeBladeProj.CurrentSkill is ChargeBlade_Axe_Held) && (chargeBladeProj.CurrentSkill is not ChargeBlade_Axe_Swing_Liberate_Super || (chargeBladeProj.CurrentSkill is ChargeBlade_Axe_Swing_Liberate_Super liberate_Super && liberate_Super.End));
    }
}
