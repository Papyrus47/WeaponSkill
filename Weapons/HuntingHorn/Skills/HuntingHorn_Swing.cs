using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Hammer;
using WeaponSkill.Weapons.InsectStaff;

namespace WeaponSkill.Weapons.HuntingHorn.Skills
{
    public class HuntingHorn_Swing : BasicHuntingHornSkill
    {
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
        public int SwingTimeMax = 30;
        public int PreSwingTimeMax = 20;
        public int TimeoutTimeMax = 60;
        /// <summary>
        /// 演奏
        /// </summary>
        public bool IsPlay;
        /// <summary>
        /// 属于重奏
        /// </summary>
        public bool IsReplay;
        public const float CHANGE_LERP_SPEED = 0.35f;
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDmg = 1f;
        public HuntingHornMelody.MelodyType melodyType;
        /// <summary>
        /// 重奏用
        /// </summary>
        public Queue<HuntingHornBuff> SaveBuff;
        public HuntingHorn_Swing(HuntingHornProj proj, Func<bool> changeCondition) : base(proj)
        {
            ChangeCondition = changeCondition;
        }
        public override void AI()
        {
            SwingAI?.Invoke();
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            switch ((int)Projectile.ai[0])
            {
                case 0: // 准备挥舞
                    PreAtk = true;
                    swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, 0);
                    if (Projectile.ai[1]++ > PreSwingTimeMax)
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        Projectile.extraUpdates = 0;
                        TheUtility.Player_ItemCheck_Shoot(player, HuntingHornProj.SpawnItem, Projectile.damage);
                        TheUtility.ResetProjHit(Projectile);
                        SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.6f }, player.position);
                        if (melodyType != default)
                        {
                            HuntingHornGlobalItem huntingHornGlobalItem = HuntingHornProj.SpawnItem.GetGlobalItem<HuntingHornGlobalItem>();
                            HuntingHornMelody hornMelody = huntingHornGlobalItem.hornMelody;
                            hornMelody.melodies.Enqueue(melodyType);
                            HuntingHornBuff item = hornMelody.FindMelody(player, Projectile);
                            if(item != null)
                                huntingHornGlobalItem.huntingHornBuffs.Enqueue(item);

                        }
                    }
                    break;
                case 1: // 挥舞
                    Projectile.extraUpdates = 1;
                    
                    if (Projectile.ai[1] < SwingTimeMax)
                        Projectile.ai[1]++;
                    float Time = HuntingHornProj.TimeChange(Projectile.ai[1] / SwingTimeMax);
                    if (Time >= 1)
                    {
                        Projectile.ai[0]++;
                        Projectile.ai[1] = 0;
                        if (IsPlay)
                        {
                            Projectile.ai[0] = 1;
                            HuntingHornGlobalItem huntingHornGlobalItem = HuntingHornProj.SpawnItem.GetGlobalItem<HuntingHornGlobalItem>();
                            Queue<HuntingHornBuff> buffs = huntingHornGlobalItem.huntingHornBuffs;
                            if (buffs.Count > 0)
                            {
                                huntingHornGlobalItem.hornMelody.melodies.Clear();
                                if (!IsReplay)
                                {
                                    HuntingHornBuff huntingHornBuff = buffs.Dequeue();
                                    huntingHornBuff.OnPlay(player, Projectile);
                                    SaveBuff.Enqueue(huntingHornBuff);
                                }
                                else
                                {
                                    while(buffs.Count > 0)
                                    {
                                        HuntingHornBuff huntingHornBuff = buffs.Dequeue();
                                        huntingHornBuff.OnPlay(player, Projectile);
                                    }
                                }
                                SoundEngine.PlaySound(SoundID.Item139 with { Pitch = -0.3f, Volume = 0.3f }, player.position);
                                Projectile.ai[2] = 1;
                            }
                            else
                                Projectile.ai[0]++;
                        }
                    }
                    if (Projectile.ai[2] == 1)
                    {
                        Time = 1;
                    }
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                    HammerProj.DrawHammerSwingShader_Index.Add(Projectile.whoAmI);
                    break;
                case 2: // 跳出
                    if (Projectile.ai[1]++ > TimeoutTimeMax)
                    {
                        SkillTimeOut = true;
                    }
                    swingHelper.ProjFixedPlayerCenter(player, 0, true);
                    swingHelper.SwingAI(HuntingHornProj.SwingLength, player.direction, SwingRot * SwingDirectionChange.ToDirectionInt());
                    break;
            }
        }
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => Projectile.ai[0] >= 2;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndAfterimage(lightColor, (x) => new Color(1f, 1f, 1f, 0.1f * x) * 0.2f);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += ActionDmg - 1;
            player.SetImmuneTimeForAllTypes(8);
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            //if (WeaponSkill.BowSlidingStep.Current && !ChangeIsTrue)
            //{
            //    melodyType = HuntingHornMelody.MelodyType.SP;
            //}
            //else

            if (player.controlUseItem && player.controlUseTile)
            {
                melodyType = HuntingHornMelody.MelodyType.LeftAndRight;
            }
            else if (player.controlUseItem)
            {
                melodyType = HuntingHornMelody.MelodyType.Left;
            }
            else if (player.controlUseTile)
            {
                melodyType = HuntingHornMelody.MelodyType.Right;
            }
            else 
                melodyType = default;
            if (IsPlay)
                SaveBuff ??= new();
            SaveBuff?.Clear();
        }
        public override void OnSkillDeactivate()
        {
            if (IsPlay && SaveBuff.Count > 0 && !IsReplay && !SkillTimeOut)
            {
                HuntingHornGlobalItem huntingHornGlobalItem = HuntingHornProj.SpawnItem.GetGlobalItem<HuntingHornGlobalItem>();
                Queue<HuntingHornBuff> buffs = huntingHornGlobalItem.huntingHornBuffs;
                while (SaveBuff.Count > 0)
                {
                    buffs.Enqueue(SaveBuff.Dequeue());
                }
                SaveBuff.Clear();
            }
            base.OnSkillDeactivate();
        }
    }
}
