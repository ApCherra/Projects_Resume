using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Spreadsheet_Apram_Cherra.Views;

public partial class ColorPickerPopup : Window
{
    public uint PickedColor;
    public ColorPickerPopup()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Change_Color(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.PropertyType.FullName == "Avalonia.Media.Color")
        {
            Console.WriteLine(e.Property.PropertyType + ":" + e.NewValue + " : " + e.OldValue);
            PickedColor = (uint)((Avalonia.Media.Color) e.NewValue).ToUint32();
        }
    }

}