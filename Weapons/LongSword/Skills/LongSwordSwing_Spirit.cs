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
    /// 气刃系列连击
    /// </summary>
    public class LongSwordSwing_Spirit : LongSwordSwing
    {
        public bool LevelUp;
        public bool SP_Spirit;
        public bool UseSpirit;
        public bool IsLevelUp;
        public LongSwordSwing_Spirit(LongSwordProj longSword, Func<bool> activationConditionFunc) : base(longSword, activationConditionFunc)
        {
        }
        public override void AI()
        {
            base.AI();
            if ((int)Projectile.ai[0] == 0 && player.velocity.X != 0)
            {
                player.ChangeDir((player.velocity.X > 0).ToDirectionInt());
            }
            LongSword.InSpiritAttack = true;
            if ((int)Projectile.ai[0] == 0 && (!SP_Spirit || (SP_Spirit && UseSpirit))) LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().Spirit -= 2;
        }
        public override bool ActivationCondition()
        {
            if (SP_Spirit) return base.ActivationCondition();
            return base.ActivationCondition() && LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().Spirit > 13;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            player.GetModPlayer<WeaponSkillPlayer>().ForesightSlash_OnHit = false;
            UseSpirit = LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>().Spirit > 13;
            IsLevelUp = false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 0.95f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            var item = LongSword.SpawnItem.GetGlobalItem<LongSwordGlobalItem>();
            if (LevelUp && !IsLevelUp)
            {
                IsLevelUp = true;
                item.Spirit = 0;
                item.Time = 255;
                if(item.SpiritLevel < 3) item.SpiritLevel++;
            }
        }
    }
}
