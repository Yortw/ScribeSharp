﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScribeSharp
{
	internal static class BaitExceptionHelper
	{
		internal static void Throw()
		{
			throw new InvalidOperationException("Incorrect reference, this assembly should never be loaded at runtime. Please reference the platform specific (ScribeSharp.dll) assembly for your application. The portable assembly should only be referenced from other PCLs.");
		}
	}
}