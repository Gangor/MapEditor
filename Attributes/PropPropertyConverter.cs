using System.ComponentModel;
using MapEditor.Modules;
using System.Linq;

namespace MapEditor.Extends
{
	public class PropPropertyConverter : StringConverter
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
			var data = CfgManager.Instance.Props
				.Select(r => r.PropName)
				.OrderBy(r => r).ToArray();

			return new StandardValuesCollection(data);
		}
	}
}
