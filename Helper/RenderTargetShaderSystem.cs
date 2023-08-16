using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Helper
{
    public class RenderTargetShaderSystem
    {
        public List<IRenderTargetShaderDraw> RenderDraw;
        public RenderTargetShaderSystem()
        {
            RenderDraw = new();
        }
        public void Draw()
        {
            foreach(var i in RenderDraw)
            {
                i.Draw();
            }
        }
        public void ResetData()
        {
            foreach (var i in RenderDraw)
            {
                i.ResetDrawData();
            }
            RenderDraw.Clear();
        }
        public bool Add(IRenderTargetShaderDraw draw)
        {
            for (int i = 0; i < RenderDraw.Count; i++)
            {
                if (RenderDraw[i].GetType().Equals(draw.GetType()))
                {
                    return false;
                }
            }
            RenderDraw.Add(draw);
            return true;
        }
        public T GetRenderDrawWithType<T>() where T : class, IRenderTargetShaderDraw
        {
            for (int i = 0; i < RenderDraw.Count; i++)
            {
                if (RenderDraw[i] is T)
                {
                    return RenderDraw[i] as T;
                }
            }
            throw new NotImplementedException();
        }
    }
}
