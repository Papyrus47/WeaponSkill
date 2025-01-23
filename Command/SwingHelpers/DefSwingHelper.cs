using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Command.SwingHelpers
{
    public class DefSwingHelper : SwingHelper
    {
        public DefSwingHelper(object spawnEntity, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(spawnEntity, oldVelLength, swingItemTex)
        {
        }
        public Vector2 center;
        public Vector2 vel;
        public float Rot;
        public float AxeRot;
        public int SPDir;
        public int Width;
        public Vector2 size;

        public new Vector2 Size { get => size; set => size = value; }
        public override Vector2 Center { get => center; set => center = value; }
        public override int frame { get; set; }
        public override int frameMax { get; set; }
        public override float rotation { get => Rot; set => Rot = value; }
        public override Vector2 velocity { get => vel; set => vel = value; }
        public override int spriteDirection { get => SPDir; set => SPDir = value; }
        public override int width { get => Width; set => Width = value; }
    }
}
