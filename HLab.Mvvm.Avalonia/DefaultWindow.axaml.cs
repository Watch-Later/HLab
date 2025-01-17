﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using HLab.Base.Avalonia;

namespace HLab.Mvvm.Avalonia;

using H = DependencyHelper<DefaultWindow>;

/// <summary>
/// Logique d'interaction pour DefaultWindow.xaml
/// </summary>
public partial class DefaultWindow : Window
{
    readonly Border _insideBorder;
    readonly ContentControl _content;

    public DefaultWindow()
    {
        InitializeComponent();

        if (ResizeGrid.NestedContent is Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is Border border) _insideBorder = border;
                if (child is ContentControl content) _content = content;
            }
        }
    }
    public object View
    {
        get => (object)GetValue(ViewProperty);
        set => SetValue(ViewProperty, value);
    }

    public static readonly StyledProperty<object> ViewProperty =
        H.Property<object>()
            .OnChangeBeforeNotification((e) =>
            {
                e._content.Content = e.View;
            })
            .Register();


    PointerPressedEventArgs? _clicked = null;

    void OnMouseDown(object? sender, PointerPressedEventArgs e)
    {
        var pos = e.GetPosition(this);
        _clicked = null;

        //Not in drag zone  (Title bar)
        if (pos.Y > 30) return;
        if(e.ClickCount>1)
        {
            if(WindowState == WindowState.Normal) 
                WindowState = WindowState.Maximized; 
            else WindowState = WindowState.Normal;

            return;
        }

        _clicked = e;

    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
    }

    void OnMouseMove(object? sender, PointerEventArgs e)
    {
        if (_clicked is null) return;

        var pos = e.GetPosition(this);

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            if (WindowState == WindowState.Maximized)
            {
                var width = Bounds.Width;
                var height = Bounds.Height;


                var xRatio = pos.X / width;

                var absPos = this.PointToScreen(pos);

                // var m = this.PresentationSource.FromVisual(this)?.CompositionTargett?.TransformToDevice;

                

                //if (m != null)
                //{
                    Bounds = new Rect(
                        (absPos.X/* / m.M11*/) - pos.X * (Width / width),
                        (absPos.Y/* / m.M22*/) - pos.Y * (Height / height),
                        Bounds.Width,
                        Bounds.Height
                    );

                    WindowState = WindowState.Normal;
                //}

            }

            var arg = _clicked;
            _clicked = null;
            try
            {
                this.BeginMoveDrag(arg);
            }
            catch (InvalidOperationException) { }
        }
    }

    void OnMouseUp(object? sender, PointerReleasedEventArgs pointerReleasedEventArgs)
    {
        _clicked = null;
    }


    CornerRadius _cornerRadius = new(0.0);
    CornerRadius _cornerRadiusZero = new(0.0);

    bool _maximize = false;

    protected override void HandleWindowStateChanged(WindowState state)
    {
        base.HandleWindowStateChanged(state);
        if (_cornerRadius == _cornerRadiusZero) _cornerRadius = _insideBorder.CornerRadius;

        switch (state)
        {
            case WindowState.Normal:
                if (_cornerRadius != _cornerRadiusZero)
                    _insideBorder.CornerRadius = _cornerRadius;

                _insideBorder.BorderThickness = new Thickness(1.0);
                break;

            case WindowState.Maximized:
                if (_cornerRadius != _cornerRadiusZero)
                    _insideBorder.CornerRadius = _cornerRadiusZero;

                _insideBorder.BorderThickness = new Thickness(0.0);

                if(!_maximize)
                {
                    _maximize = true;
                    WindowState = WindowState.Minimized;
                }
                else _maximize = false;

                break;

            case WindowState.Minimized:

                if(_maximize)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    if (_cornerRadius != _cornerRadiusZero)
                        _insideBorder.CornerRadius = _cornerRadiusZero;
                }

                break;
        }


    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        return base.ArrangeOverride(arrangeBounds);
    }
}
