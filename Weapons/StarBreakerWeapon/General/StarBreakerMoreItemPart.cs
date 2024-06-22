using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace WeaponSkill.Weapons.StarBreakerWeapon.General
{
    /// <summary>
    /// 星击的更多物品栏的介绍
    /// </summary>
    public interface StarBreakerMoreItemPart
    {
        /// <summary>
        /// 更多的介绍
        /// </summary>
        public LocalizedText PartText { get; }
    }
}
