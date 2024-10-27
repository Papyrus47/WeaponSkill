using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Configs;
using WeaponSkill.Helper;
using WeaponSkill.UI.StarBreakerUI.SkillsTreeUI;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment.Skills;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment
{
    public class FrostBombardment_Proj : StarBreakerWeaponProj, IBasicSkillProj
    {
        public override string Texture => (GetType().Namespace + "." + "FrostBombardment").Replace('.', '/');
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        /// <summary>
        /// 是否使用本枪
        /// </summary>
        public bool IsUseGun;
        public FrostBombardment SourceItem;
        /// <summary>
        /// 霜拳的滑步行走
        /// </summary>
        public int SPMove;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item?.ModItem is FrostBombardment frostBombardment)
            {
                Player = itemUse.Player;
                Projectile.Size = itemUse.Item.Size;
                SourceItem = frostBombardment;
                IsUseGun = true;
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
            Projectile.aiStyle = -1;
            Projectile.Size = new(32, 54);
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000000;
        }
        public void Init()
        {
            OldSkills = new();

            #region 技能创建
            FrostBombardmentNotUse notUse = new(this);
            FrostBombardment_Aim_Shoot frostBombardment_Aim_Shoot = new(this);
            #endregion

            #region 技能添加

            #region 显示的技能树
            #endregion

            notUse.AddSkill(frostBombardment_Aim_Shoot);
            #endregion
            CurrentSkill = notUse;
        }
        public override void AI()
        {
            if (Player.HeldItem.ModItem is not FrostBombardment || !Player.active || Player.dead) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();

            if (IsUseGun)
            {
                #region 切换形态
                if (WeaponSkill.BowSlidingStep.JustPressed)
                {
                    SourceItem.InBomMode = !SourceItem.InBomMode;
                }
                #endregion
                #region 霜星特殊移动方式,左右键冰滑
                if (GetPlayerDoubleTap(WeaponSkillPlayer.DashRight))
                {
                    SPMove = 1;
                }
                else if (GetPlayerDoubleTap(WeaponSkillPlayer.DashLeft))
                {
                    SPMove = -1;
                }

                if (SPMove != 0)
                {
                    if ((SPMove == 1 && !Player.controlRight) || (SPMove == -1 && !Player.controlLeft))
                    {
                        SPMove = 0;
                        Player.velocity.X *= 0.2f;
                        Player.fullRotationOrigin = Vector2.Zero;
                        Player.fullRotation = 0;

                        for (int i = -5; i <= 5; i++)
                        {
                            Projectile projectile = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center + Vector2.UnitX * (i + 15) * 7, Vector2.Lerp(-Vector2.UnitY, -Player.velocity, 0.05f), 961, 2, 10f, Projectile.owner, 0f, 0.05f * (6 - i));
                            projectile.friendly = true;
                            projectile.hostile = false;
                            projectile.penetrate = -1;
                            projectile.usesLocalNPCImmunity = true;
                            projectile.localNPCHitCooldown = -1;
                        }
                        return;
                    }
                    Player.velocity.X = 25 * SPMove;
                    Player.fullRotationOrigin = new Vector2(0, Player.height);
                    Player.ChangeDir((Player.velocity.X > 0).ToDirectionInt());
                    Projectile.position.Y -= 10;
                    Projectile.position.X += 10 * Projectile.direction;
                    Player.fullRotation = -1f * Player.direction;
                    Projectile projectile1 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Player.Center, Vector2.Lerp(-Vector2.UnitY, -Player.velocity, 0.05f), ModContent.ProjectileType<FrostBombardment_PlayerSPMove>(), 20, 0f, Projectile.owner);
                    //projectile.friendly = true;
                    //projectile.hostile = false;
                    //projectile.penetrate = 1;
                    projectile1.scale = 0.4f * Main.rand.NextFloat(0.6f, 1f);
                }
                #endregion
            }
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
            Player.statMana = Math.Min(Main.player[Projectile.owner].statMana + 2, Player.statManaMax2);
            Player.ManaEffect(2);
            CurrentSkill.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //SkillsTreeUI.nowSkill = CurrentSkill;
            CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D UITex = ModContent.Request<Texture2D>(Texture +"_ChangedUI").Value;
            Rectangle rect = new(0, 0, 76, 28);
            Vector2 position = Player.Center + new Vector2(0,Player.gfxOffY) - Main.screenPosition;
            position.X -= rect.Width / 2;
            position.Y -= 70;
            Main.spriteBatch.Draw(UITex, position, rect, Color.White, 0, new Vector2(0, rect.Height / 2), 1.5f, SpriteEffects.None, 0f);
            rect = new(12, 28, (int)(62 * (SourceItem.ChangeLevel / 10f)), 28);
            Main.spriteBatch.Draw(UITex, position, rect, Color.White, 0, new Vector2(-12, rect.Height / 2), 1.5f, SpriteEffects.None, 0f);
        }
    }
}
