using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    /// <summary>
    /// 袈裟斩
    /// </summary>
    public class LongSwordSwing_BackSlash : LongSwordSwing
    {
        public LongSwordSwing_BackSlash(LongSwordProj longSword, Func<bool> activationConditionFunc) : base(longSword, activationConditionFunc)
        {
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 1)
            {
                player.velocity.X = -player.direction * (MathF.Log(10 / (Projectile.ai[1] + 1)) + 4);
            }
            else if(((int)Projectile.ai[0] == 1) && Projectile.ai[2] < 7)
            {
                player.velocity.X *= 0.4f;
            }
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if ((nowSkill as BasicLongSwordSkill)?.PreAttack == true)
            {
                return player.controlUseItem && player.controlUseTile;
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
    }
}
