using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Bows.Skills
{
    public abstract class BasicBowsSkill : ProjSkill_Instantiation
    {
        public BowsProj BowsProj => modProjectile as BowsProj;
        public Player player;
        public BasicBowsSkill(ModProjectile modProjectile) : base(modProjectile)
        {
            player = BowsProj.Player;
        }
        /// <summary>
        /// 获取发射弹幕类型,自动消耗物品
        /// </summary>
        /// <returns>弹幕type,物品type</returns>
        public virtual (int,int) GetShootType(out int damage,out float speed,out float kn,out int crit)
        {
            WeaponSkillPlayer weaponSkillPlayer = player.GetModPlayer<WeaponSkillPlayer>();
            Item item = weaponSkillPlayer.AmmoItems[weaponSkillPlayer.UseAmmoIndex];
            if (ItemLoader.ConsumeItem(item,player) && !player.IsAmmoFreeThisShot(player.HeldItem, item, item.shoot))
            {
                CombinedHooks.OnConsumeAmmo(player, player.HeldItem, item);
                if (item.stack-- <= 0)
                {
                    item.active = false;
                    item.TurnToAir();
                }
            }
            damage = item.damage;
            speed = item.shootSpeed;
            kn = item.knockBack;
            crit = item.crit;
            return (item.shoot,item.type);
        }
    }
}
