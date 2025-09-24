using System;
using System.Linq;

namespace AssertThatLibrary;

[Flags]
public enum Direction
{
    None = 0,
    Actual = 1,
    Expected = 2,
    Both = Actual | Expected
}
