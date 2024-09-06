using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.Staffs.Skills;

namespace WeaponSkill.Weapons.Staffs
{
    public class MagicStaffsProj : ModProjectile, IBasicSkillProj
    {
        public override string Texture => "Terraria/Images/Item_0";
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 16, TextureAssets.Item[SpawnItem.type]);
                Projectile.ai[0] = -1;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 1f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                Init();
            }
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
            Projectile.damage = Player.GetWeaponDamage(SpawnItem);
            Projectile.CritChance = Player.GetWeaponCrit(SpawnItem);
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            //ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            //TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
        }
        public void Init()
        {
            OldSkills = new();

            CurrentSkill = new HeldMagicStaff(this);
            //CurrentSkill.AddSkill(SpawnItem.GetGlobalItem<MagicStaffsGlobalItem>().UseSkills[0]);
            var addSkill = CurrentSkill;
            #region 这里面特判添加技能
            for(int i = 0;i < SpawnItem.GetGlobalItem<MagicStaffsGlobalItem>().magicStaffsSetting.Count; i++)
            {
                var skill = SpawnItem.GetGlobalItem<MagicStaffsGlobalItem>().magicStaffsSetting[i];

                #region 特判区
                if(skill is MagicStaffsSetting_Swing swing)
                {
                    if (!swing.WillAddOther)
                    {
                        var swingSkill = new MagicStaffsSwing(this, swing.TimeChange, swing.ChangeCondition)
                        {
                            ChangeLerpSpeed = swing.ChangeLerpSpeed,
                            SwingRot = swing.SwingRot,
                            VelScale = swing.VelScale,
                            Shoot = swing.Shoot,
                            StartVel = swing.StartVel,
                            SwingDirectionChange = swing.SwingDirectionChange,
                            VisualRotation = swing.VisualRotation,
                            SwingTime = swing.SwingTime,
                            OnUse = swing.OnUse,
                        };
                        addSkill.AddSkill(swingSkill);
                        addSkill = swingSkill;
                    }
                    else if (swing.WillAddOther)
                    {
                        var swingSkill = new MagicStaffsSwing(this, swing.TimeChange, swing.ChangeCondition)
                        {
                            ChangeLerpSpeed = swing.ChangeLerpSpeed,
                            SwingRot = swing.SwingRot,
                            VelScale = swing.VelScale,
                            Shoot = swing.Shoot,
                            StartVel = swing.StartVel,
                            SwingDirectionChange = swing.SwingDirectionChange,
                            VisualRotation = swing.VisualRotation,
                            SwingTime = swing.SwingTime,
                            OnUse = swing.OnUse,
                        };
                        CurrentSkill.AddSkill(swingSkill);
                    }
                }
                #endregion
            }
            #endregion
        }
    }
}
