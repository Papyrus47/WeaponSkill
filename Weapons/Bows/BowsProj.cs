using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Helper;
using WeaponSkill.Particles;
using WeaponSkill.Weapons.Bows.Skills;

namespace WeaponSkill.Weapons.Bows
{
    public class BowsProj : ModProjectile,IBasicSkillProj
    {
        public Item SpawnItem;
        public Player Player;
        /// <summary>
        /// 弓的蓄力级别,最多三级
        /// </summary>
        public byte ChannelLevel;
        public bool NoUse;
        public bool SpawnThePlayerDrawPartcles;
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Size = TextureAssets.Item[SpawnItem.type].Size() * Projectile.scale;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem);
                Init();
            }
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
            if (ChannelLevel > 0 && !SpawnThePlayerDrawPartcles)
            {
                SpawnThePlayerDrawPartcles = true;
                BowDrawPlayer bowDrawPlayer = new(this);
                Main.ParticleSystem_World_BehindPlayers.Add(bowDrawPlayer);
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathF.Pow(time, 2.5f);
        public override bool PreDraw(ref Color lightColor) => CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
        }
        public void Init()
        {
            OldSkills = new();
            BowNotUse bowNotUse = new(this);
            BowHeld held = new(this);
            BowChannelShoot bowChannelShoot = new(this)
            {
                ChannelTime = () => (int)MathF.Log(800 / Player.itemAnimationMax) * 13,
                ShootTime = () => 12
            };
            BowJustShoot bowJustShoot = new(this)
            {
                ShootTime = () => 30
            };
            BowSlidingStep bowSlidingStep = new(this);
            BowHangingShoot bowHangingShoot = new(this)
            {
                ShootTime = () => 23
            };

            bowNotUse.AddSkill(held);

            bowJustShoot.AddSkill(bowSlidingStep);
            bowSlidingStep.AddSkill(bowJustShoot);

            bowChannelShoot.AddSkill(bowHangingShoot);
            bowJustShoot.AddSkill(bowHangingShoot);

            bowHangingShoot.AddSkill(bowJustShoot);
            bowHangingShoot.AddSkill(bowSlidingStep);

            bowJustShoot.AddSkill(bowChannelShoot);
            bowChannelShoot.AddSkill(bowJustShoot);

            bowChannelShoot.AddSkill(bowSlidingStep);
            bowSlidingStep.AddSkill(bowChannelShoot);

            held.AddSkill(bowChannelShoot).AddSkill(bowChannelShoot);
            held.AddSkill(bowJustShoot).AddSkill(bowJustShoot);
            CurrentSkill = bowNotUse;
        }
        public bool PreSkillTimeOut()
        {
            if (OldSkills.Count <= 1) return true;
            var targetSkill = OldSkills.Find(x => x is BowHeld);
            if(targetSkill == null) return true;
            CurrentSkill.OnSkillDeactivate();
            targetSkill.OnSkillActive();
            CurrentSkill = targetSkill;
            OldSkills.RemoveRange(1, OldSkills.Count - 1);
            return false;
        }
    }
}
