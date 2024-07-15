using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.NPCs.TestNPC
{
    public class TestNPC :ModNPC
    {
        public Player player;
        public override string Texture => base.Texture;
        public override void SetDefaults()
        {
            NPC.lifeMax = 203;
            NPC.defense = 0;
            NPC.aiStyle = -1;
            NPC.Size = new(150, 296);
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0.2f;
            NPC.noGravity = false;
            NPC.scale = 0.5f;
        }
        public override void AI()
        {
            if(player == null)
            {
                NPC.TargetClosest();
                if (NPC.target != -1)
                {
                    player = Main.player[NPC.target];
                    if(player.Distance(NPC.position) > 600)
                    {
                        NPC.life = 0;
                        return;
                    }
                }
                else
                {
                    NPC.life = 0;
                    return;
                }
            }
            if (NPC.ai[1]++ > 600)
            {
                NPC.ai[1] = 0;
                CombatText.NewText(NPC.Hitbox, Color.White, "10s DPS:" + ((int)NPC.ai[0] / 10).ToString(),true);
                NPC.ai[0] = 0;
            }

            if (NPC.collideY && NPC.velocity.Y >= 0)
                NPC.velocity.X = (NPC.position - player.position).X * -0.08f;
            if (NPC.velocity.Y < -4)
                NPC.velocity.Y *= 0.9f;
            if (player.Distance(NPC.position) > 900)
                player = null;
        }
        public override bool CheckDead()
        {
            NPC.life = NPC.lifeMax;
            NPC.active = true;
            return false;
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByItem(player, item, hit, damageDone);
            NPC.ai[0] += damageDone;
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitByProjectile(projectile, hit, damageDone);
            NPC.ai[0] += damageDone;
        }
    }
}
