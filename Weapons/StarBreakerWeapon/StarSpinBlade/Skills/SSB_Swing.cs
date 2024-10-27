using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Weapons.Staffs.Skills;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade.Skills
{
    public class SSB_Swing : SSB_BasicSkills
    {
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float SwingRot;
        /// <summary>
        /// 挥舞的朝向
        /// </summary>
        public bool SwingDirectionChange;
        public float VisualRotation;
        /// <summary>
        /// 攻击前时间
        /// </summary>
        public int PreTime;
        /// <summary>
        /// 挥舞所需要时间
        /// </summary>
        public float SwingTime;
        /// <summary>
        /// 挥舞变化方式
        /// </summary>
        public Func<float, float> TimeChange;
        /// <summary>
        /// 如何使用这个技能
        /// </summary>
        public Func<bool> ChangeCondition;
        /// <summary>
        /// 在这里使用特定AI
        /// </summary>
        public Action<SSB_Swing> OnUse;
        /// <summary>
        /// 结束时候调用
        /// </summary>
        public Action<SSB_Swing> OnEnd;
        /// <summary>
        /// 命中时候
        /// </summary>
        public Action<NPC,NPC.HitInfo,int> OnHit;
        /// <summary>
        /// 为true为正旋斩,flase为逆旋斩
        /// </summary>
        public bool IsTrueSlash;
        /// <summary>
        /// 回旋
        /// </summary>
        public int SpinValue;
        /// <summary>
        /// 伤害次数
        /// </summary>
        public int DmgCounts;
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDmg = 1;
        public SSB_Swing(StarSpinBladeProj modProjectile, Func<bool> changeCondition, Func<float, float> timeChange) : base(modProjectile)
        {
            ChangeCondition = changeCondition;
            TimeChange = timeChange;
        }

        public override void AI()
        {
            Projectile.spriteDirection = Player.direction * IsTrueSlash.ToDirectionInt() * SwingDirectionChange.ToDirectionInt();

            switch ((int)Projectile.ai[0])
            {
                case 0: // 渐变
                    SwingHelper.SetRotVel(0);
                    PreAtk = true;
                    Projectile.ai[1]++;
                    float time = Projectile.ai[1] / PreTime;
                    SwingHelper.Change_Lerp(StartVel, time, VelScale, time * 2, VisualRotation, time);
                    SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                    SwingHelper.SwingAI(StarSpinBladeProj.SwingLenght, Player.direction, 0);
                    if(time > 1)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0]++;
                        GetStarSpinBladeItem().SpinValue += SpinValue * IsTrueSlash.ToDirectionInt();
                        SoundEngine.PlaySound(SoundID.Item1 with { pitch = -0.5f,MaxInstances = 3 }, Player.position);
                        SoundEngine.PlaySound(SoundID.Item1 with { pitch = -0.9f, MaxInstances = 3 }, Player.position);
                    }
                    break;
                case 1: // 挥舞
                    PreAtk = false;
                    SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                    Projectile.extraUpdates = 4;
                    Projectile.ai[1]++;
                    float swingTime = Projectile.ai[1] / (SwingTime * 3);
                    if (swingTime > 1)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0]++;
                        break;
                    }

                    if(swingTime > 0.5f) // 预输入处理
                    {
                        StarSpinBladeProj.PreAtk(); 
                    }
                    OnUse?.Invoke(this);
                    swingTime = TimeChange.Invoke(swingTime);

                    SwingHelper.SwingAI(StarSpinBladeProj.SwingLenght, Player.direction, swingTime * SwingRot * SwingDirectionChange.ToDirectionInt());
                    break;
                case 2: // 超时
                    Projectile.ai[1]++;
                    Projectile.extraUpdates = 0;
                    if (Projectile.ai[1] > 60)
                    {
                        SkillTimeOut = true;
                        break;
                    }
                    if (Projectile.ai[1] > 30)
                    {
                        CanChangeToStopActionSkill = true;
                        StarSpinBladeProj.ResetPreAtk = true;
                    }
                    SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                    SwingHelper.SetNotSaveOldVel();
                    OnEnd?.Invoke(this);
                    SwingHelper.SwingAI(StarSpinBladeProj.SwingLenght, Player.direction, SwingRot * SwingDirectionChange.ToDirectionInt() * (1 + Projectile.ai[1] * 0.001f));
                    break;
            }
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            TheUtility.ResetProjHit(Projectile);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => SwingHelper.GetColliding(targetHitbox);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 3, 2, 2));
            OnHit?.Invoke(target, hit, damageDone);
            if (DmgCounts > 0)
            {
                for (int i = DmgCounts; i >= 0; i--)
                {
                    SlashDamage.SlashDamageOnHit();
                    Player.ApplyDamageToNPC(target, hit.SourceDamage, 0f, hit.HitDirection, hit.Crit, hit.DamageType, false);
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ActionDmg >= 1)
                modifiers.SourceDamage += ActionDmg - 1;
            else
                modifiers.SourceDamage -= 1 - ActionDmg;
        }
        public override bool? CanDamage()
        {
            return (int)Projectile.ai[0] == 1;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[0] >= 2 && Projectile.ai[1] > 5;
        }
        public override bool ActivationCondition()
        {
            if (SpinValue < 0 && ((IsTrueSlash && GetStarSpinBladeItem().SpinValue < SpinValue) || (!IsTrueSlash && GetStarSpinBladeItem().SpinValue > SpinValue)))
            {
                return false;
            }
            return ChangeCondition.Invoke();
        }

        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            if(!ModAsset.SwingTex2_Async.IsLoaded)
                _ = ModAsset.SwingTex2;

            Effect effect = ModAsset.StarSpinBladeShader.Value;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * projection);
            effect.Parameters["uColorChange"].SetValue(0.95f);
            Main.graphics.GraphicsDevice.Textures[1] = ModAsset.Stars1.Value;

            SwingHelper.Swing_Draw_ItemAndTrailling(lightColor, ModAsset.SwingTex2_Async.Value, (_) => Color.Purple with { A = 0 },effect);
            return false;
        }
    }
}
