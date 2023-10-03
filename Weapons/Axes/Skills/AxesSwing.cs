using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.NPCs;
using WeaponSkill.Weapons.BroadSword.Skills;
using WeaponSkill.Weapons.Spears;

namespace WeaponSkill.Weapons.Axes.Skills
{
    public class AxesSwing : BasicAxesSkill
    {
        public AxesSwing(AxesProj axeProj, Func<bool> changeCondition) : base(axeProj)
        {
            ChangeCondition = changeCondition;
        }
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float VisualRotation;
        public Func<float> TimeChange;
        public float TimeChangeMax;
        public float SwingRot;
        public Action<NPC, NPC.HitInfo, int> OnHitFunc;
        public Action<AxesSwing> SwingAction;
        public Func<bool> ChangeCondition;
        public float AddDamage;
        public float AddKn;
        public Func<NPC, NPC.HitModifiers, NPC.HitModifiers> ModifyHitNPCFunc;
        public List<NPC> HitNPC = new();
        public Vector2 KnDir;
        /// <summary>
        /// 为true默认正方向 false则为反
        /// </summary>
        public bool SwingDirectionChange = true;
        /// <summary>
        /// 第一次命中目标
        /// </summary>
        public bool OnHitTarget;
        /// <summary>
        /// 可以卡肉
        /// </summary>
        public bool CanStuck;
        public override void AI()
        {
            SwingAction?.Invoke(this);
            Projectile.rotation = 0;
            Projectile.extraUpdates = 2;
            player.itemLocation = Projectile.Center;
            swingHelper.Change(StartVel, VelScale, VisualRotation);
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            if (Projectile.ai[2] > 120 || !OnHitTarget)
            {
                Projectile.ai[0] += TimeChange.Invoke() * player.GetWeaponAttackSpeed(AxesProj.SpawnItem);
                if (Projectile.ai[2] > 40)
                {
                    Projectile.extraUpdates += 3;
                }
            }
            else if (OnHitTarget)
            {
                Projectile.ai[2]++;
                player.velocity *= 0;
                if (Projectile.ai[2] > 120)
                {
                    HitNPC.ForEach(x =>
                    {
                        x.GetGlobalNPC<WeaponSkillGlobalNPC>().CanUpdate = true;
                    });
                }
            }
            float Time = AxesProj.TimeChange(Projectile.ai[0] / TimeChangeMax);
            if (Time > 1)
            {
                Projectile.extraUpdates = 0;
                Projectile.ai[1]++;
                Time = 1 + (Projectile.ai[0] / TimeChangeMax) * 0.01f;
                swingHelper.SetNotSaveOldVel();
            }
            swingHelper.ProjFixedPlayerCenter(player, 0, true, false);
            swingHelper.SwingAI(AxesProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());

            if (Time > 1.02f)
            {
                SkillTimeOut = true;
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Projectile.rotation = 0;
            Effect effect = WeaponSkill.SwingEffect.Value;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * projection);
            effect.Parameters["uColorChange"].SetValue(0.95f);
            Main.graphics.GraphicsDevice.Textures[1] = AxesProj.DrawColorTex;
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, WeaponSkill.SwingTex.Value, (_) => new Color(255, 255, 255, 0), effect, (_) => 0);
            return false;
        }
        public override bool? CanDamage() => !OnHitTarget || (OnHitTarget && Projectile.ai[2] > 40);
        public override bool SwitchCondition() => Projectile.ai[1] > 15;
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (CanStuck)
            {
            }
            if(ModifyHitNPCFunc != null)
            {
                modifiers = ModifyHitNPCFunc.Invoke(target, modifiers);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] <= 40 && CanStuck && !target.immortal)
            {
                Vector2 dir = KnDir;
                KnDir.X *= player.direction;
                target.velocity = dir * 5 * AddKn;
                target.GetGlobalNPC<WeaponSkillGlobalNPC>().CanUpdate = false;
                HitNPC.Add(target);
            }
            if (CanStuck && Projectile.ai[2] <= 40) OnHitTarget = true;
            OnHitFunc?.Invoke(target, hit, damageDone);
        }
        public override void OnSkillActive()
        {
            Projectile.rotation = 0;
            OnHitTarget = false;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            SkillTimeOut = false;
            TheUtility.ResetProjHit(Projectile);
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            Projectile.extraUpdates = 0;
            TheUtility.ResetProjHit(Projectile);
        }
    }
}
