using System;
using System.Linq;

namespace AssertThatLibrary;

public class AssertThatException(string message) : Exception(message);
