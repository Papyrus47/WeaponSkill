using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;

namespace WeaponSkill.Weapons.Bows.Skills
{
    public class BowJustShoot : BasicBowsSkill
    {
        public BowJustShoot(ModProjectile modProjectile) : base(modProjectile)
        {

        }
        public Func<int> ShootTime;
        public bool EndShoot;
        public override void AI()
        {
            if (BowsProj.ChannelLevel == 1) Projectile.extraUpdates = 1;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * Projectile.width * 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation() - (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            player.ChangeDir(Projectile.spriteDirection);
            player.heldProj = Projectile.whoAmI;
            player.itemAnimation = player.itemTime = 2;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.spriteDirection, Projectile.velocity.X * Projectile.spriteDirection);
            BowsProj.NoUse = false;

            if (Projectile.ai[0] == 0 && Projectile.ai[2] <= 0.6f)
            {
                Projectile.ai[2] += 0.2f;
            }
            else if (player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 0)
            {
                Projectile.ai[0]++; // 计时器
                if(Projectile.ai[2] > 0) Projectile.ai[2] -= 0.05f;
                if ((int)Projectile.ai[0] == 1)
                {
                    // 调用攻击
                    Shoot();
                    if (BowsProj.ChannelLevel > 1) player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 60;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 10, 5, 3));
                }
                if (Projectile.ai[0] > ShootTime() / 2.5f)
                {
                    EndShoot = true;
                    if (Projectile.ai[0] > ShootTime() + 15)
                    {
                        SkillTimeOut = true;
                        BowsProj.ChannelLevel = 0;
                    }
                }
            }
            else
            {
                SkillTimeOut = true;
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.GetItemDrawFrame(BowsProj.SpawnItem.type, out var tex, out var rect);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale * new Vector2(Projectile.ai[2] + 1f, 1 - Projectile.ai[2] * 0.15f), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override bool SwitchCondition()
        {
            return EndShoot; // 结束发射时候,可以切换到其它技能
        }

        public override bool ActivationCondition()
        {
            if (BowsProj.CurrentSkill is not BowJustShoot) // 不是刚射的话
            {
                return player.controlUseTile; // 无视蓄力级别,可以直接切换
            }
            return BowsProj.ChannelLevel < 3 && player.controlUseTile; // 蓄力等级为3级以下可以切换到这个技能
        }

        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            if(BowsProj.ChannelLevel < 3) BowsProj.ChannelLevel++; // 切换一次这个技能,提升一级蓄力等级
            SkillTimeOut = false;
            EndShoot = false;
            Projectile.velocity = (Main.MouseWorld - player.Center).SafeNormalize(default);
            var sound = SoundID.Item62;
            sound.MaxInstances = 3;
            sound.Pitch = -0.5f;
            sound.Volume = 0.8f;
            SoundEngine.PlaySound(sound, Projectile.position);
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.extraUpdates = 0;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            EndShoot = false;
            BowsProj.NoUse = true;
        }
        public virtual void Shoot()
        {
            int Count = 2 + BowsProj.ChannelLevel;
            for (int i = 0; i < Count; i++)
            {
                (int, int) value = GetShootType(out int dmg, out float speed, out float kn, out int crit);
                int shootType = value.Item1;
                int ammoType=value.Item2;
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(BowsProj.SpawnItem,ammoType), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.35f) * BowsProj.SpawnItem.shootSpeed * speed, shootType, (int)((Projectile.damage + dmg) * BowsProj.ChannelLevel * 0.9f), Projectile.knockBack + kn, player.whoAmI);
                Main.projectile[proj].OriginalCritChance += crit;
                Main.projectile[proj].usesLocalNPCImmunity = true;
                Main.projectile[proj].localNPCHitCooldown = -1;
                Main.projectile[proj].timeLeft /= 3;
                Main.projectile[proj].extraUpdates += 2;
            }
        }
    }
}
