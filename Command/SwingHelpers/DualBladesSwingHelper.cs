using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.DualBlades;

namespace WeaponSkill.Command.SwingHelpers
{
    public class DualBladesSwingHelper : SwingHelper
    {
        public Ref<Vector2> vel;
        public int spriteDir;
        public override Vector2 velocity { get => vel.Value; set => vel.Value = value; }
        public override int spriteDirection { get => spriteDir; set => spriteDir = value; }
        public DualBladesSwingHelper(DualBladesProj proj, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(proj.Projectile, oldVelLength, swingItemTex)
        {
        }
    }
}
