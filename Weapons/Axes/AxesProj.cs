using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.NPCs;
using WeaponSkill.Weapons.Axes.Skills;
using WeaponSkill.Weapons.BroadSword;
using WeaponSkill.Weapons.LongSword;

namespace WeaponSkill.Weapons.Axes
{
    public class AxesProj : ModProjectile, IBasicSkillProj
    {
        /// <summary>
        /// 处于战斗状态
        /// </summary>
        public bool InFighting;
        public bool CanUpdatePos;
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public Texture2D DrawColorTex => SpawnItem.GetGlobalItem<AxesGlobalItem>().DrawColorTex;
        public Texture2D DrawSwingColorTex => SpawnItem.GetGlobalItem<AxesGlobalItem>().DrawSwingColorTex;
        public override string Texture => "Terraria/Images/Item_0";
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 18, TextureAssets.Item[SpawnItem.type]);
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.6f;
                Projectile.Size = SpawnItem.Size * Projectile.scale * 2;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
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
        }
        public override void AI()
        {
            if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            TheUtility.SetProjFrameWithItem(Projectile, SpawnItem);
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            OldSkills.TrimExcess();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
            if (InFighting && Projectile.soundDelay-- < 0)
            {
                Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Firework_Red);
                dust.noGravity = true;
                dust.velocity.Y -= 3;
                Projectile.soundDelay = 15;
            }
        }
        public override bool ShouldUpdatePosition() => CanUpdatePos;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathF.Pow(time,2.5f);
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ItemLoader.ModifyHitNPC(SpawnItem, Player, target, ref modifiers);
            CurrentSkill.ModifyHitNPC(target,ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
        }

        public void Init()
        {
            OldSkills = new List<ProjSkill_Instantiation>();

            AxesNotUse notUse = new(this);
            #region 工具形态
            AxesSwing Tool_Swing1 = new(this, () => !InFighting && Player.controlUseItem)
            {
                StartVel = (-Vector2.UnitY).RotatedBy(-0.9),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi * 1.65f,
                VisualRotation = 0,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 3,
            };
            AxesSwing Tool_Swing2 = new(this, () => !InFighting && Player.controlUseItem)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.9),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi * 1.65f,
                VisualRotation = 0,
                SwingDirectionChange = false,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 3,
            };
            #endregion
            #region 武器形态
            AxesSwing axesSwing_Channel = new(this, () => Player.controlUseItem) // 短蓄力下劈
            {
                StartVel = -Vector2.UnitX,
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 2,
                CanChannel = true,
                ChannelTime = 360
            };
            AxesSwing axesSwing_BackSlash = new(this, () => Player.controlUseItem) // 回斜横劈
            {
                StartVel = (-Vector2.UnitX).RotatedBy(0.4),
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0.4f,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 1.6f,
                SwingDirectionChange = false,
                AddKn = 1.3f,
                AddDamage = 1.8f,
                SwingAction = (x) =>
                {
                    Projectile.rotation = 0.5f;
                }
            };
            AxesSwing axesSwing_FastSlashDown = new(this, () => Player.controlUseItem) // 快速下劈
            {
                StartVel = -Vector2.UnitX,
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 0.8f,
                AddDamage = 1.2f,
            };

            AxesSwing axesSwing_WeakSlashUp = new(this, () => Player.controlUseTile) // 弱上挑
            {
                StartVel = (-Vector2.UnitX).RotatedBy(0.4),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0f,
                AddDamage = 0.9f,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 1.6f,
                SwingDirectionChange = false,
            };
            AxesSwing axesSwing_WeakSlashDown = new(this, () => Player.controlUseTile) // 弱下砸
            {
                StartVel = (-Vector2.UnitX).RotatedBy(-0.4),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0f,
                AddDamage = 1.2f,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 1.6f,
            };

            AxesSwing axesSwing_SlashUp = new(this, () => InFighting && Player.controlUseTile) // 上挑
            {
                StartVel = (-Vector2.UnitX).RotatedBy(0.4),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0f,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 2.6f,
                AddDamage = 2,
                AddKn = 1.3f,
                SwingDirectionChange = false,
                OnHitFunc = OnHitNPC_SlashUp
            };
            AxesSwing axesSwing_SlashDown = new(this, () => Player.controlUseTile) // 下砸
            {
                StartVel = (-Vector2.UnitX).RotatedBy(-0.4),
                VelScale = Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0f,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 2.6f,
                AddDamage = 1.3f,
                AddKn = 2,
                OnHitFunc = OnHitNPC_SlashDown
            };

            AxesSwing axesSwing_Slash = new(this, () => Player.controlUseItem) // 横斩
            {
                StartVel = (-Vector2.UnitX).RotatedBy(-0.4),
                VelScale = new Vector2(1.3f,0.3f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = 0.7f,
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 2.6f,
                AddDamage = 1.8f,
                AddKn = 2.5f,
                SwingAction = (x) =>
                {
                    Projectile.rotation = -0.1f;
                }
            };

            AxesSpurts axesSpurts = new(this, null)
            {
                TimeChange = () => 1f / Player.itemAnimationMax,
                TimeChangeMax = 2.4f
            }; // 斧突刺
            #endregion

            #region 添加技能表

            #region 工具形态
            notUse.AddSkill(Tool_Swing1).AddSkill(Tool_Swing2).AddSkill(Tool_Swing1);
            #endregion

            #region 武器形态
            notUse.AddSkill(axesSwing_Channel).AddSkill(axesSwing_BackSlash).AddSkill(axesSwing_FastSlashDown);
            notUse.AddSkill(axesSwing_SlashUp);

            axesSwing_Channel.AddSkill(axesSwing_SlashUp).AddSkill(axesSwing_SlashDown);
            axesSwing_Channel.AddSkill(axesSpurts);
            axesSwing_BackSlash.AddSkill(axesSwing_WeakSlashUp).AddSkill(axesSwing_WeakSlashDown);

            axesSpurts.AddSkilles(axesSwing_Slash, axesSwing_WeakSlashDown);

            axesSwing_SlashUp.AddSkill(axesSwing_Slash);
            #endregion

            #endregion
            CurrentSkill = notUse;
        }
        public void OnHitNPC_SlashUp(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(target.knockBackResist != 0)
            {
                target.velocity.Y -= hit.Knockback;
                target.velocity.X *= 0.001f;
                if (target.velocity.Y < -8) target.velocity.Y = -8;
            }
        }
        public void OnHitNPC_SlashDown(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.knockBackResist != 0)
            {
                target.velocity.Y += hit.Knockback;
                if (target.velocity.Y > 8) target.velocity.Y = 8;
            }
        }
    }
}
