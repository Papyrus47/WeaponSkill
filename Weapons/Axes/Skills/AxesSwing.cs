using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
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
        /// <summary>
        /// 可蓄力
        /// </summary>
        public bool CanChannel;
        /// <summary>
        /// 蓄力时间
        /// </summary>
        public int ChannelTime;
        /// <summary>
        /// 增加的伤害动作值
        /// </summary>
        public float AddDamage;
        /// <summary>
        /// 增加的击退动作值
        /// </summary>
        public float AddKn;
        public Func<NPC, NPC.HitModifiers, NPC.HitModifiers> ModifyHitNPCFunc;
        /// <summary>
        /// 为true默认正方向 false则为反
        /// </summary>
        public bool SwingDirectionChange = true;
        public override void AI()
        {
            SwingAction?.Invoke(this);
            Projectile.extraUpdates = 2;
            player.itemLocation = Projectile.Center;
            swingHelper.Change(StartVel, VelScale, VisualRotation);
            Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
            Projectile.localAI[0]++;
            if (Projectile.numHits > 0)
            {
                Projectile.numHits--;
                Projectile.ai[0] += TimeChange.Invoke() * player.GetWeaponAttackSpeed(AxesProj.SpawnItem) * 0.1f;
            }
            else if (!ActivationCondition() || Projectile.ai[0] > 0 || !CanChannel || Projectile.ai[2]-- <= 0)
            {
                Projectile.ai[0] += TimeChange.Invoke() * player.GetWeaponAttackSpeed(AxesProj.SpawnItem);
                if(CanChannel && (int)Projectile.ai[2] <= 0)
                {
                    Projectile.extraUpdates = 6;
                }
                if(TimeChangeMax > 3 && Projectile.ai[0] < 0.2f)
                {
                    Projectile.ai[0] -= TimeChange.Invoke() * player.GetWeaponAttackSpeed(AxesProj.SpawnItem) * 0.2f;
                }
            }
            //else if(Projectile.ai[2] <= 0)
            //{

            //}
            float Time = AxesProj.TimeChange(Projectile.ai[0] / TimeChangeMax);
            if (Time > 1)
            {
                Projectile.extraUpdates = 0;
                Projectile.ai[1]++;
                Time = 1 + (Projectile.ai[0] / TimeChangeMax) * 0.01f;
                swingHelper.SetNotSaveOldVel();
            }
            else if ((int)(Projectile.ai[0] / TimeChange.Invoke()) % 3 == 1)
            {
                TheUtility.Player_ItemCheck_EmitUseVisuals(player, AxesProj.SpawnItem, Projectile.Hitbox);
            }
            if (Projectile.ai[0] - 0.1f < 0.1f)
            {
                var sound = SoundID.Item152;
                sound.Pitch = -0.78f;
                sound.Volume = 1f;
                SoundEngine.PlaySound(sound, Projectile.position);
            }
            swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
            swingHelper.SwingAI(AxesProj.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());

            if (Time > 1.015f)
            {
                SkillTimeOut = true;
            }

            if (!AxesProj.InFighting && Projectile.localAI[0] > player.itemTimeMax)
            {
                Projectile.localAI[0] = 0;
                player.itemTime = 0;
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            if (CanChannel && Projectile.ai[0] <= 0)
            {
                Texture2D tex2 = TextureAssets.Extra[174].Value;
                sb.Draw(tex2, player.Center - Main.screenPosition, null, new Color(150, 150, 255, 0) * (1 - Projectile.ai[2] / ChannelTime), 0, tex2.Size() * 0.5f, Math.Max(0, Projectile.ai[2] / ChannelTime) * 2f, SpriteEffects.None, 0f);
            }
            Effect effect = WeaponSkill.SwingEffect.Value;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * projection);
            effect.Parameters["uColorChange"].SetValue(0.95f);
            if (AxesProj.InFighting)
            {
                Main.graphics.GraphicsDevice.Textures[1] = AxesProj.DrawSwingColorTex;
                Vector2[] oldVels = swingHelper.oldVels.Clone() as Vector2[];
                for (int i = 0; i < oldVels.Length; i++)
                {
                    swingHelper.oldVels[i] *= 1.05f;
                }
                swingHelper.Swing_TrailingDraw(TextureAssets.Extra[201].Value, (_) => new Color(255, 255, 255, 0), effect);
                swingHelper.oldVels = oldVels;
            }
            Main.graphics.GraphicsDevice.Textures[1] = AxesProj.DrawColorTex;
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, WeaponSkill.SwingTex.Value, (_) => new Color(255, 255, 255, 0), effect, (_) => 0);
            return false;
        }
        public override bool? CanDamage()
        {
            if (ActivationCondition() && CanChannel && Projectile.ai[2]-- > 0 && Projectile.ai[0] <= 0)
                return false;
            return true;
        }
        public override bool SwitchCondition() => Projectile.ai[1] > 5;
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => swingHelper.GetColliding(targetHitbox);
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if(ModifyHitNPCFunc != null)
            {
                modifiers = ModifyHitNPCFunc.Invoke(target, modifiers);
            }
            modifiers.Knockback += AddKn;
            modifiers.SourceDamage += AddDamage;
            if (CanChannel)
            {
                modifiers.SourceDamage += (ChannelTime - Projectile.ai[2]) / ChannelTime * 2.5f;
                modifiers.Knockback += (ChannelTime - Projectile.ai[2]) / ChannelTime / 3f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitFunc?.Invoke(target, hit, damageDone);
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 9, 3, 1));
            Projectile.numHits += 8;
            if (CanChannel)
            {
                Projectile.numHits += 10;
            }
            if (Projectile.numHits > 20) Projectile.numHits = 20;
            var sound = SoundID.NPCHit17;
            sound.Pitch = 1f;
            sound.Volume = 1f;
            //sound.MaxInstances = 2;
            //for (int i = 0; i < 2; i++)
            //{
            SoundEngine.PlaySound(sound, Projectile.position);
            //}
        }
        public override void OnSkillActive()
        {
            Projectile.rotation = 0;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            SkillTimeOut = false;
            TheUtility.ResetProjHit(Projectile);
            if (CanChannel)
            {
                Projectile.ai[2] = ChannelTime;
            }
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.rotation = 0;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            Projectile.extraUpdates = 0;
            Projectile.localAI[0] = 0;
            player.itemTime = 0;
            TheUtility.ResetProjHit(Projectile);
        }
    }
}
