using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command.ResidueSwing
{
    /// <summary>
    /// 残留的刀光
    /// </summary>
    public class ResidueSwing
    {
        private static ResidueSwing _instance;
        public static ResidueSwing Instance
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }
        private ResidueSwing()
        {
            residueSwing_Draws = new();
            residueSwing_Time = new();
        }
        /// <summary>
        /// 绘制用的委托<para>请手动Begin与End,调用Render的时候它不负责</para>
        /// </summary>
        /// <param name="time">所经过的时间</param>
        /// <returns>如果这玩意返回了false,那么就从这个残留刀光里面移除</returns>
        public delegate bool ResidueSwing_DrawTrailing(int time);
        /// <summary>
        /// 对应的委托类List
        /// </summary>
        public List<ResidueSwing_DrawTrailing> residueSwing_Draws;
        /// <summary>
        /// 委托类对应的调用时间长度
        /// </summary>
        public List<int> residueSwing_Time;
        /// <summary>
        /// 添加绘制残影刀光
        /// </summary>
        /// <param name="residueSwing_DrawTrailing">残影刀光委托</param>
        /// <param name="timeLeft">残影刀光存在时间</param>
        public void AddResidueSwing(ResidueSwing_DrawTrailing residueSwing_DrawTrailing,int timeLeft)
        {
            residueSwing_Draws.Add(residueSwing_DrawTrailing);
            residueSwing_Time.Add(timeLeft);
        }
        /// <summary>
        /// 这是希斯克里夫,它永远不会与Draw相见
        /// </summary>
        public void Update()
        {
            if (residueSwing_Time == null || residueSwing_Time.Count == 0)
                return;
            #region 计时器增加
            for (int i = 0; i < residueSwing_Time.Count; i++)
            {
                residueSwing_Time[i]--;
            }
            #endregion
        }
        /// <summary>
        /// 这是▢▢▢,它永远不会与Update相见
        /// </summary>
        public void Draw()
        {
            if (residueSwing_Time == null || residueSwing_Time.Count == 0 || residueSwing_Draws == null || residueSwing_Draws.Count == 0)
                return;
            #region 试图 Clear All Cathy
            List<int> removeIndex = new List<int>();
            for (int i = 0; i < residueSwing_Draws.Count; i++)
            {
                if (!residueSwing_Draws[i].Invoke(residueSwing_Time[i]) || residueSwing_Time[i] < 0)
                    removeIndex.Add(i);
            }

            removeIndex.ForEach(x =>
            {
                residueSwing_Draws.RemoveAt(x);
                residueSwing_Time.RemoveAt(x);
            });
            #endregion
        }
    }
}
