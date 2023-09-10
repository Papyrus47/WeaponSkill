using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Items.ChargeBlade;
using WeaponSkill.NPCs;
using static WeaponSkill.Items.LongSword.CutTheFire;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    /// <summary>
    /// 属性解放斩
    /// </summary>
    public class ChargeBlade_Axe_Swing_Liberate : ChargeBlade_Axe_Swing
    {
        public class LiberateOnHit : WeaponSkillGlobalNPCComponent
        {
            public int Time;
            public BasicChargeBlade basicChargeBlade;
            public Player player;
            public LiberateOnHit(int time, BasicChargeBlade basicChargeBlade, Player player)
            {
                Time = time;
                this.basicChargeBlade = basicChargeBlade;
                this.player = player;
            }

            public override void AI(NPC npc)
            {
                if(Time-- < 0)
                {
                    basicChargeBlade.LiberateHit(npc,player);
                    Remove = true;
                }
            }
        }
        public bool UseBottles;
        public ChargeBlade_Axe_Swing_Liberate(ChargeBladeProj chargeBlade, Func<bool> activationConditionFunc) : base(chargeBlade, activationConditionFunc)
        {
        }
        public BasicChargeBlade basicChargeBlade => ChargeBladeProj.SpawnItem.ModItem as BasicChargeBlade;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(UseBottles && !Rehit[target.whoAmI]) WeaponSkillGlobalNPC.AddComponent(target, new LiberateOnHit(15,basicChargeBlade,player));
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 0.5f;
            if (Rehit[target.whoAmI])
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            if (ChargeBladeProj.chargeBladeGlobal.StatChargeBottle > 0)
            {
                ChargeBladeProj.chargeBladeGlobal.StatChargeBottle--;
                UseBottles = true;
            }
            else UseBottles = false;
            ChargeBladeProj.chargeBladeGlobal.AxeStrengtheningTime = 900;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) =>
            {
                Color color = basicChargeBlade.LiberateColor;
                color.A = 0;
                return color;
            });
            return false;
        }
    }
}
