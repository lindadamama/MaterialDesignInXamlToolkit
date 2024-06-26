﻿using System.Globalization;
using System.Windows.Media;
using MaterialDesignThemes.Wpf.Converters;

namespace MaterialDesignThemes.Wpf.Tests.Converters;

public class FloatingHintTransformConverterTests
{
    public static IEnumerable<object?[]> InvalidParameters =>
        new[]
        {
            [null, null, null, null],
            [1.0, null, null, null],
            [null, 1.0, null, null],
            [null, null, 1.0, null],
            [null, null, null, new Point()],
            [1.0, DependencyProperty.UnsetValue, DependencyProperty.UnsetValue, DependencyProperty.UnsetValue],
            [DependencyProperty.UnsetValue, 1.0, DependencyProperty.UnsetValue, DependencyProperty.UnsetValue],
            [DependencyProperty.UnsetValue, DependencyProperty.UnsetValue, 1.0, DependencyProperty.UnsetValue],
            new object?[] {DependencyProperty.UnsetValue, DependencyProperty.UnsetValue, DependencyProperty.UnsetValue, new Point() },
        };

    [Theory]
    [MemberData(nameof(InvalidParameters))]
    public void WhenParametersAreNotSetItReturnsIdentity(object? scale, object? lower, object? upper, object? offset)
    {
        FloatingHintTransformConverter converter = new ();

        object? result = converter.Convert([scale, lower, upper, offset],
            typeof(Transform), null, CultureInfo.CurrentUICulture);

        Assert.Equal(Transform.Identity, result);
    }

    [Theory]
    [InlineData(2.0, 1.5, 3.0, 3.0, -4.0)]
    [InlineData(1.5, 2.0, 3.0, 2.0, -3.0)]
    public void WhenParametersAreSpecifiedItReturnsTransforms(double scale, double lower, double upper, double x, double y)
    {
        FloatingHintTransformConverter converter = new();

        var result = (TransformGroup?)converter.Convert([scale, lower, upper, new Point(x, y), 0],
            typeof(Transform), null, CultureInfo.CurrentUICulture);

        Assert.NotNull(result);
        var scaleTransform = (ScaleTransform)result!.Children[0];
        var translateTransform = (TranslateTransform)result.Children[1];

        Assert.Equal(upper + (lower - upper) * scale, scaleTransform.ScaleX);
        Assert.Equal(upper + (lower - upper) * scale, scaleTransform.ScaleY);

        Assert.Equal(scale * x, translateTransform.X);
        Assert.Equal(scale * y, translateTransform.Y);
    }

    [Theory]
    [InlineData(2.0, 1.5, 3.0, 3.0, 4.0)]
    [InlineData(1.5, 2.0, 3.0, 2.0, 3.0)]
    public void WhenParametersAreSpecifiedAndScaleTransformDisabledItReturnsTransforms(double scale, double lower, double upper, double x, double y)
    {
        FloatingHintTransformConverter converter = new () { ApplyScaleTransform = false };

        var result = (TransformGroup?)converter.Convert([scale, lower, upper, new Point(x, y), 0],
            typeof(Transform), null, CultureInfo.CurrentUICulture);

        Assert.NotNull(result);
        Assert.Single(result.Children);
        Assert.IsType<TranslateTransform>(result.Children[0]);
    }

    [Theory]
    [InlineData(2.0, 1.5, 3.0, 3.0, 4.0)]
    [InlineData(1.5, 2.0, 3.0, 2.0, 3.0)]
    public void WhenParametersAreSpecifiedAndTranslateTransformDisabledItReturnsTransforms(double scale, double lower, double upper, double x, double y)
    {
        FloatingHintTransformConverter converter = new () { ApplyTranslateTransform = false };

        var result = (TransformGroup?)converter.Convert([scale, lower, upper, new Point(x, y), 0],
            typeof(Transform), null, CultureInfo.CurrentUICulture);

        Assert.NotNull(result);
        Assert.Single(result.Children);
        Assert.IsType<ScaleTransform>(result.Children[0]);
    }
}
