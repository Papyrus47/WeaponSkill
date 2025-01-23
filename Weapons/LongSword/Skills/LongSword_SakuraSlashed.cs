using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Particles;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    public class LongSword_SakuraSlashed : LongSwordSwing_Spirit
    {
        public bool SlashOnHit;
        public NPC HitTarget;
        public int Time;
        public LongSword_SakuraSlashed(LongSwordProj longSword, Func<bool> activationConditionFunc) : base(longSword, activationConditionFunc)
        {
            SwingRot = MathHelper.TwoPi * 2;
            SP_Spirit = true;
        }
        public override void AI()
        {
            base.AI();
            player.SetImmuneTimeForAllTypes(5);
            player.immuneAlpha = 0;
            if ((int)Projectile.ai[0] == 1)
            {
                Projectile.ai[1] -= 0.5f;
                Projectile.extraUpdates = 3;
                player.velocity.X = player.direction * 30;
            }
            else if ((int)Projectile.ai[0] == 2)
            {
                Time = 600;
                Projectile.ai[2] += 2;
                player.velocity.X *= 0.3f;
                if (SlashOnHit)
                {
                    if (Projectile.ai[2] < 5 && HitTarget != null)
                    {
                        Vector2 vel = player.velocity.SafeNormalize(default).RotatedByRandom(MathHelper.TwoPi);
                        for (int i = 0; i < 6; i++)
                        {
                            var proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), HitTarget.Center - vel.RotatedBy(MathHelper.PiOver4 * i) * LongSword.SwingLength * 1.5f, vel.RotatedBy(MathHelper.PiOver4 * i), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner, LongSword.SwingLength * 2.3f, 80, TextureAssets.Heart.Value);
                            proj.FixedPos = false;
                        }
                    }

                    if (!IsLevelUp)
                    {
                        IsLevelUp = true;
                        if (LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().SpiritLevel < 3) LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().SpiritLevel++;
                        LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().Spirit = 0;
                    }
                }
            }
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if(Time > 0)
            {
                if(--Time <= 0)
                {
                    SpearsStar spearsStar = new(player, new Vector2(0.4f, 0.8f) * 3f)
                    {
                        ScaleVelocity = new Vector2(0.1f, 0.2f) * -0.7f,
                        TimeLeft = 12
                    };
                    Main.ParticleSystem_World_OverPlayers.Add(spearsStar);
                }
                return false;
            }
            if ((nowSkill as BasicLongSwordSkill)?.PreAttack == true)
            {
                return base.ActivationCondition();
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            SlashOnHit = false;
            SkillTimeOut = false;
            HitTarget = null;
            IsLevelUp = false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 1.5f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (!SlashOnHit)
            {
                TheUtility.ResetProjHit(Projectile);
                SlashOnHit = true;
            }
            HitTarget = target;
        }
    }
}
