using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.ChargeBlade.Skills
{
    public class ChargeBlade_Axe_Swing : ChargeBlade_Axe_Basic
    {
        public ChargeBlade_Axe_Swing(ChargeBladeProj chargeBlade, Func<bool> activationConditionFunc) : base(chargeBlade)
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
        public bool[] Rehit;
        public Action<NPC, NPC.HitInfo, int> OnHit;
        public bool DefAttack;
        public const int SLASH_TIME = 63;
        public const int PREATTACK_TIME = 12;
        public const int TIMEOUT_TIME = 120;
        public const float CHANGE_LERP_SPEED = 0.15f;
        public override void AI()
        {
            base.AI();
            SwingAI?.Invoke(); 
            ChargeBladeProj.chargeBladeGlobal.InAxe = true;
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            switch ((int)Projectile.ai[0])
            {
                case 0: // 准备挥舞
                    {
                        PreAttack = true;
                        swingHelper.Change_Lerp(StartVel, CHANGE_LERP_SPEED, VelScale, CHANGE_LERP_SPEED, VisualRotation, CHANGE_LERP_SPEED);
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, 0);
                        if (Projectile.ai[1]++ > 15)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 1;
                            TheUtility.Player_ItemCheck_Shoot(player, ChargeBladeProj.SpawnItem, Projectile.damage);
                            TheUtility.ResetProjHit(Projectile);
                        }
                        break;
                    }
                case 1: // 挥舞进行
                    {
                        PreAttack = false;
                        if (Projectile.numHits > 0)
                        {
                            Projectile.numHits = 0;
                            if(ChargeBladeProj.chargeBladeGlobal.AxeStrengthening) Projectile.ai[2] = 14;
                            else Projectile.ai[2] = 6;
                        }
                        if (Projectile.ai[2] > 0)
                        {
                            if (--Projectile.ai[2] <= 0 && ChargeBladeProj.chargeBladeGlobal.AxeStrengthening)
                            {
                                TheUtility.ResetProjHit(Projectile);
                            }
                            if (ChargeBladeProj.CurrentSkill is not ChargeBlade_Axe_Swing_Liberate_Super) Projectile.ai[1] += 0.3f;
                            else Projectile.ai[1]++;
                        }
                        else
                        {
                            Projectile.ai[1]++;
                        }
                        player.heldProj = Projectile.whoAmI;
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / SLASH_TIME);
                        if (Time > 1)
                        {
                            Projectile.ai[0]++;
                        }
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        break;
                    }
                case 2: // 后摇
                    {
                        float Time = ChargeBladeProj.TimeChange(Projectile.ai[1] / SLASH_TIME);
                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(ChargeBladeProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());
                        Projectile.ai[2]++;
                        if (Projectile.ai[2] > TIMEOUT_TIME)
                        {
                            SkillTimeOut = true;
                        }
                        break;
                    }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += 0.5f;
            if (Rehit[target.whoAmI])
            {
                modifiers.FinalDamage *= 0.1f;
            }
        }
        public override bool? CanDamage()
        {
            return (int)Projectile.ai[0] == 1;
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (DefAttack && ChargeBladeProj.shield.DefSucceeded && ChargeBladeProj.shield.KNLevel != ChargeBladeShield.KNLevelEnum.Big)
            {
                return ActivationCondition();
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            OnHit?.Invoke(target, hit, damageDone);
            Rehit[target.whoAmI] = true;
            TheUtility.SetPlayerImmune(player,35);
            player.itemAnimation = player.itemAnimationMax;
            player.itemTime = player.itemTimeMax;
            if (ChargeBladeProj.chargeBladeGlobal.AxeStrengthening)
            {
                for (int i = 0; i < 4; i++)
                {
                    var fire = new Particles.Fire(25);
                    fire.SetBasicInfo(null, null, (Projectile.velocity.RotatedBy(MathHelper.PiOver2 * SwingDirectionChange.ToDirectionInt()) * Main.rand.NextFloat(0.02f,0.05f)).RotatedByRandom(0.6), target.Center);
                    Main.ParticleSystem_World_BehindPlayers.Add(fire);
                }
            }
            TheUtility.VillagesItemOnHit(ChargeBladeProj.SpawnItem, player, Projectile.Hitbox, hit.Damage, hit.Knockback, target.whoAmI, damageDone, damageDone);
            if(SwingRot > MathHelper.PiOver2) Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Vector2.UnitY, 3, 5, 2));
            //ItemLoader.OnHitNPC(LongSword.SpawnItem, player,target, hit, damageDone);
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, TextureAssets.Extra[209].Value, (_) => new Color(255, 255, 255, 0));
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox) || ChargeBladeProj.shield.swingHelper.GetColliding(targetHitbox);
        public override bool ActivationCondition()
        {
            if (DefAttack && ChargeBladeProj.DefSucceededTime <= 0) return false;
            return ChangeCondition.Invoke() /*&& (!DefAttack ? true : WeaponSkill.BowSlidingStep.Current)*/;
        }
        public override bool SwitchCondition() => (int)Projectile.ai[0] == 2 && Projectile.ai[2] > 9;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            Projectile.numHits = 0;
            Rehit = new bool[Main.npc.Length];
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            Projectile.extraUpdates = 0;
            Rehit = null;
        }
    }
}
