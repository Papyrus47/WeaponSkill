using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Command;
using WeaponSkill.Command.SwingHelpers;
using WeaponSkill.Weapons.SwordShield.Skills;

namespace WeaponSkill.Weapons.SwordShield
{
    public class SwordShieldProj : ModProjectile,IBasicSkillProj
    {
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public SwordShield_Shield swordShield_Shield;
        //public Texture2D ShieldTex => SpawnItem.GetGlobalItem<SwordShieldGlobalItem>().ShieldTex.Value;
        public override void SetDefaults()
        {
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.hide = true;
        }
        public virtual float TimeChange(float time)
        {
            //if (chargeBladeGlobal.InAxe)
            //{
            //    //return MathF.Pow(time, 4f);
            //    return MathHelper.SmoothStep(0, 2f, MathF.Pow(time * 0.6f, 2f));
            //}
            return MathF.Pow(time, 3f);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                SwingHelper = new(Projectile, 16, TextureAssets.Item[SpawnItem.type]);
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.8f;
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
            swordShield_Shield ??= new(this, SpawnItem.GetGlobalItem<SwordShieldGlobalItem>().ShieldTex);
            Player.GetModPlayer<WeaponSkillPlayer>().HeldShield = swordShield_Shield;
            TheUtility.SetProjFrameWithItem(Projectile, SpawnItem);
            Projectile.timeLeft = 2;
            swordShield_Shield.Defence = SpawnItem.defense;
            CurrentSkill.AI();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
            swordShield_Shield.DefSucceeded = false;
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
            return CurrentSkill.PreDraw(Main.spriteBatch,ref lightColor);
        }
        public void Init()
        {
            OldSkills = new();
            #region 技能创建 
            SwordShield_NoUse swordShield_NoUse = new(this);

            SwordShield_Swing SlashDown = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.7f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = true
            };

            SwordShield_Swing SlashAcross = new(this, () => Player.controlUseItem)
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = false
            };

            SwordShield_Swing SlashTwo_1 = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = true
            };
            SwordShield_Swing SlashTwo_2 = new(this, () => true)
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = false
            };

            SwordShield_Swing SlashBackAcross = new(this, () => Player.controlUseTile)
            {
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = true,
                ActionDmg = 1.1f
            };

            SwordShield_Swing SlashUp = new(this, () => Player.controlUseTile)
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = false,
                ActionDmg = 1.2f
            };

            SwordShield_Swing RotSlash = new(this, () => Player.controlUseTile)
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.TwoPi,
                SwingDirectionChange = false,
                ActionDmg = 1.3f
            };
            SwordShield_BackMove swordShield_BackMove = new(this);

            SwordShield_Def swordShield_Def = new(this);

            SwordShield_ShieldFly swordShield_ShieldFly = new(this);
            SwordShield_Fall swordShield_Fall = new(this);

            SwordShield_Swing StrongSlash1 = new(this, () => Player.controlUseItem)
            {
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = true,
                StrongSlash = true,
                ActionDmg = 2,
                SwingAI =() =>
                {
                    if (Projectile.ai[1] < 10 && (int)Projectile.ai[0] == 0)
                    {
                        Player.velocity.X = Player.direction * 50;
                    }
                    else
                    {
                        Player.velocity.X = 0;
                    }
                }
            };
            SwordShield_Swing StrongSlash2 = new(this, () => true)
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = false,
                StrongSlash = true,
                ActionDmg = 2.5f,
            };
            SwordShield_Swing StrongSlash3 = new(this, () => true)
            {
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = true,
                StrongSlash = true,
                ActionDmg = 3f,
            };
            SwordShield_Swing StrongSlash4 = new(this, () => true)
            {
                StartVel = Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = false,
                StrongSlash = true,
                ActionDmg = 2.5f,
            };
            SwordShield_Swing StrongSlash5 = new(this, () => true)
            {
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = true,
                StrongSlash = true,
                ActionDmg = 3f,
            };
            SwordShield_Swing StrongSlash6 = new(this, () => true) //升龙
            {
                StartVel = -Vector2.UnitY,
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0,
                SwingRot = MathHelper.Pi,
                SwingDirectionChange = true,
                StrongSlash = true,
                ActionDmg = 4,
                SwingAI = () =>
                {
                    if (Projectile.ai[1] < 10 && (int)Projectile.ai[0] == 1)
                    {
                        Player.velocity.Y = -10;
                    }
                }
            };
            #endregion
            #region 技能连接
            swordShield_BackMove.AddSkill(StrongSlash1).AddSkill(StrongSlash2).AddSkill(StrongSlash3).AddSkill(StrongSlash4).AddSkill(StrongSlash5).AddSkill(StrongSlash6).AddSkill(swordShield_Fall);

            swordShield_NoUse.AddSkill(swordShield_Def).AddSkill(swordShield_BackMove);
            swordShield_NoUse.AddSkill(SlashDown).AddSkill(SlashAcross).AddSkill(SlashTwo_1).AddSkill(SlashTwo_2);

            SlashBackAcross.AddSkill(SlashUp).AddSkill(RotSlash).AddSkill(swordShield_BackMove);
            SlashBackAcross.AddBySkill(SlashDown, SlashAcross, SlashTwo_2,swordShield_NoUse);

            swordShield_BackMove.AddSkill(swordShield_ShieldFly).AddSkill(swordShield_Fall);
            #endregion
            CurrentSkill = swordShield_NoUse;
        }
    }
}
