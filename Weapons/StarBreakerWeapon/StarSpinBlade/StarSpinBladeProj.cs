using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.UI.StarBreakerUI.SkillsTreeUI;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;
using WeaponSkill.Weapons.StarBreakerWeapon.General;
using WeaponSkill.Weapons.StarBreakerWeapon.General.ElementDamage;
using WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade.Skills;

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade
{
    public class StarSpinBladeProj : StarBreakerWeaponProj, IBasicSkillProj
    {
        public override string Texture => (GetType().Namespace + ".StarSpinBlade").Replace('.','/');
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public SwingHelper SwingHelper;
        public float SwingLenght;
        public bool CanChangeToStopActionSkill;
        public SSB_NoUse noUse;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                Player = itemUse.Player;
                Projectile.Size = itemUse.Item.Size;
                SwingHelper = new(Projectile, 36);
                SwingLenght = itemUse.Item.Size.Length();
                Init();
            }
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            if (Player.HeldItem.ModItem is not StarSpinBlade || !Player.active || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DamageVariationScale *= 0;
            CurrentSkill.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SlashDamage.SlashDamageOnHit();
            CurrentSkill.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SkillsTreeUI.nowSkill = CurrentSkill;
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }
        public void Init()
        {
            OldSkills = new List<ProjSkill_Instantiation>();
            noUse = new(this);
            LeftCombo();
            CurrentSkill = noUse;
        }
        /// <summary>
        /// 左键短按攻击注册
        /// </summary>
        protected void LeftCombo()
        {
            #region 技能创建
            SSB_Swing LeftStart = new(this, () => Player.controlUseItem, (time) => MathF.Pow(time, 4))
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 4,
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0.4f,
                SwingDirectionChange = true,
                SwingTime = 30,
            };

            SSB_Swing TwoSlash_1 = new(this, () => Player.controlUseItem, (time) => MathF.Pow(time, 4))
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 6,
                StartVel = -Vector2.UnitX,
                VelScale = new Vector2(1, 1f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0.4f,
                SwingDirectionChange = false,
                SwingTime = 20,
            };
            SSB_Swing TwoSlash_2 = new(this, () => Player.controlUseItem, (time) => MathF.Pow(time, 4))
            {
                IsTrueSlash = true,
                SpinValue = 100,
                PreTime = 6,
                StartVel = Vector2.UnitY.RotatedBy(0.6),
                VelScale = new Vector2(1, 0.6f),
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VisualRotation = -0.4f,
                SwingDirectionChange = false,
                SwingTime = 15,
                OnUse = (skill) =>
                {
                    SwingHelper.SetRotVel(-0.4f);
                }
            };
            #endregion
            #region 技能连接
            noUse.AddSkill(LeftStart).AddSkill(TwoSlash_1).AddSkill(TwoSlash_2);
            #endregion
        }
    }
}
