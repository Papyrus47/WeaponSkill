using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Particles;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    /// <summary>
    /// 特殊纳刀
    /// </summary>
    public class LongSword_Naknotsu : BasicLongSwordSkill
    {
        public LongSword_Naknotsu(LongSwordProj longSword) : base(longSword)
        {
        }
        public override void AI()
        {
            Vector2 rotVector = (-Vector2.UnitX).RotatedBy(-0.3f);
            LongSword.CanChangeScabbardRot = true;
            PreAttack = true;
            bool flag = false;
            if ((int)Projectile.ai[0]++ > 16) // 收入完毕
            {
                PreAttack = false;
                swingHelper.Change(rotVector, Vector2.One);
                if (Projectile.ai[1]++ > 180)
                {
                    SkillTimeOut = true;
                }
                else if ((int)Projectile.ai[1] == 5)
                {
                    SpearsStar spearsStar = new(player, new Vector2(0.2f, 0.8f) * 3f)
                    {
                        ScaleVelocity = new Vector2(0.3f, 1) * -0.1f,
                        TimeLeft = 12
                    };
                    Main.ParticleSystem_World_OverPlayers.Add(spearsStar);
                }
            }
            else // 渐变收刀
            {
                swingHelper.Change_Lerp(rotVector, 0.2f, Vector2.One, 1f);
                flag = true;
            }
            player.velocity.X *= 0;
            player.ChangeDir(-Projectile.direction);
            player.GetModPlayer<WeaponSkillPlayer>().Naknotsu_Slash_OnHit = false;
            Projectile.spriteDirection = -player.direction;
            swingHelper.SetSwingActive();
            swingHelper.ProjFixedPos(player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, Projectile.height * 0.1f), -LongSword.SwingLength * 0.45f, true);
            swingHelper.SwingAI(LongSword.SwingLength, player.direction, 0);
            LongSword.swordScabbard.DrawPos = Projectile.Center;
            LongSword.swordScabbard.Dir = -player.direction;
            if (flag)
            {
                Projectile.Center -= Projectile.velocity * (0.45f + LongSword.TimeChange((1 - (Projectile.ai[0] / 16)) * 0.95f));
            }
            Projectile.numHits = 0;
            LongSword.swordScabbard.Rot += MathHelper.WrapAngle((rotVector.ToRotation() - 1.5f) * -player.direction - LongSword.swordScabbard.Rot) * 0.4f;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullNone);
            swingHelper.DrawSwingItem(lightColor);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);

            return false;
        }
        public override bool ActivationCondition() => WeaponSkill.RangeChange.Current;
        public override bool SwitchCondition() => true;
        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = 0;
            SkillTimeOut = false;
            Projectile.rotation = 0;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = Projectile.ai[1] = 0;
            Projectile.velocity = Vector2.UnitY.RotatedBy(0.15f);
        }
    }
}
