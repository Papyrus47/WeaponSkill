using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Command.SwingHelpers
{
    public class ShieldSwingHelper : SwingHelper
    {
        public ShieldSwingHelper(BasicShield Shield, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(Shield, oldVelLength, swingItemTex)
        {
            frame = 0;
            frameMax = 1;
        }
        public BasicShield Shield => SpawnEntity as BasicShield;
        public Vector2 center;
        public Vector2 vel;
        public float Rot;
        public float ShieldRot;
        public int SPDir;
        public bool ChangeLerp { get => _changeLerpInvoke; set => _changeLerpInvoke = value; }
        protected override Vector2 Size { get => Shield.Size; set => Shield.Size = value; }
        protected override Vector2 Center { get => center; set => center = value; }
        protected override int frame { get; set; }
        protected override int frameMax { get; set; }
        protected override float rotation { get => Rot; set => Rot = value; }
        protected override Vector2 velocity { get => vel; set => vel = value; }
        protected override int spriteDirection { get => SPDir; set => SPDir = value; }
        protected override int width { get => Shield.width; set => Shield.width = value; }
        public override Vector2 GetDrawCenter(int index = 0)
        {
            Vector2 pos = Center + velocity * 0.5f;
            if (_drawCorrections)
            {
                pos += oldVels[index].SafeNormalize(default) * _changeHeldLength;
            }
            return pos;
        }
        public override bool GetColliding(Rectangle targetHitBox)
        {
            float r = 0;
            //if (_drawCorrections)
            //{
            //    return Collision.CheckAABBvLineCollision(targetHitBox.TopLeft(), targetHitBox.Size(), Center, Center + velocity.RotatedBy(rotation * spriteDirection),
            //    width / 2, ref r);
            //}
            Vector2 vector2 = velocity.RotatedBy(rotation * spriteDirection);
            return Collision.CheckAABBvLineCollision(targetHitBox.TopLeft(), targetHitBox.Size(), Center - vector2, Center + vector2,
                width / 2, ref r);
        }
        public override void DrawSwingItem(Color drawColor)
        {
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (projectile != null)
            {
                SwingItemTex ??= TextureAssets.Projectile[projectile.type];
            }
            //var origin = gd.RasterizerState;
            //RasterizerState rasterizerState = new()
            //{
            //    CullMode = CullMode.None,
            //    FillMode = FillMode.WireFrame
            //};
            //gd.RasterizerState = rasterizerState;

            Vector2 velocity = GetOldVel(-1, true);
            Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(VisualRotation * spriteDirection).SafeNormalize(default)
                * _halfSizeLength * spriteDirection;
            //halfLength = halfLength;

            Vector2 center = GetDrawCenter();
            if (_drawCorrections)
            {
                center = Center + (center - Center);
            }
            Vector2 halfVelPos = center + velocity * 0.5f;
            Vector2[] pos = new Vector2[4]
            {
                 center - Main.screenPosition,
                 halfVelPos - halfLength - Main.screenPosition,
                 center + velocity - Main.screenPosition,
                 halfVelPos + halfLength  - Main.screenPosition
            };

            Vector2 rotPos = (pos[0] + pos[1] + pos[2] + pos[3]) / 4;
            for (int i = 0; i < 4; i++)
            {
                Vector2 v = (pos[i] - rotPos).RotatedBy(ShieldRot);
                pos[i] = rotPos + v.SafeNormalize(default) * halfLength * 2;
            }

            float factor = (frame + 1f) / frameMax;
            CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
            customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(0, factor, 0)); // 柄
            customVertices[1] = new(pos[1], drawColor, new Vector3(0, factor - 1f, 0)); // 左上角
            customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(1, factor - 1f, 0)); // 头
            customVertices[4] = new(pos[3], drawColor, new Vector3(1, factor, 0)); // 右下角

            gd.Textures[0] = SwingItemTex.Value;
            //gd.Textures[0] = TextureAssets.MagicPixel.Value;
            gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
            //gd.RasterizerState = origin;
        }
    }
}
