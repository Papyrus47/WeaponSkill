using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.Spears.Skills
{
    public class SpearsSpurts_Coiled : BasicSpearsSkill
    {
        public SpearsSpurts_Coiled(SpearsProj SpearsProj, Func<float> spurtsUseTime, Func<int> spurtsCount = null) : base(SpearsProj)
        {
            SpurtsUseTime = spurtsUseTime;
            SpurtsCount = spurtsCount;
            Sound = new("WeaponSkill/Sounds/Spears/SpearsSpurts_Coiled");
            Sound.MaxInstances = 5;
        }
        public Func<float> SpurtsUseTime;
        public Func<int> SpurtsCount;
        public Vector2 SpurtsVel;
        public SoundStyle Sound;
        public const int SKILLTIMEOUT_TIME = 15;
        public override void AI()
        {
            player.heldProj = Projectile.whoAmI;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * player.direction, Projectile.velocity.X * player.direction);
            player.itemTime = player.itemAnimation = 2;
            #region 连续突刺
            player.ChangeDir(Projectile.direction);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 - (player.direction == 1 ? 0 : -MathHelper.PiOver2);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * SpearsProj.WeaponLength * ((Projectile.ai[0] / SpurtsUseTime() * 0.5f + 1) - 1);
            if (Projectile.ai[0] > SpurtsUseTime() && Projectile.ai[1] <= SpurtsCount())
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1]++; // 次数记录
                Projectile.velocity = SpurtsVel.RotatedByRandom(0.2);
                TheUtility.ResetProjHit(Projectile);
                SoundEngine.PlaySound(Sound, Projectile.Center);
                SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, Projectile.originalDamage / 2, Projectile.knockBack * 0.1f, Projectile.owner, Projectile.width * 3, Projectile.height,SpearsProj.DrawColorTex);
            }
            else
            {
                if(Projectile.ai[1] > SpurtsCount())
                {
                    Projectile.ai[2]++;
                }
                else
                {
                    Projectile.ai[0]++;
                }
            }

            if (Projectile.ai[2] > SKILLTIMEOUT_TIME) // 跳出
            {
                SkillTimeOut = true;
            }
            #endregion
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.instance.LoadProjectile(SpearsProj.SpawnItem_OriginShootProj);
            var tex = TextureAssets.Projectile[SpearsProj.SpawnItem_OriginShootProj].Value;
            float rot = Projectile.rotation;
            if (SpearsProj.SpawnItem.type == ItemID.MonkStaffT2) rot -= MathHelper.PiOver2 * Projectile.spriteDirection;
            sb.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor,rot, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 4, 5, 3, -1, "SSC_OnHit"));
        }
        public override bool ActivationCondition()
        {
            return player.controlUseItem;
        }
        public override bool SwitchCondition()
        {
            return Projectile.ai[2] > 0;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float r = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projHitbox.Center(), projHitbox.Center() + Projectile.velocity * SpearsProj.WeaponLength,
                Projectile.height / 2f, ref r);
        }
        public override void OnSkillActive()
        {
            SkillTimeOut = false;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            Projectile.rotation = 0;
            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(default);
            SpurtsVel = Projectile.velocity;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            Projectile.damage = Projectile.originalDamage;
            Projectile.rotation = 0;
        }
    }
}
