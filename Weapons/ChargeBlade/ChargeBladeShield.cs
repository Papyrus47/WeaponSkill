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
            swingHelper.Change_Lerp(StartVel,0.8f, Vector2.One,0.8f, VisualRotation,1);
            swingHelper.ChangeLerp = false;
            if ((swingHelper.center - Pos).LengthSquared() > 50 && !Fixed) Pos = Vector2.Lerp(swingHelper.center, Pos, 0.1f) + chargeBladeProj.Player.velocity;
            else if ((swingHelper.center - Pos).LengthSquared() <= 50 && !Fixed) Fixed = true;
            swingHelper.ProjFixedPos(Pos, 0, true);
            swingHelper.SetSwingActive();
            swingHelper.SPDir = Dir;
            swingHelper.Rot = MathHelper.Pi;
            float rot = MathHelper.PiOver4;
            if (!chargeBladeProj.chargeBladeGlobal.InShieldStreng_InAxe)
            {
                swingHelper.AxeRot = AxeRot;
                if(Dir == -1)
                {
                    swingHelper.AxeRot += 0.15f;
                }
            }
            else
            {
                swingHelper.AxeRot = Main.GlobalTimeWrappedHourly * 10;
            }

            swingHelper.SwingAI(Size.Length(), -Dir, rot);
        }
        /// <summary>
        /// 防御攻击者的攻击
        /// </summary>
        public virtual void CheckProjHitMe()
        {
            Player player = chargeBladeProj.Player;
            if(player == null || !InDef) return;
            InDef = false;
            DefSucceeded = false;
            float maxReduction = chargeBladeProj.shieldData.MaxReduction;
            if (GP)
            {
                maxReduction *= 0.7f;
            }
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
                    player.SetItemTime(10);
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
                    player.SetItemTime(10);
                    break;
                }
            }
        }
        public virtual void Draw(SpriteBatch sb, Color color)
        {
            if (!chargeBladeProj.chargeBladeGlobal.InAxe)
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
                if(flag) sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                    Main.Rasterizer, null, Main.Transform);

            }
            else
            {

            }
        }
    }
}
