using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Items.GunLances;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.GunLances.Skills;

namespace WeaponSkill.Weapons.GunLances
{
    public class GunLancesProj : ModProjectile, IBasicSkillProj
    {
        public class GunPart : PartSwingHelper.Part
        {
            public GunPart(PartSwingHelper onwer) : base(onwer)
            {
            }
            public override void DrawSwingItem(Color drawColor)
            {
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                //if (Onwer.projectile != null)
                //{
                //    Onwer.SwingItemTex ??= TextureAssets.Projectile[Onwer.projectile.type];
                //}
                //var origin = gd.RasterizerState;
                //RasterizerState rasterizerState = new()
                //{
                //    CullMode = CullMode.None,
                //    FillMode = FillMode.WireFrame
                //};
                //gd.RasterizerState = rasterizerState;

                Vector2 velocity = GetOldVel(-1, true);
                //velocity = velocity.RotatedBy(Rot);
                Vector2 halfLength = new Vector2(-velocity.Y, velocity.X).RotatedBy(Onwer.VisualRotation * Onwer.spriteDirection * SPDir).SafeNormalize(default)
                    * Size.Length() * 0.5f * Onwer.spriteDirection * SPDir;

                Vector2 center = Onwer.GetDrawCenter();
                center += OffestCenter;
                if (Onwer._drawCorrections)
                {
                    center = Onwer.Center + (center - Onwer.Center);
                }
                Vector2 halfVelPos = center + velocity * 0.5f;
                Vector2[] pos = new Vector2[4]
                {
                    center - Main.screenPosition,
                    halfVelPos - halfLength - Main.screenPosition,
                    center + velocity - Main.screenPosition,
                    halfVelPos + halfLength  - Main.screenPosition
                };
                Vector2 rotPos = pos[0];
                for (int i = 0; i < 4; i++)
                {
                    Vector2 v = (pos[i] - rotPos).RotatedBy(Rot);
                    pos[i] = rotPos + v;
                }

                float factor = (Onwer.frame + 1f) / Onwer.frameMax;
                if (SPDir == -1)
                    factor = 1 - factor;
                CustomVertexInfo[] customVertices = new CustomVertexInfo[6];
                customVertices[0] = customVertices[5] = new(pos[0], drawColor, new Vector3(0, factor, 0)); // 柄
                customVertices[1] = new(pos[1], drawColor, new Vector3(0, factor - 1f, 0)); // 左上角
                customVertices[2] = customVertices[3] = new(pos[2], drawColor, new Vector3(1, factor - 1f, 0)); // 头
                customVertices[4] = new(pos[3], drawColor, new Vector3(1, factor, 0)); // 右下角

                gd.Textures[0] = DrawTex.Value;
                //gd.Textures[0] = TextureAssets.MagicPixel.Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleList, customVertices, 0, 2);
                //gd.RasterizerState = origin;
            }
            //public Part(object spawnEntity, int oldVelLength, Asset<Texture2D> swingItemTex = null) : base(spawnEntity, oldVelLength, swingItemTex)
            //{
            //}
        }
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public PartSwingHelper SwingHelper;
        public GunLancesShield shield;
        public GunLancesHeld gunLancesHeld;
        public GunLancesGlobalItem GunLancesGlobalItem => SpawnItem.GetGlobalItem<GunLancesGlobalItem>();
        public Asset<Texture2D> ShieldTex => GunLancesGlobalItem.ShieldTex;
        public Asset<Texture2D> ProjTex1 => GunLancesGlobalItem.ProjTex1;
        public Asset<Texture2D> ProjTex2 => GunLancesGlobalItem.ProjTex2;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 16);
                SwingHelper.Parts.Add("Handle", new(SwingHelper)
                {
                    DrawTex = ProjTex1,
                    Size = (SpawnItem.ModItem as BasicGunLancesItem).Proj1Size
                });
                SwingHelper.Parts.Add("Gun", new GunPart(SwingHelper)
                {
                    DrawTex = ProjTex2,
                    Size = (SpawnItem.ModItem as BasicGunLancesItem).Proj2Size,
                    Rot = MathHelper.Pi
                });
                SwingHelper.Parts.Add("Reset", new(SwingHelper)
                {
                    DrawTex = ModAsset.GunLancesDetonateLonghangReset,
                    Size = new(56)
                });
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
            OldSkills = new();

            #region 技能注册
            GunLancesNoUse gunLancesNoUse = new(this);

            gunLancesHeld = new(this);

            GunLancesSpurts gunLancesSpurts1 = new(this,() => Player.controlUseItem);
            GunLancesSpurts gunLancesSpurts2 = new(this, () => Player.controlUseItem);
            GunLancesSpurts gunLancesSpurts3 = new(this, () => Player.controlUseItem);

            GunLancesSwing gunLancesSwing_SwingUp = new(this, () => Player.controlUseItem && Player.controlUseTile)
            {
                StartVel = Vector2.UnitX.RotatedBy(0.8),
                Rot = MathHelper.PiOver2 + 0.8f,
                VelScale = new Vector2(1,1f),
                VisualRotation = 0f,
                SwingDir = -1,
                ActionDmg = 3
            };
            GunLancesSwing gunLancesSwing_SwingDown = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.4),
                Rot = MathHelper.Pi + MathHelper.PiOver2,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                SwingDir = 1,
                ActionDmg = 3
            };
            GunLancesSwing gunLancesSwing_SwingAcross = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.4),
                Rot = MathHelper.Pi + MathHelper.PiOver2,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDir = -1,
                ActionDmg = 5
            };

            GunLances_SwingReset gunLances_SwingReset = new(this, () => Player.controlUseTile && WeaponSkill.BowSlidingStep.Current)
            {
                StartVel = -Vector2.UnitX.RotatedBy(-0.4),
                Rot = MathHelper.Pi + MathHelper.PiOver2,
                VelScale = new Vector2(1, 0.6f),
                VisualRotation = 0.4f,
                SwingDir = 1,
            };

            GunLancesDef gunLancesDef = new(this);

            GunLancesBombardment gunLancesBombardment = new(this, () => Player.controlUseTile);

            GunLancesDetonateLonghangUse longhang = new(this, () => Player.controlUseTile);

            GunLancesReset gunLancesReset = new(this);

            GunLancesCompletelyBurst gunLancesCompletelyBurst = new(this);

            GunLancesDrogueHit gunLancesDrogueHit = new(this);

            GunLancesSuperCompletelyBurst gunLancesSuperCompletelyBurst = new(this);
            #endregion
            #region 技能连接
            gunLancesNoUse.AddSkill(gunLancesHeld);

            gunLancesHeld.AddSkill(gunLancesSuperCompletelyBurst);
            gunLancesDef.AddSkill(gunLancesDrogueHit);

            gunLancesDef.AddSkill(gunLancesReset);

            gunLances_SwingReset.AddSkill(gunLancesSwing_SwingDown);
            gunLances_SwingReset.AddBySkill(gunLancesSpurts1, gunLancesSpurts2, gunLancesSpurts3, gunLancesSwing_SwingUp,gunLancesBombardment,gunLancesSwing_SwingAcross, gunLancesSwing_SwingDown);
            gunLancesCompletelyBurst.AddSkill(longhang);
            gunLancesSwing_SwingUp.AddSkill(gunLancesSwing_SwingDown).AddSkill(gunLancesCompletelyBurst).AddSkill(gunLancesSwing_SwingAcross).AddSkill(longhang);
            gunLancesSwing_SwingUp.AddBySkill(gunLancesHeld, gunLancesSpurts1, gunLancesSpurts2, gunLancesSpurts3);

            gunLancesBombardment.AddSkill(gunLancesBombardment).AddSkill(longhang);
            gunLancesBombardment.AddSkill(gunLancesSwing_SwingDown);
            gunLancesBombardment.AddBySkill(gunLancesHeld, gunLancesSpurts1, gunLancesSpurts2, gunLancesSpurts3, gunLancesSwing_SwingUp);

            gunLancesHeld.AddSkill(gunLancesDef);
            gunLancesNoUse.AddSkill(gunLancesDef);

            gunLancesHeld.AddSkill(gunLancesSpurts1).AddSkill(gunLancesSpurts2).AddSkill(gunLancesSpurts3);

            #endregion
            CurrentSkill = gunLancesNoUse;
            
        }
        public bool PreSkillTimeOut()
        {
            if (CurrentSkill is not GunLancesNoUse && CurrentSkill is not GunLancesHeld)
            {
                CurrentSkill.OnSkillDeactivate();
                gunLancesHeld.OnSkillActive();
                CurrentSkill = gunLancesHeld;
                return false;
            }
            return true;
        }
        public void SwitchSkill()
        {
            if (CurrentSkill.SkillTimeOut)
            {
                if (PreSkillTimeOut())
                {
                    var targetSkill = OldSkills[0];
                    CurrentSkill.OnSkillDeactivate();
                    targetSkill.OnSkillActive();
                    CurrentSkill = targetSkill;
                    OldSkills.Clear();
                }
                return;
            }
            foreach (var targetSkill in CurrentSkill.switchToSkill)
            {
                if (((CurrentSkill as BasicGunLancesSkill).SwitchCondition(targetSkill) && targetSkill.ActivationCondition()) || targetSkill.CompulsionSwitchSkill(CurrentSkill))
                {
                    (CurrentSkill as BasicGunLancesSkill).OnSkillDeactivate(targetSkill);
                    targetSkill.OnSkillActive();
                    OldSkills.Add(CurrentSkill);
                    CurrentSkill = targetSkill;
                    return;
                }
            }
        }
    }
}
