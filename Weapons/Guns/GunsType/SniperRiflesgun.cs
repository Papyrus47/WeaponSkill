using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Guns.GunsType
{
    /// <summary>
    /// 狙击步枪
    /// </summary>
    public class SniperRiflesgun : BasicGunsType
    {
        public SniperRiflesgun(int maxBullet = 0) : base(maxBullet)
        {
            if (maxBullet == 0)
                MaxBullet = 4;
            HasBullet = MaxBullet;
            ResetTime = 75;
        }
        // 按X换弹,F进行狙杀(效果,自身隐身,伤害增加1000%,期间被命中即死,发射后则进入一个冷却CD,CD:15s)

        /// <summary>
        /// 技能CD
        /// </summary>
        public int SkillCD;
        /// <summary>
        /// 狙杀模式
        /// </summary>
        public bool InKiller;
        // 按X换弹,按F进行高速射击(CD:5s)
        public override void OnHold(Player player, Item item)
        {
            MaxBullet = 4;
            GunsGlobalItem gunsGlobalItem = item.GetGlobalItem<GunsGlobalItem>();
            if (WeaponSkill.SpKeyBind.Current)
            {
                gunsGlobalItem.ResetBullet = true;
            }

            if (SkillCD == 0 && WeaponSkill.BowSlidingStep.JustPressed)
                InKiller = true;

            if (InKiller)
            {
                player.aggro = -1200;
                player.immuneAlpha = 200;
                if (player.GetModPlayer<WeaponSkillPlayer>().PlayerOnHurt)
                {
                    InKiller = false;
                    player.KillMe(PlayerDeathReason.LegacyDefault(), 10.0, 1);
                }
            }

            for (int i = 0; i < 30; i++)
            {
                if (SkillCD <= i * 30)
                    continue;
                Dust dust = Dust.NewDustDirect(player.RotatedRelativePoint(player.MountedCenter), 1, 1, DustID.GoldFlame);
                dust.noGravity = true;
                dust.velocity *= 0;
                dust.velocity += player.velocity;
                dust.position += Vector2.UnitX.RotatedBy(i / 30f * MathHelper.TwoPi) * 50;
            }
        }
        public override void UpdateInventory(Player player, Item item)
        {
            if (SkillCD > 0)
            {
                SkillCD--;
            }
        }
        public override void OnShoot(Player player, Item item)
        {
            if (InKiller)
            {
                InKiller = false;
                SkillCD = 900;
            }
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (InKiller)
                damage += 10;
        }
    }
}
