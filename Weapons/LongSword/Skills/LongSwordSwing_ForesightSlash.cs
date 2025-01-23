using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Command;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    /// <summary>
    /// 见切
    /// </summary>
    public class LongSwordSwing_ForesightSlash : LongSwordSwing
    {
        public LongSwordSwing_ForesightSlash(LongSwordProj longSword, Func<bool> activationConditionFunc) : base(longSword, activationConditionFunc)
        {
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.ai[1] -= 0.7f;
                player.velocity.X = -player.direction * (MathF.Log(10 / (Projectile.ai[1] + 1)) + 4.5f);
                LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().Spirit = 0;
                player.GetModPlayer<WeaponSkillPlayer>().InForesightSlash = true;
            }
            else if (((int)Projectile.ai[0] == 1) && Projectile.ai[2] < 7)
            {
                player.velocity.X += player.direction * (MathF.Log(10 / (Projectile.ai[1] + 1)) + 0.85f);
                player.GetModPlayer<WeaponSkillPlayer>().InForesightSlash = false;
            }
            else
            {
                player.velocity.X *= 0.8f;
                if ((int)Projectile.ai[0] == 2)
                {
                    Projectile.ai[2] += 2;
                }
            }
        }
        public override bool SwitchCondition() => base.SwitchCondition() && player.GetModPlayer<WeaponSkillPlayer>().ForesightSlash_OnHit;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (player.GetModPlayer<WeaponSkillPlayer>().ForesightSlash_OnHit)
            {
                var item = LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>();
                item.Spirit = item.SpiritMax;
            }
        }
    }
}
