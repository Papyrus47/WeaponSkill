using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Lances
{
    public class LancesShield : BasicShield
    {
        public readonly LancesProj lancesProj;
        public Asset<Texture2D> drawTex;
        /// <summary>
        /// 强化防御
        /// </summary>
        public bool StrongDef;
        /// <summary>
        /// 力量防御
        /// </summary>
        public bool PowerDef;
        /// <summary>
        /// GP格挡攻击
        /// </summary>
        public bool GP;
        public Vector2 Pos;
        public int Dir;
        public bool DefSucceeded_GP => DefSucceeded && GP;
        public LancesShield(LancesProj lancesProj, Asset<Texture2D> drawTex)
        {
            this.lancesProj = lancesProj;
            this.drawTex = drawTex;
        }
        public override float GetDefence()
        {
            float def = base.GetDefence();
            if (StrongDef)
                def *= 1.5f;
            if (PowerDef)
                def += lancesProj.Player.statDefense;
            return def;
        }
        /// <summary>
        /// 更新盾
        /// </summary>
        /// <param name="Pos">盾的位置</param>
        /// <param name="Dir">盾的朝向</param>
        public virtual void Update(Vector2 Pos, int Dir)
        {
            InDef = false;
            StrongDef = false;
            GP = false;
            PowerDef = false;
            this.Pos = Pos;
            this.Dir = Dir;
        }
        /// <summary>
        /// 盾的绘制
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="color"></param>
        public virtual void Draw(SpriteBatch sb, Color color)
        {
            bool flag = false;
            if (TheUtility.InBegin())
            {
                flag = true;
                sb.End();
            }
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone);
            if (StrongDef)
            {
                sb.Draw(drawTex.Value, Pos - Main.screenPosition, null, Color.Blue with { A = 100 }, 0, drawTex.Size() * 0.5f, 1.3f, Dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                sb.Draw(drawTex.Value, Pos - Main.screenPosition, null, Color.Blue with { A = 100 }, 0, drawTex.Size() * 0.5f, 1.4f, Dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                sb.Draw(drawTex.Value, Pos - Main.screenPosition, null, Color.Blue with { A = 100 }, 0, drawTex.Size() * 0.5f, 1.5f, Dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            if (PowerDef)
            {
                sb.Draw(drawTex.Value, Pos - Main.screenPosition, null, Color.Blue with { A = 100 }, 0, drawTex.Size() * 0.5f, 2f, Dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            sb.Draw(drawTex.Value, Pos - Main.screenPosition, null, color, 0, drawTex.Size() * 0.5f, 1f, Dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            sb.End();
            if (flag)
            {
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None,
                                    Main.Rasterizer, null, Main.Transform);
            }
        }
        public override bool GetDefSucced(Rectangle hitbox) => true;
        public override void ModifyHit(ref Player.HurtModifiers hurtModifiers)
        {
            base.ModifyHit(ref hurtModifiers);
            if (PowerDef)
            {
                hurtModifiers.SourceDamage *= 0.95f;
            }
        }
        public override void OnDefSucceeded()
        {
            base.OnDefSucceeded();
        }
    }
}
