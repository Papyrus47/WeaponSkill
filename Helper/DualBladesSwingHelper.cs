﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.DualBlades;

namespace WeaponSkill.Helper
{
    public class DualBladesSwingHelper : SwingHelper
    {
        public Ref<Vector2> vel;
        public int spriteDir;
        protected override Vector2 velocity { get => vel.Value; set => vel.Value = value; }
        protected override int spriteDirection { get => spriteDir; set => spriteDir = value; }
        public DualBladesSwingHelper(DualBladesProj proj, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(proj.Projectile, oldVelLength, swingItemTex)
        {
        }
    }
}
