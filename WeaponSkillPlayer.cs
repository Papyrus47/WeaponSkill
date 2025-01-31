using StarBreaker.Content.Appraise;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using WeaponSkill.Buffs;
using WeaponSkill.Buffs.HuntingHornBuffs;
using WeaponSkill.Command;
using WeaponSkill.Items.DualBlades;
using WeaponSkill.Weapons;
using WeaponSkill.Weapons.General;
using WeaponSkill.Weapons.GunLances;
using WeaponSkill.Weapons.Lances;
using WeaponSkill.Weapons.LongSword;
using WeaponSkill.Weapons.LongSword.Skills;
using WeaponSkill.Weapons.StarBreakerWeapon.FrostFist;
using WeaponSkill.Weapons.StarBreakerWeapon.General;

namespace WeaponSkill
{
    public class WeaponSkillPlayer : ModPlayer, IAppraiseEntity
    {
        /// <summary>
        /// 发送短信给玩家
        /// </summary>
        public bool SendText;
        public bool InBlocking;
        public bool IsBlockAttack;
        public bool Player_BowSidingStep;
        public byte BowChannelLeave;
        /// <summary>
        /// 太刀 见切
        /// </summary>
        public bool InForesightSlash;
        /// <summary>
        /// 见切成功
        /// </summary>
        public bool ForesightSlash_OnHit;
        /// <summary>
        /// 居合无敌
        /// </summary>
        public bool Naknotsu_Slash;
        /// <summary>
        /// 居合被命中
        /// </summary>
        public bool Naknotsu_Slash_OnHit;
        /// <summary>
        /// 拥有的弹药,用于切换
        /// </summary>
        public List<Item> AmmoItems;
        /// <summary>
        /// 展示远程攻击弹药切换UI
        /// </summary>
        public bool ShowTheRangeChangeUI;
        /// <summary>
        /// 展示耐力槽UI
        /// </summary>
        public bool ShowTheStamina;
        /// <summary>
        /// 非物品栏索引,一种独特的索引
        /// </summary>
        public int UseAmmoIndex;
        /// <summary>
        /// 耐力槽
        /// </summary>
        public int StatStamina;
        /// <summary>
        /// 耐力槽最大值
        /// </summary>
        public int StatStaminaMax;
        /// <summary>
        /// 根据这个确定是否回复耐力槽
        /// </summary>
        public int StatStaminaAddTime;
        /// <summary>
        /// 旧的耐力槽
        /// </summary>
        public int OldStatStamina;
        /// <summary>
        /// 太刀水月架势
        /// </summary>
        public bool SerenePose;
        /// <summary>
        /// 水月架势被命中
        /// </summary>
        public bool SerenePoseOnHit;
        /// <summary>
        /// 大锤水面击
        /// </summary>
        public bool WaterStrike;
        /// <summary>
        /// 大锤水面击成功
        /// </summary>
        /// </summary>
        public bool WaterStrike_OnHit;
        /// <summary>
        /// 神圣反击判定
        /// </summary>
        public bool HolyStrikesBack;
        /// <summary>
        /// 神圣反击被命中
        /// </summary>
        public bool HolyStrikesBack_OnHit;
        /// <summary>
        /// 玩家下落速度
        /// </summary>
        public float playerFallSpeed;

        public BasicShield HeldShield;

        public const int DashRight = 2;
        public const int DashLeft = 3;
        public int DashTimer;
        public int DashDir = -1;
        public bool PlayerOnHurt;
        #region 星击 评价系统

        public object[] UseAttack { get => useAttack; set => useAttack = value; }
        private object[] useAttack;
        public float DamageFactor = 0.1f;
        #endregion
        public override void ResetEffects()
        {
            PlayerOnHurt = false;
            ShowTheRangeChangeUI = false; 
            ShowTheStamina = false;
            AmmoItems ??= new();
            BowChannelLeave = 0;
            if (DashTimer > 0) DashTimer--;

            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[DashRight] < 15)
            {
                DashDir = DashRight;
                DashTimer = 30;
            }
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[DashLeft] < 15)
            {
                DashDir = DashLeft;
                DashTimer = 30;
            }
            else if(DashTimer <= 0)
            {
                DashDir = -1;
            }
        }
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(SendText), SendText);    
        }
        public override void LoadData(TagCompound tag)
        {
            SendText = tag.Get<bool>(nameof(SendText));
        }
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;
            if (inWater && Main.rand.NextBool(5,100))
            {
                itemDrop = ModContent.ItemType<WhetfishSabers>();
                return;
            }
        }
        public override void PostUpdateEquips()
        {
            if (Player.HeldItem.ModItem is FrostFistItem)
            {
                if (Player.statManaMax2 < 1000) Player.statManaMax2 = 1000;
                Player.statManaMax2 = (int)(Player.statManaMax2 * 1.5f);
            }
            if (playerFallSpeed > 0)
            {
                Player.maxFallSpeed = playerFallSpeed;
                playerFallSpeed = -1;
            }
            #region 特殊物品无法恢复魔力
            if (Player.HeldItem?.ModItem is SPHealMana healMana)
            {
                healMana.HealMana(Player);
            }
            #endregion
        }
        public override void PostUpdate()
        {
            if (StatStamina < OldStatStamina)
            {
                StatStaminaAddTime = 0;
            }
            OldStatStamina = StatStamina;
            if(StatStaminaAddTime < 90)
            {
                StatStaminaAddTime++;
            }
            else
            {
                StatStamina++;
                if(Player.velocity.LengthSquared() < 1f)
                {
                    StatStamina += 2;
                }
            }
            if (StatStamina > StatStaminaMax)
            {
                StatStamina = StatStaminaMax;
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            base.OnHurt(info);
            PlayerOnHurt = true;
        }
        public override void OnEnterWorld()
        {
            if(StatStaminaMax <= 0) StatStaminaMax = 600;
            _ = AppraiseSystem.Instance;
            AppraiseSystem.Instance.Load(Player, this);

            if (!SendText)
            {
                SendText = true;
                Main.NewText(TheUtility.RegisterText("Mods.WeaponSkill.SendText"));
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ShowTheRangeChangeUI && WeaponSkill.RangeChange.JustPressed)
            {
                UseAmmoIndex++;
                if (UseAmmoIndex >= AmmoItems.Count)
                    UseAmmoIndex = 0;
            }
            if (WeaponSkill.BowSlidingStep.JustPressed)
            {
                Player_BowSidingStep = true;
            }
            else
            {
                Player_BowSidingStep = false;
            }
        }
        public override void PostUpdateRunSpeeds()
        {
            if(Player.HasBuff<SelfPowerUpBuff>())
                Player.accRunSpeed *= 1.3f;
        }
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (InBlocking)
            {
                modifiers.ModifyHurtInfo += BlockingDamage;
                IsBlockAttack = true;
            }
            //modifiers.FinalDamage *= 0;
        }
        public override bool? CanMeleeAttackCollideWithNPC(Item item, Rectangle meleeAttackHitbox, NPC target)
        {
            return base.CanMeleeAttackCollideWithNPC(item, meleeAttackHitbox, target);
        }
        private void BlockingDamage(ref Player.HurtInfo info)
        {
            info.Damage -= Player.statDefense * 10;
        }
        public override void PreUpdateMovement()
        {

        }
        public override void ModifyDrawLayerOrdering(IDictionary<PlayerDrawLayer, PlayerDrawLayer.Position> positions)
        {
            //var crossbowPlayerDraw = new CrossbowPlayerDrawLayer();
            //positions.Add(crossbowPlayerDraw, crossbowPlayerDraw.GetDefaultPosition());
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            #region 玩家头上绘制斩击连击数量
            if (SlashDamage.SlashCount > 0)
            {
                Vector2 center = Player.Top + new Vector2(0, -30) - Main.screenPosition;
                string text = (Language.ActiveCulture.LegacyId == 7 ? "斩击伤害段数:" : "Slash Damage Count:") + SlashDamage.SlashCount.ToString();
                Utils.DrawBorderString(Main.spriteBatch, text, center - FontAssets.MouseText.Value.MeasureString(text) * 0.5f, Color.White * (1 - SlashDamage.SlashCountRemoveTime / 360f),1.1f);
            }
            #endregion
            switch (BowChannelLeave)
            {
                case 1:
                    {
                        r = 0.7f;
                        b = 0.4f;
                        g = 0.4f;
                        a = 0;
                        break;
                    }
                case 2:
                    {
                        r = 0.9f;
                        b = 0.6f;
                        g = 0.6f;
                        a = 0;
                        break;
                    }
                case 3:
                    {
                        r = 1;
                        b = 0.5f;
                        g = 0.5f;
                        a = 0;
                        break;
                    }
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if(HeldShield != null && HeldShield.InDef && HeldShield.GetDefSucced(npc.Hitbox))
            {
                HeldShield.DefSucceeded = true;
                HeldShield.ModifyHit(ref modifiers);
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (HeldShield != null && HeldShield.InDef && HeldShield.GetDefSucced(proj.Hitbox))
            {
                HeldShield.DefSucceeded = true;
                HeldShield.ModifyHit(ref modifiers);
            }
        }
        public override bool FreeDodge(Player.HurtInfo info)
        {
            #region 大剑防御
            if (InBlocking && info.Damage <= 1)
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Player.Center, Main.rand.NextVector2Unit(), 4, 3, 1));
                var sound = SoundID.Item106;
                sound.MaxInstances = 2;
                SoundEngine.PlaySound(sound.WithVolumeScale(5).WithPitchOffset(-0.8f), Player.position);
                SoundEngine.PlaySound(sound.WithVolumeScale(0.5f).WithPitchOffset(0.9f), Player.position);
                Player.SetImmuneTimeForAllTypes(20);
                return true;
            }
            #endregion
            #region 太刀的
            if (InForesightSlash)
            {
                ForesightSlash_OnHit = true;
                Player.SetImmuneTimeForAllTypes(180);
                for (int i = 0; i < 9; i++)
                {
                    var dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.Clentaminator_Red);
                    dust.scale = 1.5f;
                    dust.color = Color.Gold;
                    dust.fadeIn = 0.1f;
                    dust.velocity = Vector2.One.RotatedBy(i / 6f * MathHelper.TwoPi) * 3;
                    dust.noGravity = true;
                }
                return true;
            }

            if (Naknotsu_Slash)
            {
                Naknotsu_Slash_OnHit = true;
                Player.SetImmuneTimeForAllTypes(180);
                for (int i = 0; i < 9; i++)
                {
                    var dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.Clentaminator_Red);
                    dust.scale = 1.5f;
                    dust.color = Color.Gold;
                    dust.fadeIn = 0.1f;
                    dust.velocity = Vector2.One.RotatedBy(i / 6f * MathHelper.TwoPi) * 3;
                    dust.noGravity = true;
                }
                return true;
            }
            if (SerenePose)
            {
                SerenePoseOnHit = true;
                Player.SetImmuneTimeForAllTypes(180);
                return true;
            }
            #endregion
            #region 大锤水面击
            if (WaterStrike)
            {
                WaterStrike_OnHit = true;
                Player.SetImmuneTimeForAllTypes(60);
                for (int i = 0; i < 9; i++)
                {
                    var dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.Clentaminator_Red);
                    dust.scale = 1.5f;
                    dust.color = Color.Gold;
                    dust.fadeIn = 0.1f;
                    dust.velocity = Vector2.One.RotatedBy(i / 6f * MathHelper.TwoPi) * 3;
                    dust.noGravity = true;
                }
                return true;
            }
            #endregion
            #region 霜拳神圣反击
            if (HolyStrikesBack)
            {
                HolyStrikesBack_OnHit = true;
                Player.SetImmuneTimeForAllTypes(60);
                return true;
            }
            #endregion
            #region 盾系列防御
            if (HeldShield != null)
            {
                if (HeldShield.InDef)
                {
                    for (int i = 0; i < 15; i++) 
                    {
                        Dust dust = Dust.NewDustDirect(Player.Center,1,1,DustID.YellowStarDust);
                        dust.velocity = Vector2.One.RotatedBy(i / 15f * MathHelper.TwoPi);
                    }
                    SoundEngine.PlaySound(SoundID.NPCHit4 with { Pitch = -0.4f }, Player.position);
                    float prosses = info.Damage / Player.statLifeMax2;
                    if(prosses < 0.1f)
                    {
                        HeldShield.KNLevel = BasicShield.KNLevelEnum.Small;
                    }
                    else if(prosses < 0.5f)
                    {
                        HeldShield.KNLevel = BasicShield.KNLevelEnum.Medium;
                    }
                    else
                    {
                        HeldShield.KNLevel = BasicShield.KNLevelEnum.Big;
                    }
                    #region 这里是特判区
                    if(HeldShield is LancesShield lancesShield)
                    {
                        if (lancesShield.DefSucceeded_GP) // GP成功
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit4 with { Pitch = 0.4f }, Player.position);
                            for (int i = 0; i < 15; i++)
                            {
                                Dust dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.BlueMoss);
                                dust.velocity = Vector2.One.RotatedBy(i / 15f * MathHelper.TwoPi) * 1.5f;
                            }
                            Player.AddBuff(ModContent.BuffType<PowerUp>(), 600);
                        }
                    }
                    else if(HeldShield is GunLancesShield gunLancesShield)
                    {
                        if (gunLancesShield.DefSucceeded_GP) // GP成功
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit4 with { Pitch = 0.4f }, Player.position);
                            for (int i = 0; i < 15; i++)
                            {
                                Dust dust = Dust.NewDustDirect(Player.Center, 1, 1, DustID.BlueMoss);
                                dust.velocity = Vector2.One.RotatedBy(i / 15f * MathHelper.TwoPi) * 1.5f;
                            }
                            Player.AddBuff(ModContent.BuffType<PowerUp>(), 600);
                        }
                    }
                    #endregion
                    if (info.Damage <= Player.statLifeMax2 * 0.01f)
                    {
                        return false;
                    }
                }
                HeldShield = null;
            }
            #endregion
            return base.FreeDodge(info);
        }
        #region 命中东西的判定
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            AddAttack(item);
            AppraiseSystem.Instance.OnHit(this, target, hit.Damage);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.ModProjectile is IBasicSkillProj skill)
            {
                AddAttack(skill.CurrentSkill);
            }
            if (proj.owner >= 0 && proj.owner != 255 && proj.friendly)
            {
                AppraiseSystem.Instance.OnHit(this, target, hit.Damage);
            }
        }
        //public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        //{
        //    AddAttack(item);
        //    AppraiseSystem.Instance.OnHit(this, target, modifiers.GetDamage(Player.GetWeaponDamage(item),false));
        //}
        //public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        //{
        //    if (proj.ModProjectile is not SkillProj)
        //    {
        //        AddAttack(proj.Name);
        //    }
        //    if (proj.owner >= 0 && proj.owner != 255 && proj.friendly)
        //    {
        //        AppraiseSystem.Instance.OnHit(this, target, damage);
        //    }
        //}
        #endregion
        #region 被命中判定

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            AppraiseSystem.Instance.OnHurt(this, npc, hurtInfo.Damage);
        }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            AppraiseSystem.Instance.OnHurt(this, proj, hurtInfo.Damage);
        }
        //public override void OnHitByNPC(NPC npc, int damage, bool crit)
        //{
        //    AppraiseSystem.Instance.OnHurt(this, npc, damage);
        //}
        //public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        //{
        //    AppraiseSystem.Instance.OnHurt(this, proj, damage);
        //}
        #endregion
        public void AddAttack(object attackID)
        {
            useAttack ??= new object[10];
            //DamageFactor = 0.1f;
            if (attackID != useAttack[0])
            {
                for (int i = useAttack.Length - 1; i > 0; i--)
                {
                    useAttack[i] = useAttack[i - 1];
                }
                useAttack[0] = attackID; // 0存着现在命中的技能
            }
        }
        public float OnHit(Entity target, int damage)
        {
            useAttack ??= new object[10];
            for (int i = 1;i < 4;i++)
            {
                if (useAttack[0] == useAttack[i])
                {
                    return 0;
                }
            }
            return damage * DamageFactor;
        }
        public float OnHurt(Entity target, int damage)
        {
            return damage;
        }

        public void Draw(float progress, AppraiseID id) // 评价的绘制
        {
            string drawFont = TheUtility.GetAppraiseDrawFont(id);
            Color color = TheUtility.GetAppraiseDrawColor(id);
            Rectangle drawRect = TheUtility.GetApprasieDrawRect(id);
            if (drawFont != null)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone,
                    null);

                Vector2 position = new(100, (Main.screenHeight / 2) - 180);
                //position = Player.Top - Main.screenPosition - new Vector2(0,50);
                Vector2 origin = drawRect.Size() * 0.5f;

                spriteBatch.Draw(ModAsset.AppraiseTex.Value, position, drawRect, color,
                    0f, origin, 1.4f, SpriteEffects.None, 0f);
                int height = drawRect.Height;
                drawRect.Height = (int)(drawRect.Height * progress);
                //drawRect.Y = height - drawRect.Height;
                spriteBatch.Draw(ModAsset.AppraiseTex.Value, position, drawRect, Color.Black * 0.8f,
                    0f, origin, 1.4f, SpriteEffects.None, 0f);

                spriteBatch.End();
            }
        }
    }
}
