using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Items.ChargeBlade;

namespace WeaponSkill.Weapons.ChargeBlade
{
    public class ChargeBlade_SuperLiberateProj : ModProjectile
    {
        /// <summary>
        /// 属性瓶的判定
        /// </summary>
        public bool BottleIsAttribute => Projectile.ai[2] > 0;
        public int UseBottle => (int)Projectile.ai[0];
        public BasicChargeBlade basicChargeBlade;
        public IEntitySource entitySource;
        public override string Texture => "Terraria/Images/Item_0";
        public bool OnCollideTile;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 20;
            Projectile.friendly = true;
            Projectile.Size = new Vector2(5);
            Projectile.hide = true;
            Projectile.penetrate = -1;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.Y = 0;
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            entitySource = source;
            if(source is EntitySource_ItemUse entity && entity.Item.ModItem is BasicChargeBlade basic)
            {
                basicChargeBlade = basic;
                Projectile.ai[2] = basic.Item.GetGlobalItem<ChargeBladeGlobalItem>().BottleIsAttribute.ToInt();
            }
        }
        public override void AI()
        {
            if (BottleIsAttribute) // 属性瓶
            {

            }
            else // 榴弹瓶
            {
                if(!OnCollideTile) Projectile.velocity.Y = 16;
                for (int i = 0; i < 3; i++)
                {
                    var dust = Dust.NewDustDirect(Projectile.Center, 100, 1, DustID.FireworksRGB);
                    dust.velocity.Y = -13 * Main.rand.NextFloat(0.5f, 1f);
                    dust.scale = 0.7f;
                    dust.velocity.X = 0;
                    dust.noGravity = true;
                    dust.color = basicChargeBlade.LiberateColor;
                }
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override void Kill(int timeLeft)
        {
            if (BottleIsAttribute) // 属性瓶
            {
                basicChargeBlade.SuperLiberateDust(Projectile);
            }
            else // 榴弹瓶
            {
                if(UseBottle > 1) Projectile.NewProjectile(entitySource,Projectile.Center + new Vector2(Projectile.velocity.X, 0), Projectile.velocity, ModContent.ProjectileType<ChargeBlade_SuperLiberateProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, UseBottle -1);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (BottleIsAttribute)
            {
                return basicChargeBlade.SuperLiberateCollided(projHitbox,targetHitbox);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (!BottleIsAttribute)
            {
                modifiers.ScalingArmorPenetration += 1f;
            }
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(ref hitbox);
            if (!BottleIsAttribute)
            {
                hitbox.Height += 150;
                hitbox.Width += 100;
                //hitbox.X = 0;
                hitbox.Y -= hitbox.Height;
            }
        }
    }
}
