using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Weapons.InsectStaff
{
    public class InsectDust_Boom : ModNPC
    {
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetDefaults()
        {
            NPC.Size = new(60);
            NPC.lifeMax = 1;
            NPC.friendly = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
        }
        public override void AI()
        {
            NPC.ai[1]++;
            //NPC.ai[2] = 100;
            if (NPC.ai[0]++ > 5)
            {
                NPC.ai[0] = 0;
                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.AncientLight,Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(),0,Color.OrangeRed);
                    dust.noGravity = true;
                }
            }
        }
        public override bool CanBeHitByNPC(NPC attacker) => NPC.ai[1] > 60;
        public override bool? CanBeHitByProjectile(Projectile projectile) => NPC.ai[1] > 60;
        public override bool? CanBeHitByItem(Player player, Item item) => NPC.ai[1] > 60;
        public override void OnKill()
        {
            var proj = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TransparentProj>(), (int)NPC.ai[2] * 2, 0f,Main.myPlayer);
            proj.Resize(120, 120);
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position - NPC.Size, NPC.width * 2, NPC.height * 2, DustID.AncientLight, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection(), 0, Color.OrangeRed);
                dust.noGravity = true;
            }
        }
    }
    //public class InsectDust_Health : ModNPC
    //{
    //    public override string Texture => "Terraria/Images/Item_0";
    //}
}
