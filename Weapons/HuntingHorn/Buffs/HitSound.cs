using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.HuntingHorn.Buffs
{
    /// <summary>
    /// 打周波
    /// </summary>
    public class HitSound : HuntingHornBuff
    {
        public HitSound(List<HuntingHornMelody.MelodyType> melodyTypes) : base(melodyTypes)
        {
        }

        public override void OnPlay(Player player, Projectile projectile)
        {
            var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(player.HeldItem), projectile.Center + projectile.velocity, Vector2.Zero, ModContent.ProjectileType<TransparentProj>(), projectile.damage * 3, projectile.knockBack, player.whoAmI);
            proj.Resize(60, 60);
            for(int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(proj.Center, 1, 1, DustID.Smoke);
                dust.velocity = Vector2.One.RotatedBy(i / 20f * MathHelper.TwoPi);
            }
        }
        public override void Register()
        {
        }
    }
}
