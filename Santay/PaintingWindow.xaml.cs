using System;
using System.Windows.Input;
using Jbh;

namespace Santay;

public partial class PaintingWindow
{
    public PaintingWindow()
    {
        InitializeComponent();
    }

    private void PaintCanvas_MouseDown(object sender, MouseButtonEventArgs e)
    {
        DialogResult = false;
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        // string title = GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
        string copyright = "Copyright Jonathan Hepworth 2022";
        string description ="Health and fitness records";

        // textblockVersion.Text = "Version ";
        // DateTime startDate = new DateTime(2000, 1, 1);
        // Version versionInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        // int diffDays = versionInfo.Build;

        textblockTitle.Text = AppManager.AppName;
        textblockDescription.Text = description;
        textblockCopyright.Text = copyright;

        textblockVersion.Text =$".NET6.0 application version {1}.{0}";
    }

    // public string GetAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
    // {
    //     T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
    //     return value.Invoke(attribute);
    // }
}