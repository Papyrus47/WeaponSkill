 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using WeaponSkill.Helper;
using WeaponSkill.Particles;
using WeaponSkill.Weapons.Crossbow.Skills;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostBombardment;

namespace WeaponSkill.Weapons.Crossbow
{
    public class CrossbowProj : ModProjectile,IBasicSkillProj
    {
        public Item SpawnItem;
        public Player Player;
        public override string Texture => "Terraria/Images/Item_0";
        public List<ProjSkill_Instantiation> OldSkills { get; set; }
        public ProjSkill_Instantiation CurrentSkill { get; set; }
        public CrossbowGlobalItem globalItem => SpawnItem.GetGlobalItem<CrossbowGlobalItem>();
        public CrossbowHeld HeldShoot;
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
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem);
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
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
            TheUtility.SetProjFrameWithItem(Projectile, SpawnItem);
            Projectile.timeLeft = 2;
            globalItem.CosumeAmmo = true;
            CurrentSkill.AI();
            IBasicSkillProj basicSkillProj = this;
            basicSkillProj.SwitchSkill();
            Projectile.damage = Player.GetWeaponDamage(Player.HeldItem);

            if (Player.controlUseTile)
            {
                Main.instance.CameraModifiers.Add(new CameraModifierToScreenPos(CameraModifierToScreenPos.GetScreenPos(Player.Center + (Main.MouseWorld - Player.Center) * 0.3f), 2));
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => CurrentSkill.CanDamage();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CurrentSkill.Colliding(projHitbox, targetHitbox);
        public virtual float TimeChange(float time) => MathF.Pow(time, 2.5f);
        public override bool PreDraw(ref Color lightColor)
        {
            ReLogic.Graphics.DynamicSpriteFont font = FontAssets.MouseText.Value;
            WeaponSkillPlayer weaponSkillPlayer = Player.GetModPlayer<WeaponSkillPlayer>();
            Item item = weaponSkillPlayer.AmmoItems[weaponSkillPlayer.UseAmmoIndex];
            if (SpawnItem.GetGlobalItem<CrossbowGlobalItem>().CrossbowLoadArrow.TryGetValue(item, out var @ref))
            {
                Color textColor = Color.White;
                string text = @ref.Value + "/ 10";
                Vector2 origin = font.MeasureString(text);
                Vector2 textPos = Main.MouseScreen + new Vector2(0, -origin.Y * (Main.mouseY > Main.screenHeight * 0.5f).ToDirectionInt());
                Utils.DrawBorderStringFourWay(Main.spriteBatch, font, text, textPos.X, textPos.Y, textColor, Color.Black, origin * 0.5f, 0.8f);
            }
            return CurrentSkill.PreDraw(Main.spriteBatch, ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CurrentSkill.OnHitNPC(target, hit, damageDone);
        }
        public void Init()
        {
            OldSkills = new();
            CrossbowNotUse notUse = new(this);
            HeldShoot = new(this);
            CrossbowResetArrow crossbowResetArrow = new(this);
            HeldShoot.AddSkill(crossbowResetArrow);
            notUse.AddSkill(HeldShoot);
            CurrentSkill = notUse;
        }
        public bool PreSkillTimeOut()
        {
            if(CurrentSkill is CrossbowResetArrow)
            {
                CurrentSkill.OnSkillDeactivate();
                HeldShoot.OnSkillActive();
                OldSkills.Add(CurrentSkill);
                CurrentSkill = HeldShoot;
                return false;
            }
            return true;
        }
    }
}
