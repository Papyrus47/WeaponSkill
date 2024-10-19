using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade.Skills
{
    public class SSB_Throw : SSB_BasicSkills
    {
        public SSB_Throw(StarSpinBladeProj modProjectile, Func<bool> changeCondition, bool isTrueSlash) : base(modProjectile)
        {
            ChangeCondition = changeCondition;
            IsTrueSlash = isTrueSlash;
        }
        /// <summary>
        /// 伤害次数
        /// </summary>
        public int DmgCounts;
        /// <summary>
        /// 动作值
        /// </summary>
        public float ActionDmg = 1;
        /// <summary>
        /// 为true为正旋斩,flase为逆旋斩
        /// </summary>
        public bool IsTrueSlash;
        /// <summary>
        /// 如何使用这个技能
        /// </summary>
        public Func<bool> ChangeCondition;
        public override void AI()
        {
            Projectile.position += Projectile.velocity;
            switch ((int)Projectile.ai[0])
            {
                case 0: // 投掷预备动作
                    Player.itemTime = Player.itemAnimation = 10;
                    Player.heldProj = Projectile.whoAmI;
                    Vector2 vel = (Main.MouseWorld - Player.Center).SafeNormalize(default) * 20;
                    Player.itemRotation = MathF.Atan2(vel.Y * Player.direction, vel.X * Player.direction);
                    Projectile.velocity = vel;
                    Projectile.ai[0]++;
                    break;
                case 1: // 刃追踪
                    if(Projectile.ai[1]++ > 9)
                    {
                        Projectile.ai[1] = 0;
                        TheUtility.ResetProjHit(Projectile);
                    }

                    Projectile.extraUpdates = 3;
                    if (GetStarSpinBladeItem().SpinValue < 0)
                        GetStarSpinBladeItem().SpinValue += 1;
                    else if (GetStarSpinBladeItem().SpinValue > 0)
                        GetStarSpinBladeItem().SpinValue -= 1;
                    else
                        Projectile.ai[0]++;

                    Projectile.rotation += GetStarSpinBladeItem().SpinValue / 6f;
                    Projectile.spriteDirection = Projectile.direction;
                    Projectile.spriteDirection *= IsTrueSlash ? 1 : -1;
                    NPC target = null;
                    float maxDis = 1800;
                    foreach(NPC npc in Main.npc)
                    {
                        float dis = Vector2.Distance(npc.position, Player.position);
                        if (dis < maxDis && npc.CanBeChasedBy())
                        {
                            target = npc;
                            maxDis = dis;
                        }
                    }
                    if (target != null)
                    {
                        Projectile.velocity = ((target.position - Projectile.position) * 0.05f + Projectile.velocity * 50) / 51f;
                    }
                    else
                    {
                        Projectile.velocity = ((Player.position - Projectile.position) * 0.01f + Projectile.velocity * 100) / 101f;
                        if(Vector2.Distance(Projectile.position, Player.position) < 20)
                        {
                            Projectile.ai[0]++;
                        }
                    }
                    break;
                case 2: // 直接跳出
                    SkillTimeOut = true;
                    Projectile.extraUpdates = 0;
                    break;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 1, 2, 2));
            //OnHit?.Invoke(target, hit, damageDone);
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
        public override bool? CanDamage() => true;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => null;
        public override bool ActivationCondition() => ChangeCondition.Invoke();
        public override bool SwitchCondition() => false;
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            for(int  i = 0; i < Projectile.oldPos.Length; i++)
            {
                float factor = (float)i / Projectile.oldPos.Length;
                Color color = Color.Purple;
                color *= 1 - factor;
                sb.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, null, color, Projectile.oldRot[i], Projectile.Size * 0.5f, 1f, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            return false;
        }
    }
}
