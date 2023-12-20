using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ThreeRow
{
    class AnimationHelper
    {
        public static void AnimateExplosion(Image bombImage)
        {
            Image explosionImage = new Image
            {
                Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/boom.png")),
                Width = 100,
                Height = 100,
            };
            Canvas.SetLeft(explosionImage, Canvas.GetLeft(bombImage) + bombImage.Width / 2 - explosionImage.Width / 2);
            Canvas.SetTop(explosionImage, Canvas.GetTop(bombImage) + bombImage.Height / 2 - explosionImage.Height / 2);
            ((Canvas)bombImage.Parent).Children.Add(explosionImage);
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
            };
            animation.Completed += (s, e) => ((Canvas)bombImage.Parent).Children.Remove(explosionImage);
            explosionImage.BeginAnimation(Image.OpacityProperty, animation);
        }
    }
}
