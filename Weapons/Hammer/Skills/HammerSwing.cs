using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.Hammer.Skills
{
    public class HammerSwing : BasicHammerSkill
    {
        public HammerSwing(HammerProj hammerProj, Func<bool> changeCondition) : base(hammerProj)
        {
            ChangeCondition = changeCondition;
        }
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float VisualRotation;
        /// <summary>
        /// 用于计时器变化的委托
        /// </summary>
        public Func<float,float> TimeChangeFunc;
        /// <summary>
        /// ai1的上限
        /// </summary>
        public float TimeChangeMax;
        /// <summary>
        /// 挥舞的弧度
        /// </summary>
        public float SwingRot;
        /// <summary>
        /// 为true默认正方向 false则为反
        /// </summary>
        public bool SwingDirectionChange = true;
        /// <summary>
        /// 切换技能的条件
        /// </summary>
        public Func<bool> ChangeCondition;
        public const float CHANGE_LERP_SPEED = 0.105f;
        public float AddDamage;
        public Action SwingAI;
        public override void AI()
        {
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            SwingAI?.Invoke();
            switch ((int)Projectile.ai[0])
            {
                case 0: // 蓄力
                    {
                        PreAttack = true;
                        Projectile.ai[1]++;
                        swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(hammerProj.SwingLength, player.direction, 0);
                        if (Projectile.ai[1] > 15)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                        }
                        break;
                    }
                case 1: // 挥舞
                    {
                        PreAttack = false;
                        Projectile.extraUpdates = 2;
                        Projectile.ai[1]++;
                        if(Projectile.numHits > 0)
                        {
                            Projectile.numHits--;
                            Projectile.ai[1] -= 0.8f;
                        }
                        float Time = Projectile.ai[1] / TimeChangeMax; // 你知道我要干什么
                        if(Time > 1)
                        {
                            Projectile.ai[0]++;
                        }
                        Time = TimeChangeFunc.Invoke(Time);
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(hammerProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt()); // 挥舞
                        HammerProj.DrawHammerSwingShader_Index.Add(Projectile.whoAmI);
                        break;
                    }
                case 2: // 挥舞结束
                    {
                        float Time = Projectile.ai[1] / TimeChangeMax; // 你知道我要干什么
                        Time = TimeChangeFunc.Invoke(Time);
                        Projectile.extraUpdates = 0;

                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(hammerProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        HammerProj.DrawHammerSwingShader_Index.Add(Projectile.whoAmI);
                        Projectile.ai[2]++;
                        if (Projectile.ai[2] > 30)
                        {
                            SkillTimeOut = true;
                        }
                        break;
                    }
            }
        }
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => Projectile.ai[2] > 10 && Projectile.ai[0] > 0;
        public override bool? CanDamage() => (int)Projectile.ai[0] == 1 && Projectile.ai[1] / TimeChangeMax > 0.4f;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return swingHelper.GetColliding(targetHitbox);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += AddDamage;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if(Projectile.numHits < 15) Projectile.numHits += 3;
            TheUtility.SetPlayerImmune(player);
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            //swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) => new Color(1f,1f,1f,0));
            swingHelper.Swing_Draw_ItemAndAfterimage(lightColor, (x) => new Color(1f, 1f, 1f, 0.1f * x) * 0.2f);
            return false;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            TheUtility.ResetProjHit(Projectile);
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            Projectile.rotation = 0;
            Projectile.extraUpdates = 0;
        }
    }
}
