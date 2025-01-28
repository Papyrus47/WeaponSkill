using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.HuntingHorn.Melodies;
using WeaponSkill.Weapons.InsectStaff;

namespace WeaponSkill.Weapons.HuntingHorn
{
    public class HuntingHornGlobalItem : BasicWeaponItem<HuntingHornGlobalItem>
    {
        /// <summary>
        /// 笛子的旋律
        /// </summary>
        public HuntingHornMelody hornMelody;
        public Queue<HuntingHornBuff> huntingHornBuffs = new();
        public override void SetDefaults(Item entity)
        {
            entity.DamageType = DamageClass.Summon;
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            entity.useTurn = false;
            entity.useAnimation = entity.useTime = 1;
            entity.noMelee = true;
            //Insect = new(ModContent.ItemType<TestInsect>());
        }
        public override bool? UseItem(Item item, Player player)
        {
            //int i = Main.rand.Next(5);
            //if(i == 0) 
            //    hornMelody.melodies.Enqueue(HuntingHornMelody.MelodyType.Left);
            //else if(i == 1)
            //    hornMelody.melodies.Enqueue(HuntingHornMelody.MelodyType.Right);
            //else if (i == 2)
            //    hornMelody.melodies.Enqueue(HuntingHornMelody.MelodyType.LeftAndRight);
            //else if (i == 3)
            //    hornMelody.melodies.Enqueue(HuntingHornMelody.MelodyType.SP);
            return base.UseItem(item, player);
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<HuntingHornProj>()] <= 0) // 生成手持弹幕
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<HuntingHornProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;
            }
        }
    }
}
