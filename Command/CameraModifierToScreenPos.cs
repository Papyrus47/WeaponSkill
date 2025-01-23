using Terraria.Graphics.CameraModifiers;

namespace WeaponSkill.Command
{
    internal class CameraModifierToScreenPos : ICameraModifier
    {
        private Vector2 _screenPos;
        public int TimeLeft;
        /// <summary>
        /// 用于控制会不会重复
        /// <para>当移动相机的东西拥有同一个字符串的时候,移除前者,保留传入者</para>
        /// </summary>
        public string UniqueIdentity { get; private set; }
        /// <summary>
        /// 为true时,把这个东西从控制屏幕位置的数组移除
        /// </summary>
        public bool Finished { get; private set; }

        public void Update(ref CameraInfo cameraPosition)
        {
            cameraPosition.CameraPosition = _screenPos;
            TimeLeft--;
            if (TimeLeft < 0)
            {
                Finished = true;
            }
        }
        public CameraModifierToScreenPos(Vector2 screenPos, int timeLeft, string uniqueIdentity = null)
        {
            _screenPos = screenPos;
            TimeLeft = timeLeft;
            UniqueIdentity = uniqueIdentity;
        }
        public static Vector2 GetScreenPos(Vector2 pos) => pos - new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
    }
}
