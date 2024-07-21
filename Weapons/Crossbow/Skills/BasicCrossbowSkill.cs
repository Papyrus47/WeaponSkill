using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.Bows;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;

namespace WeaponSkill.Weapons.Crossbow.Skills
{
    public abstract class BasicCrossbowSkill : ProjSkill_Instantiation
    {
        public CrossbowProj CrossProj => modProjectile as CrossbowProj;
        public Player player;
        public BasicCrossbowSkill(ModProjectile modProjectile) : base(modProjectile)
        {
            player = CrossProj.Player;
        }
        /// <summary>
        /// 获取发射弹幕类型,自动消耗物品
        /// </summary>
        /// <returns>弹幕type,物品type</returns>
        public virtual (int, int) GetShootType(out int damage, out float speed, out float kn, out int crit,out bool resetArrow)
        {
            resetArrow = false;
            WeaponSkillPlayer weaponSkillPlayer = player.GetModPlayer<WeaponSkillPlayer>();
            Item item = weaponSkillPlayer.AmmoItems[weaponSkillPlayer.UseAmmoIndex];
            if(CrossProj.SpawnItem.GetGlobalItem<CrossbowGlobalItem>().CrossbowLoadArrow.TryGetValue(item,out var @ref))
            {
                @ref.Value--;
                if(@ref.Value <= 0) 
                {
                    @ref.Value = 0;
                    resetArrow = true;
                }
            }
            if (ItemLoader.ConsumeItem(item, player) && !player.IsAmmoFreeThisShot(player.HeldItem, item, item.shoot))
            {
                CombinedHooks.OnConsumeAmmo(player, player.HeldItem, item);
                if (item.consumable && item.stack-- <= 0)
                {
                    item.active = false;
                    item.TurnToAir();
                }
            }
            damage = item.damage;
            speed = item.shootSpeed;
            kn = item.knockBack;
            crit = item.crit;
            return (item.shoot, item.type);
        }
    }
}