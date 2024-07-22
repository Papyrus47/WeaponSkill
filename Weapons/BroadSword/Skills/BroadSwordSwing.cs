using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Configs;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.BroadSword.Skills
{
    public class BroadSwordSwing : BasicBroadSwordSkill
    {
        public BroadSwordSwing(BroadSwordProj broadSwordProj) : base(broadSwordProj)
        {
            ChangeCondition = () => player.controlUseItem;
        }
        public Vector2 StartVel;
        public Vector2 VelScale;
        public float ChangeLerpSpeed; 
        public float VisualRotation;
        public Func<float> TimeChange;
        public float TimeChangeMax;
        public float SwingRot;
        public Action<BroadSwordSwing> SwingAction;
        public Func<bool> ChangeCondition;
        public bool CanChannel;
        public bool DirectChange;
        /// <summary>
        /// 为true默认正方向 false则为反
        /// </summary>
        public bool SwingDirectionChange = true;
        public override void AI()
        {
            player.itemLocation = Projectile.Center;
            switch ((int)Projectile.ai[0])
            {
                case 0: // 蓄力
                    {
                        if (CanChannel && ChangeCondition.Invoke() && Projectile.ai[2] < 180)
                        {
                            Projectile.ai[2]++;
                            if(Projectile.ai[1] < 15) Projectile.ai[1]++;
                        }
                        else
                        {
                            Projectile.ai[1]++;
                        }
                        Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
                        swingHelper.Change_Lerp(StartVel, ChangeLerpSpeed, VelScale, ChangeLerpSpeed, VisualRotation, ChangeLerpSpeed);
                        swingHelper.ProjFixedPlayerCenter(player, 0, true, true);
                        swingHelper.SwingAI(broadSword.SwingLength, player.direction, 0);
                        float soundPitch = -0.8f;
                        int channelLevel = 0;
                        switch ((int)Projectile.ai[2])
                        {
                            case 30:
                                soundPitch = -0.2f;
                                channelLevel = 1;
                                goto case 150;
                            case 90:
                                soundPitch = -0.4f;
                                channelLevel = 2;
                                goto case 150;
                            case 150:
                                if ((int)Projectile.ai[2] == 150)
                                {
                                    channelLevel = 3;
                                }
                                broadSword.ChangeLevel++;
                                Projectile.damage += (int)(Projectile.originalDamage * 1.95f);
                                var sound = SoundID.Item62;
                                if (!NormalConfig.Init.DarkSword) // 不启用暗黑大剑
                                {
                                    sound.MaxInstances = 3;
                                    sound.Pitch = soundPitch;
                                    sound.Volume = 0.8f;
                                }
                                else
                                {
                                    sound = new SoundStyle("WeaponSkill/Sounds/GreatSword/En_" + channelLevel.ToString());
                                }
                                SoundEngine.PlaySound(sound, Projectile.position);
                                for (int i = 0; i < 6; i++)
                                {
                                    var dust = Dust.NewDustDirect(player.Center,1,1, DustID.Fireworks);
                                    dust.scale = 1.5f;
                                    dust.color = Color.Gold;
                                    dust.fadeIn = 0.1f;
                                    dust.velocity = Vector2.One.RotatedBy(i / 6f * MathHelper.TwoPi) * 3;
                                    dust.noGravity = true;
                                }
                                break;
                            case 180:
                                Projectile.damage -= Projectile.originalDamage * 5;
                                break;
                        }
                        if (Projectile.ai[1] > 20)
                        {
                            if (!NormalConfig.Init.DarkSword) // 不启用暗黑大剑
                                SoundEngine.PlaySound(broadSword.SpawnItem.UseSound, player.position);
                            else
                            {
                                if(this is BroadSwordSwing_Strong)
                                {
                                    if (Projectile.localAI[0] <= 0)
                                        SoundEngine.PlaySound(new SoundStyle("WeaponSkill/Sounds/GreatSword/Shit"), player.position);
                                }
                                else
                                    SoundEngine.PlaySound(new SoundStyle("WeaponSkill/Sounds/GreatSword/FuckYou_Norm"), player.position);
                            }
                            Projectile.ai[1] = Projectile.ai[2] = 0;
                            Projectile.ai[0]++;
                            Projectile.extraUpdates = 1;
                            TheUtility.ResetProjHit(Projectile);
                            if (broadSword.OldSkills.Count > 1 && broadSword.OldSkills[^1] is BroadSwordSwing swing && swing?.CanChannel == true && !CanChannel)
                            {
                                Projectile.damage *= 3;
                            }
                            Projectile.numHits = 0;
                        }
                        break;
                    }
                case 1: // 挥舞
                    {
                        SwingAction?.Invoke(this);
                        Projectile.spriteDirection = player.direction * SwingDirectionChange.ToDirectionInt();
                        if (Projectile.numHits > 0 && Projectile.ai[2] <= -6)
                        {
                            Projectile.localAI[2] = 1;
                            Projectile.numHits = 0;
                            Projectile.ai[2] = TimeChange.Invoke() * 1.5f;
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 9, 3, 1));
                        }
                        else if(Projectile.ai[2]-- <= 0) Projectile.ai[1] += TimeChange.Invoke() * player.GetWeaponAttackSpeed(broadSword.SpawnItem);
                        else Projectile.ai[1] += TimeChange.Invoke() * player.GetWeaponAttackSpeed(broadSword.SpawnItem) * 0.07f;
                        float Time = broadSword.TimeChange(Projectile.ai[1] / TimeChangeMax);
                        if (Time > 1)
                        {
                            Time = 1 + (Projectile.ai[1] / TimeChangeMax) * 0.01f;
                            swingHelper.SetNotSaveOldVel();
                            if (this is BroadSwordSwing_Strong && NormalConfig.Init.DarkSword && Projectile.localAI[0] == 1 && Projectile.localAI[1] >= 50 && Projectile.localAI[2] < 2)
                            {
                                SoundStyle style = new SoundStyle("WeaponSkill/Sounds/GreatSword/FuckYou_HitNull");
                                if (Projectile.localAI[2] == 1)
                                    style.Pitch = 0.3f;
                                SoundEngine.PlaySound(style, player.position);
                                Projectile.localAI[2] = 2;
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 18, 5, 18));
                            }
                        }
                        else if ((int)(Projectile.ai[1] / TimeChange.Invoke()) % 3 == 0)
                        {
                            TheUtility.Player_ItemCheck_EmitUseVisuals(player,broadSword.SpawnItem,Projectile.Hitbox);
                        }
                        swingHelper.ProjFixedPlayerCenter(player, 0, true,true);
                        swingHelper.SwingAI(broadSword.SwingLength, player.direction, Time * SwingRot * SwingDirectionChange.ToDirectionInt());

                        if(Time > 1.02f)
                        {
                            SkillTimeOut = true;
                            Projectile.extraUpdates = 0;
                        }
                        break;
                    }
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Effect effect = WeaponSkill.SwingEffect.Value;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
            effect.Parameters["uTransform"].SetValue(model * projection);
            effect.Parameters["uColorChange"].SetValue(0.95f);
            Main.graphics.GraphicsDevice.Textures[1] = broadSword.DrawColorTex;
            swingHelper.Swing_Draw_ItemAndTrailling(lightColor, WeaponSkill.SwingTex.Value, (_) => new Color(255,255,255,0), effect,(_) => 0);
            return false;
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 0;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[0] > 0 && Projectile.ai[1] > TimeChangeMax;
        }
        public override bool ActivationCondition()
        {
            return ChangeCondition.Invoke() || DirectChange;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return swingHelper.GetColliding(targetHitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ItemLoader.OnHitNPC(broadSword.SpawnItem, player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(broadSword.SpawnItem, player, new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height),Projectile.damage,Projectile.knockBack,target.whoAmI,Projectile.damage,damageDone);
            TheUtility.SetPlayerImmune(player);
            if (hit.Crit)
            {
                for (int i = 0; i < 2; i++)
                {
                    var proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_OnHit(target), target.Top, Projectile.velocity.RotatedBy(MathHelper.PiOver2 + MathHelper.PiOver4 * player.direction).SafeNormalize(default), 0, 0, player.whoAmI, 150, 50, TextureAssets.Extra[191].Value);
                    proj.FixedPos = false;
                }
            }
        }
        public override void OnSkillActive()
        {
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            Projectile.localAI[2] = 0;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.damage = Projectile.originalDamage;
            Projectile.ai[1] = Projectile.ai[2] = Projectile.ai[0] = 0;
            Projectile.localAI[2] = 0;
            Projectile.extraUpdates = 0;
            broadSword.ChangeLevel = 0;
        }
    }
}
