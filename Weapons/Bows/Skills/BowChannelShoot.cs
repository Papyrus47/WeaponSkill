using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;

namespace WeaponSkill.Weapons.Bows.Skills
{
    public class BowChannelShoot : BasicBowsSkill
    {
        public BowChannelShoot(ModProjectile modProjectile) : base(modProjectile)
        {

        }
        public Func<int> ChannelTime;
        public Func<int> ShootTime;
        public bool EndShoot;
        public override void AI()
        {
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * Projectile.width * 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation() - (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            player.ChangeDir(Projectile.spriteDirection);
            player.heldProj = Projectile.whoAmI;
            player.itemAnimation = player.itemTime = 2;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.spriteDirection, Projectile.velocity.X * Projectile.spriteDirection);
            BowsProj.NoUse = false;

            if ((player.controlUseItem || Projectile.ai[2] <= 0.4f) && Projectile.ai[1] <= 0) // 蓄力中
            {
                if (Projectile.ai[2] < 0.6f) Projectile.ai[2] += 0.02f;
                Projectile.velocity = (Main.MouseWorld - player.Center).SafeNormalize(default);
                if (player.controlUseItem) Projectile.ai[0]++; // 计时器
                if (BowsProj.ChannelLevel < 3) // 小于三的蓄力等级时候,可以增加蓄力
                {
                    if (Projectile.ai[0] > ChannelTime()) // 超过蓄力时间
                    {
                        BowsProj.ChannelLevel++; // 增加弓箭的蓄力等级
                        var sound = SoundID.Item62;
                        sound.MaxInstances = 3;
                        sound.Pitch = -0.5f;
                        sound.Volume = 0.8f;
                        SoundEngine.PlaySound(sound, Projectile.position);
                        Projectile.ai[0] = 0; // 重置计时器
                    }
                }

                if ((int)Projectile.ai[0] % 8 == 0)
                {
                    player.GetModPlayer<WeaponSkillPlayer>().StatStamina--;
                    player.GetModPlayer<WeaponSkillPlayer>().StatStaminaAddTime = 0;
                }
            }
            else if(player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 0) //取消蓄力
            {
                if (Projectile.ai[2] > 0) Projectile.ai[2] -= 0.15f;
                if ((int)Projectile.ai[1]++ == 1)
                {
                    // 调用攻击
                    Shoot();
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 3, 1, 3));
                    player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 15;
                }
                if (Projectile.ai[1] > ShootTime() || Projectile.ai[2] <= 0)
                {
                    EndShoot = true;
                    if (Projectile.ai[1] > ShootTime() + 30)
                    {
                        SkillTimeOut = true;
                        BowsProj.ChannelLevel = 0;
                    }
                }

            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.GetItemDrawFrame(BowsProj.SpawnItem.type, out var tex, out var rect);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale * new Vector2(Projectile.ai[2] + 1f,1 - Projectile.ai[2] * 0.15f), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override bool SwitchCondition() => EndShoot; // 结束发射时候,可以切换到其它技能
        public override bool ActivationCondition()
        {
            if (BowsProj.OldSkills[^1] is not BowJustShoot && BowsProj.ChannelLevel == 3)
            {
                return false;
            }
            return BowsProj.ChannelLevel < 3 && player.controlUseItem; // 蓄力等级为3级以下可以切换到这个技能
        }

        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            BowsProj.ChannelLevel++; // 切换一次这个技能,提升一级蓄力等级
            var sound = SoundID.Item62;
            sound.MaxInstances = 3;
            sound.Pitch = -0.5f;
            sound.Volume = 0.8f;
            SoundEngine.PlaySound(sound, Projectile.position);
            SkillTimeOut = false;
            EndShoot = false;
            if (player.GetModPlayer<WeaponSkillPlayer>().StatStamina <= 0)
            {
                SkillTimeOut = true;
            }
        }
        public override void OnSkillDeactivate()
        {
            SkillTimeOut = false;
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            EndShoot = false;
            BowsProj.NoUse = true;
        }
        public virtual void Shoot()
        {
            for (int i = 1; i <= BowsProj.ChannelLevel; i++)
            {
                int shootType = GetShootType(out int dmg, out float speed, out float kn, out int crit);
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(BowsProj.SpawnItem), Projectile.Center, Projectile.velocity * BowsProj.SpawnItem.shootSpeed * speed, shootType, (Projectile.damage + dmg) * BowsProj.ChannelLevel, Projectile.knockBack + kn, player.whoAmI);
                Main.projectile[proj].OriginalCritChance += crit;
                Main.projectile[proj].usesLocalNPCImmunity = true;
                Main.projectile[proj].extraUpdates += i - 1;
                Main.projectile[proj].localNPCHitCooldown = -1;
            }
        }
    }
}
