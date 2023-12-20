using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ThreeRow
{
    class CGameDeck
    {
        private Canvas container;
        private Label scoreLabel;
        private int score;
        private CParentGem[,] gems;
        private CParentGem selectedGem;
        private Random rand;
        private int rows = 7;
        private int columns = 7;
        private int size = 50;
        private DispatcherTimer timer;
        private int timeLeft;
        private Label timeLabel;

        public CGameDeck(Canvas container, Label scoreLabel, Label timeLabel)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timeLeft = 30;
            timer.Start();
            this.timeLabel = timeLabel;
            this.container = container;
            this.scoreLabel = scoreLabel;
            score = 0;
            rand = new Random();
            gems = new CParentGem[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    CreateGem(i, j);
                }
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLabel.Content = timeLeft;
            timeLeft--;
            if (timeLeft <= 0)
            {
                timer.Stop();
                MessageBox.Show("Время вышло! Игра окончена.");
                Application.Current.Shutdown();
            }
        }
        private void CreateGem(int row, int column)
        {
            if (rand.Next(100) < 5)
            {
                gems[row, column] = new CBombGem(rand, container, row, column, size, this);
            }
            else
            {
                gems[row, column] = new CGem(rand, container, row, column, size, this);
            }
        }
        public void OnGemClicked(CParentGem gem)
        {
            if (selectedGem != null)
            {
                if (IsNextTo(gem, selectedGem))
                {
                    int row1 = GetRow(selectedGem.img);
                    int col1 = GetColumn(selectedGem.img);
                    int row2 = GetRow(gem.img);
                    int col2 = GetColumn(gem.img);
                    CParentGem temp = gems[row1, col1];
                    gems[row1, col1] = gems[row2, col2];
                    gems[row2, col2] = temp;
                    if (HasMatch())
                    {
                        SwapGems(gem, selectedGem);
                        CheckForMatches();
                    }
                    else
                    {
                        gems[row2, col2] = gems[row1, col1];
                        gems[row1, col1] = temp;
                    }

                    selectedGem = null;
                }
            }
            else
            {
                selectedGem = gem;
            }
        }

        private bool HasMatch()
        {
            for (int i = 0; i < gems.GetLength(0); i++)
            {
                for (int j = 0; j < gems.GetLength(1); j++)
                {
                    if (j < gems.GetLength(1) - 2 && gems[i, j].color == gems[i, j + 1].color && gems[i, j].color == gems[i, j + 2].color)
                    {
                        return true;
                    }

                    if (i < gems.GetLength(0) - 2 && gems[i, j].color == gems[i + 1, j].color && gems[i, j].color == gems[i + 2, j].color)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void OnBombGemClicked(CParentGem gem)
        {
            AnimationHelper.AnimateExplosion(gem.img);
            Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ExplodeBomb(gem);
                    score += 5;
                    scoreLabel.Content = score.ToString();
                });
            });
        }
        private int GetRow(Image im)
        {
            return (int)Canvas.GetTop(im) / size;
        }

        private int GetColumn(Image im)
        {
            return (int)Canvas.GetLeft(im) / size;
        }
        private bool IsNextTo(CParentGem gem1, CParentGem gem2)
        {
            int row1 = GetRow(gem1.img);
            int col1 = GetColumn(gem1.img);
            int row2 = GetRow(gem2.img);
            int col2 = GetColumn(gem2.img);
            return (row1 == row2 && Math.Abs(col1 - col2) == 1) || (col1 == col2 && Math.Abs(row1 - row2) == 1);
        }

        private void SwapGems(CParentGem gem1, CParentGem gem2)
        {
            double tempX = Canvas.GetLeft(gem1.img);
            double tempY = Canvas.GetTop(gem1.img);
            Canvas.SetLeft(gem1.img, Canvas.GetLeft(gem2.img));
            Canvas.SetTop(gem1.img, Canvas.GetTop(gem2.img));
            Canvas.SetLeft(gem2.img, tempX);
            Canvas.SetTop(gem2.img, tempY);
        }

        private void CheckForMatches()
        {
            bool matchFound;
            do
            {
                matchFound = false;
                for (int i = 0; i < gems.GetLength(0); i++)
                {
                    for (int j = 0; j < gems.GetLength(1); j++)
                    {
                        int horizontalMatchLength = 1;
                        while (j + horizontalMatchLength < gems.GetLength(1) && gems[i, j].color == gems[i, j + horizontalMatchLength].color)
                        {
                            horizontalMatchLength++;
                        }
                        if (horizontalMatchLength >= 3)
                        {
                            for (int k = 0; k < horizontalMatchLength; k++)
                            {
                                RemoveGem(gems[i, j + k]);
                                score++;
                            }
                            matchFound = true;
                        }
                        int verticalMatchLength = 1;
                        while (i + verticalMatchLength < gems.GetLength(0) && gems[i, j].color == gems[i + verticalMatchLength, j].color)
                        {
                            verticalMatchLength++;
                        }
                        if (verticalMatchLength >= 3)
                        {
                            for (int k = 0; k < verticalMatchLength; k++)
                            {
                                RemoveGem(gems[i + k, j]);
                                score++;
                            }
                            matchFound = true;
                        }
                    }
                }
            }
            while (matchFound);
            scoreLabel.Content = score.ToString();
        }

        private void ExplodeBomb(CParentGem bomb)
        {
            int bombRow = (int)Canvas.GetTop(bomb.img) / size;
            int bombColumn = (int)Canvas.GetLeft(bomb.img) / size;
            for (int i = Math.Max(0, bombRow - 1); i <= Math.Min(gems.GetLength(0) - 1, bombRow + 1); i++)
            {
                for (int j = Math.Max(0, bombColumn - 1); j <= Math.Min(gems.GetLength(1) - 1, bombColumn + 1); j++)
                {
                    RemoveGem(gems[i, j]);
                }
            }
            score += 5;
            scoreLabel.Content = score.ToString();
        }
        private void RemoveGem(CParentGem gem)
        {
            container.Children.Remove(gem.img);
            int row = (int)Canvas.GetTop(gem.img) / size;
            int column = (int)Canvas.GetLeft(gem.img) / size;
            CreateGem(row, column);
        }
    }
}
