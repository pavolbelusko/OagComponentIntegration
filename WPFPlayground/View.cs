using System.Windows;
using System.Windows.Markup;

namespace WPFPlayground
{
    [ContentProperty(nameof(Template))]
    public abstract class View
    {
        public DataTemplate Template { get; set; }
    }
}