using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Buffs.HuntingHornBuffs;

namespace WeaponSkill.Weapons.HuntingHorn.Buffs
{
    public class AttackUp_Small : HuntingHornBuff
    {
        public override string Name
        {
            get
            {
                if (Main.gameMenu) 
                    return base.Name;
                Player player = Main.LocalPlayer;
                if (player != null && player.HasBuff<AttackUp_SmallBuff>() || player.HasBuff<AttackUp_MiddleBuff>())
                {
                     return TheUtility.RegisterText("Mods." + GetType().Namespace + "." + nameof(AttackUp_Middle));
                }
                return base.Name;
            }
        }

        public AttackUp_Small(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }

        public AttackUp_Small()
        {
        }

        public override void OnPlay(Player player, Projectile projectile)
        {
            if(!player.HasBuff<AttackUp_SmallBuff>() && !player.HasBuff<AttackUp_MiddleBuff>())
            {
                player.AddBuff(ModContent.BuffType<AttackUp_SmallBuff>(), 3600 * 3);
                foreach(Projectile proj in Main.projectile)
                {
                    if(proj.owner == player.whoAmI && proj.minion && proj.friendly)
                    {
                        if (proj.TryGetGlobalProjectile<HuntingHorn_AddBuffToMinion>(out var result))
                        {
                            result.HuntingHorn_AttackAdd = 0.05f;
                            result.HuntingHorn_AttackUp = 3600f * 3;
                        }
                    }
                }
            }
            else
            {
                player.AddBuff(ModContent.BuffType<AttackUp_MiddleBuff>(), 3600 * 3);

                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.owner == player.whoAmI && proj.minion && proj.friendly)
                    {
                        if (proj.TryGetGlobalProjectile<HuntingHorn_AddBuffToMinion>(out var result))
                        {
                            result.HuntingHorn_AttackAdd = 0.1f;
                            result.HuntingHorn_AttackUp = 3600f * 3;
                        }
                    }
                }
            }
        }
        public override void Register()
        {
        }
    }
    public class AttackUp_Middle : HuntingHornBuff
    {
        public override string Name
        {
            get
            {
                if (Main.gameMenu)
                    return base.Name;
                Player player = Main.LocalPlayer;
                if (player != null && player.HasBuff<AttackUp_MiddleBuff>() || player.HasBuff<AttackUp_BigBuff>())
                {
                    return TheUtility.RegisterText("Mods." + GetType().Namespace + "." + nameof(AttackUp_Middle));
                }
                return base.Name;
            }
        }
        public AttackUp_Middle(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }

        public AttackUp_Middle()
        {
        }

        public override void OnPlay(Player player, Projectile projectile)
        {
            if (!player.HasBuff<AttackUp_MiddleBuff>() && !player.HasBuff<AttackUp_BigBuff>())
            {
                player.AddBuff(ModContent.BuffType<AttackUp_MiddleBuff>(), 3600 * 3);
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.owner == player.whoAmI && proj.minion && proj.friendly)
                    {
                        if (proj.TryGetGlobalProjectile<HuntingHorn_AddBuffToMinion>(out var result))
                        {
                            result.HuntingHorn_AttackAdd = 0.1f;
                            result.HuntingHorn_AttackUp = 3600f * 3;
                        }
                    }
                }
            }
            else
            {
                player.AddBuff(ModContent.BuffType<AttackUp_BigBuff>(), 3600 * 3);
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.owner == player.whoAmI && proj.minion && proj.friendly)
                    {
                        if (proj.TryGetGlobalProjectile<HuntingHorn_AddBuffToMinion>(out var result))
                        {
                            result.HuntingHorn_AttackAdd = 0.15f;
                            result.HuntingHorn_AttackUp = 3600f * 3;
                        }
                    }
                }
            }
        }
    }
    public class AttackUp_Big : HuntingHornBuff
    {
        public AttackUp_Big()
        {
        }

        public AttackUp_Big(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }
        public override string Name
        {
            get
            {
                if (Main.gameMenu)
                    return base.Name;
                Player player = Main.LocalPlayer;
                if (player != null && player.HasBuff<AttackUp_MiddleBuff>() || player.HasBuff<AttackUp_BigBuff>())
                {
                    return TheUtility.RegisterText("Mods." + GetType().Namespace + "." + "AttackUp_Super");
                }
                return base.Name;
            }
        }
        public override void OnPlay(Player player, Projectile projectile)
        {
            if (!player.HasBuff<AttackUp_BigBuff>() && !player.HasBuff<AttackUp_SuperBuff>())
            {
                player.AddBuff(ModContent.BuffType<AttackUp_BigBuff>(), 3600 * 3);
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.owner == player.whoAmI && proj.minion && proj.friendly)
                    {
                        if (proj.TryGetGlobalProjectile<HuntingHorn_AddBuffToMinion>(out var result))
                        {
                            result.HuntingHorn_AttackAdd = 0.15f;
                            result.HuntingHorn_AttackUp = 3600f * 3;
                        }
                    }
                }
            }
            else
            {
                player.AddBuff(ModContent.BuffType<AttackUp_SuperBuff>(), 3600 * 3);
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.owner == player.whoAmI && proj.minion && proj.friendly)
                    {
                        if (proj.TryGetGlobalProjectile<HuntingHorn_AddBuffToMinion>(out var result))
                        {
                            result.HuntingHorn_AttackAdd = 0.2f;
                            result.HuntingHorn_AttackUp = 3600f * 3;
                        }
                    }
                }
            }
        }
    }
}
