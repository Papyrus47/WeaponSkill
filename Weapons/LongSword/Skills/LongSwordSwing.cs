using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.LongSword.Skills
{
    public class LongSwordSwing : BasicLongSwordSkill
    {
        public LongSwordSwing(LongSwordProj longSword, Func<bool> activationConditionFunc) : base(longSword)
        {
            ChangeCondition = activationConditionFunc;
        }
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float VisualRotation;
        /// <summary>
        /// 切换技能的条件
        /// </summary>
        public Func<bool> ChangeCondition;
        public float SwingRot;
        /// <summary>
        /// 为true默认正方向 false则为反
        /// </summary>
        public bool SwingDirectionChange = true;
        public Action SwingAI;
        public Action<NPC,NPC.HitInfo,int> OnHit;

        public const int SLASH_TIME = 36;
        public const int PREATTACK_TIME = 12;
        public const int TIMEOUT_TIME = 23;
        public const float CHANGE_LERP_SPEED = 0.35f;
        public override void AI()
        {
            LongSword.swordScabbard.DrawPos = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(player.direction * -10, 0);
            LongSword.swordScabbard.Dir = player.direction;
            SwingAI?.Invoke();
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            switch ((int)Projectile.ai[0])
            {
                case 0: // 准备挥舞
                    {
                        PreAttack = true;
                        swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(LongSword.SwingLength, player.direction, 0);
                        if(Projectile.ai[1]++ > 8)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 1;
                            TheUtility.ResetProjHit(Projectile);
                        }
                        break;
                    }
                case 1: // 挥舞进行
                    {
                        Projectile.ai[1]++;
                        float Time = LongSword.TimeChange(Projectile.ai[1] / SLASH_TIME);
                        if (Time > 1)
                        {
                            Projectile.ai[0]++;
                        }
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(LongSword.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        LongSwordProj.DrawLongSwordSwingShader_Index.Add(Projectile.whoAmI);
                        break;
                    }
                case 2: // 后摇
                    {
                        float Time = LongSword.TimeChange(Projectile.ai[1] / SLASH_TIME);
                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(LongSword.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        LongSwordProj.DrawLongSwordSwingShader_Index.Add(Projectile.whoAmI);
                        Projectile.ai[2]++;
                        if (Projectile.ai[2] > TIMEOUT_TIME)
                        {
                            SkillTimeOut = true;
                        }
                        break;
                    }
            }
        }
        public override bool? CanDamage()
        {
            return (int)Projectile.ai[0] == 1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            OnHit?.Invoke(target,hit,damageDone);
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) => !LongSword.InSpiritAttack ? new Color(0.2f, 0.2f, 0.2f, 0f) : new Color(2f,2f,2f, 0f));
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return swingHelper.GetColliding(targetHitbox);
        }
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => (int)Projectile.ai[0] == 2 && Projectile.ai[2] > 9;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            Projectile.extraUpdates = 0;
        }
    }
}
