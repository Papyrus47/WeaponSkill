using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaponSkill.Command
{
    public interface IRenderTargetShaderDraw
    {
        public bool Remove { get; set; }
        public void Draw();
        public void ResetDrawData();
    }
}
