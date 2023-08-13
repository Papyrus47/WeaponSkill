using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    /// <summary>
    /// 水月架势
    /// </summary>
    public class LongSword_SerenePose : LongSwordSwing
    {
        public bool PlayerOnHit
        {
            get
            {
                return player.GetModPlayer<WeaponSkillPlayer>().SerenePoseOnHit;
            }
            set
            {
                player.GetModPlayer<WeaponSkillPlayer>().SerenePoseOnHit = value;
            }
        }

        public LongSword_SerenePose(LongSwordProj longSword, Func<bool> activationConditionFunc) : base(longSword, activationConditionFunc)
        {
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == -1)
            {
                // 水月架势--举刀准备
                player.GetModPlayer<WeaponSkillPlayer>().SerenePose = true;
                swingHelper.Change_Lerp(new Vector2(2f,-1), CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, 0.5f, CHANGE_LERP_SPEED);
                swingHelper.ProjFixedPlayerCenter(player, -LongSword.SwingLength * 0.35f, true);
                swingHelper.SetNotSaveOldVel();
                swingHelper.SwingAI(LongSword.SwingLength, player.direction, 0);
                player.heldProj = Projectile.whoAmI;
                Projectile.ai[1]++;
                if (PlayerOnHit)
                {
                    PlayerOnHit = false;
                    Projectile.ai[0]++;
                    Projectile.ai[1] = 0;
                    player.GetModPlayer<WeaponSkillPlayer>().SerenePose = false;
                }
                else if(Projectile.ai[1] > 90)
                {
                    SkillTimeOut = true;
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 0;
                    player.GetModPlayer<WeaponSkillPlayer>().SerenePose = false;
                }
                player.velocity.X = 0;
                player.immuneAlpha = 0;
            }
            PreAttack = false;
        }
        public override bool? CanDamage()
        {
            if ((int)Projectile.ai[0] == -1) return true;
            return base.CanDamage();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.FinalDamage += 1;
        }
        public override bool ActivationCondition()
        {
            return base.ActivationCondition() && LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().SpiritLevel > 0;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            PlayerOnHit = false;
            Projectile.ai[0] = -1;
            LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().SpiritLevel--;
            LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().Spirit = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
        }
    }
}
