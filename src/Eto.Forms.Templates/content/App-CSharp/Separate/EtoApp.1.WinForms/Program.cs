﻿using System;
using Eto.Forms;

namespace EtoApp._1.WinForms
{
	class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			new Application(Eto.Platforms.WinForms).Run(new MainForm());
		}
	}
}
