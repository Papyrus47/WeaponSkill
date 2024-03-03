using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_SwordSwing : FrostFist_BasicSword
    {
        public FrostFist_SwordSwing(FrostFistProj modProjectile, Func<bool> changeCondition) : base(modProjectile)
        {
            ChangeCondition = changeCondition;
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
        public Action<NPC, NPC.HitInfo, int> OnHit;

        /// <summary>
        /// 攻击前摇所需要的时间
        /// </summary>
        public int PreAtkTime;
        /// <summary>
        /// 攻击所需要的时间
        /// </summary>
        public float AtkTime;
        /// <summary>
        /// 攻击后摇时间(用这个来切换技能,等待固定为后摇的1/3时)
        /// </summary>
        public int PostAtkTime;
        /// <summary>
        /// 增加的伤害百分比
        /// </summary>
        public float AddDmg;
        /// <summary>
        /// 用于时间变化的函数
        /// </summary>
        public Func<float,float> TimeChange;
        /// <summary>
        /// 可以改变朝向
        /// </summary>
        public bool CanChangeDir;
        public override void AI()
        {
            SwingAI?.Invoke();
            Projectile.spriteDirection = Player.direction * SwingDirectionChange.ToDirectionInt();
            for (int i = 0; i < 4; i++)
            {
                Dust dust = FrostFistDust();
                dust.velocity = -Projectile.velocity.SafeNormalize(default) * i * 0.6f;
                dust.position = Player.HandPosition.Value + Projectile.velocity.SafeNormalize(default) * Player.width;
            }
            switch ((int)Projectile.ai[0])
            {
                case 0: // 攻击的前摇
                    {
                        PreAtk = true;
                        float time = 3f / PreAtkTime;
                        swingHelper.Change_Lerp(StartVel, time, VelScale, time, VisualRotation, time);
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, 0);
                        if (Projectile.ai[1]++ > PreAtkTime) // 大于前摇时间
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 2;
                            TheUtility.ResetProjHit(Projectile);
                        }
                        break;
                    }
                case 1: // 挥舞
                    {
                        PreAtk = false;
                        Projectile.ai[1]++;
                        Player.heldProj = Projectile.whoAmI;
                        float Time = TimeChange.Invoke(Projectile.ai[1] / AtkTime);
                        if(Time > 1)
                        {
                            Projectile.ai[0]++;
                        }
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());

                        //for (int i = 0; i < 30; i++)
                        //{
                        //    Vector2 center = Player.HandPosition.Value - Player.velocity;
                        //    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.Frost, Player.direction * 2, 0, 150, default, 1.3f);
                        //    dust.position = center;
                        //    dust.velocity *= 0;
                        //    dust.noGravity = true;
                        //    dust.fadeIn = 1;
                        //    dust.velocity = new Vector2(Projectile.velocity.Y,-Projectile.velocity.X).SafeNormalize(default).RotatedBy(i / 30f * MathHelper.Pi) * 6;
                        //    dust.velocity += Player.velocity;
                        //    dust.position = Projectile.Center + Projectile.velocity * i * 0.035f;
                        //}
                        break;
                    }
                case 2: // 后摇
                    {
                        Projectile.extraUpdates = 0;
                        float Time = TimeChange.Invoke(Projectile.ai[1] / AtkTime);
                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        Projectile.ai[2]++;
                        if (Projectile.ai[2] > PostAtkTime)
                        {
                            SkillTimeOut = true;
                        }
                        else if (Projectile.ai[2] > PostAtkTime / 3)
                        {
                            CanChangeToStopActionSkill = true;
                        }
                        break;
                    }
            }
        }
        public override bool? CanDamage() => (int)Projectile.ai[0] == 1;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            FrostFist.AddSwordRender = true;
            //swingHelper.Swing_Draw_All(lightColor, WeaponSkill.SwingTex.Value, (factor) => new Color(1f, 1, 1, 0) * 0.2f * MathF.Pow(1f - factor, 2f), (_) => new Color(0.2f, 0.4f, 0.8f, 0f), -1);
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, WeaponSkill.SwingTex.Value, (_) => new Color(1f,1f,1f, 0f));
            return false;
        }
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => (int)Projectile.ai[0] == 2;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.SourceDamage += AddDmg;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            OnHit?.Invoke(target,hit,damageDone);
            TheUtility.SetPlayerImmune(Player);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            TheUtility.ResetProjHit(Projectile);
            CanChangeToStopActionSkill = false;
            Projectile.rotation = 0;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            TheUtility.ResetProjHit(Projectile);
            CanChangeToStopActionSkill = false;
            Projectile.extraUpdates = 0;
            swingHelper.SetRotVel(0);
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;

            if (CanChangeDir)
            {
                Vector2 vel = (Main.MouseWorld - Projectile.Center);
                Player.ChangeDir((vel.X > 0).ToDirectionInt());
            }
        }
    }
}
