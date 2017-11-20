using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace genshinframework
{
    public class VInput
    {
        public VInput() { }

        public static void PerformLeftClick(int sleep = 50)
        {
            InputManager.Mouse.ButtonDown(InputManager.Mouse.MouseKeys.Left);
            System.Threading.Thread.Sleep(sleep);
            InputManager.Mouse.ButtonUp(InputManager.Mouse.MouseKeys.Left);
        }

        public static void PerformRightClick(int sleep = 50)
        {
            InputManager.Mouse.ButtonDown(InputManager.Mouse.MouseKeys.Right);
            System.Threading.Thread.Sleep(sleep);
            InputManager.Mouse.ButtonUp(InputManager.Mouse.MouseKeys.Right);
        }
    }
}
