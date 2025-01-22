using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using WeaponSkill.Effects;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    public class DualBladesSwing_InDemon : DualBladesSwing
    {
        /// <summary>
        /// 修正
        /// </summary>
        public float DemonMode_AddCorrection;
        public bool IsDemonDance;
        public DualBladesSwing_InDemon(DualBladesProj dualBladesProj, SwingSet swingSet, DoubleSwingSpeed swingSetSpeed, Func<bool> changeCondition) : base(dualBladesProj, swingSet, swingSetSpeed, changeCondition)
        {
        }
        public override void AI()
        {
            Projectile.rotation = MathHelper.PiOver4 * 0.45f;
            base.AI();
            bladesGlobalItem.AddCorrection += DemonMode_AddCorrection;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ID.Contains("demonSlash_Dance"))
            {
                modifiers.FinalDamage += ID switch
                {
                    "demonSlash_Dance_1" => 0.05f,
                    "demonSlash_Dance_2" => 0.1f,
                    "demonSlash_Dance_3" => 0.15f,
                    "demonSlash_Dance_4" => 0.25f,
                    "demonSlash_Dance_End" => 0.5f,
                    _ => 0
                };
            }
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (IsDemonDance)
            {
                return Projectile.ai[0] <= 0.1f && ActivationCondition();
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override bool ActivationCondition()
        {
            return base.ActivationCondition() && bladesProj.InDemonMode;
        }
    }
}
