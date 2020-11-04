using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewsPackFrameworkExample
{
	public class DataItem
	{
		public DataItem(string path)
		{
			Path = path;
		}

		public string Path { get; }

		public static DataItem[] DataItems
		{
			get
			{
				string dirName = "F:\\images";
				DirectoryInfo dir = new DirectoryInfo(dirName);
				if (!dir.Exists) { return null; }

				List<DataItem> items = new List<DataItem>();
				foreach (var file in dir.GetFiles())
				{
					string extension = file.Extension.ToLower();
					if (extension == ".jpg" || extension == ".png")
					{
						items.Add(new DataItem(file.FullName));
					}
				}
				return items.ToArray();
			}
		}
	}
}
