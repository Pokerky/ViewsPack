using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ViewsPackExample
{
	public class DataItem
	{
		public DataItem(string color)
		{
			Color = color;
		}

		public string Color { get; }

		public static DataItem[] DataItems { get; } = new DataItem[] 
		{
			new DataItem("#BC8F8F"),
			new DataItem("#B3EE3A"),
			new DataItem("#9BCD9B"),
			new DataItem("#9AC0CD"),
			new DataItem("#8B7765"),
			new DataItem("#00C5CD")
		};
	}
}
