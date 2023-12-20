using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace ThreeRow
{
    class CBombGem : CParentGem
    {
        public CBombGem(Random rand, Canvas container, int row, int column, int size, CGameDeck gameDeck)
        {
            var path = @"pack://application:,,,/Resources/bomb.png";
            this.size = size;
            img = new Image
            {
                Width = size,
                Height = size,
                Margin = new Thickness(0),
                Source = new BitmapImage(new Uri(path)),
                Stretch = Stretch.Fill,
            };
            img.MouseLeftButtonDown += (s, e) => OnClick(gameDeck);
            container.Children.Add(img);
            Canvas.SetLeft(img, size * column);
            Canvas.SetTop(img, size * row);
            Canvas.SetZIndex(img, 1);
        }
        public override void OnClick(CGameDeck gameDeck)
        {
            gameDeck.OnBombGemClicked(this);
        }
    }
}
