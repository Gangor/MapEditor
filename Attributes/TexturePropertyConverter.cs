using System.ComponentModel;
using MapEditor.Modules;
using System.Linq;

namespace MapEditor.Extends
{
	public class TexturePropertyConverter : StringConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			var data = CfgManager.Instance.Textures
				.Select(r => r.TextureName)
				.OrderBy(r => r).ToList();

			return new StandardValuesCollection(data);
		}
	}
}
