using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.CameraModifiers;

namespace WeaponSkill.Weapons.InsectStaff
{
    public abstract class InsectProj : ModProjectile
    {
        public struct InsectSetting
        {
            /// <summary>
            /// 虫子的速度
            /// </summary>
            public float Speed;
            public InsectSetting()
            {
                Speed = 10f;
            }
        }
        public class InsectProjSource : IEntitySource
        {
            private readonly string context;
            public InsectStaffProj staffProj;
            public string Context => context;
            public InsectProjSource(InsectStaffProj proj, string context = null)
            {
                staffProj = proj;
                this.context = context;
            }
        }
        public Item SpawnItem;
        public InsectStaffProj SpawnProj;
        public Player Player;
        public InsectSetting Setting;
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.Size = new(10);
            Setting = new();
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is InsectProjSource proj)
            {
                SpawnItem = proj.staffProj.SpawnItem;
                SpawnProj = proj.staffProj;
                Player = proj.staffProj.Player;
            }
        }
        public override void AI()
        {
            if (SpawnItem != Player.HeldItem || SpawnProj == null || SpawnProj is not InsectStaffProj || !SpawnProj.Projectile.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.damage = Projectile.originalDamage;
            Item insect = SpawnItem.GetGlobalItem<InsectStaffGlobalItem>().Insect;
            if (insect != null)
            {
                Projectile.damage += insect.damage;
            }
            Projectile.localNPCHitCooldown = 15;
            Projectile.timeLeft = 3;
            if (Projectile.numHits > 15) // 显示吸够了灯
            {
                Dust.NewDustDirect(Projectile.Center,Projectile.width, Projectile.height, DustID.FireflyHit);
            }
            switch ((int)Projectile.ai[0])
            {
                case 0: // 游荡在玩家周围
                    Projectile.spriteDirection = Projectile.direction;
                    Vector2 pos = Player.Center + new Vector2(-Player.direction, -1) * 100;
                    Vector2 vel = (Player.Center - Projectile.Center);
                    for (int i = 0; i < 40; i++)
                    {
                        Dust dust1 = Dust.NewDustDirect(Player.Center + vel.SafeNormalize(default).RotatedBy(i / 40f * MathHelper.TwoPi) * 100, 1, 1, DustID.FireflyHit);
                        dust1.velocity *= 0;
                        dust1.alpha = 20;
                        dust1.noGravity = true;
                    }
                    if (Projectile.numHits > 15)
                    {
                        if (vel.Length() < 100)
                        {
                            Projectile.numHits = 0;
                            SpawnItem.GetGlobalItem<InsectStaffGlobalItem>().StrongTime = 1800;
                        }
                    }
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Player.Center + new Vector2(-Player.direction, -1) * 100 - Projectile.Center).SafeNormalize(default) * Setting.Speed, 0.01f);
                    if (Projectile.velocity.LengthSquared() < 1)
                    {
                        Projectile.velocity *= 2;
                    }
                    if (Player.HasMinionAttackTargetNPC)
                        Projectile.ai[0] = 1;
                    break;
                case 1: // 飞出去自动攻击
                    if (!Player.HasMinionAttackTargetNPC)
                    {
                        Projectile.ai[0] = 0;
                        break;
                    }
                    NPC target = Main.npc[Player.MinionAttackTargetNPC];
                    Vector2 targetPos = target.Center;
                    Vector2 vector2 = targetPos - Projectile.Center;
                    if (vector2.LengthSquared() > 20)
                    {
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (targetPos - Projectile.Center).SafeNormalize(default) * Setting.Speed, 0.08f);
                    }
                    Projectile.spriteDirection = (vector2.X > 0).ToDirectionInt();
                    break;
                case 2: // 点灯冲刺
                    Projectile.velocity *= 0.5f;
                    Projectile.Center = SpawnProj.Projectile.velocity * 1.5f + Player.Center;
                    Projectile.spriteDirection = Projectile.direction;
                    Projectile.ai[0] = 0;
                    break;
                case 3: // 贯虫
                    Projectile.localNPCHitCooldown = 0;
                    if (Player.velocity.Y == 0) // 落地
                    {
                        Vector2 downPos = Player.Center;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, (downPos - Projectile.Center).SafeNormalize(default) * Setting.Speed * 1.5f, 0.1f);
                        if ((downPos - Projectile.Center).Length() < 20)
                        {
                            Projectile.localNPCHitCooldown = 15;
                            Projectile.ai[1] = 0;
                            Projectile.ai[0] = 0;
                        }
                    }
                    break;
                case 4: // 觉虫击
                    Projectile.localNPCHitCooldown = 2;
                    Projectile.numHits = 0;
                    Projectile.spriteDirection = Projectile.direction;
                    Projectile.damage *= 15;
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.FireworkFountain_Red);
                    dust.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.2f;
                    dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.FireworkFountain_Red);
                    dust.velocity = Projectile.velocity.RotatedBy(-MathHelper.PiOver2) * 0.2f;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(Player.Center, Projectile.velocity.SafeNormalize(default), Projectile.ai[1] * 3, 0.1f, 10, -1));
                    if (Projectile.ai[1]++ > 60 * (Projectile.extraUpdates + 1))
                    {
                        Projectile.extraUpdates = 0;
                        Projectile.ai[1] = 0;
                        Projectile.ai[0] = 5;
                    }
                    break;
                case 5: // 觉虫击冷却
                    Projectile.velocity *= 0.8f;
                    Vector2 toProjVel = (Projectile.Center - Player.Center);
                    Player.velocity = toProjVel.SafeNormalize(default) * 36;
                    Player.SetImmuneTimeForAllTypes(5);
                    if ((int)Projectile.ai[1] % 20 == 0)
                    {
                        for (int i = 0; i < 40; i++)
                        {
                            Dust dust1 = Dust.NewDustDirect(Projectile.Center, 10, 10, DustID.FireworkFountain_Red);
                            dust1.velocity = Vector2.One.RotatedBy(i / 40f * MathHelper.TwoPi) * 1.5f;
                            dust1.noGravity = true;
                        }
                    }
                    if (toProjVel.Length() < 40 || Projectile.ai[1]++ > 90)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0] = 0;
                        Player.velocity *= 0.5f;
                    }
                    break;
                case 6: // 粉尘爆炸
                    foreach(NPC npc in Main.npc)
                    {
                        if (npc.active && npc.type == ModContent.NPCType<InsectDust_Boom>())
                        {
                            npc.velocity = (Projectile.Center - npc.Center) * 0.1f;
                            npc.ai[1] = 59;
                        }
                    }
                    Projectile.velocity *= 0.5f;
                    Projectile.ai[1]++;
                    if (Projectile.ai[1] > 60)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[0] = 0;
                    }
                    break;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
            {
                NPC npc = NPC.NewNPCDirect(Projectile.GetSource_OnHit(target), (int)target.position.X, (int)target.position.Y,ModContent.NPCType<InsectDust_Boom>());
                npc.ai[2] = Projectile.damage;
            }
        }
    }
}
