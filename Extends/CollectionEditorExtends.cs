using MapEditor.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace MapEditor.Extends
{
	public class CollectionEditorExtends : CollectionEditor
	{
		public delegate void OkClickEventHandler(object sender, EventArgs e);
		public delegate void PropertyValueChangedEventHandler(object sender, PropertyValueChangedEventArgs e);
		
		public CollectionEditorExtends(Type type) : base(type) { }
		
		protected override CollectionForm CreateCollectionForm()
		{
			var collectionForm = base.CreateCollectionForm();
			var frmCollectionEditorForm = collectionForm as Form;
			var tlpLayout = frmCollectionEditorForm.Controls[0] as TableLayoutPanel;

			if (tlpLayout != null)
			{
				frmCollectionEditorForm.FormClosing += propertyGrid_OkClick;

				if (tlpLayout.Controls[6].Controls[1] is Button)
					(tlpLayout.Controls[6].Controls[1] as Button).Click += propertyGrid_OkClick;

				if (tlpLayout.Controls[4] is ListBox)
				{
					var listBox = tlpLayout.Controls[4] as ListBox;
				}
				
				if (tlpLayout.Controls[5] is PropertyGrid)
				{
					PropertyGrid propertyGrid = tlpLayout.Controls[5] as PropertyGrid;
					propertyGrid.BrowsableAttributes = new AttributeCollection(new PropertyGridBrowsableAttribute(true));
					propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
				}
			}

			return collectionForm;
		}

		void propertyGrid_OkClick(object sender, EventArgs e)
		{
			OkClick?.Invoke(this, e);
		}

		void propertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			PropertyValueChanged?.Invoke(this, e);
		}

		public static event OkClickEventHandler OkClick;
		public static event PropertyValueChangedEventHandler PropertyValueChanged;
	}
}
