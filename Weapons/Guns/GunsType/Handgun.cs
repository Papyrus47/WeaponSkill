using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Guns.GunsType
{
    /// <summary>
    /// 手枪
    /// </summary>
    public class Handgun : BasicGunsType
    {
        public Handgun(int maxBullet = 0) : base(maxBullet)
        {
            if(maxBullet == 0)
                MaxBullet = 7;
            HasBullet = MaxBullet;
            ResetTime = 40;
        }

        public override void OnHold(Player player, Item item)
        {
        }
        public override void UpdateInventory(Player player, Item item)
        {
            base.UpdateInventory(player, item);
        }
    }
}
