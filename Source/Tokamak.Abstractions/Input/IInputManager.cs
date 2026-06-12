using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Abstractions.Input
{
    public delegate void KeyDown(int key);

    public delegate void KeyUp(int key);

    public interface IInputManager
    {
        event KeyDown KeyDown;

        event KeyUp KeyUp;
    }
}
