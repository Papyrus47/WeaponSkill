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
        public override string Texture => "Terraria/Images/Item_0";
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 18, TextureAssets.Item[SpawnItem.type]);
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 1.6f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
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
        }
        public override bool ShouldUpdatePosition() => CanUpdatePos;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathHelper.SmoothStep(0, 2f, time);
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

            #region 工具形态攻击
            AxesSwing tool_SlashUp = new(this, () => !InFighting && Player.controlUseItem)
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = false,
                TimeChangeMax = 180,
                TimeChange = () => 2f * Player.pickSpeed,
                SwingAction = AxesCutTile
            };
            AxesSwing tool_SlashDown = new(this, () => !InFighting && Player.controlUseItem)
            {
                StartVel = (-Vector2.UnitY).RotatedBy(0.2),
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                TimeChangeMax = 180,
                TimeChange = () => 2f * Player.pickSpeed,
                SwingAction = AxesCutTile
            };

            AxesSwing tool_Spurt = new(this, () => !InFighting && Player.controlUseTile)
            {
                StartVel = (-Vector2.UnitY).RotatedBy(0.2),
                VelScale = new Vector2(1, 0.4f),
                VisualRotation = 0.6f,
                SwingRot = MathHelper.PiOver2 + 0.64f,
                TimeChangeMax = 180,
                TimeChange = () => 3f * Player.pickSpeed,
                SwingAction = (swing) =>
                {
                    AxesCutTile(swing);
                    AxesCutTile(swing);
                    if (Projectile.ai[0] < swing.TimeChangeMax / 2)
                    {
                        Player.velocity.X = Player.direction * 4;
                    }
                }
            };
            #endregion

            #region 战斗形态攻击
            AxesSwing fight_SlashUp = new(this, () => InFighting && Player.controlUseItem)
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = false,
                TimeChangeMax = 180,
                CanStuck = true,
                TimeChange = () => 2f,
                AddKn = 3,
                KnDir = -Vector2.UnitY
            };

            AxesSwing fight_InclinedSlash = new(this, () => InFighting && Player.controlUseItem)
            {
                StartVel = (-Vector2.UnitY).RotatedBy(-0.6),
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                TimeChangeMax = 180,
                CanStuck = true,
                TimeChange = () => 2f,
                AddDamage = 0.05f,
                AddKn = 3.5f,
                KnDir = new Vector2(1,0.5f)
            };

            AxesSwing fight_AcrossSlash = new(this, () => InFighting && Player.controlUseItem)
            {
                StartVel = Vector2.UnitY.RotatedBy(0.6),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = false,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                TimeChangeMax = 180,
                CanStuck = true,
                TimeChange = () => 2f,
                AddDamage = 0.1f,
                AddKn = 4,
                KnDir = new Vector2(1.5f, 0f)
            };
            #endregion

            #region 工具形态使用
            notUse.AddSkill(tool_SlashUp).AddSkill(tool_SlashDown).AddSkill(tool_SlashUp);
            tool_Spurt.AddBySkill(tool_SlashUp, tool_SlashDown);
            #endregion
            #region 战斗形态使用
            notUse.AddSkill(fight_SlashUp).AddSkill(fight_InclinedSlash).AddSkill(fight_AcrossSlash);
            #endregion
            CurrentSkill = notUse;
        }
        public void AxesCutTile(AxesSwing axesSwing)
        {
            if (axesSwing.TimeChangeMax / 1.2f <= Projectile.ai[0]) return;
            Vector2 center = Projectile.Center + Projectile.velocity; // 获取头部位置

            Point point = (center / 16).ToPoint();
            for(int i = -Projectile.width / 32; i <= Projectile.width / 32; i++)
            {
                for(int j = -Projectile.width / 32; j <= Projectile.width / 32; j++)
                {
                    Point point1 = point + new Point(i, j);
                    if (!Main.tileAxe[Main.tile[point1].TileType])
                    {
                        continue;
                    }
                    Player.PickTile(point1.X, point1.Y,SpawnItem.axe);
                }
            }
        }
    }
}
