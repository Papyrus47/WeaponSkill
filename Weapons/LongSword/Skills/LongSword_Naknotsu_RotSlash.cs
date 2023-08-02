using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Shortsword;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    public class LongSword_Naknotsu_RotSlash : LongSwordSwing_Spirit
    {
        public LongSword_Naknotsu_RotSlash(LongSwordProj longSword, Func<bool> activationConditionFunc) : base(longSword, activationConditionFunc)
        {
            SP_Spirit = true;
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.ai[1] -= 0.65f;
                player.GetModPlayer<WeaponSkillPlayer>().Naknotsu_Slash = true;
            }
            else if ((int)Projectile.ai[0] == 1)
            {
                Projectile.extraUpdates = 2;
                player.velocity.X = player.direction * 30;
                player.GetModPlayer<WeaponSkillPlayer>().Naknotsu_Slash = false;
            }
            else if ((int)Projectile.ai[0] == 2)
            {
                player.velocity.X *= 0.3f;
                if (!IsLevelUp && !player.GetModPlayer<WeaponSkillPlayer>().Naknotsu_Slash_OnHit)
                {
                    IsLevelUp = true;
                    if(LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().SpiritLevel > 0) LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().SpiritLevel--;
                    var proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), player.Center - player.velocity.SafeNormalize(default) * LongSword.SwingLength * 5f, player.velocity.SafeNormalize(default), Projectile.damage, Projectile.knockBack, Projectile.owner, LongSword.SwingLength * 9f, 80, TextureAssets.Heart.Value);
                    proj.FixedPos = false;
                }
            }
            LongSword.CanChangeScabbardRot = true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage += 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
