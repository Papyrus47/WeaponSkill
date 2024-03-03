using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using WeaponSkill.Helper;
using WeaponSkill.NPCs;
using WeaponSkill.Weapons.General;

namespace WeaponSkill.Weapons.StarBreakerWeapon.FrostFist.Skills
{
    public class FrostFist_SwordSwing_Dash : FrostFist_SwordSwing
    {
        public FrostFist_SwordSwing_Dash(FrostFistProj modProjectile, Func<bool> changeCondition) : base(modProjectile, changeCondition)
        {
            frostFistLongSwordSwingHelper = new(Projectile, 20, ModContent.Request<Texture2D>(modProjectile.Texture + "_LongSword"));
            HitNPCs = new();
        }
        public FrostFistLongSwordSwingHelper frostFistLongSwordSwingHelper;
        public override SwingHelper swingHelper
        {
            get
            {
                if (Projectile.ai[0] > 2) return FrostFist.SwordSwingHelper;
                return frostFistLongSwordSwingHelper;
            }
        }

        public List<NPC> HitNPCs;
        public override void AI()
        {
            base.AI();
            switch ((int)Projectile.ai[0])
            {
                case 1:
                    {
                        if ((int)Projectile.ai[1] == (int)AtkTime / 2)
                        {
                            var proj = SpurtsProj.NewSpurtsProj(Projectile.GetSource_FromAI(), Player.Center - Player.velocity * 10, Player.velocity.SafeNormalize(default), (int)(Projectile.damage * 2.5f), Projectile.knockBack, Projectile.owner, 500, 120, TextureAssets.Frozen.Value);
                            proj.FixedPos = false;
                        }
                        else if((int)Projectile.ai[1] < (int)AtkTime / 2)
                        {
                            Player.velocity.X = Player.direction * 60;
                        }
                        else
                        {
                            Player.velocity.X *= 0.1f;
                        }
                        break;
                    }
                case 2:
                    {
                        Player.velocity.X *= 0.1f;
                        if(HitNPCs.Count > 0)
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.ai[2] = 0;
                        }
                        break;
                    }
                case 3: // 摆个pose
                    {
                        Player.SetImmuneTimeForAllTypes(5);
                        Player.immuneAlpha = 0;
                        HitNPCs.ForEach(n => n.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime = 2);
                        swingHelper.Change_Lerp((-Vector2.UnitY).RotatedBy(0.3), 0.1f, Vector2.One, 0.1f, 0,0.1f);
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, 0);
                        if (Projectile.ai[1]++ > 60) // 大于前摇时间
                        {
                            Projectile.ai[0]++;
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 2;
                            TheUtility.ResetProjHit(Projectile);
                        }
                        break;
                    }
                case 4: // 砍击
                    {
                        Projectile.ai[1]++;
                        Player.heldProj = Projectile.whoAmI;
                        HitNPCs.ForEach(n => n.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime = 2);
                        float Time = TimeChange.Invoke(Projectile.ai[1] / AtkTime);
                        if (Time > 1)
                        {
                            Projectile.ai[0]++;
                            HitNPCs.ForEach(n =>
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    FrostFist_FistBoom frostFist_FistBoom = new(10 + i, Player, Main.rand.NextVector2Unit() * 5, (int)(Projectile.damage * 1.5f));
                                    if (i == 4)
                                    {
                                        frostFist_FistBoom.ExtraAI = (NPC npc) =>
                                        {
                                            npc.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime = 120;
                                        };
                                    }
                                    WeaponSkillGlobalNPC.AddComponent(n, frostFist_FistBoom);
                                }
                            });

                        }
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * -SwingRot * SwingDirectionChange.ToDirectionInt());

                        //for (int i = 0; i < 30; i++)
                        //{
                        //    Vector2 center = Player.HandPosition.Value - Player.velocity;
                        //    Dust dust = Dust.NewDustDirect(center, 1, 1, DustID.Frost, Player.direction * 2, 0, 150, default, 1.3f);
                        //    dust.position = center;
                        //    dust.velocity *= 0;
                        //    dust.noGravity = true;
                        //    dust.fadeIn = 1;
                        //    dust.velocity = new Vector2(Projectile.velocity.Y, -Projectile.velocity.X).SafeNormalize(default).RotatedBy(i / 30f * MathHelper.Pi) * 6;
                        //    dust.velocity += Player.velocity;
                        //    dust.position = Projectile.Center + Projectile.velocity * i * 0.035f;
                        //}
                        break;
                    }
                case 5: // 摆完Pose
                    {
                        Projectile.extraUpdates = 0;
                        float Time = TimeChange.Invoke(Projectile.ai[1] / AtkTime);
                        swingHelper.SetNotSaveOldVel();
                        swingHelper.ProjFixedPlayerCenter(Player, 0, true, true);
                        swingHelper.SwingAI(FrostFist.SwordLength, Player.direction, Time * -SwingRot * SwingDirectionChange.ToDirectionInt());
                        Projectile.ai[2]++;
                        if (Projectile.ai[2] > PostAtkTime)
                        {
                            SkillTimeOut = true;
                        }
                        else if (Projectile.ai[2] > PostAtkTime / 3)
                        {
                            CanChangeToStopActionSkill = true;
                        }
                        break;
                    }
            }
        }
        public override bool SwitchCondition()
        {
            if(HitNPCs.Count > 0)
            {
                return Projectile.ai[0] > 4;
            }
            return base.SwitchCondition();
        }
        public override bool ActivationCondition()
        {
            return base.ActivationCondition() && FrostFist.frostFistItem.ChangeLevel >= 5;
        }
        public override void OnSkillActive()
        {
            base.OnSkillActive();
            FrostFist.frostFistItem.ChangeLevel -= 5;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitNPCs.Add(target);
            target.GetGlobalNPC<WeaponSkillGlobalNPC>().FrostFist_FrozenNPCTime = 1000;
        }
        public override void OnSkillDeactivate()
        {
            base.OnSkillDeactivate();
            HitNPCs.Clear();
        }
    }
}
