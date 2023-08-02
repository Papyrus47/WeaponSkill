using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.Shortsword.Skills;

namespace WeaponSkill.Weapons.Shortsword
{
    public class ShortswordProj : ModProjectile, IBasicSkillProj
    {
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public Texture2D DrawColorTex => SpawnItem.GetGlobalItem<ShortswordGlobalItem>().DrawColorTex;
        public override string Texture => "Terraria/Images/Item_0";

        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                SwingHelper = new(Projectile, 8, TextureAssets.Item[SpawnItem.type]);
                if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.ai[0] = -1;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) * Projectile.scale;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Init();
            }
        }
        public override void SetDefaults()
        {
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1.3f;
        }
        public override void AI()
        {
            if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Player.ChangeDir(((Main.MouseWorld - Player.position).X > 0).ToDirectionInt());
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathF.Pow(time, 2.5f);
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
        }

        public virtual void Init()
        {
            OldSkills = new();
            Func<float> swingSpeed = GetSwingSpeed;
            Func <float> fastSwingSpeed = GetSwingSpeed_Fast;
            ShortswordSwing shortswordSwing1 = new(this)
            {
                VelScale = new Vector2(1, 0.6f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-60)),
                TimeChangeMax = 180,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.4f,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash1")
        };
            ShortswordSwing shortswordSwing2 = new(this)
            {
                VelScale = new Vector2(1, 0.3f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-60)),
                TimeChangeMax = 180,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.6f,
                SwingDirectionChange = false,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash2")
            };
            ShortswordSwing shortswordSwing3 = new(this)
            {
                VelScale = new Vector2(1, 0.8f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-60)),
                TimeChangeMax = 180,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.2f,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash1")
            };

            ShortswordSpurts shortswordSpurts = new(this)
            {
                GetSpurtDir = () => (Main.MouseWorld - Player.Center).SafeNormalize(default),
                SpurtTimeMax = () => Player.itemAnimationMax * 0.75f,
            };
            ShortswordSpurts shortswordSpurts1 = new(this)
            {
                GetSpurtDir = () => (Main.MouseWorld - Player.Center).SafeNormalize(default),
                SpurtTimeMax = () => Player.itemAnimationMax,
            };
            ShortswordSpurts shortswordSpurts2 = new(this)
            {
                GetSpurtDir = () => (Main.MouseWorld - Player.Center).SafeNormalize(default),
                SpurtTimeMax = () => Player.itemAnimationMax,
            };
            ShortswordSpurts Dush = new(this)
            {
                GetSpurtDir = () => (Main.MouseWorld - Player.Center).SafeNormalize(default),
                SpurtTimeMax = () => Player.itemAnimationMax,
                SpurtAction = () =>
                {
                    if ((int)Projectile.ai[0] == 2)
                    {
                        Projectile.damage *= 3;
                    }
                    Player.SetImmuneTimeForAllTypes(2);
                    Player.immuneAlpha = 0;
                    Player.velocity = Projectile.velocity.SafeNormalize(default) * 90;
                    if (Projectile.ai[0] * 2 >= Player.itemAnimationMax)
                    {
                        Player.velocity *= 0.03f;
                    }
                },
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_MoveSpurts")
            };

            ShortswordSwing BehindSpurt2_Slash1 = new(this)
            {
                VelScale = new Vector2(1, 0.3f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-60)),
                TimeChangeMax = 180,
                TimeChange = fastSwingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.7f,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash1")
            };
            ShortswordSwing BehindSpurt2_Slash2 = new(this)
            {
                VelScale = new Vector2(1, 0.8f),
                StartVel = Vector2.UnitY.RotatedBy(MathHelper.ToRadians(60)),
                TimeChangeMax = 180,
                TimeChange = fastSwingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.2f,
                SwingDirectionChange = false,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash2")
            };
            ShortswordSwing BehindSpurt2_Slash3 = new(this)
            {
                VelScale = new Vector2(1, 1f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-60)),
                TimeChangeMax = 180,
                TimeChange = fastSwingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0f,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash1")
            };
            ShortswordSwing BehindSpurt2_Slash4 = new(this)
            {
                VelScale = new Vector2(1, 0.3f),
                StartVel = Vector2.UnitY.RotatedBy(MathHelper.ToRadians(60)),
                TimeChangeMax = 180,
                TimeChange = fastSwingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.7f,
                SwingDirectionChange = false,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash2")
            };
            ShortswordSwing BehindSpurt2_Slash5 = new(this)
            {
                VelScale = new Vector2(1, 0.3f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-60)),
                TimeChangeMax = 180,
                TimeChange = fastSwingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.7f,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash1")
            };

            ShortswordSwing BehindDash_Slash = new(this)
            {
                VelScale = new Vector2(2.5f, 0.3f),
                StartVel = (-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(-60)),
                TimeChangeMax = 120,
                TimeChange = swingSpeed,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                VisualRotation = 0.7f,
                Sound = new SoundStyle("WeaponSkill/Sounds/Shortsword/Shortsword_Slash1")
            };



            ShortswordNotUse notuse = new(this);

            notuse.AddSkill(shortswordSwing1).AddSkill(shortswordSwing2).AddSkill(shortswordSwing3);
            notuse.AddSkill(shortswordSpurts).AddSkill(shortswordSpurts1).AddSkill(shortswordSpurts2);
            shortswordSwing3.AddSkill(shortswordSpurts);
            shortswordSpurts2.AddSkill(shortswordSwing1);
            shortswordSpurts.AddSkill(shortswordSwing2);
            shortswordSwing1.AddSkill(shortswordSpurts1);

            shortswordSwing2.AddSkill(Dush).AddSkill(BehindDash_Slash).AddSkill(shortswordSpurts);

            shortswordSpurts1.AddSkill(BehindSpurt2_Slash1).AddSkill(BehindSpurt2_Slash2).AddSkill(BehindSpurt2_Slash3).AddSkill(BehindSpurt2_Slash4).AddSkill(BehindSpurt2_Slash5);
            Dush.AddBySkill(BehindSpurt2_Slash1, BehindSpurt2_Slash2, BehindSpurt2_Slash3, BehindSpurt2_Slash4, BehindSpurt2_Slash5);

            CurrentSkill = notuse;
        }

        public virtual float GetSwingSpeed() => 60 / Player.itemAnimationMax;
        public virtual float GetSwingSpeed_Fast() => 90 / Player.itemAnimationMax;
    }
}
