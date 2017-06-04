using System.ComponentModel;
using System.Resources;

namespace MapEditor.Extends
{
    public static class ResourceManagerExtends
    {
        public static string GetStringFormat(this ResourceManager manager, string name, params object[] param)
        {
            return string.Format(manager.GetString(name), param);
        }
    }
}
