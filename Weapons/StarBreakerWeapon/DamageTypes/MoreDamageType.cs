using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WeaponSkill.Weapons.StarBreakerWeapon.DamageTypes
{
    /// <summary>
    /// 多伤害类型
    /// </summary>
    public class MoreDamageType : DamageClass
    {
        /// <summary>
        /// 一种伤害类型的类
        /// </summary>
        public class OneDamageType
        {
            public DamageClass damageClass;
            /// <summary>
            /// 激活这个伤害类型的条件
            /// </summary>
            public Func<Item,bool> ActiveDamageClass;
        }
        /// <summary>
        /// 伤害类型储存
        /// </summary>
        public List<OneDamageType> DamageTypes = new()
        {
            new()
            {
                damageClass = DamageClass.Default,
                ActiveDamageClass = (_) => false
            }
        };
        /// <summary>
        /// 使用的伤害类型的索引
        /// </summary>
        public int UseDamageType;
        public override void SetDefaultStats(Player player)
        {
            DamageTypes[UseDamageType].damageClass.SetDefaultStats(player);
        }
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            DamageClass dmgClass = DamageTypes[UseDamageType].damageClass;
            if(dmgClass is VanillaDamageClass)
            {
                if(dmgClass == Ranged && damageClass == Ranged)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == Magic && damageClass == Magic)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == MagicSummonHybrid && damageClass == MagicSummonHybrid)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == Melee && damageClass == Melee)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == MeleeNoSpeed && damageClass == MeleeNoSpeed)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == Summon && damageClass == Summon)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == SummonMeleeSpeed && damageClass == SummonMeleeSpeed)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == Throwing && damageClass == Throwing)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == Generic && damageClass == Generic)
                {
                    return StatInheritanceData.Full;
                }
                if (dmgClass == Default && damageClass == Default)
                {
                    return StatInheritanceData.Full;
                }
            }
            return dmgClass.GetModifierInheritance(damageClass);
        }
        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return DamageTypes[UseDamageType].damageClass.GetEffectInheritance(damageClass);
        }
        public override bool ShowStatTooltipLine(Player player, string lineName)
        {
            return DamageTypes[UseDamageType].damageClass.ShowStatTooltipLine(player,lineName);
        }
        public void AddDamageType(OneDamageType damageType)
        {
            if(DamageTypes.Find(x => x.damageClass == damageType.damageClass) == null)
            {
                DamageTypes.Add(damageType);
            }
        }
    }

    // 下面这个是更多伤害类型的GlobalItem类
    public class MoreDamageType_GloablItem: GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.DamageType is MoreDamageType;
        }
        public override void UpdateInventory(Item item, Player player)
        {
            MoreDamageType moreDamageType = (item.DamageType as MoreDamageType);
            for(int i = 1;i<moreDamageType.DamageTypes.Count;i++)
            {
                if (moreDamageType.DamageTypes[i].ActiveDamageClass.Invoke(item))
                {
                    moreDamageType.UseDamageType = i;
                    break;
                }
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(x => x.Name == "Damage") + 1;
            if(index != -1)
            {
                MoreDamageType moreDamageType = (item.DamageType as MoreDamageType);
                int count = moreDamageType.DamageTypes.Count;

                tooltips.Insert(index, new(Mod, "MixedDamageType", Language.GetTextValue("Mods.WeaponSkill.MoreDamageType.MixedShow")));
                index++;
                for(int i =1; i < count; i++)
                {
                    TooltipLine tooltip = new(Mod, "MixedDamageType" + i.ToString(), "    " + (item.DamageType as MoreDamageType).DamageTypes[i].damageClass.DisplayName.Value);
                    tooltips.Insert(index, tooltip);
                    if(i == moreDamageType.UseDamageType)
                    {
                        tooltip.OverrideColor = Color.Lerp(Color.Gold * 1.8f, Color.White, Main.DiscoR / 255f);
                    }
                    index++;
                }
            }
        }
    }
}
