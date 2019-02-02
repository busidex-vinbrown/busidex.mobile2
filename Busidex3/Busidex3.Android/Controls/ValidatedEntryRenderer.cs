using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Busidex3.Controls;
using Busidex3.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ValidatedEntry), typeof(ValidatedEntryRenderer))]
namespace Busidex3.Droid.Controls
{
    public class ValidatedEntryRenderer : EntryRenderer
    {
        public ValidatedEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var nativeEditText = (global::Android.Widget.EditText)Control;
            var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RectShape());
            shape.Paint.Color = Xamarin.Forms.Color.Red.ToAndroid();
            shape.Paint.SetStyle(Paint.Style.Stroke);
            nativeEditText.Background = shape;
        }
    }
}