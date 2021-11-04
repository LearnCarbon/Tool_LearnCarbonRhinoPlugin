using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using LearnCarbon.View;

namespace LearnCarbon
{
    [Guid("eeb02f3d-d63d-4f7e-8eeb-0480386b3c74")]
    public class LearnCarbonHost : RhinoWindows.Controls.WpfElementHost
    {
        public LearnCarbonHost(uint docSn) : base(new MainWindow(docSn), null)
        {
        }
    }
}
