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
    }
}
