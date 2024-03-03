using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;

namespace WeaponSkill.Weapons.Hammer.Skills
{
    /// <summary>
    /// 蓄力锤
    /// </summary>
    public class HammerSwing_Channel : HammerSwing
    {
        public HammerSwing_Channel(HammerProj hammerProj, Func<bool> changeCondition) : base(hammerProj, changeCondition)
        {
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 0)
            {
                if (ActivationCondition())
                {
                    Projectile.ai[2]++;
                    if (Projectile.ai[2] > 90)
                    {
                        Projectile.ai[2] = 0;
                        if(hammerProj.ChannelLevel < 2) hammerProj.ChannelLevel++;
                    }
                    if(Projectile.ai[1] > 13) Projectile.ai[1] = 13;
                }
            }
            else if ((int)Projectile.ai[0] == 1) Projectile.ai[2] = 0;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            //swingHelper.DrawTrailing(TextureAssets.Extra[209].Value, (_) => new Color(1f, 1f, 1f, 0), null, null);
            swingHelper.Swing_Draw_Afterimage((x) => new Color(1f, 1f, 1f, 0.1f * x) * 0.2f);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);

            var effect = WeaponSkill.HammerChannelShader.Value;
            Vector4 color = hammerProj.GetDrawColor().ToVector4();
            color *= 1.2f;
            color.W = 255;
            effect.Parameters["uColor"].SetValue(color);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.7f);
            effect.Parameters["uImageSize"].SetValue(swingHelper.SwingItemTex.Value.Size());
            effect.CurrentTechnique.Passes[0].Apply();
            swingHelper.DrawSwingItem(lightColor);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                Main.Rasterizer, null, Main.Transform);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (hammerProj.ChannelLevel == 2)
            {
                modifiers.SourceDamage += 0.8f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            hammerProj.ChannelLevel = 1;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            hammerProj.ChannelLevel = 0;
        }
    }
}
