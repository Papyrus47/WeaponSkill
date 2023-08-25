using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.ChargeBlade
{
    public class ChargeBladeGlobalItem : BasicWeaponItem<ChargeBladeGlobalItem>
    {
        /// <summary>
        /// 剑强化
        /// </summary>
        public int SwordStrengthening;
        /// <summary>
        /// 盾强化
        /// </summary>
        public int ShieldStrengthening;
        /// <summary>
        /// 斧强化
        /// </summary>
        public bool AxeStrengthening;
        /// <summary>
        /// 斧模式减少瓶子时间
        /// </summary>
        public int AxeStrengtheningTime;
        /// <summary>
        /// 充能量
        /// </summary>
        public int StatCharge;
        /// <summary>
        /// 目前填充瓶子
        /// </summary>
        public byte StatChargeBottle;
        /// <summary>
        /// 瓶子总数
        /// </summary>
        public byte StatChargeBottleMax;
        /// <summary>
        /// 在斧模式下
        /// </summary>
        public bool InAxe;
        /// <summary>
        /// 处于剑形态且剑模式强化
        /// </summary>
        public bool InSwordStreng => SwordStrengthening > 0 && !InAxe;
        /// <summary>
        /// 处于盾强化,斧头形态
        /// </summary>
        public bool InShieldStreng_InAxe => ShieldStrengthening > 0 && InAxe;
        public override void SetDefaults(Item entity)
        {
            StatChargeBottleMax = 5; // 默认五个瓶子
            AxeStrengtheningTime = 300;
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (InShieldStreng_InAxe) // 红盾下斧模式
            {
                damage += 0.2f;
            }
            else if (InSwordStreng) // 红剑下剑模式
            {
                damage += 0.1f;
            }
        }


        public override void UpdateInventory(Item item, Player player)
        {
            #region 剑模式强化相关
            if (SwordStrengthening > 0)
            {
                SwordStrengthening--;
            }
            #endregion
            #region 盾强化相关
            if (ShieldStrengthening > 0)
            {
                ShieldStrengthening--;
            }
            #endregion
            #region 斧模式强化相关
            if (AxeStrengthening) // 处于红斧状态下,减少计时器,然后减少瓶子
            {
                if(AxeStrengtheningTime-- <= 0)
                {
                    StatChargeBottle--;
                }
            }

            if(StatChargeBottle == 0) // 重置斧强化
            {
                AxeStrengthening = false;
                AxeStrengtheningTime = 300;
            }
            #endregion
        }
    }
}
