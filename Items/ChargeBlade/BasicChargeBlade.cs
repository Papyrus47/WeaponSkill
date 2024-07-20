using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.ChargeBlade;

namespace WeaponSkill.Items.ChargeBlade
{
    public abstract class BasicChargeBlade : ModItem
    {
        public abstract Asset<Texture2D> ShieldTex { get; }
        public sealed override void SetDefaults()
        {
            _ = ShieldTex;
            ChargeBladeGlobalItem.WeaponID ??= new();
            ChargeBladeGlobalItem.WeaponID.Add(Type);
            InitDefaults();
        }
        public virtual void InitDefaults() { }
        /// <summary>
        /// 属性解放爆炸
        /// </summary>
        /// <param name="target"></param>
        public virtual void LiberateHit(NPC target,Player player)
        {
            int def = target.defense;
            target.defense = 0;
            player.ApplyDamageToNPC(target,player.GetWeaponDamage(Item), 0f, player.direction, false);
            for (int i = 0; i < 15; i++)
            {
                var fire = new Particles.Fire(25);
                fire.SetBasicInfo(null, null, Vector2.One.RotatedBy(i / 15f * MathHelper.TwoPi) * 3, target.Center);
                fire.color = Color.Gold * 0.8f;
                fire.ScaleVelocity = new Vector2(-0.04f);
                fire.Scale = new Vector2(1f);
                fire.color.A = 100;
                Main.ParticleSystem_World_BehindPlayers.Add(fire);
            }
            target.defense = def;
        }
        /// <summary>
        /// 属性解放的颜色
        /// </summary>
        /// <returns></returns>
        public virtual Color LiberateColor => Color.Gold;
        public virtual void SuperLiberateDust(Projectile projectile) { }
        public virtual bool SuperLiberateCollided(Rectangle projHitBox,Rectangle targetHitBox) => false;
    }
}
