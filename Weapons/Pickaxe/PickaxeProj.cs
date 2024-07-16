using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Helper;
using WeaponSkill.Weapons.Bows;
using WeaponSkill.Weapons.Hammer;

namespace WeaponSkill.Weapons.Pickaxe
{
    public class PickaxeProj : ModProjectile
    {
        public Item SpawnItem;
        public Player Player;
        /// <summary>
        /// 物理绳子
        /// </summary>
        public PhysicalRope physicalRope;
        public override string Texture => "Terraria/Images/Item_0";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000000;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_ItemUse itemUse && itemUse.Item != null)
            {
                SpawnItem = itemUse.Item;
                Player = itemUse.Player;
                Projectile.Name = SpawnItem.Name;
                Projectile.scale = Player.GetAdjustedItemScale(SpawnItem) + 0.5f;
                Projectile.Size = SpawnItem.Size * Projectile.scale;
                Main.projFrames[Type] = TheUtility.GetItemFrameCount(SpawnItem);
                RopeInit();
            }
        }
        public override void AI()
        {
            if (Player.HeldItem != SpawnItem || Player.dead || !Player.HeldItem.GetGlobalItem<PickaxeGlobalItem>().InAttackMode || (SpawnItem.TryGetGlobalItem<PickaxeGlobalItem>(out var pickaxeGlobalItem) && pickaxeGlobalItem.SP_PickaxeMode == 1)) // 玩家手上物品不是生成物品,则清除
            {
                Projectile.Kill();
                return;
            }

            #region 绳子头尾绑定
            physicalRope.rope_Points[0].pos = Player.RotatedRelativePoint(Player.MountedCenter);
            physicalRope.rope_Points[0].locked = true; // 上锁
            #endregion
            switch ((int)Projectile.ai[0])
            {
                case 0: // 常规情况小,拖拽
                    {
                        Projectile.ai[2] = -1;
                        Projectile.tileCollide = true;
                        Projectile.numHits = 0;
                        physicalRope.rope_Lines[^1].endPoint.locked = false;
                        Projectile.velocity.X *= 0.5f;
                        physicalRope.gravity = Vector2.Zero;
                        Projectile.rotation = (physicalRope.rope_Points[^1].pos - physicalRope.rope_Points[^7].pos).ToRotation() + MathHelper.PiOver4;
                        if (Projectile.velocity.Y < 12) Projectile.velocity.Y += 0.6f;
                        Tile tile = Main.tile[(Projectile.Center / 16).ToPoint()];
                        if (tile.HasTile && tile.HasUnactuatedTile)
                        {
                            Projectile.position.Y -= 6f;
                        }
                        if (Projectile.ai[1] <= 0)
                        {
                            Projectile.ai[1]++;
                            SetRopeLength(3.6f);
                        }
                        else
                        {
                            #region 左键的情况,旋转,丢出
                            if (Player.controlUseItem)
                            {
                                Projectile.ai[0] = 1;
                                Projectile.ai[1] = 0;
                                TheUtility.ResetProjHit(Projectile);
                            }
                            #endregion
                            #region 右键的情况,拉扯
                            if (Player.controlUseTile)
                            {
                                Projectile.ai[1] = 0;
                                Projectile.ai[0] = 3;
                            }
                            #endregion
                        }

                        #region 绳子更新
                        physicalRope.rope_Lines[^1].endPoint.pos += (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        physicalRope.Update();
                        Projectile.Center -= (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        #endregion
                        break;
                    }
                case 1: // 旋转
                    {
                        Projectile.numHits = 0;
                        Projectile.tileCollide = false;
                        Projectile.rotation = (Projectile.Center - Player.Center).ToRotation() + MathHelper.PiOver4;
                        if (!Player.controlUseItem)
                        {
                            Projectile.velocity = (Main.MouseWorld - Player.Center).SafeNormalize(default) * 70;
                            SetRopeLength(65f);
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 0;
                            Projectile.ai[0]++;
                            Projectile.tileCollide = true;
                            TheUtility.ResetProjHit(Projectile);
                            break;
                        }
                        Player.itemAnimation = Player.itemTime = 2;
                        if (Projectile.ai[1] == 0)
                        {
                            Projectile.ai[1]++;
                            SetRopeLength(0.6f);
                        }
                        Projectile.extraUpdates = 3;
                        Projectile.velocity = -(Projectile.Center - (Player.Center + ((Projectile.Center - Player.Center).SafeNormalize(default).RotatedBy(1f * Player.direction) * 20))) * 0.3f;

                        #region 绳子更新
                        physicalRope.rope_Lines[^1].endPoint.pos += (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        physicalRope.Update();
                        Projectile.Center -= (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        #endregion
                        break;
                    }
                case 2: // 飞出
                    {
                        Projectile.numHits = 0;
                        Projectile.velocity.X *= 0.99f - (float)((Player.position - Projectile.position).LengthSquared() * 0.00000000001);
                        physicalRope.gravity = Vector2.UnitY * 0.2f;
                        if (Projectile.velocity.Y < 12) Projectile.velocity.Y += 0.6f;
                        Tile tile = Main.tile[(Projectile.Center / 16).ToPoint()];
                        if (tile.HasTile && tile.HasUnactuatedTile)
                        {
                            Projectile.position.Y -= 6f;
                        }


                        #region 绳子更新
                        physicalRope.rope_Lines[^1].endPoint.pos += (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        if ((Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos).LengthSquared() > 500 && (Projectile.Center - Player.Center).Length() > 500)
                        {
                            Projectile.velocity *= 0.8f;
                        }
                        physicalRope.Update();
                        if (!physicalRope.rope_Lines[^1].endPoint.locked)
                        {
                            Projectile.rotation = (physicalRope.rope_Points[^1].pos - physicalRope.rope_Points[^7].pos).ToRotation() + MathHelper.PiOver4;
                            Projectile.Center -= (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        }
                        else
                        {
                            if (Player.controlUseItem)
                            {
                                for (int i = -1; i <= 1; i++)
                                {
                                    for (int j = -1; j <= 1; j++)
                                        Player.PickTile((int)Projectile.Bottom.X / 16 + i, (int)Projectile.Bottom.Y / 16 + j, Player.HeldItem.pick);
                                }
                                Projectile.ai[0] = 0;
                                Projectile.ai[1] = -10;
                            }
                            else if (Player.controlUseTile)
                            {
                                SetRopeLength(physicalRope.rope_Lines[0].Lenght - 1f);
                                if (Player.Distance(Projectile.position) < 100)
                                {
                                    Projectile.ai[0] = 0;
                                    Projectile.ai[1] = -10;
                                }
                            }
                            physicalRope.Update();
                            Projectile.position -= Projectile.velocity;
                            if (Player.Distance(Projectile.position) > physicalRope.rope_Lines[0].Lenght * 25)
                            {
                                Player.velocity -= (Player.Center - physicalRope.rope_Lines[3].endPoint.pos) * 0.1f;
                            }
                        }
                        #endregion
                        break;
                    }
                case 3: // 挥舞
                    {
                        Player.itemAnimation = Player.itemTime = 2;
                        physicalRope.gravity = (Projectile.Center - Player.Center).SafeNormalize(default);
                        Projectile.tileCollide = false;
                        Projectile.rotation = (Projectile.Center - Player.Center).ToRotation() + MathHelper.PiOver4;
                        Player.itemAnimation = Player.itemTime = 2;
                        if (Projectile.ai[1] == 0)
                        {
                            Projectile.ai[1]++;
                            SetRopeLength(0.3f);
                        }
                        Projectile.ai[1]++;
                        Projectile.extraUpdates = 3;
                        Projectile.velocity = (Player.Center + (-Vector2.UnitY * 100).RotatedBy((Projectile.ai[1] / 25f) * MathHelper.PiOver2 * Player.direction) - Projectile.Center) * 0.5f;

                        #region 绳子更新
                        physicalRope.rope_Lines[^1].endPoint.pos += (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        physicalRope.Update();
                        Projectile.Center -= (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        #endregion

                        if (Projectile.ai[1] > 51)
                        {
                            TheUtility.ResetProjHit(Projectile);
                            Projectile.ai[1] = 0;
                            Projectile.extraUpdates = 0;
                            Projectile.ai[0] = 0;
                            if(Projectile.numHits > 0)
                            {
                                Projectile.velocity = (Main.MouseWorld - Player.Center).SafeNormalize(default) * 50;
                                SetRopeLength(50f);
                                Projectile.ai[0] = 4;
                                Projectile.ai[2] = -1;
                                TheUtility.ResetProjHit(Projectile);
                            }
                        }
                        break;
                    }
                case 4: // 钩出吸附
                    {
                        Player.itemAnimation = Player.itemTime = 2;
                        Projectile.tileCollide = false;
                        Projectile.velocity.X *= 0.99f - (float)((Player.position - Projectile.position).LengthSquared() * 0.00000000001);
                        physicalRope.gravity = Vector2.UnitY * 0.2f;
                        if (Projectile.velocity.Y < 12) Projectile.velocity.Y += 0.6f;
                        if (Projectile.ai[2] != -1) // 钩住NPC
                        {
                            NPC npc = Main.npc[(int)Projectile.ai[2]];
                            if (npc.active && npc.CanBeChasedBy())
                            {
                                Projectile.velocity = npc.velocity;
                                physicalRope.rope_Lines[^1].endPoint.locked = true;
                            }
                            else
                            {
                                Projectile.ai[2] = -1;
                                physicalRope.rope_Lines[^1].endPoint.locked = false;
                                Projectile.ai[0] = 0;
                                Projectile.ai[1] = 0;
                                break;
                            }
                        }
                        #region 绳子更新
                        physicalRope.rope_Lines[^1].endPoint.pos += (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        if ((Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos).LengthSquared() > 500 && (Projectile.Center - Player.Center).Length() > 500)
                        {
                            Projectile.velocity *= 0.8f;
                        }
                        physicalRope.Update();
                        if (!physicalRope.rope_Lines[^1].endPoint.locked)
                        {
                            Projectile.ai[1]++;
                            if (Projectile.ai[1] > 30)
                            {
                                Projectile.ai[1] = 0;
                                Projectile.ai[0] = 0;
                            }
                            Projectile.rotation = (physicalRope.rope_Points[^1].pos - physicalRope.rope_Points[^7].pos).ToRotation() + MathHelper.PiOver4;
                            Projectile.Center -= (Projectile.Center - physicalRope.rope_Lines[^1].endPoint.pos) * 0.4f;
                        }
                        else
                        {
                            if (Player.controlUseItem)
                            {
                                if (Projectile.ai[1]++ > 3)
                                {
                                    Projectile.ai[1] = 0;
                                    Player.ApplyDamageToNPC(Main.npc[(int)Projectile.ai[2]], Projectile.damage / 3, Projectile.knockBack, Player.direction, Main.rand.Next(100) < Player.GetWeaponCrit(Player.HeldItem), Player.HeldItem.DamageType);
                                }
                            }
                            if (Player.controlUseTile || (!Main.npc[(int)Projectile.ai[2]].active || !Main.npc[(int)Projectile.ai[2]].CanBeChasedBy()))
                            {
                                SetRopeLength(physicalRope.rope_Lines[0].Lenght - 1f);
                                if (Player.Distance(Projectile.position) < 100 || physicalRope.rope_Lines[0].Lenght <= 5f)
                                {
                                    Projectile.ai[0] = 0;
                                    Projectile.ai[1] = -10;
                                }
                            }
                            physicalRope.Update();
                            if (Player.Distance(Projectile.position) > physicalRope.rope_Lines[0].Lenght * 25)
                            {
                                Player.velocity -= (Player.Center - physicalRope.rope_Lines[3].endPoint.pos) * 0.1f;
                            }
                        }
                        #endregion
                        break;
                    }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[2] = target.whoAmI;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.X *= 0.9f;
            Projectile.velocity.Y = -oldVelocity.Y * 0.1f;
            switch ((int)Projectile.ai[0])
            {
                //case 1: // 破坏方块
                //    {
                //        for (int i = -1; i <= 1; i++)
                //        {
                //            for (int j = -1; j <= 1; j++)
                //                Player.PickTile((int)Projectile.Bottom.X / 16 + i, (int)Projectile.Bottom.Y / 16 + j, Player.HeldItem.pick);
                //        }
                //        break;
                //    }
                case 2:
                    {
                        if (Projectile.velocity.Y >= -0.4f)
                        {
                            physicalRope.rope_Lines[^1].endPoint.locked = true;
                        }
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                                Player.PickTile((int)Projectile.Center.X / 16 + i, (int)Projectile.Bottom.Y / 16 + j, Player.HeldItem.pick);
                        }
                        break;
                    }
            }
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            switch ((int)Projectile.ai[0])
            {
                case 1: // 破坏方块
                    {
                        fallThrough = true;
                        break;
                    }
                case 2: // 减小物块碰撞
                    {
                        width /= 25;
                        height /= 25;
                        fallThrough = true;
                        break;
                    }
            }
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            physicalRope.Draw();
            Main.GetItemDrawFrame(SpawnItem.type, out var tex, out var rect);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rect, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            switch ((int)Projectile.ai[0])
            {
                case 3:
                    for(int i = 0; i < Projectile.oldPos.Length; i++)
                    {
                        Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + (Projectile.Size * 0.5f) - Main.screenPosition, rect, lightColor * (1f - (float)i / Projectile.oldPos.Length), Projectile.oldRot[i],tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                    }
                    break;
            }
            return false;
        }
        public void RopeInit()
        {
            physicalRope ??= new();
            physicalRope.rope_Lines.Clear();
            physicalRope.rope_Points.Clear();
            physicalRope.RopeRigidity = 4;
            physicalRope.width = 3;
            int length = 20;
            for (int i = 0; i < length; i++)
            {
                physicalRope.rope_Points.Add(new() { pos = Projectile.Center + (Player.position - Projectile.position) * 0.5f, oldPos = Projectile.Center });
            }
            physicalRope.rope_Points[0].locked = true; // 上锁
            //physicalRope.rope_Points[^1].locked = true; // 上锁
            for (int i = 1; i < physicalRope.rope_Points.Count; i++)
            {
                physicalRope.rope_Lines.Add(new(physicalRope.rope_Points[i - 1], physicalRope.rope_Points[i], 10));
            }
        }
        public void SetRopeLength(float lenght)
        {
            physicalRope.rope_Lines.Clear();
            for (int i = 1; i < physicalRope.rope_Points.Count; i++)
            {
                physicalRope.rope_Lines.Add(new(physicalRope.rope_Points[i - 1], physicalRope.rope_Points[i], lenght));
            }
        }
    }
}
