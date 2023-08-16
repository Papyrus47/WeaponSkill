using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeaponSkill.Weapons.Crossbow;

namespace WeaponSkill.WeaponSkillPlayerDrawLayers
{
    public class CrossbowPlayerDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HeldItem.TryGetGlobalItem<CrossbowGlobalItem>(out _) && base.GetDefaultVisibility(drawInfo) && drawInfo.drawPlayer.ItemTimeIsZero;
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            DrawData drawData = new DrawData();
            Player player = drawInfo.drawPlayer;
            int type = player.HeldItem.type;
            Texture2D tex = TextureAssets.Item[type].Value;
            drawData.texture = tex;
            drawData.color = Lighting.GetColor((player.Center / 16).ToPoint());
            drawData.rotation = MathHelper.PiOver2;
            drawData.scale = new Vector2(1f);
            drawData.position = player.Center - Main.screenPosition + new Vector2(player.direction * -player.width, -tex.Width * 0.1f);
            drawData.sourceRect = ((Main.itemAnimations[type] == null) ? tex.Frame() : Main.itemAnimations[type].GetFrame(tex));
            drawData.origin = drawData.sourceRect.Value.Size() * 0.5f;
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}
