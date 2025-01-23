using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeaponSkill.Weapons.DualBlades.Skills.DualBladesSwing;

namespace WeaponSkill.Weapons.SwordShield.Skills
{
    /// <summary>
    /// 后撤步
    /// </summary>
    public class SwordShield_BackMove : BasicSwordShieldSkill
    {
        public SwordShield_BackMove(SwordShieldProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            swingHelper.Change_Lerp(Vector2.UnitX.RotatedBy(0.4), 1, Vector2.One, 0.2f, 0f);
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SetSwingActive();
            Projectile.spriteDirection = player.direction;
            swingHelper.SwingAI(SwordShieldProj.SwingLength, player.direction, 0);
            switch ((int)Projectile.ai[0])
            {
                case 0: // 前摇
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        Projectile.extraUpdates = 1;
                        player.velocity.Y = -3;
                        break;
                    }
                case 1: // 后跳
                    {
                        player.velocity.X = -player.direction * 10;
                        if (Projectile.ai[1]++ > 30)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                        }
                        break;
                    }
                case 2: // 后摇
                    {
                        if (Projectile.ai[1]++ > 30)
                        {
                            SkillTimeOut = true;
                        }
                        break;
                    }
            }
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction));
            #region 盾的更新
            player.itemRotation = (player.velocity.X * 0.1f);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            SwordShieldProj.swordShield_Shield.Update(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, player.itemRotation), player.direction, 0);

            #endregion
        }
        public override bool ActivationCondition() => player.controlUseTile;
        public override bool SwitchCondition() => Projectile.ai[0] >= 2;
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
