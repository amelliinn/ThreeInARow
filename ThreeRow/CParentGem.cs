using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ThreeRow
{
    enum Color { blue, green, orange, red, yellow, purple, bomb}
    abstract class CParentGem
    {
        public Color color;
        public Image img;
        public int size;
        public abstract void OnClick(CGameDeck gameDeck);
    }
}
