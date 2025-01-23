using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Lances;

namespace WeaponSkill.Weapons.SwordShield.Skills
{
    public class SwordShield_Def : BasicSwordShieldSkill
    {
        public SwordShield_Def(SwordShieldProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            swingHelper.Change_Lerp(Vector2.UnitX, 1, Vector2.One, 0.2f, 0f);
            swingHelper.ProjFixedPlayerCenter(player, 0,true);
            swingHelper.SetSwingActive();
            Projectile.spriteDirection = player.direction;
            swingHelper.SwingAI(SwordShieldProj.SwingLength, player.direction, 0);
            SwordShield_Shield Shield = SwordShieldProj.swordShield_Shield;
            #region 盾更新
            player.itemRotation = MathHelper.Pi;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            SwordShieldProj.swordShield_Shield.Update(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, player.itemRotation), player.direction, 0);
            Shield.InDef = true;
            #endregion
            if (!WeaponSkill.BowSlidingStep.Current || player.GetModPlayer<WeaponSkillPlayer>().StatStamina < 0)
            {
                SkillTimeOut = true;
            }
            if (Math.Abs(player.velocity.X) > 1) player.velocity.X = 1 * (player.velocity.X > 0).ToDirectionInt();
            #region 防御成功击退代码
            player.GetModPlayer<WeaponSkillPlayer>().StatStaminaAddTime = 0;
            player.GetModPlayer<WeaponSkillPlayer>().ShowTheStamina = true;
            if (Shield.DefSucceeded)
            {
                player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 100 * ((int)Shield.KNLevel);
                player.velocity.X -= 100 * ((int)Shield.KNLevel) * player.direction;
            }
            #endregion
        }
        public override bool ActivationCondition() => WeaponSkill.BowSlidingStep.Current && player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 0;
        public override bool SwitchCondition() => true;
        public override void SwordDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, null, null);
        }
        public override void ShieldDraw(SpriteBatch sb, ref Color lightColor)
        {
            SwordShieldProj.swordShield_Shield.Draw(sb, lightColor);
        }
    }
}
