using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.ChargeBlade;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.Lances.Skills;

namespace WeaponSkill.Weapons.Lances
{
    public class LancesProj : ModProjectile, IBasicSkillProj
    {
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public LancesShield shield;
        public LancesGlobalItem lancesGlobal => SpawnItem.GetGlobalItem<LancesGlobalItem>();
        public Asset<Texture2D> ShieldTex => lancesGlobal.ShieldTex;
        public Asset<Texture2D> ProjTex => lancesGlobal.ProjTex;
        public LancesHeld lancesHeld;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 16, ProjTex);
                Projectile.ai[0] = -1;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem);
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
            shield ??= new(this, ShieldTex);
            Projectile.timeLeft = 2;
            Player.GetModPlayer<WeaponSkillPlayer>().HeldShield = shield;
            shield.Defence = SpawnItem.defense;
            Projectile.damage = Player.GetWeaponDamage(SpawnItem);
            if (OldSkills.Count > 10) OldSkills.RemoveAt(1);
            CurrentSkill.AI();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
            shield.DefSucceeded = false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathF.Pow(time, 3f);

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            DrawShieldAndWeaponSystem.DrawShieldAndWeapon.Add(Projectile.whoAmI); // 绘制
            //CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor); // 斧模式专门的绘制
            return false;
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
                TheUtility.CritProj(Projectile, target, Projectile.velocity.SafeNormalize(default));
            }
        }
        public void Init()
        {
            OldSkills = new List<ProjSkill_Instantiation>();

            LancesNoUse noUse = new LancesNoUse(this);
            lancesHeld = new LancesHeld(this);

            LancesSpurts lancesSpurts1 = new(this);

            LancesSpurts lancesSpurts2 = new(this);

            LancesSpurts lancesSpurts3 = new(this);

            LancesSpurts lancesSpurtsS1 = new(this)
            {
                ActivationConditionFunc = () => Player.controlUseTile,
                IsMove = false,
                ActionDmg = 1.2f
            };

            LancesSpurts lancesSpurtsS2 = new(this)
            {
                ActivationConditionFunc = () => Player.controlUseTile,
                IsMove = false,
                ActionDmg = 1.2f
            };

            LancesSpurts lancesSpurtsS3 = new(this)
            {
                ActivationConditionFunc = () => Player.controlUseTile,
                IsMove = false,
                ActionDmg = 1.2f
            };

            LancesDef lancesDef =  new(this);
            LancesStrongDef lancesStrongDef = new(this);
            LancesSpurts StrongSpurts = new(this) 
            { 
                ActionDmg = 2,
                ActivationConditionFunc = () => true
            };
            LancesPowerDef lancesPowerDef = new(this);

            LancesDash lancesDash = new(this);

            LancesSwing lancesSwing1 = new(this);
            LancesSwing lancesSwing2 = new(this);
            LancesSpurts StrongSpurts1 = new(this)
            {
                ActionDmg = 2.5f,
            };
            LancesSpurts StrongSpurt2 = new(this)
            {
                ActionDmg = 2,
                ActivationConditionFunc = () => Player.controlUseTile
            };

            LancesShieldDash lancesShieldDash = new(this);
            LancesSpurts_FlySpurts lancesSpurts_FlySpurts = new(this);

            lancesDash.AddSkilles(lancesDef, StrongSpurts1);
            lancesDash.AddBySkill(lancesPowerDef, lancesDef);

            lancesShieldDash.AddSkilles(lancesDash,lancesSpurts_FlySpurts);
            lancesPowerDef.AddSkill(lancesSpurts_FlySpurts);

            lancesDef.AddSkill(lancesShieldDash).AddSkill(lancesSpurts_FlySpurts);
            lancesSpurtsS1.AddSkill(lancesSwing1).AddSkill(lancesSpurtsS2).AddSkill(lancesSwing2).AddSkill(lancesSpurtsS3);
            lancesSpurts1.AddSkill(lancesSwing1).AddSkill(lancesSpurts2).AddSkill(lancesSwing2).AddSkill(lancesSpurts3);
            lancesSpurtsS1.AddSkill(lancesSpurts1).AddSkill(lancesSpurtsS2).AddSkill(lancesSpurts2).AddSkill(lancesSpurtsS3);
            lancesStrongDef.AddSkill(lancesPowerDef);
            lancesPowerDef.AddSkill(StrongSpurts1);
            lancesStrongDef.AddSkill(StrongSpurts);
            lancesStrongDef.AddBySkill(lancesDef);

            lancesDef.AddBySkill(lancesSpurts1, lancesSpurtsS1, lancesSpurtsS2, lancesSpurtsS3, lancesSpurts2, lancesSpurts3, noUse, lancesHeld);

            lancesHeld.AddSkill(lancesSpurtsS1).AddSkill(lancesSpurtsS2).AddSkill(lancesSpurtsS3);
            noUse.AddSkill(lancesHeld).AddSkill(lancesSpurts1).AddSkill(lancesSpurts2).AddSkill(lancesSpurts3);
            CurrentSkill = noUse;
        }
        public bool PreSkillTimeOut()
        {
            if(CurrentSkill is not LancesHeld && CurrentSkill is not LancesNoUse)
            {
                CurrentSkill.OnSkillDeactivate();
                lancesHeld.OnSkillActive();
                CurrentSkill = lancesHeld;
                return false;
            }
            return true;
        }
    }
}
