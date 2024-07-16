using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage
{
    /// <summary>
    /// 属性伤害中心系统类
    /// </summary>
    public class ElementDamageSystem
    {
        public static ElementDamageSystem Instance => _instance;
        private static ElementDamageSystem _instance;
        public static void Load()
        {
            _instance = new ElementDamageSystem();
        }
        /// <summary>
        /// 属性伤害应用
        /// </summary>
        /// <param name="elementDamageType">属性伤害类型</param>
        /// <param name="npc">命中的目标</param>
        public virtual float ElementDamageApply(ElementDamageType elementDamageType,NPC npc)
        {
            ElementDamage_GlobalNPC elementDamage_GlobalNPC = npc.GetGlobalNPC<ElementDamage_GlobalNPC>();
            float damage = elementDamageType.GetElementDamage();
            if (elementDamage_GlobalNPC.ElementDef.TryGetValue(elementDamageType.DamageName, out int def))
            {
                damage -= def;
            }
            if (elementDamage_GlobalNPC.ElementImmuneScale.TryGetValue(elementDamageType.DamageName, out float scale))
            {
                damage *= scale;
            }

            #region 受伤
            if (damage <= 0)
                return 0f;

            damage *= SlashDamage.ElementDamageScale;
            damage *= HitDamage.GetHitDamageMultiple_Element();
            damage *= SpurtsDamage.GetHitDamageMultiple_Element();

            if (npc.CanBeChasedBy())
                npc.life -= (int)damage;
            CombatText.NewText(npc.Hitbox, elementDamageType.HitColor, (int)damage, false);
            elementDamageType.SetDefaults();
            npc.checkDead();
            elementDamageType.OnApply(npc, (int)damage);
            #endregion
            return damage;
        }
        /// <summary>
        /// 属性伤害应用 (暂时不用)
        /// </summary>
        /// <param name="elementDamageType"></param>
        /// <param name="npc"></param>
        public virtual float ElementDamageApply(ElementDamageType elementDamageType, Player player)
        {
            return 0;
        }
    }
    public class ElementDamage_GlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        /// <summary>
        /// 属性的防御力
        /// </summary>
        public Dictionary<string, int> ElementDef = new();
        /// <summary>
        /// 属性的免疫补正
        /// </summary>
        public Dictionary<string, float> ElementImmuneScale = new();
        public override void SetDefaults(NPC entity)
        {
            switch (entity.type)
            {
                case NPCID.KingSlime:
                    ElementImmuneScale.Add(nameof(IceElementDamage), 0.2f);
                    ElementDef.Add(nameof(FireElementDamage), 0);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 3);
                    break;
                case NPCID.QueenSlimeBoss:
                    ElementImmuneScale.Add(nameof(IceElementDamage), 0.2f);
                    ElementDef.Add(nameof(FireElementDamage), 0);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 3);
                    break;
                case NPCID.QueenBee: // 疯王
                case 370: // 诛杀
                    ElementDef.Add(nameof(IceElementDamage), 5);
                    ElementImmuneScale.Add(nameof(IceElementDamage), 1.8f);
                    ElementDef.Add(nameof(FireElementDamage), 40);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 1.5f);
                    break;
                case NPCID.EyeofCthulhu: // 克眼
                    ElementDef.Add(nameof(IceElementDamage), 30);
                    ElementDef.Add(nameof(FireElementDamage), 100);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 1);
                    break;
                case 113: // 肉山
                case 114:
                    ElementDef.Add(nameof(IceElementDamage), 50);
                    ElementImmuneScale.Add(nameof(IceElementDamage), 2f);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 0);
                    break;
                case 13:
                case 14:
                case 15: // 世吞
                    ElementDef.Add(nameof(IceElementDamage), 10);
                    ElementImmuneScale.Add(nameof(IceElementDamage), 1.2f);
                    ElementDef.Add(nameof(FireElementDamage), 150);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 1);
                    break;
                case 35:
                case 36: // 吴克
                case 68: // 地守
                    ElementDef.Add(nameof(IceElementDamage), 30);
                    ElementImmuneScale.Add(nameof(IceElementDamage), 0.4f);
                    ElementDef.Add(nameof(FireElementDamage), 1000);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 0.1f);
                    break;
                case 668: // 鹿
                    ElementDef.Add(nameof(IceElementDamage), 200);
                    ElementImmuneScale.Add(nameof(IceElementDamage), 0.01f);
                    ElementDef.Add(nameof(FireElementDamage), 80);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 2);
                    break;
                case 636: // 光女
                    ElementDef.Add(nameof(IceElementDamage), 60);
                    ElementImmuneScale.Add(nameof(IceElementDamage), 0.8f);
                    ElementDef.Add(nameof(FireElementDamage), 100);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 0.7f);
                    break;
                case 398:
                case 397:
                case 396: // 月
                    ElementImmuneScale.Add(nameof(IceElementDamage), 0.6f);
                    ElementDef.Add(nameof(FireElementDamage), 500);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 0.5f);
                    break;
                case >= 245 and <= 249: // 石巨人
                    ElementImmuneScale.Add(nameof(IceElementDamage), 0f);
                    ElementDef.Add(nameof(FireElementDamage), 100);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 0.1f);
                    break;
                case >= 134 and <= 136: // 铁长
                case 125:
                case 126: // 双子
                case >= 127 and <= 131: // 铁吴克
                    ElementDef.Add(nameof(IceElementDamage), 20);
                    ElementImmuneScale.Add(nameof(IceElementDamage), 0.6f);
                    ElementDef.Add(nameof(FireElementDamage), 200);
                    ElementImmuneScale.Add(nameof(FireElementDamage), 1.4f);
                    break;
            }
            ElementDef.TryAdd(nameof(FireElementDamage), 50);
        }
    }
}
