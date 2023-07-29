using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper
{
    public class SpearsSwingHelper : SwingHelper
    {
        public SpearsSwingHelper(Projectile proj, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(proj, oldVelLength, swingItemTex)
        {
        }
        public override void DrawSwingItem(Color drawColor)
        {
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            _SwingItemTex ??= TextureAssets.Projectile[projectile.type];
            //var origin = gd.RasterizerState;
            //RasterizerState rasterizerState = new()
            //{
            //    CullMode = CullMode.None,
            //    FillMode = FillMode.Solid
            //};
            //gd.RasterizerState = rasterizerState;

            Vector2 velocity = GetOldVel(-1, true);
            Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(VisualRotation * projectile.spriteDirection).SafeNormalize(default)
                * _halfSizeLength * projectile.spriteDirection;

            Vector2 center = GetDrawCenter();
            if (_drawCorrections)
            {
                center = projectile.Center + (center - projectile.Center);
            }
            //center -= projectile.velocity;
            Vector2 halfVelPos = center + velocity * 0.5f;
            Vector2[] pos = new Vector2[4]
            {
                 center - Main.screenPosition,
                 halfVelPos - halfLength - Main.screenPosition,
                 center + velocity - Main.screenPosition,
                 halfVelPos + halfLength  - Main.screenPosition
            };

            float factor = (projectile.frame + 1f) / Main.projFrames[projectile.type];
            CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
            customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(1, factor, 0)); // 右下角
            customVertices[1] = new(pos[1], drawColor, new Vector3(0, factor, 0)); // 左下角
            customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(0, factor - 1f, 0)); // 左上角
            customVertices[4] = new(pos[3], drawColor, new Vector3(1, factor - 1f, 0)); // 右上角


            gd.Textures[0] = _SwingItemTex.Value;
            //gd.Textures[0] = TextureAssets.MagicPixel.Value;
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
            //gd.RasterizerState = origin;
        }
    }
}
