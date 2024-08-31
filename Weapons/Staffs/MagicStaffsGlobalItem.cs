using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.Pickaxe;
using WeaponSkill.Weapons.Staffs.Skills;

namespace WeaponSkill.Weapons.Staffs
{
    public class MagicStaffsGlobalItem : BasicWeaponItem<MagicStaffsGlobalItem>
    {
        public bool CanShootProj;
        public List<MagicStaffsSetting> magicStaffsSetting = new();
        public override void Load()
        {
            SetWeaponID(3069);
        }
        public override void SetDefaults(Item entity)
        {
            entity.noUseGraphic = true;

            switch (entity.type)
            {
                case 3069: // 火花魔杖
                    var Spurt1 = new MagicStaffsSetting_Swing()
                    {
                        Shoot = (Proj) => SpurtsProj.NewSpurtsProj(Proj.Projectile.GetSource_FromAI(), Proj.Projectile.Center, Proj.Projectile.velocity.SafeNormalize(default), Proj.Projectile.originalDamage / 2, Proj.Projectile.knockBack, Proj.Projectile.owner, Proj.Projectile.width * 3, Proj.Projectile.height * 2, TextureAssets.Mana.Value),
                        StartVel = Vector2.UnitY,
                        VelScale = new(1f,0.2f),
                        SwingTime = 20,
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
                        SwingTime = 20,
                        ChangeLerpSpeed = 0.5f,
                        SwingDirectionChange = true,
                        TimeChange = (time) => MathHelper.SmoothStep(0, 1.5f, time),
                        ChangeCondition = (player) => player.controlUseItem,
                        SwingRot = MathHelper.PiOver2,
                        OnUse = (skill) => skill.SwingHelper.SetRotVel(skill.Player.direction == 1 ? (Main.MouseWorld - skill.Player.Center).ToRotation() : (skill.Player.Center - Main.MouseWorld).ToRotation() * skill.Player.direction)
                    };
                    magicStaffsSetting.Add(Spurt1);
                    magicStaffsSetting.Add(Spurt2);
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
