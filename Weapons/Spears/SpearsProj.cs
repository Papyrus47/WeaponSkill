using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.Spears.Skills;
using Terraria.ID;
using WeaponSkill.Weapons.Shortsword;
using Terraria.Enums;

namespace WeaponSkill.Weapons.Spears
{
    public class SpearsProj : ModProjectile, IBasicSkillProj
    {
        public Item SpawnItem;
        public Player Player;
        public int SpawnItem_OriginShootProj;
        public float WeaponLength;
        public SwingHelper SwingHelper;
        public float RotCorrect;
        public Texture2D DrawColorTex => SpawnItem.GetGlobalItem<SpearsGlobalItem>().DrawColorTex;
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public override void OnSpawn(IEntitySource source)
        {
            if(source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                SpawnItem_OriginShootProj = SpawnItem.shoot;
                Player = itemUse.Player;
                if (SpawnItem.type != ItemID.MonkStaffT2) SwingHelper = new SpearsSwingHelper(Projectile, 14, TextureAssets.Projectile[SpawnItem_OriginShootProj]);
                else SwingHelper = new SwingHelper(Projectile, 14, TextureAssets.Projectile[SpawnItem_OriginShootProj]);
                if (Player.HeldItem != SpawnItem || Player.dead) // 玩家手上物品不是生成物品,则清除
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Size = TextureAssets.Projectile[SpawnItem_OriginShootProj].Size() * Projectile.scale;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.2f;
                WeaponLength = Projectile.Size.Length();
                Main.projFrames[Type] = Main.projFrames[SpawnItem_OriginShootProj];
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
            if (Player.HeldItem != SpawnItem || Player.dead || SpawnItem.GetGlobalItem<SpearsGlobalItem>().SPItem) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            TheUtility.SetProjFrameWithItem(Projectile, SpawnItem);
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
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ItemLoader.ModifyHitNPC(SpawnItem, Player, target, ref modifiers);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float rot = Projectile.rotation;
            Projectile.rotation += RotCorrect * Projectile.spriteDirection;
            bool flag = CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
            Projectile.rotation = rot;
            return flag;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
        }

        public void Init()
        {
            OldSkills = new();
            Func<float> swingSpeed = () => 60 / SpawnItem.useAnimation;
            Func<int> spurtsUseTime = () => Player.itemAnimationMax / 2;
            SpearsNotUse spearsNotUse = new(this);

            SpearsSpurts_Channel ssc1 = new(this,() => (int)(Player.itemAnimationMax * 2.5f), spurtsUseTime, () => Projectile.originalDamage * 5);
            ssc1.ChannelEndSound = new("WeaponSkill/Sounds/Spears/Spears_ChannelEnd_Big");
            SpearsSpurts_Channel ssc2 = new(this, () => (int)(Player.itemAnimationMax * 1.5f), spurtsUseTime, () => Projectile.originalDamage * 3);
            SpearsSpurts_Channel ssc3 = new(this, () => Player.itemAnimationMax, spurtsUseTime, () => Projectile.originalDamage);

            SpearsSwing swing1 = new(this)
            {
                StartVel = -Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VelScale = Vector2.One,
                TimeChangeMax = 180,
                TimeChange = swingSpeed,
                SwingDirectionChange = false,
                VisualRotation = 0,
                Sound = new("WeaponSkill/Sounds/Spears/Spears_SlashUp"),
                OnHitFunc = SlashUp_OnHit
            };
            SpearsSwing swing2 = new(this)
            {
                StartVel = -Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                TimeChangeMax = 180,
                TimeChange = swingSpeed,
                VelScale = Vector2.One,
                Sound = new("WeaponSkill/Sounds/Spears/Spears_SlashDown"),
                VisualRotation = 0,
            };
            SpearsSwing swing3 = new(this)
            {
                StartVel = (-Vector2.One).RotatedBy(-0.5),
                SwingRot = MathHelper.PiOver2 * 3.5f,
                TimeChangeMax = 180,
                TimeChange = swingSpeed,
                VelScale = new Vector2(1,0.6f),
                SwingDirectionChange = false,
                Sound = new("WeaponSkill/Sounds/Spears/Spears_SlashUp"),
                VisualRotation = 0.4f
            };
            SpearsSpurts_Coiled coiled1 = new(this, () => Player.itemAnimationMax / 5, () => (int)((180 - Player.itemAnimationMax) / 18f));

            SpearsSwing SlashUp = new(this)
            {
                StartVel = -Vector2.One,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                VelScale = Vector2.One,
                TimeChangeMax = 60,
                TimeChange = swingSpeed,
                SwingDirectionChange = false,
                VisualRotation = 0,
                Sound = new("WeaponSkill/Sounds/Spears/Spears_SlashUp"),
                OnHitFunc = SlashUp_OnHit
            };

            spearsNotUse.AddSkill(ssc1).AddSkill(ssc2).AddSkill(ssc3);
            spearsNotUse.AddSkill(swing1).AddSkill(swing2).AddSkill(swing3).AddSkill(coiled1);

            ssc1.AddSkill(SlashUp).AddSkill(ssc2);
            ssc2.AddSkill(swing3);

            swing1.AddSkill(ssc2);
            swing2.AddSkill(ssc3);
            CurrentSkill = spearsNotUse;
        }
        public virtual void SlashUp_OnHit(NPC npc,NPC.HitInfo hitInfo,int damageDone)
        {
            if(npc.knockBackResist != 0) npc.velocity.Y -= MathF.Log(Projectile.knockBack * (10 / (npc.knockBackResist + 0.1f)) * 3);
        }
    }
}
