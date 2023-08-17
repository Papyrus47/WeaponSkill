using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Renderers;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.DualBlades.Skills;

namespace WeaponSkill.Weapons.DualBlades
{
    public class DualBladesProj : ModProjectile, IBasicSkillProj
    {
        public class DualBlades
        {
            public DualBladesProj dualBladesProj;
            public SwingHelper SwingHelper;
            public DualBlades(DualBladesProj dualBladesProj,SwingHelper swingHelper)
            {
                SwingHelper = swingHelper;
                this.dualBladesProj = dualBladesProj;
            }
            public virtual void AI()
            {

            }
            public virtual void Draw(ref PlayerDrawSet drawinfo)
            {
                DrawData data = new();
                Texture2D tex = dualBladesProj.DrawProjTex.Value;
                Projectile projectile = dualBladesProj.Projectile;
                data.texture = tex;
                data.sourceRect = tex.Frame(1, Main.projFrames[dualBladesProj.Type]);
                data.rotation = projectile.rotation;
                data.scale = new(projectile.scale);
                data.effect = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                drawinfo.DrawDataCache.Add(data);
            }
            public virtual void Draw(SpriteBatch sb,Color drawColor)
            {

            }
        }
        /// <summary>
        /// 玩家手持的刀
        /// </summary>
        public DualBlades HeldBlades;
        /// <summary>
        /// 在玩家底下拿着的刀
        /// </summary>
        public DualBlades BackBlades;
        public Asset<Texture2D> DrawProjTex;
        public Item SpawnItem;
        public Player Player;
        public float SwingLength;
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                Projectile.ai[0] = -1;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.2f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                SwingLength = Projectile.Size.Length();
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                Init();
            }
        }
        public override void SetDefaults()
        {
            Projectile.scale = 1.5f;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            On_PlayerDrawLayers.DrawPlayer_10_BackAcc += On_PlayerDrawLayers_DrawPlayer_10_BackAcc;
        }

        public void On_PlayerDrawLayers_DrawPlayer_10_BackAcc(On_PlayerDrawLayers.orig_DrawPlayer_10_BackAcc orig, ref PlayerDrawSet drawinfo)
        {
            //orig.Invoke(ref drawinfo);
        }
        public override void Kill(int timeLeft)
        {
            On_PlayerDrawLayers.DrawPlayer_10_BackAcc -= On_PlayerDrawLayers_DrawPlayer_10_BackAcc;
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
            Player.ResetMeleeHitCooldowns();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathHelper.SmoothStep(0, 2f, time);
        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.Draw(DrawColorTex, new Vector2(500), null, Color.White, 0f, default, 4, SpriteEffects.None, 0f);
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ItemLoader.ModifyHitNPC(SpawnItem,Player, target, ref modifiers);
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
            CurrentSkill = new DualBladesNotUse(this);
        }
    }
}
