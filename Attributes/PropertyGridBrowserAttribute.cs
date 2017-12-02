﻿using System;

namespace MapEditor.Attributes
{
	public class PropertyGridBrowsableAttribute : Attribute
	{
		private bool browsable;
		public PropertyGridBrowsableAttribute(bool browsable)
		{
			this.browsable = browsable;
		}
	}
}
