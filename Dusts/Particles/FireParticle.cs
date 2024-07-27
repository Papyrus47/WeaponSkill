using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Renderers;

namespace WeaponSkill.Dusts.Particles
{
    public class FireParticle : ABasicParticle
    {
        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            if(Scale.LengthSquared() <= 1)
                ShouldBeRemovedFromRenderer = true;
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            
        }
    }
}
