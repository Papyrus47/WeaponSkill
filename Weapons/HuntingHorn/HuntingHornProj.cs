using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.InsectStaff.Skills;
using WeaponSkill.Weapons.InsectStaff;
using WeaponSkill.Weapons.HuntingHorn.Skills;
using WeaponSkill.Weapons.Hammer;

namespace WeaponSkill.Weapons.HuntingHorn
{
    public class HuntingHornProj : ModProjectile,IBasicSkillProj
    {
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public override string Texture => "Terraria/Images/Item_0";
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
            if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品
            {
                Projectile.Kill();
                return;
            }
            TheUtility.SetProjFrameWithItem(Projectile, SpawnItem);
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
            (CurrentSkill as BasicHuntingHornSkill).PreAtk = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 16, TextureAssets.Item[SpawnItem.type]);
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem);
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                Init();
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ItemLoader.ModifyHitNPC(SpawnItem, Player, target, ref modifiers);
            CurrentSkill.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
            if (hit.Crit)
            {
                TheUtility.CritProj(Projectile, target, Projectile.velocity.RotatedBy(MathHelper.PiOver2 + MathHelper.PiOver4 * Player.direction).SafeNormalize(default));
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            if (!WeaponSkill.RenderTargetShaderSystem.RenderDraw.Any(x => x is HammerSwingRenderDraw))
            {
                WeaponSkill.RenderTargetShaderSystem.RenderDraw.Add(new HammerSwingRenderDraw());
            }
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }

        public virtual float TimeChange(float time)
        {
            return MathF.Pow(time, 2.5f);
        }
        public void Init()
        {
            OldSkills = new();

            #region 传入技能
            HuntingHornNoUse huntingHornNoUse = new(this);

            HuntingHorn_Swing Knock_Left = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1, 0.5f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi + 0.8f,
                ActionDmg = 1.2f,
                SwingDirectionChange = true
            };
            HuntingHorn_Swing Knock_Right = new(this, () => Player.controlUseTile)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3),
                VelScale = new Vector2(1, 0.5f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi + 0.8f,
                ActionDmg = 1.2f,
                SwingDirectionChange = false
            };
            HuntingHorn_Swing KnockDown_Left = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi + 0.8f,
                ActionDmg = 1.8f,
                SwingDirectionChange = true
            };
            HuntingHorn_Swing KnockUp_Right1 = new(this, () => Player.controlUseTile)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3),
                VelScale = new Vector2(1, 0.5f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi + 0.8f,
                ActionDmg = 1.2f,
                SwingDirectionChange = false
            };
            HuntingHorn_Swing KnockUp_Right2 = new(this, () => true)
            {
                StartVel = -Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi + 1.3f,
                ActionDmg = 1.5f,
                SwingDirectionChange = false,
                PreSwingTimeMax = 5,
                SwingTimeMax = 60
            };

            HuntingHorn_TwoControlSwing huntingHorn_BackSwing = new(this, () => Player.controlUseItem && Player.controlUseTile)
            {
                StartVel = Vector2.UnitX.RotatedBy(0.3),
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi + 1.3f,
                ActionDmg = 2f,
                SwingDirectionChange = false,
                SwingTimeMax = 60,
                SwingAI = () =>
                {
                    Player.velocity.X = -Player.direction;
                }
            };
            HuntingHorn_GroundHit huntingHorn_GroundHit = new(this, () => Player.controlUseItem && Player.controlUseTile);

            HuntingHorn_ShankStrike huntingHorn_ShankStrike = new(this, () => (Player.direction == 1 ? Player.controlLeft : Player.controlRight) && (Player.controlUseItem || Player.controlUseTile));

            HuntingHorn_PoLan huntingHorn_PoLan = new(this);

            HuntingHorn_Swing huntingHorn_PlaySound = new(this, () => WeaponSkill.RangeChange.Current)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.3),
                VelScale = new Vector2(1, 0.5f),
                VisualRotation = 0,
                SwingRot = MathHelper.TwoPi,
                ActionDmg = 2f,
                SwingDirectionChange = false,
                IsPlay = true,
                SwingTimeMax = 60,
            };
            #endregion
            #region 技能连接

            huntingHornNoUse.AddSkill(huntingHorn_PlaySound);

            huntingHorn_PoLan.AddSkill(huntingHorn_ShankStrike).AddSkill(huntingHorn_PoLan);
            huntingHorn_PoLan.AddBySkill(Knock_Left, KnockDown_Left, KnockUp_Right2, Knock_Right, huntingHorn_GroundHit, huntingHorn_BackSwing);
            huntingHorn_ShankStrike.AddBySkill(Knock_Left, KnockDown_Left, KnockUp_Right2, Knock_Right, huntingHorn_GroundHit, huntingHorn_BackSwing);

            huntingHorn_BackSwing.AddBySkill(Knock_Left, KnockDown_Left, Knock_Right);
            huntingHorn_BackSwing.AddSkill(huntingHorn_GroundHit).AddSkill(huntingHorn_BackSwing);
            KnockUp_Right2.AddSkill(Knock_Left);
            Knock_Right.AddSkill(KnockUp_Right1);
            KnockUp_Right1.AddSkill(KnockUp_Right2).AddSkill(Knock_Right);
            Knock_Left.AddSkill(KnockDown_Left).AddSkill(Knock_Right);
            huntingHornNoUse.AddSkill(Knock_Left).AddSkill(Knock_Right).AddSkill(Knock_Left);
            huntingHornNoUse.AddSkill(Knock_Right);
            #endregion
            CurrentSkill = huntingHornNoUse;
        }
        public bool PreSkillTimeOut()
        {
            //if (OldSkills.Count <= 1) return true;
            //if (CurrentSkill is InsectStaff_SkyHeld)
            //{
            //    CurrentSkill.OnSkillDeactivate();
            //    held.OnSkillActive();
            //    CurrentSkill = held;
            //    return false;
            //}
            return true;
        }
    }
}
