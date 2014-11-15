using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace TrafficReport.Extensions
{
    public static class PropertySetExtensions
    {
        public static T Get<T>(this IPropertySet set, string key, T defaultValue)
        {
            return set.ContainsKey(key) ? (T)set[key] : defaultValue;
        }
    }

    // From http://www.wiredprairie.us/blog/index.php/archives/1730
    public static class FrameworkElementExtensions
    {
        public static FrameworkElement FindDescendantByName(this FrameworkElement element, string name)
        {
            if ((element == null) || String.IsNullOrWhiteSpace(name))
            {
                return null; 
            }

            if (name.Equals(element.Name, StringComparison.OrdinalIgnoreCase))
            {
                return element;
            }

            var childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var child = (FrameworkElement)VisualTreeHelper.GetChild(element, i);
                FrameworkElement result = child.FindDescendantByName(name);
                if (result != null) 
                {
                    return result; 
                }
            }

            return null;
        }
    }
}
