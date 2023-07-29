using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Renderers;
using WeaponSkill.Weapons.Bows;

namespace WeaponSkill.Particles
{
    public class BowDrawPlayer : ABasicParticle
    {
        public BowDrawPlayer(BowsProj proj)
        {
            this.proj = proj;
        }
        public BowsProj proj;
        public byte ChannelLevel => proj.ChannelLevel;
        public override void Update(ref ParticleRendererSettings settings)
        {
            if(ChannelLevel <= 0 || !proj.Projectile.active)
            {
                ShouldBeRemovedFromRenderer = true;
                proj.SpawnThePlayerDrawPartcles = false;
            }
        }
        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            if (ChannelLevel > 0)
            {
                Player player = proj.Player.Clone() as Player;
                player.GetModPlayer<WeaponSkillPlayer>().BowChannelLeave = ChannelLevel;
                for (int i = 0; i < 4; i++)
                {
                    Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position + Vector2.One.RotatedBy(MathHelper.PiOver2 * i) * (3 + ChannelLevel * 0.8f), player.fullRotation, player.fullRotationOrigin, 0, 1);
                }
                player.GetModPlayer<WeaponSkillPlayer>().BowChannelLeave = 0;
            }
        }
    }
}
