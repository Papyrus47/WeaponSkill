using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Bows;

namespace WeaponSkill.Weapons.Crossbow.Skills
{
    public class CrossbowHeld : BasicCrossbowSkill
    {
        public CrossbowHeld(ModProjectile modProjectile) : base(modProjectile)
        {
        }
        public const int HELDTIME = 20;
        public override void AI()
        {
            Projectile.extraUpdates = 0;

            if (Projectile.ai[2] > 0)
                Projectile.ai[2]--;
            if (WeaponSkill.BowSlidingStep.Current)
            {
                Projectile.ai[2] += 2;
            }

            if (Projectile.ai[0]++ < HELDTIME)
            {
                Projectile.velocity = (Projectile.velocity * 10f + (Main.MouseWorld - player.Center).SafeNormalize(default) * 10f) / 11f;
                Projectile.Center = Vector2.Lerp(Projectile.Center, player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity.SafeNormalize(default) * Projectile.width * 0.5f, Projectile.ai[0] / HELDTIME);
            }
            else
            {
                Projectile.velocity = (Main.MouseWorld - player.Center).SafeNormalize(default);
                Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + Projectile.velocity * Projectile.width * 0.5f;
                if (Projectile.ai[0] > 90)
                {
                    SkillTimeOut = true;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0);
            CrossProj.globalItem.CosumeAmmo = false;
            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            player.ChangeDir(Projectile.spriteDirection);
            player.heldProj = Projectile.whoAmI;
            player.itemAnimation = player.itemTime = 2;
            player.itemRotation = MathF.Atan2(Projectile.velocity.Y * Projectile.spriteDirection, Projectile.velocity.X * Projectile.spriteDirection);

            if (player.controlUseItem & Projectile.ai[1]++ > player.GetWeaponAttackSpeed(player.HeldItem) * 6)
            {
                CrossProj.globalItem.CosumeAmmo = true;
                Projectile.ai[1] = 0;
                (int, int) value = GetShootType(out int dmg, out float speed, out float kn, out int crit);
                int shootType = value.Item1;
                int ammoType = value.Item2;
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoType), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.02f) * player.HeldItem.shootSpeed * speed, shootType, (int)((Projectile.damage + dmg) * Math.Log10((Projectile.ai[2] + 600) * 10) / 2), Projectile.knockBack + kn, player.whoAmI);
                Main.projectile[proj].OriginalCritChance += crit;
                SoundEngine.PlaySound(player.HeldItem.UseSound, player.position);
                //Main.projectile[proj].usesLocalNPCImmunity = true;
                //Main.projectile[proj].localNPCHitCooldown = -1;
                //Main.projectile[proj].timeLeft /= 3;
                //Main.projectile[proj].extraUpdates += 2;

                if (Projectile.ai[2] > 120)
                {
                    Projectile.ai[2] = -600;
                    for(int i = 0; i < 15; i++)
                    {
                        value = GetShootType(out dmg, out speed, out kn, out crit);
                        shootType = value.Item1;
                        ammoType = value.Item2;
                        proj = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoType), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.02f) * player.HeldItem.shootSpeed * speed, shootType, Projectile.damage + dmg, Projectile.knockBack + kn, player.whoAmI);
                        Main.projectile[proj].OriginalCritChance += crit;
                    }
                }
            }
            else if (player.controlUseTile && Projectile.ai[1] > player.GetWeaponAttackSpeed(player.HeldItem) * 20)
            {
                for (int i = 0; i < player.HeldItem.damage / 5; i++)
                {
                    CrossProj.globalItem.CosumeAmmo = true;
                    Projectile.ai[1] = 0;
                    (int, int) value = GetShootType(out int dmg, out float speed, out float kn, out int crit);
                    int shootType = value.Item1;
                    int ammoType = value.Item2;
                    int proj = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoType), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.2f) * player.HeldItem.shootSpeed * speed * Main.rand.NextFloat(0.4f,1f), shootType, (int)((Projectile.damage + dmg) * Math.Log10((Projectile.ai[2] + 600) * 10) / 6), Projectile.knockBack + kn, player.whoAmI);
                    Main.projectile[proj].OriginalCritChance += crit;
                    SoundEngine.PlaySound(player.HeldItem.UseSound, player.position);
                    //Main.projectile[proj].usesLocalNPCImmunity = true;
                    //Main.projectile[proj].localNPCHitCooldown = -1;
                    //Main.projectile[proj].timeLeft /= 3;
                    //Main.projectile[proj].extraUpdates += 2;
                }

                if (Projectile.ai[2] > 120)
                {
                    Projectile.ai[2] = -600;
                    for (int i = 0; i < player.HeldItem.damage / 3; i++)
                    {
                        (int, int) value = GetShootType(out int dmg, out float speed, out float kn, out int crit);
                        int shootType = value.Item1;
                        int ammoType = value.Item2;
                        int proj = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, ammoType), Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.2f) * player.HeldItem.shootSpeed * speed * Main.rand.NextFloat(0.4f, 1f), shootType, (Projectile.damage + dmg) / 3, Projectile.knockBack + kn, player.whoAmI);
                        Main.projectile[proj].OriginalCritChance += crit;
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor)
        {
            Main.GetItemDrawFrame(CrossProj.SpawnItem.type, out var tex, out var rect);
            sb.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            #region 过热条
            Texture2D line = TextureAssets.FishingLine.Value;
            float factor = Projectile.ai[2] / 120f;
            sb.Draw(line, player.Top - Main.screenPosition - new Vector2(0, 10), new Rectangle(0, 0, line.Width, (int)(line.Height)), Color.Gray, MathHelper.PiOver2, line.Size() * 0.5f, 4f, SpriteEffects.None, 0f);
            sb.Draw(line, player.Top - Main.screenPosition - new Vector2(0, 10), new Rectangle(0, 0, line.Width, (int)(line.Height * factor)), Color.Lerp(Color.White, Color.Red, factor), MathHelper.PiOver2, line.Size() * 0.5f, 4f, SpriteEffects.None, 0f);
            #endregion
            return false;
        }
        public override bool? CanDamage() => false;
        public override bool ActivationCondition() => true;
        public override bool SwitchCondition()
        {
            return (player.controlUseTile || player.controlUseItem) && Projectile.ai[0] > HELDTIME;
        }
        public override void OnSkillActive()
        {
            if (CrossProj.OldSkills.Count == 1) Projectile.ai[0] = 0;
            else Projectile.ai[0] = HELDTIME / 2.5f;
            SkillTimeOut = false;
        }
        public override void OnSkillDeactivate()
        {
            Projectile.ai[0] = 0;
        }
    }
}
