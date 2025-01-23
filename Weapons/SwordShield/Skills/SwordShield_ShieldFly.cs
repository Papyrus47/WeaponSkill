using Microsoft.Xna.Framework.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.SwordShield.Skills
{
    public class SwordShield_ShieldFly : BasicSwordShieldSkill
    {
        public SwordShield_ShieldFly(SwordShieldProj proj) : base(proj)
        {
        }
        public override void AI()
        {
            swingHelper.Change_Lerp(Vector2.UnitX.RotatedBy(0.4), 1, Vector2.One, 0.2f, 0f);
            swingHelper.ProjFixedPlayerCenter(player, 0, true);
            swingHelper.SetSwingActive();
            Projectile.spriteDirection = player.direction;
            swingHelper.SwingAI(SwordShieldProj.SwingLength, player.direction, 0);
            TheUtility.ResetProjHit(Projectile);
            switch ((int)Projectile.ai[0])
            {
                case 0: // 飞！
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        player.velocity.Y = -15;
                        player.velocity.X *= 0.1f;
                        break;
                    }
                case 1: // 后跳
                    {
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
            player.itemRotation = MathHelper.Pi;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            SwordShieldProj.swordShield_Shield.Update(player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, player.itemRotation), player.direction, 0);

            #endregion
        }
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => player.Hitbox.Intersects(targetHitbox);
        public override bool ActivationCondition() => player.controlUseTile;
        public override bool SwitchCondition() => Projectile.ai[0] >= 2;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 3;
        }
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
