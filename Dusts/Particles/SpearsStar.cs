using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Renderers;

namespace WeaponSkill.Dusts.Particles
{
    public class SpearsStar : ABasicParticle
    {
        public Entity entity;
        public int TimeLeft;
        public SpearsStar(Entity entity, Vector2 Scale)
        {
            this.entity = entity;
            this.Scale = Scale;
        }
        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            LocalPosition = entity.Center;
            if (TimeLeft-- < 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Texture2D tex = TextureAssets.Extra[89].Value;
            Color color = Color.SkyBlue;
            color.A = 0;
            spritebatch.Draw(tex, LocalPosition - Main.screenPosition, null, color, 0, tex.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
            spritebatch.Draw(tex, LocalPosition - Main.screenPosition, null, color, MathHelper.PiOver2, tex.Size() * 0.5f, Scale + new Vector2(0, 0.8f), SpriteEffects.None, 0f);
        }
    }
}
