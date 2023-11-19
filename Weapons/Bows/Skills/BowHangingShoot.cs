using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;

namespace WeaponSkill.Weapons.Bows.Skills
{
    public class BowHangingShoot : BasicBowsSkill
    {
        public BowHangingShoot(ModProjectile modProjectile) : base(modProjectile)
        {

        }
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

            if ((player.controlUseTile || player.controlUseItem || Projectile.ai[2] <= 0.4f) && Projectile.ai[1] <= 0) // 蓄力中
            {
                if (Projectile.ai[2] < 0.6f) Projectile.ai[2] += 0.02f;
                Projectile.velocity = (Main.MouseWorld - new Vector2(0,Main.screenHeight * 1.5f) - player.Center).SafeNormalize(default);
                if (player.controlUseTile || player.controlUseItem) Projectile.ai[0]++; // 计时器
                player.GetModPlayer<WeaponSkillPlayer>().StatStaminaAddTime = 0;
            }
            else if (player.GetModPlayer<WeaponSkillPlayer>().StatStamina > 0) //取消蓄力
            {
                if (Projectile.ai[2] > 0) Projectile.ai[2] -= 0.15f;
                if ((int)Projectile.ai[1]++ == 1)
                {
                    // 调用攻击
                    Shoot();
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 3, 1, 3));
                    player.GetModPlayer<WeaponSkillPlayer>().StatStamina -= 10;
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
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale * new Vector2(Projectile.ai[2] + 1f, 1 - Projectile.ai[2] * 0.15f), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            if(Projectile.ai[1] <= 0 && Projectile.ai[0] > 8) // 辅助瞄准
            {
                sb.Draw(TextureAssets.FishingLine.Value, Main.MouseScreen, null, Color.Red, 0f,new Vector2(0,TextureAssets.FishingLine.Height()) , new Vector2(1, 100), SpriteEffects.None, 0f);
                sb.Draw(TextureAssets.Extra[59].Value, Main.MouseScreen, null, new Color(255,100,100,0) * 0.5f, 0f, TextureAssets.Extra[59].Size() * 0.5f, new Vector2(3, 0.5f), SpriteEffects.None, 0f);
            }
            return false;
        }
        public override bool SwitchCondition()
        {
            return EndShoot; // 结束发射时候,可以切换到其它技能
        }

        public override bool ActivationCondition()
        {
            return BowsProj.ChannelLevel > 1 && player.controlUseTile && BowsProj.CurrentSkill is not BowChannelShoot;
        }
        public override bool CompulsionSwitchSkill(ProjSkill_Instantiation nowSkill)
        {
            if (nowSkill is BowChannelShoot channelShoot)
            {
                return player.controlUseTile && !channelShoot.EndShoot;
            }
            return base.CompulsionSwitchSkill(nowSkill);
        }

        public override bool? CanDamage() => false;
        public override void OnSkillActive()
        {
            Projectile.ai[0] = Projectile.ai[1] = Projectile.ai[2] = 0;
            SkillTimeOut = false;
            EndShoot = false;
            Projectile.velocity = (Main.MouseWorld - player.Center).SafeNormalize(default);
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
            _ = GetShootType(out int dmg, out _, out float kn, out _);
            int proj = Projectile.NewProjectile(player.GetSource_ItemUse(BowsProj.SpawnItem), Projectile.Center, (new Vector2(Main.MouseWorld.X,Main.screenPosition.Y - Main.screenHeight * 0.25f) - Projectile.Center) / 15f, ModContent.ProjectileType<HangingShootProj>(), (Projectile.damage + dmg) * BowsProj.ChannelLevel, Projectile.knockBack + kn, player.whoAmI);
            Main.projectile[proj].timeLeft += 15 * (BowsProj.ChannelLevel - 1);
        }
    }
}
