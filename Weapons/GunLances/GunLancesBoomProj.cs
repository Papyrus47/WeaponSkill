using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.GunLances
{
    public class GunLancesBoomProj : TransparentProj
    {
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ScalingArmorPenetration += 1f;
        }
    }
}
