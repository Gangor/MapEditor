using MapEditor.Attributes;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace MapEditor.Extends
{
    public class CollectionEditorExtends : CollectionEditor
    {
        // Define a static event to expose the inner PropertyGrid's
        // PropertyValueChanged event args...
        public delegate void OkClickEventHandler(object sender, EventArgs e);
        public delegate void PropertyValueChangedEventHandler(object sender, PropertyValueChangedEventArgs e);

        // Inherit the default constructor from the standard
        // Collection Editor...
        public CollectionEditorExtends(Type type) : base(type) { }

        // Override this method in order to access the containing user controls
        // from the default Collection Editor form or to add new ones...
        protected override CollectionForm CreateCollectionForm()
        {
            // Getting the default layout of the Collection Editor...
            CollectionForm collectionForm = base.CreateCollectionForm();

            Form frmCollectionEditorForm = collectionForm as Form;
            TableLayoutPanel tlpLayout = frmCollectionEditorForm.Controls[0] as TableLayoutPanel;

            if (tlpLayout != null)
            {
                /*
                if (this.CollectionType.BaseType == typeof(Array))
                {
                    // Find the Add button
                    if (tlpLayout.Controls[1] is TableLayoutPanel)
                        (tlpLayout.Controls[1] as TableLayoutPanel).Visible = false;
                }
                */
                
                if (tlpLayout.Controls[6].Controls[0] is Button)
                    (tlpLayout.Controls[6].Controls[0] as Button).Click += propertyGrid_OkClick;

                if (tlpLayout.Controls[4] is ListBox)
                {
                    var listBox = tlpLayout.Controls[4] as ListBox;
                }

                // Get a reference to the inner PropertyGrid and hook
                // an event handler to it.
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
            // Fire our customized collection event...
            PropertyValueChanged?.Invoke(this, e);
        }

        public static event OkClickEventHandler OkClick;
        public static event PropertyValueChangedEventHandler PropertyValueChanged;
    }
}
