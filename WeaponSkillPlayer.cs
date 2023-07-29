using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.Graphics.CameraModifiers;
using Terraria.ModLoader.IO;

namespace WeaponSkill
{
    public class WeaponSkillPlayer : ModPlayer
    {
        public bool InBlocking;
        public bool IsBlockAttack;
        public bool Player_BowSidingStep;
        public byte BowChannelLeave;
        /// <summary>
        /// 拥有的弹药,用于切换
        /// </summary>
        public List<Item> AmmoItems;
        /// <summary>
        /// 展示远程攻击弹药切换UI
        /// </summary>
        public bool ShowTheRangeChangeUI;
        /// <summary>
        /// 展示耐力槽UI
        /// </summary>
        public bool ShowTheStamina;
        /// <summary>
        /// 非物品栏索引,一种独特的索引
        /// </summary>
        public int UseAmmoIndex;
        /// <summary>
        /// 耐力槽
        /// </summary>
        public int StatStamina;
        /// <summary>
        /// 耐力槽最大值
        /// </summary>
        public int StatStaminaMax;
        /// <summary>
        /// 根据这个确定是否回复耐力槽
        /// </summary>
        public int StatStaminaAddTime;
        /// <summary>
        /// 旧的耐力槽
        /// </summary>
        public int OldStatStamina;
        public override void ResetEffects()
        {
            ShowTheRangeChangeUI = false; 
            ShowTheStamina = false;
            AmmoItems ??= new();
            BowChannelLeave = 0;
        }
        public override void PostUpdate()
        {
            if (StatStamina < OldStatStamina)
            {
                StatStaminaAddTime = 0;
            }
            OldStatStamina = StatStamina;
            if(StatStamina > StatStaminaMax)
            {
                StatStamina = StatStaminaMax;
            }
            if(StatStaminaAddTime < 90)
            {
                StatStaminaAddTime++;
            }
            else
            {
                StatStamina++;
                if(Player.velocity.LengthSquared() < 1f)
                {
                    StatStamina++;
                }
            }
        }
        public override void OnEnterWorld()
        {
            if(StatStaminaMax <= 0) StatStaminaMax = 600;
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ShowTheRangeChangeUI && WeaponSkill.RangeChange.JustPressed)
            {
                UseAmmoIndex++;
            }
            if (WeaponSkill.BowSlidingStep.JustPressed)
            {
                Player_BowSidingStep = true;
            }
            else
            {
                Player_BowSidingStep = false;
            }
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (InBlocking)
            {
                modifiers.ModifyHurtInfo += BlockingDamage;
                IsBlockAttack = true;
            }
        }
        public override bool? CanMeleeAttackCollideWithNPC(Item item, Rectangle meleeAttackHitbox, NPC target)
        {
            return base.CanMeleeAttackCollideWithNPC(item, meleeAttackHitbox, target);
        }
        private void BlockingDamage(ref Player.HurtInfo info)
        {
            info.Damage -= Player.statDefense * 10;
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            switch (BowChannelLeave)
            {
                case 1:
                    {
                        r = 0.7f;
                        b = 0.4f;
                        g = 0.4f;
                        a = 0;
                        break;
                    }
                case 2:
                    {
                        r = 0.9f;
                        b = 0.6f;
                        g = 0.6f;
                        a = 0;
                        break;
                    }
                case 3:
                    {
                        r = 1;
                        b = 0.5f;
                        g = 0.5f;
                        a = 0;
                        break;
                    }
            }
        }

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (InBlocking && info.Damage <= 1)
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Player.Center, Main.rand.NextVector2Unit(), 4, 3, 1));
                var sound = SoundID.Item106;
                sound.MaxInstances = 2;
                SoundEngine.PlaySound(sound.WithVolumeScale(5).WithPitchOffset(-0.8f), Player.position);
                SoundEngine.PlaySound(sound.WithVolumeScale(0.5f).WithPitchOffset(0.9f), Player.position);
                Player.SetImmuneTimeForAllTypes(20);
                return true;
            }
            return base.FreeDodge(info);
        }
    }
}
