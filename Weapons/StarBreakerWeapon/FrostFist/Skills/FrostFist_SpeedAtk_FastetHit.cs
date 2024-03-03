using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.CameraModifiers;
using WeaponSkill.Helper;
using WeaponSkill.NPCs;
using static WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills.FrostFist_FistHit;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_SpeedAtk_FastetHit : FrostFist_FistHit
    {
        public FrostFist_SpeedAtk_FastetHit(FrostFistProj modProjectile, Func<bool> activationConditionFunc) : base(modProjectile, activationConditionFunc)
        {
            HitNPCs = new();
        }
        public List<NPC> HitNPCs;
        public Vector2 FixedNPCCenter;
        public override void AI()
        {
            Projectile.Center = Player.Center;
            Vector2 vel = (Main.MouseWorld - Projectile.Center);
            Player.velocity.X *= 0.2f;
            Player.velocity.Y = 0;
            Player.ChangeDir((vel.X > 0).ToDirectionInt());
            Projectile.direction = Player.direction;
            Player.itemTime = Player.itemAnimation = 2;
            Player.itemRotation = MathF.Atan2(vel.Y * Projectile.direction, vel.X * Projectile.direction);
            switch ((int)Projectile.ai[0])
            {
                case 0: // 打出一拳开
                    {
                        if (Projectile.ai[1] < 1)
                        {
                            Vector2 shootVel = vel.SafeNormalize(default) * 6;
                            Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj>(), (int)(Projectile.damage), Projectile.knockBack, Player.whoAmI);
                            proj.alpha = 0;
                            proj.extraUpdates = 5;
                            proj.timeLeft = 60;
                            OnHit = (n, _, _) =>
                            {
                                HitNPCs.Add(n);
                                n.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime += 180;
                            };
                        }
                        if(Projectile.ai[1]++ > 10) 
                        {
                            if (HitNPCs.Count > 0)// 进入丢
                            {
                                Projectile.ai[1] = 0;
                                Projectile.ai[0]++;
                                FixedNPCCenter = Player.Center + new Vector2(150 * Player.direction, 0);
                            }
                            else
                            {
                                Projectile.ai[1] = 0;
                                SkillTimeOut = true;
                            }
                        }
                        break;
                    }
                case 1: // 丢
                    {
                        Player.SetImmuneTimeForAllTypes(5);
                        if (Projectile.ai[1] < 1)
                        {
                            Projectile.ai[1]++;
                            Vector2 shootVel = vel.SafeNormalize(default) * 6;
                            Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj_MoveFist>(), (int)(Projectile.damage * 2), Projectile.knockBack, Player.whoAmI);
                            proj.alpha = 0;
                            FistHitProjChange?.Invoke(proj);
                            (proj.ModProjectile as FrostFist_Proj_FistHitProj_MoveFist).MoveFistAI = (proj) =>
                            {
                                Vector2 StartVel = -Vector2.UnitY * 60;
                                Vector2 vel = StartVel.RotatedBy(Player.direction * (proj.ai[0] / 60f) * MathHelper.Pi);
                                Vector2 center = Player.Center + vel;
                                proj.velocity = (center - proj.Center);
                                HitNPCs.ForEach(x => x.Center = proj.Center);
                                if(proj.timeLeft < 2)
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        if (!Player.CheckMana(50, true))
                                        {
                                            SkillTimeOut = true;
                                            return;
                                        }
                                    }
                                    Projectile.ai[0] = 2;
                                    for (int i = 0; i < 32; i++)
                                    {
                                        Dust dust = FrostFistDust();
                                        dust.velocity = -proj.velocity.RotatedByRandom(0.4) * Main.rand.NextFloat(5,12);
                                        dust.position = Projectile.Center;
                                    }
                                }
                            };
                        }
                        break;
                    }
                case 2: // 打出200拳
                    {
                        Player.SetImmuneTimeForAllTypes(5);
                        HitNPCs.ForEach(x => 
                        {
                            x.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime = 60;
                            x.Center = FixedNPCCenter;
                        });
                        if (Projectile.ai[1]++ < 50)
                        {
                            Player.Center = FixedNPCCenter - new Vector2(120 * (Projectile.ai[1] % 2 == 1).ToDirectionInt(),0);
                            vel = FixedNPCCenter - Player.Center;
                            Main.instance.CameraModifiers.Add(new CameraModifierToScreenPos(CameraModifierToScreenPos.GetScreenPos(FixedNPCCenter + vel * 0.08f), 10));
                            for (int j = 0; j < 4; j++)
                            {
                                Vector2 shootVel = vel.SafeNormalize(default).RotatedByRandom(0.4) * Main.rand.NextFloat(4, 8);
                                Projectile proj = Projectile.NewProjectileDirect(GetSource(), Projectile.Center, shootVel, ModContent.ProjectileType<FrostFist_Proj_FistHitProj>(), (int)(Projectile.damage * 0.2f), Projectile.knockBack, Player.whoAmI, Main.rand.NextFloat(40));
                                proj.alpha = 200;
                                FistHitProjChange?.Invoke(proj);
                                for (int i = 0; i < 32; i++)
                                {
                                    Dust dust = FrostFistDust();
                                    dust.velocity = -vel.SafeNormalize(default).RotatedByRandom(0.2) * i * 0.3f;
                                    dust.position = Player.HandPosition.Value + vel.SafeNormalize(default) * Player.width;
                                }
                                if (!Player.CheckMana(5, true))
                                {
                                    Projectile.ai[1] = 50;
                                    break;
                                }
                            }
                        }
                        else if (Projectile.ai[1] > 90)
                        {
                            Projectile.ai[1] = 0;
                            SkillTimeOut = true;
                        }
                        break;
                    }
            }
        }
        public override bool ActivationCondition()
        {
            return base.ActivationCondition() && FrostFist.frostFistItem.ChangeLevel > 10;
        }
        public override bool PreDraw(SpriteBatch sb, ref Color lightColor) => false;
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            FrostFist.frostFistItem.ChangeLevel -= 10;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            HitNPCs.Clear();
            Projectile.ai[1] = 0;
        }
    }
}
