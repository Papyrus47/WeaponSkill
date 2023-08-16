using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using System.Reflection;
using WeaponSkill.Weapons.BroadSword.Skills;
using Terraria;

namespace WeaponSkill.Weapons.BroadSword
{
    public class BroadSwordProj : ModProjectile, IBasicSkillProj
    {
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public override string Texture => "Terraria/Images/Item_0";
        public Texture2D DrawColorTex => SpawnItem.GetGlobalItem<BroadSwordGlobalItem>().DrawColorTex;

        public List<ProjSkill_Instantiation> OldSkills {get;set;}
        public ProjSkill_Instantiation CurrentSkill {get;set;}

        public override void OnSpawn(IEntitySource source)
        {
            if(source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile,16, TextureAssets.Item[SpawnItem.type]);
                Projectile.ai[0] = -1;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 2f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Init();
            }
        }
        public override void SetDefaults()
        {
            Projectile.scale = 1.5f;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void AI()
        {
            if(Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathHelper.SmoothStep(0,2f,time);
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Type type = Player.GetType();
            type.GetField("_spawnVolcanoExplosion", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Player, true);
            type.GetField("_spawnBloodButcherer", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Player, true);
            type.GetField("_batbatCanHeal", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Player, true);
            type.GetField("_spawnTentacleSpikes", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(Player, true);
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem,Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player,Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack,target.whoAmI,Projectile.damage,damageDone);
        }

        public virtual void Init()
        {
            OldSkills = new();
            Func<float> swingSpeed = GetSwingSpeed;
            BroadSwordSwing broadSwordSwing1 = new(this)
            {
                VelScale = Vector2.One,
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-60)),
                ChangeLerpSpeed = 0.2f,
                TimeChangeMax = 300,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0,
                CanChannel = true
            };

            BroadSwordSwing broadSwordSwing2= new(this)
            {
                VelScale = Vector2.One,
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-80)),
                ChangeLerpSpeed = 0.08f,
                TimeChangeMax = 300,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0,
                CanChannel = true,
                SwingAction = (_) =>
                {
                    if ((int)Projectile.ai[1] == 0)Projectile.damage = (int)(Projectile.damage * 1.25f);
                }
            };

            BroadSwordSwing_Strong broadSwordSwing3 = new(this)
            {
                VelScale = Vector2.One,
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-120)),
                ChangeLerpSpeed = 0.08f,
                TimeChangeMax = 300,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0,
                CanChannel = true,
                AIAction = () =>
                {
                    if ((int)Projectile.localAI[0] > 0)
                    {
                        if (Projectile.localAI[1]++ < 50)
                        {
                            Player.fullRotation = MathHelper.TwoPi * (Projectile.localAI[1] / 50f) * Player.direction;
                            Player.fullRotationOrigin = new Vector2(0, Player.height);
                            Player.itemRotation -= Player.fullRotation;
                            Player.velocity.X = Player.direction * (5f + SwingLength / 50);
                            Projectile.ai[1] = 14;
                            Projectile.ai[2] = 180;
                            Player.SetImmuneTimeForAllTypes(120);
                            Player.immuneAlpha = 0;
                            SwingHelper.ProjFixedPlayerCenter(Player, 0, true);
                            for(int i = 0; i < SwingHelper.oldVels.Length; i++)
                            {
                                SwingHelper.oldVels[i] = Vector2.Zero;
                            }
                            return false;
                        }
                    }
                    return true;
                },
                SwingAction = (broadSword) =>
                {
                    Player.immuneAlpha = 0;
                    Player.velocity.X *= 0.6f;
                    if (Projectile.ai[1] == 0)
                    {
                        Player.fullRotation = 0;
                        if ((int)Projectile.localAI[0] > 0) Projectile.damage += Projectile.originalDamage;
                        else Projectile.damage = (int)(Projectile.damage * 1.5f);
                        SpawnItem.GetGlobalItem<BroadSwordGlobalItem>().ProjCanShoot = true;
                        TheUtility.Player_ItemCheck_Shoot(Player, SpawnItem, Projectile.damage);

                    }
                    else if ((int)Projectile.ai[1] == 180)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var dust = Dust.NewDustDirect(Projectile.Center + Projectile.velocity, 10, 10, DustID.Stone);
                            dust.scale = 2;
                            dust.velocity = (-Vector2.UnitY).RotatedByRandom(0.4) * 3;
                            dust.noGravity = true;
                        }

                    }
                    else if (Projectile.ai[1] > 180)
                    {
                        SwingHelper.SetNotSaveOldVel();
                        Player.velocity.X *= 0.4f;
                        if ((int)Projectile.localAI[0] == 0)
                        {
                            Projectile.ai[0] = 0;
                            Projectile.localAI[0]++;
                        }
                    }

                }
            };
            BroadSwordSwing Slash1 = new(this)
            {
                VelScale = new Vector2(1,0.4f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-80)),
                ChangeLerpSpeed = 0.6f,
                TimeChangeMax = 230,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.6f,
                ChangeCondition = () => Player.controlUseTile,
            };
            BroadSwordSwing Slash2 = new(this)
            {
                VelScale = new Vector2(1, 0.6f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-80)),
                SwingDirectionChange = false,
                ChangeLerpSpeed = 0.4f,
                TimeChangeMax = 230,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.4f,
                ChangeCondition = () => Player.controlUseTile,
                SwingAction = (_) =>
                {
                    if (Projectile.ai[1] == 0)
                    {
                        SpawnItem.GetGlobalItem<BroadSwordGlobalItem>().ProjCanShoot = true;
                        TheUtility.Player_ItemCheck_Shoot(Player, SpawnItem, Projectile.damage);
                    }
                }
            };
            BroadSwordBlock broadSwordBlock = new(this);
            BroadSwordNotUse notUse = new(this);

            notUse.AddSkill(broadSwordSwing1).AddSkill(broadSwordSwing2).AddSkill(broadSwordSwing3);
            Slash1.AddBySkill(notUse,Slash2);
            Slash2.AddBySkill(Slash1, broadSwordSwing2);
            broadSwordBlock.AddBySkill(broadSwordSwing1);
            broadSwordBlock.AddSkill(broadSwordSwing3);
            CurrentSkill = notUse;
        }
        public virtual float GetSwingSpeed() => 100 / SpawnItem.useAnimation;
    }
}
