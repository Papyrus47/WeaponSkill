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

namespace WeaponSkill.Weapons.StarBreakerWeapon.StarSpinBlade
{
    public class StarSpinBladeProj : StarBreakerWeaponProj, IBasicSkillProj
    {
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public SwingHelper SwingHelper;
        public float SwingLenght;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                Player = itemUse.Player;
                Projectile.Size = itemUse.Item.Size;
                SwingHelper = new(Projectile, 24);
                SwingLenght = 120;
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
            if (Player.HeldItem.ModItem is not FrostFistItem || !Player.active || Player.dead) // 玩家手上物品不是生成物品,则清除
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
        }
    }
}
