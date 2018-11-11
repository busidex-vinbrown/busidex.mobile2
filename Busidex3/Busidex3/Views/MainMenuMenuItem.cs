using System;
using Xamarin.Forms;

namespace Busidex3.Views
{

    public class MainMenuMenuItem
    {
        public MainMenuMenuItem()
        {
            TargetType = typeof(MainMenuDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public ImageSource Image { get; set; }
        public Type TargetType { get; set; }
    }
}