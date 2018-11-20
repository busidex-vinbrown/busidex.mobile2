using System;
using Busidex3.Views;
using Xamarin.Forms;

namespace Busidex3.ViewModels
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