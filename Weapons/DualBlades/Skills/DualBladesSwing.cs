using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.DualBlades.Skills
{
    /// <summary>
    /// 双刀唯一的攻击模式--挥舞
    /// </summary>
    public class DualBladesSwing : BasicDualBladesSkill
    {
        public DualBladesSwing(DualBladesProj dualBladesProj, SwingSet swingSet, DoubleSwingSpeed swingSetSpeed, Func<bool> changeCondition) : base(dualBladesProj)
        {
            ChangeCondition = changeCondition;
            this.swingSet = swingSet;
            this.swingSetSpeed = swingSetSpeed;
        }
        public enum DoubleSwingSpeed : byte
        {
            /// <summary>
            /// 相同速度
            /// </summary>
            Same = 0,
            /// <summary>
            /// 不同速度,背着的手慢
            /// </summary>
            BackLow = 1,
            /// <summary>
            /// 不同速度,手上的手慢
            /// </summary>
            HeldLow = 2,
            /// <summary>
            /// 没有速度
            /// </summary>
            NoSpeed = 255
        }
        public enum SwingSet : byte
        {
            /// <summary>
            /// 双刀同时挥舞
            /// </summary>
            TwoBlades = 0,
            /// <summary>
            /// 背着的手单独挥舞
            /// </summary>
            BackBlades = 1,
            /// <summary>
            /// 拿着的手单独挥舞
            /// </summary>
            HeldBlades = 2,
            /// <summary>
            /// 相反的挥舞
            /// </summary>
            IntersectBlades = 3,
            /// <summary>
            /// 不舞
            /// </summary>
            None = 255
        }
        /// <summary>
        /// 切换技能的条件
        /// </summary>
        public Func<bool> ChangeCondition;
        public SwingSet swingSet;
        public DoubleSwingSpeed swingSetSpeed;
        public Vector2 StartVel;
        public Vector2 VelScale;
        /// <summary>
        /// 双刀如有另一只刀未使用,则为那只刀的手持
        /// </summary>
        public Vector2 DefaultVel;
        public float VisualRotation;
        public float SwingRot;
        public bool SwingDirectionChange;
        public Func<float> AITimeChange;
        public bool CanChangeSkill;
        public override void AI()
        {
            Projectile.extraUpdates = 2;
            Projectile.spriteDirection = -player.direction * SwingDirectionChange.ToDirectionInt();
            SwingHelperGetSetting();
            TimeChange(out float Time,out float Time1);
            player.heldProj = Projectile.whoAmI;
            float time = BladeSwing(Time,Time1);
            if(time >= 2f)
            {
                SkillTimeOut = true;
            }
            else if(time > 1f)
            {
                CanChangeSkill = true;
            }
            AIAction?.Invoke(this);
        }
        public virtual void SwingHelperGetSetting()
        {
            switch (swingSet)
            {
                case SwingSet.TwoBlades:
                    {
                        HeldBlade.SwingHelper.Change(StartVel, VelScale, VisualRotation);
                        BackBlade.SwingHelper.Change(StartVel, VelScale, VisualRotation);
                        HeldBlade.SwingDirectionChange = BackBlade.SwingDirectionChange = SwingDirectionChange;
                        HeldBlade.SwingRot = BackBlade.SwingRot = SwingRot;

                        BackBlade.spDir = HeldBlade.spDir = -Projectile.spriteDirection;
                        BackBlade.SwingRot += 0.2f;
                        break;
                    }
                case SwingSet.HeldBlades:
                    {
                        HeldBlade.SwingHelper.Change(StartVel, VelScale, VisualRotation);
                        HeldBlade.SwingDirectionChange = SwingDirectionChange;
                        HeldBlade.SwingRot = SwingRot;

                        BackBlade.SwingHelper.Change(DefaultVel, VelScale, VisualRotation);
                        BackBlade.spDir = HeldBlade.spDir = -Projectile.spriteDirection;
                        BackBlade.SwingRot += 0.2f;
                        break;
                    }
                case SwingSet.BackBlades:
                    {
                        BackBlade.SwingHelper.Change(StartVel, VelScale, VisualRotation);
                        BackBlade.SwingDirectionChange = SwingDirectionChange;
                        BackBlade.SwingRot = SwingRot;

                        HeldBlade.SwingHelper.Change(DefaultVel, VelScale, VisualRotation);
                        BackBlade.spDir = HeldBlade.spDir = -Projectile.spriteDirection;
                        BackBlade.SwingRot += 0.2f;
                        break;
                    }
                case SwingSet.IntersectBlades:
                    {
                        HeldBlade.SwingHelper.Change(StartVel, VelScale, VisualRotation);
                        BackBlade.SwingHelper.Change(new Vector2(StartVel.X,-StartVel.Y), VelScale, VisualRotation);
                        HeldBlade.SwingDirectionChange = !SwingDirectionChange;
                        BackBlade.SwingDirectionChange = SwingDirectionChange;
                        HeldBlade.SwingRot = BackBlade.SwingRot = SwingRot;

                        BackBlade.spDir = Projectile.spriteDirection;
                        HeldBlade.spDir = -Projectile.spriteDirection;
                        break;
                    }

                case SwingSet.None:
                default:
                    break;
            }
        }
        public virtual void TimeChange(out float Time,out float Time1)
        {
            Projectile.ai[0] += AITimeChange.Invoke();
            Projectile.ai[1] += AITimeChange.Invoke();
            Time = bladesProj.TimeChange(Projectile.ai[0]);
            Time1 = bladesProj.TimeChange(Projectile.ai[1]);
            const float change = 0.15f;
            switch (swingSetSpeed)
            {
                case DoubleSwingSpeed.BackLow:
                    {
                        Time1 = bladesProj.TimeChange(Projectile.ai[1] - change);

                        break;
                    }
                case DoubleSwingSpeed.HeldLow:
                    {
                        Time = bladesProj.TimeChange(Projectile.ai[0] - change);
                        break;
                    }
            }
            if (swingSet == SwingSet.BackBlades) Time = 0;
            else if (swingSet == SwingSet.HeldBlades) Time1 = 0;
        }
        /// <summary>
        /// 刃的挥舞处理
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="Time1"></param>
        /// <returns>最小的时间</returns>
        public virtual float BladeSwing(float Time,float Time1)
        {
            float min = 0;
            switch (swingSetSpeed)
            {
                case DoubleSwingSpeed.Same:
                    {
                        HeldBlade.AI(Time);
                        BackBlade.AI(Time1);
                        min = Time;
                        break;
                    }
                case DoubleSwingSpeed.BackLow:
                    {
                        HeldBlade.AI(Time);
                        BackBlade.AI(Time1);
                        min = Time1;
                        break;
                    }
                case DoubleSwingSpeed.HeldLow:
                    {
                        HeldBlade.AI(Time);
                        BackBlade.AI(Time1);
                        min = Time;
                        break;
                    }
                case DoubleSwingSpeed.NoSpeed:
                default:
                    {
                        break;
                    }
            }
            Vector2 frontVel = HeldBlade.vel.Value;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathF.Atan2(frontVel.Y * player.direction,frontVel.X * player.direction) - MathHelper.PiOver2 * player.direction);
            Vector2 backVel = BackBlade.vel.Value;
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, MathF.Atan2(backVel.Y * player.direction, backVel.X * player.direction) - MathHelper.PiOver2 * player.direction);
            return min;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            HeldBlade.Draw(sb, lightColor);
            return false;
        }
        public override void BackDraw(SpriteBatch spriteBatch, Color color)
        {
            BackBlade.Draw(spriteBatch, color);
        }
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => CanChangeSkill;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return HeldBlade.SwingHelper.GetColliding(targetHitbox) || BackBlade.SwingHelper.GetColliding(targetHitbox);
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (swingSet)
            {
                case SwingSet.TwoBlades:
                case SwingSet.IntersectBlades:
                    {
                        if (HeldBlade.NPCHit[target.whoAmI])
                        {
                            BackBlade.NPCHit[target.whoAmI] = true;
                        }
                        HeldBlade.NPCHit[target.whoAmI] = true;
                        break;
                    }
                case SwingSet.HeldBlades:
                case SwingSet.BackBlades:
                    {
                        HeldBlade.NPCHit[target.whoAmI] = true;
                        BackBlade.NPCHit[target.whoAmI] = true;
                        break;
                    }

                case SwingSet.None:
                default:
                    break;
            }
            TheUtility.SetPlayerImmune(player,16);
        }
        public override void OnSkillActive()
        {
            Projectile.rotation = 0;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            SkillTimeOut = false;
            HeldBlade.ResetHit();
            BackBlade.ResetHit();
            CanChangeSkill = false;
            TheUtility.Player_ItemCheck_Shoot(player, player.HeldItem, Projectile.damage);
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            Projectile.extraUpdates = 0;
            HeldBlade.ResetHit();
            BackBlade.ResetHit();
            CanChangeSkill = false;
        }
    }
}
