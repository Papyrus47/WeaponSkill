using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Items.ChargeBlade;
using WeaponSkill.Weapons.ChargeBlade.Skills;

namespace WeaponSkill.Weapons.ChargeBlade
{
    // 盾斧充能机制
    // 左键攻击按一点算
    public class ChargeBladeProj : ModProjectile, IBasicSkillProj
    {
        /// <summary>
        /// 盾牌的数据
        /// </summary>
        public record ShieldData
        {
            /// <summary>
            /// 最大防御伤害量
            /// </summary>
            public int MaxDmg { get; init; }
            /// <summary>
            /// 伤害减少量
            /// </summary>
            public float MaxReduction { get; init; }
        }
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public SwingHelper SwingHelper;
        public ChargeBladeShield shield;
        public bool shieldCanDraw;
        public ChargeBladeGlobalItem chargeBladeGlobal => SpawnItem.GetGlobalItem<ChargeBladeGlobalItem>();
        public Asset<Texture2D> ShieldTex => chargeBladeGlobal.ShieldTex;
        public ShieldData shieldData;
        public static List<int> DrawChargeBlade;
        public override void Load()
        {
            base.Load();
            DrawChargeBlade = new();
            On_PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart += On_PlayerDrawLayers_DrawPlayer_32_FrontAcc_FrontPart;
            On_Main.DrawPlayers_AfterProjectiles += On_Main_DrawPlayers_AfterProjectiles;
            Main.OnPostDraw += Main_OnPostDraw;
        }

        private void On_Main_DrawPlayers_AfterProjectiles(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig.Invoke(self);
            if (DrawChargeBlade.Count > 0)
            {
                for (int i = 0; i < DrawChargeBlade.Count; i++) // 盾绘制
                {
                    if (Main.projectile[DrawChargeBlade[i]].ModProjectile is not ChargeBladeProj modProj) continue;
                    if (modProj.chargeBladeGlobal.InAxe || !modProj.shieldCanDraw) continue;

                    var shield = modProj.shield;
                    shield.Draw(Main.spriteBatch, Lighting.GetColor((shield.swingHelper.center / 16).ToPoint()));

                    //if (Main.projectile[DrawChargeBlade[i]].ModProjectile is not ChargeBladeProj modProj) continue;
                    //Color color = Lighting.GetColor((modProj.Projectile.Center / 16).ToPoint());
                    //modProj.CurrentSkill.PreDraw(Main.spriteBatch, ref color);
                }
            }
        }
        private static void Main_OnPostDraw(GameTime obj)
        {
            DrawChargeBlade.Clear();
        }
        private static void On_PlayerDrawLayers_DrawPlayer_32_FrontAcc_FrontPart(On_PlayerDrawLayers.orig_DrawPlayer_32_FrontAcc_FrontPart orig, ref PlayerDrawSet drawinfo)
        {
            orig.Invoke(ref drawinfo);
            if (DrawChargeBlade.Count > 0)
            {
                for (int i = 0; i < DrawChargeBlade.Count; i++) // 剑绘制
                {
                    //if (Main.projectile[DrawChargeBlade[i]].ModProjectile is not ChargeBladeProj modProj) continue;
                    //var shield = modProj.shield;
                    //shield.Draw(Main.spriteBatch, Lighting.GetColor((shield.swingHelper.center / 16).ToPoint()));

                    if (Main.projectile[DrawChargeBlade[i]].ModProjectile is not ChargeBladeProj modProj) continue;
                    if (modProj.chargeBladeGlobal.InAxe) continue;

                    Color color = Lighting.GetColor((modProj.Projectile.Center / 16).ToPoint());
                    modProj.CurrentSkill.PreDraw(Main.spriteBatch, ref color);
                }
            }
        }
        public override void Unload()
        {
            DrawChargeBlade = null;
            On_PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart -= On_PlayerDrawLayers_DrawPlayer_32_FrontAcc_FrontPart;
            On_Main.DrawPlayers_AfterProjectiles -= On_Main_DrawPlayers_AfterProjectiles;
            Main.OnPostDraw -= Main_OnPostDraw;
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
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.3f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                if(SpawnItem.ModItem is BasicChargeBlade blade)
                {
                    blade.SetShieldData(ref shieldData);
                }
                if(shieldData == default)
                {
                    shieldData = new()
                    {
                        MaxDmg = 100,
                        MaxReduction = 0.1f
                    };
                }
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
            shield ??= new(this,ShieldTex);

            Projectile.timeLeft = 2;
            shieldCanDraw = true;
            CurrentSkill.AI();
            shield.CheckProjHitMe();
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathHelper.SmoothStep(0, 2f, time * 0.6f);
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            DrawChargeBlade.Add(Projectile.whoAmI);
            if (chargeBladeGlobal.InAxe)
            {
                CurrentSkill.PreDraw(Main.spriteBatch,ref lightColor); // 斧模式专门的绘制
            }
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ItemLoader.ModifyHitNPC(SpawnItem, Player, target, ref modifiers);
            CurrentSkill.ModifyHitNPC(target,ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
            ItemLoader.OnHitNPC(SpawnItem, Player, target, hit, damageDone);
            TheUtility.VillagesItemOnHit(SpawnItem, Player, Projectile.Hitbox, Projectile.originalDamage, Projectile.knockBack, target.whoAmI, Projectile.damage, damageDone);
        }
        public void Init()
        {
            OldSkills = new List<ProjSkill_Instantiation>();
            #region 技能创建
            ChargeBladeNotUse chargeBladeNotUse = new ChargeBladeNotUse(this);

            #region 剑形态下的
            ChargeBlade_OnHeld_Sword chargeBlade_OnHeld_Sword = new ChargeBlade_OnHeld_Sword(this); // 手持

            ChargeBlade_Sword_Swing Sword_SlashDown = new(this, () => Player.controlUseItem) // 直斩
            {
                StartVel = (-Vector2.UnitY).RotatedBy(-0.3f),
                VelScale = new Vector2(1, 0.8f),
                VisualRotation = 0.2f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = true
            };

            ChargeBlade_Sword_Swing Sword_SlashUP = new(this, () => Player.controlUseItem) // 上捞斩
            {
                StartVel = Vector2.UnitY.RotatedBy(0.5f),
                VelScale = new Vector2(1, 1f),
                VisualRotation = 0f,
                SwingRot = MathHelper.Pi + MathHelper.PiOver2,
                SwingDirectionChange = false
            };

            ChargeBlade_Sword_Swing_RotSlash Sword_RotSlash = new(this, () => Player.controlUseItem) // 回旋斩
            {
                StartVel = (-Vector2.UnitX).RotatedBy(0.6),
                VelScale = new Vector2(1, 0.3f),
                VisualRotation = 0.7f,
                SwingDirectionChange = true
            };

            ChargeBlade_OnDef Sword_Def = new(this); // 防御攻击

            ChargeBlade_AddBottles chargeBlade_AddBottles = new(this); // 填瓶子
            #endregion

            #endregion
            #region 技能添加
            chargeBladeNotUse.AddSkill(chargeBlade_OnHeld_Sword).AddSkill(Sword_Def);
            #region 剑形态技能添加
            chargeBlade_OnHeld_Sword.AddSkill(Sword_SlashDown).AddSkill(Sword_SlashUP).AddSkill(Sword_RotSlash);
            Sword_Def.AddBySkill(Sword_SlashDown, Sword_SlashUP, Sword_RotSlash);
            Sword_Def.AddSkill(chargeBlade_AddBottles);
            //Sword_Def.AddBySkill(chargeBladeNotUse, chargeBlade_OnHeld_Sword);
            #endregion
            #endregion
            CurrentSkill = chargeBladeNotUse;
        }
        public bool PreSkillTimeOut()
        {
            if (OldSkills.Count <= 1) return true;

            if (CurrentSkill is ChargeBlade_Sword_Swing || CurrentSkill is ChargeBlade_OnDef || CurrentSkill is ChargeBlade_AddBottles) // 如果是剑挥舞类,或者防御类,或者装瓶
            {
                var targetSkill = OldSkills.Find(x => x is ChargeBlade_OnHeld_Sword && !x.GetType().IsSubclassOf(typeof(ChargeBlade_OnHeld_Sword)));
                if(targetSkill != null)
                {
                    CurrentSkill.OnSkillDeactivate();
                    targetSkill.OnSkillActive();
                    CurrentSkill = targetSkill;
                    return false;
                }
            }
            return true;
        }
    }
}
