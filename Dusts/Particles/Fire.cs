using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;

namespace WeaponSkill.Dusts.Particles
{
    public class Fire : ABasicParticle
    {
        public int TimeLeft;
        public Color color;
        public Fire(int timeLeft)
        {
            TimeLeft = timeLeft;
            color = Color.Lerp(Color.Firebrick, Color.Orange, Main.rand.NextFloat());
        }
        public override void SetBasicInfo(Asset<Texture2D> textureAsset, Rectangle? frame, Vector2 initialVelocity, Vector2 initialLocalPosition)
        {
            base.SetBasicInfo(ModContent.Request<Texture2D>(GetType().Namespace.Replace('.', '/') + "/Fire"), frame, initialVelocity, initialLocalPosition);
            ScaleVelocity = new(-0.02f);
            Scale = new(0.6f, 1f);
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            spritebatch.Draw(_texture.Value, LocalPosition - Main.screenPosition, null, color, Velocity.ToRotation() - MathHelper.PiOver2, _texture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
        }
        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            Velocity *= 0.99f;
            if (TimeLeft-- <= 0)
            {
                ShouldBeRemovedFromRenderer = true;
            }
            color.A = 100;
        }
    }
}
