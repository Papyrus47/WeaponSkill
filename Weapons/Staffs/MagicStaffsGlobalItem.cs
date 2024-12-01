using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.Pickaxe;
using WeaponSkill.Weapons.Staffs.Skills;
using Terraria.ID;

namespace WeaponSkill.Weapons.Staffs
{
    public class MagicStaffsGlobalItem : BasicWeaponItem<MagicStaffsGlobalItem>, IVanillaWeapon
    {
        public bool CanShootProj;
        public List<MagicStaffsSetting> magicStaffsSetting = new();
        public override void Load()
        {
            SetWeaponID(3069,5147,4062,739,740,741,742,743,744,3377,64,112,3787, 3209,1264, 726,3051, 2750,1308, 683, 3852,2188,788, 1445,1801, 1931,1296,1446);
        }
        public override void SetDefaults(Item entity)
        {
            entity.noUseGraphic = true;
            entity.useStyle = ItemUseStyleID.Rapier;
            #region 通用区
            var Spurt1 = new MagicStaffsSetting_Swing()
            {
                Shoot = (Proj) => SpurtsProj.NewSpurtsProj(Proj.Projectile.GetSource_FromAI(), Proj.Projectile.Center, Proj.Projectile.velocity.SafeNormalize(default), Proj.Projectile.originalDamage / 2, Proj.Projectile.knockBack, Proj.Projectile.owner, Proj.Projectile.width * 3, Proj.Projectile.height * 2, TextureAssets.Mana.Value),
                StartVel = Vector2.UnitY,
                VelScale = new(1f, 0.2f),
                SwingTime = entity.useTime,
                ChangeLerpSpeed = 0.5f,
                SwingDirectionChange = false,
                TimeChange = (time) => MathHelper.SmoothStep(0, 1.5f, time),
                ChangeCondition = (player) => player.controlUseItem,
                SwingRot = MathHelper.PiOver2,
                OnUse = (skill) => skill.SwingHelper.SetRotVel(skill.Player.direction == 1 ? (Main.MouseWorld - skill.Player.Center).ToRotation() : (skill.Player.Center - Main.MouseWorld).ToRotation() * skill.Player.direction)
            };
            var Spurt2 = new MagicStaffsSetting_Swing()
            {
                Shoot = (Proj) => SpurtsProj.NewSpurtsProj(Proj.Projectile.GetSource_FromAI(), Proj.Projectile.Center, Proj.Projectile.velocity.SafeNormalize(default), Proj.Projectile.originalDamage / 2, Proj.Projectile.knockBack, Proj.Projectile.owner, Proj.Projectile.width * 3, Proj.Projectile.height * 2, TextureAssets.Mana.Value),
                StartVel = -Vector2.UnitY,
                VelScale = new(1f, 0.2f),
                SwingTime = entity.useTime,
                ChangeLerpSpeed = 0.5f,
                SwingDirectionChange = true,
                TimeChange = (time) => MathHelper.SmoothStep(0, 1.5f, time),
                ChangeCondition = (player) => player.controlUseItem,
                SwingRot = MathHelper.PiOver2,
                OnUse = (skill) => skill.SwingHelper.SetRotVel(skill.Player.direction == 1 ? (Main.MouseWorld - skill.Player.Center).ToRotation() : (skill.Player.Center - Main.MouseWorld).ToRotation() * skill.Player.direction)
            };
            var Swing1 = new MagicStaffsSetting_Swing()
            {
                StartVel = Vector2.UnitY,
                VelScale = new(1f, 0.6f),
                VisualRotation = 0.4f,
                SwingTime = entity.useTime,
                ChangeLerpSpeed = 0.5f,
                SwingDirectionChange = false,
                TimeChange = (time) => MathHelper.SmoothStep(0, 1.5f, time),
                ChangeCondition = (player) => player.controlUseItem,
                SwingRot = MathHelper.PiOver2,
                OnUse = (skill) => skill.SwingHelper.SetRotVel(skill.Player.direction == 1 ? (Main.MouseWorld - skill.Player.Center).ToRotation() : (skill.Player.Center - Main.MouseWorld).ToRotation() * skill.Player.direction)
            };
            var Swing2 = new MagicStaffsSetting_Swing()
            {
                StartVel = -Vector2.UnitY,
                VelScale = new(1.3f, 0.6f),
                VisualRotation = 0.4f,
                SwingTime = entity.useTime,
                ChangeLerpSpeed = 0.5f,
                SwingDirectionChange = true,
                TimeChange = (time) => MathHelper.SmoothStep(0, 1.5f, time),
                ChangeCondition = (player) => player.controlUseItem,
                SwingRot = MathHelper.PiOver2,
                OnUse = (skill) => skill.SwingHelper.SetRotVel(skill.Player.direction == 1 ? (Main.MouseWorld - skill.Player.Center).ToRotation() : (skill.Player.Center - Main.MouseWorld).ToRotation() * skill.Player.direction)
            };
            var Swing3 = new MagicStaffsSetting_Swing()
            {
                StartVel = -Vector2.UnitY,
                VelScale = new(1f, 0.9f),
                VisualRotation = 0.1f,
                SwingTime = entity.useTime,
                ChangeLerpSpeed = 0.5f,
                SwingDirectionChange = true,
                TimeChange = (time) => MathHelper.SmoothStep(0, 1.5f, time),
                ChangeCondition = (player) => player.controlUseItem,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                OnUse = (skill) => skill.SwingHelper.SetRotVel(skill.Player.direction == 1 ? (Main.MouseWorld - skill.Player.Center).ToRotation() : (skill.Player.Center - Main.MouseWorld).ToRotation() * skill.Player.direction)
            };
            var Swing4 = new MagicStaffsSetting_Swing()
            {
                StartVel = Vector2.UnitY,
                VelScale = new(1.1f, 1f),
                VisualRotation = 0.1f,
                SwingTime = entity.useTime,
                ChangeLerpSpeed = 0.5f,
                SwingDirectionChange = false,
                TimeChange = (time) => MathHelper.SmoothStep(0, 1.5f, time),
                ChangeCondition = (player) => player.controlUseItem,
                SwingRot = MathHelper.Pi + MathHelper.PiOver4,
                OnUse = (skill) => skill.SwingHelper.SetRotVel(skill.Player.direction == 1 ? (Main.MouseWorld - skill.Player.Center).ToRotation() : (skill.Player.Center - Main.MouseWorld).ToRotation() * skill.Player.direction)
            };
            #endregion

            switch (entity.type)
            {
                case 1446: // 幽灵法杖 加特林
                    var Swing_SpeedAtk = new MagicStaffsSetting_Swing()
                    {
                        StartVel = -Vector2.UnitY,
                        VelScale = new(1f, 1f),
                        SwingTime = 30,
                        ChangeLerpSpeed = 0.5f,
                        SwingDirectionChange = true,
                        TimeChange = (time) => MathHelper.SmoothStep(0, 2f, time),
                        ChangeCondition = (player) => player.controlUseItem,
                        SwingRot = MathHelper.TwoPi + MathHelper.PiOver2,
                        OnUse = (skill) =>
                        {
                            Projectile proj = skill.Projectile;
                            proj.extraUpdates = 3;
                            proj.ai[1]++;
                            if ((int)proj.ai[1] % 10 == 0)
                            {
                                entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = true;
                                if (skill.Player.CheckMana(skill.Player.HeldItem, -1, true))
                                    TheUtility.Player_ItemCheck_Shoot(skill.Player, skill.Player.HeldItem, proj.damage);
                                entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = false;
                            }
                        }
                    };
                    for (int i = 0; i < 35; i++)
                    {
                        magicStaffsSetting.Add(Swing_SpeedAtk);
                    }
                    break;
                case 3069: // 火花魔杖
                case 5147: // 结霜魔杖
                case 4062: // 霹雳法杖
                case 64: // 魔刺
                case 3209: // 水晶蛇
                case 3051: // 魔晶碎块
                case 683: // 邪恶三叉戟
                case 788: // 爆裂藤曼
                case 1445: // 烈火叉
                    magicStaffsSetting.Add(Spurt1);
                    magicStaffsSetting.Add(Spurt2);
                    #region 特判区
                    if (entity.type == ItemID.ThunderStaff)
                    {
                        var Swing_ThunderStaff = new MagicStaffsSetting_Swing()
                        {
                            StartVel = -Vector2.UnitY,
                            VelScale = new(1f, 1f),
                            SwingTime = 60,
                            ChangeLerpSpeed = 0.5f,
                            SwingDirectionChange = true,
                            TimeChange = (time) => MathHelper.SmoothStep(0, 2f, time),
                            ChangeCondition = (player) => player.controlUseItem,
                            SwingRot = MathHelper.TwoPi + MathHelper.PiOver2,
                            OnUse = (skill) =>
                            {
                                Projectile proj = skill.Projectile;
                                proj.ai[1]++;
                                if ((int)proj.ai[1] % 10 == 0)
                                {
                                    entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = true;
                                    if (skill.Player.CheckMana(skill.Player.HeldItem, -1, true))
                                        TheUtility.Player_ItemCheck_Shoot(skill.Player, skill.Player.HeldItem, proj.damage);
                                    entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = false;
                                }
                            }
                        };
                        magicStaffsSetting.Add(Swing_ThunderStaff);
                        magicStaffsSetting.Add(Swing_ThunderStaff);
                        magicStaffsSetting.Add(Swing_ThunderStaff);
                        magicStaffsSetting.Add(Spurt1);
                    }
                    else if (entity.type == ItemID.CrystalSerpent || entity.type == ItemID.InfernoFork)
                    {
                        for(int i = 0; i < 3; i++)
                        {
                            magicStaffsSetting.Add(Spurt1);
                            magicStaffsSetting.Add(Spurt2);
                        }
                    }
                    #endregion
                    break;
                case >= 739 and <= 744: // 水晶法杖们
                case 3377: // 琥珀法杖
                case 726: // 寒霜法杖
                case 2750: // 流星法杖
                case 1308: // 剧毒法杖
                case 3852: // 智慧巨著
                case 2188: // 毒液法杖
                case 1801: // 蝙蝠法杖
                case 1296: // 大地法杖
                    magicStaffsSetting.Add(Swing1);
                    magicStaffsSetting.Add(Swing2);
                    magicStaffsSetting.Add(Spurt1);
                    magicStaffsSetting.Add(Swing2);
                    magicStaffsSetting.Add(Spurt1);
                    magicStaffsSetting.Add(Spurt2);
                    magicStaffsSetting.Add(Swing1);
                    #region 特判区
                    if (entity.type == ItemID.BookStaff)
                    {
                        var Swing_BookStaff = new MagicStaffsSetting_Swing()
                        {
                            WillAddOther = true,
                            StartVel = -Vector2.UnitY,
                            VelScale = new(1f, 1f),
                            SwingTime = 60,
                            ChangeLerpSpeed = 0.5f,
                            SwingDirectionChange = true,
                            TimeChange = (time) => MathHelper.SmoothStep(0, 2f, time),
                            ChangeCondition = (player) => player.controlUseTile,
                            SwingRot = MathHelper.Pi,
                        };
                    }
                    #endregion
                    break;
                case 112: // 火之花
                case 1264: // 寒霜之花
                    for (int i = 0; i < 5; i++)
                    {
                        magicStaffsSetting.Add(Swing3);
                        magicStaffsSetting.Add(Swing4);
                    }
                    break;
                case 3787: // 裂天剑
                    entity.scale = 3;
                    Swing1.SwingTime *= 5;
                    Swing2.SwingTime *= 5;

                    var SkyFracture_Swing1 = Swing1.DeepClone();
                    SkyFracture_Swing1.StartVel = Vector2.UnitY.RotatedBy(0.6);
                    SkyFracture_Swing1.SwingRot = MathHelper.Pi + 0.6f;
                    SkyFracture_Swing1.OnUse = (skill) =>
                    {
                        Projectile proj = skill.Projectile;
                        proj.ai[1]++;
                        if ((int)proj.ai[1] % 4 == 0)
                        {
                            entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = true;
                            if (skill.Player.CheckMana(skill.Player.HeldItem, -1, true))
                                TheUtility.Player_ItemCheck_Shoot(skill.Player, skill.Player.HeldItem, proj.damage);
                            entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = false;
                        }
                    };

                    var SkyFracture_Swing2 = Swing2.DeepClone();
                    SkyFracture_Swing2.StartVel = (-Vector2.UnitY).RotatedBy(-0.6);
                    SkyFracture_Swing2.SwingRot = MathHelper.Pi + 0.6f;
                    SkyFracture_Swing2.OnUse = (skill) =>
                    {
                        Projectile proj = skill.Projectile;
                        proj.ai[1]++;
                        if ((int)proj.ai[1] % 4 == 0)
                        {
                            entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = true;
                            if (skill.Player.CheckMana(skill.Player.HeldItem, -1, true))
                                TheUtility.Player_ItemCheck_Shoot(skill.Player, skill.Player.HeldItem, proj.damage);
                            entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = false;
                        }
                    };

                    Spurt1.VelScale = new Vector2(1.5f, 0.01f);

                    magicStaffsSetting.Add(Swing1);
                    magicStaffsSetting.Add(Swing2);
                    magicStaffsSetting.Add(SkyFracture_Swing1);
                    magicStaffsSetting.Add(SkyFracture_Swing2);
                    magicStaffsSetting.Add(Spurt1);
                    break;
                case 1931: // 寒霜法杖
                    var Swing_BlizzardStaff = new MagicStaffsSetting_Swing()
                    {
                        StartVel = -Vector2.UnitY,
                        VelScale = new(1f, 1f),
                        SwingTime = entity.useTime * 5,
                        ChangeLerpSpeed = 0.5f,
                        SwingDirectionChange = true,
                        TimeChange = (time) => MathHelper.SmoothStep(0, 2f, time),
                        ChangeCondition = (player) => player.controlUseItem,
                        SwingRot = MathHelper.TwoPi + MathHelper.PiOver2,
                        OnUse = (skill) =>
                        {
                            Projectile proj = skill.Projectile;
                            proj.ai[1]++;
                            if ((int)proj.ai[1] % 5 == 0)
                            {
                                entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = true;
                                if (skill.Player.CheckMana(skill.Player.HeldItem, -1, true))
                                    TheUtility.Player_ItemCheck_Shoot(skill.Player, skill.Player.HeldItem, proj.damage);
                                entity.GetGlobalItem<MagicStaffsGlobalItem>().CanShootProj = false;
                            }
                        }
                    };
                    for(int i = 0; i < 5; i++)
                    {
                        magicStaffsSetting.Add(Swing_BlizzardStaff);
                        magicStaffsSetting.Add(Spurt1);
                        magicStaffsSetting.Add(Swing_BlizzardStaff);
                        magicStaffsSetting.Add(Spurt2);
                    }
                    break;
            }
        }
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MagicStaffsProj>()] <= 0)
            {
                int proj = Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, ModContent.ProjectileType<MagicStaffsProj>(), player.GetWeaponDamage(item), player.GetWeaponKnockback(item), player.whoAmI);
                Main.projectile[proj].originalDamage = Main.projectile[proj].damage;

            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!CanShootProj)
                return false;
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }
}
