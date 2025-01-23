using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.SwingHelpers
{
    /// <summary>
    /// 霜拳特殊拖尾
    /// </summary>
    public class FrostFistLongSwordSwingHelper : SwingHelper
    {
        public FrostFistLongSwordSwingHelper(Projectile spawnEntity, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(spawnEntity, oldVelLength, swingItemTex)
        {
        }
        public override Vector2 GetDrawCenter(int index = 0)
        {
            Vector2 pos = projectile.oldPos[index];
            if (index == 0)
            {
                pos = projectile.Center;
            }
            if (_drawCorrections)
            {
                pos += oldVels[index].SafeNormalize(default) * _changeHeldLength;
            }
            return pos;
        }
    }
}
