using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WeaponSkill.Configs
{
    public abstract class BasicWeaponSkillConfig<T> : ModConfig where T : BasicWeaponSkillConfig<T>
    {
        public static T Init => ModContent.GetInstance<T>();
        public override bool NeedsReload(ModConfig pendingConfig)
        {
            return base.NeedsReload(pendingConfig);
        }
    }
}
