using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private int _x, _y, _score = 0, _horizontalCount = 10, _verticalCount = 21, _left = 0, _top = 0, _right = 225, _bottom = 600, _w = 25, _h = 25, _m = 30;
        private string _titleStr = "Тетрис";

        private int[] _scoreValuesCond =
        {
            3000, 10000, 20000, 30000, 40000, 50000, 60000, 70000, 80000
        };

        private Color _colorGameGround = (Color)ColorConverter.ConvertFromString("#eeeee4");
        private Color _colorTarget = (Color)ColorConverter.ConvertFromString("#50514f");
        private Color _colorEmpty = (Color)ColorConverter.ConvertFromString("#ffffff");

        private Cell[][] _cells, _cellsNextFigure;
        private Figure[] _figures;
        private Figure _currentFig, _nextFigure;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            _figures = new Figure[]
            {
                new Figure(1 , 4 , new int[4][]
                    {
                        new int[1] {0}, new int[1] {0}, new int[1] {0}, new int[1] {0}
                    }, new Color[] { _colorGameGround,  _colorTarget}),

                new Figure(2 , 2 , new int[2][]
                    {
                        new int[2] {0, 1}, new int[2] {0, 1}
                    }, new Color[] { _colorGameGround,  _colorTarget}),


                new Figure(3 , 2 , new int[2][]
                    {
                        new int[3] {-1, 1, -1}, new int[3] {0, 1, 2}
                    }, new Color[] { _colorGameGround,  _colorTarget}),

                new Figure(3 , 2 , new int[2][]
                    {
                        new int[3] {-1, -1 , 2}, new int[3] {0, 1, 2}
                    }, new Color[] { _colorGameGround,  _colorTarget}),

                new Figure(3 , 2 , new int[2][]
                    {
                        new int[3] {0, -1, -1}, new int[3] {0, 1, 2}
                    }, new Color[] { _colorGameGround,  _colorTarget}),

                new Figure(3 , 2 , new int[2][]
                    {
                        new int[3] {-1, 1, 2}, new int[3] {0, 1, -1}
                    }, new Color[] { _colorGameGround,  _colorTarget}),

                new Figure(3 , 2 , new int[2][]
                    {
                        new int[3] {0, 1, -1}, new int[3] {-1, 1 , 2}
                    }, new Color[] { _colorGameGround,  _colorTarget})
            };
        }

        private void GameGroundInit()
        {
            _cells = new Cell[_verticalCount][];

            int left = _left, top = _top, right = _right, bottom = _bottom, w = _w, h = _h, m = _m;

            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i] = new Cell[_horizontalCount];
                for (int j = 0; j < _cells[i].Length; j++)
                {
                    _cells[i][j] = new Cell();
                    gameGround.Children.Add(_cells[i][j].Grid);

                    if (i == 0)
                        _cells[i][j].Grid.Visibility = Visibility.Hidden;

                    _cells[i][j].Grid.Width = w;
                    _cells[i][j].Grid.Height = h;
                    _cells[i][j].Grid.Margin = new Thickness(left, top, right, bottom);
                    _cells[i][j].Grid.Background = new SolidColorBrush(_colorGameGround);
                    left += m;
                    right -= m;
                }

                left = _left;
                right = _right;
                top += m;
                bottom -= m;
            }
        }

        private async Task GetComment()
        {
            string[] messages =
            {
                "Тетрис!",
                "Отлично!",
                "Вот это да!",
                "Комбинация!",
                "Супер!"
            };

            double maxOpacity = 1d;
            Random r = new Random();
            int left = 213, top = 303, right = 46, bottom = 181, bottomMin = 136, rightMin = 20, speed = 10, fontSizeMax = 25;

            comment.Visibility = Visibility.Visible;

            comment.Content = messages[r.Next(0, messages.Length)];

            while (comment.Margin.Bottom >= bottomMin || comment.Margin.Right >= rightMin)
            {
                comment.Margin = new Thickness(comment.Margin.Left, comment.Margin.Top, comment.Margin.Right - 1, comment.Margin.Bottom - 1);

                if (comment.FontSize < fontSizeMax)
                    comment.FontSize++;
                if (comment.Opacity < maxOpacity)
                    comment.Opacity++;
                await Task.Run(() => Thread.Sleep(speed));
            }

            while (comment.Opacity > 0)
            {
                comment.Opacity--;
                await Task.Run(() => Thread.Sleep(speed));
            }

            comment.Margin = new Thickness(left, top, right, bottom);
            comment.Visibility = Visibility.Hidden;
        }

        private async void startGame_Click(object sender, RoutedEventArgs e)
        {
            startGame.Visibility = Visibility.Hidden;
            gameGround.Children.Clear();
            GameGroundInit();
            _score = 0;
            score.Content = $"Счет: {_score}";

            Title = _titleStr;

            await GameProcess();
        }

        private async Task DeathMessage(string message, int ms)
        {
            Title = "";

            while (true)
            {
                if (Title == message)
                    Title = "";
                else
                {
                    Title += "";
                    await Task.Run(() => Thread.Sleep(ms));

                    foreach (char s in message)
                    {
                        if (_score == 0)
                        {
                            Title = _titleStr;
                            return;
                        }
                        Title += s;
                        await Task.Run(() => Thread.Sleep(ms));
                    }
                }
            }
        }

        private async Task GameProcess()
        {
            int delay = 1000, figIndex, busyCellsCount = 0, left, top, right, bottom, w = _w, h = _h, m = _m, _lastConditionIndex = 0;
            bool commentHasBeenReceived = false;
            Random r = new Random();

            while (true)
            {
                _x = 3;
                _y = 0;

                left = 0;
                top = 0;
                right = 259;
                bottom = 259;

                if (_nextFigure == null)
                {
                    figIndex = r.Next(0, _figures.Length);
                    _currentFig = _figures[figIndex].Copy();
                    figIndex = r.Next(0, _figures.Length);
                    _nextFigure = _figures[figIndex].Copy();
                }
                else
                {
                    _currentFig = _nextFigure;
                    figIndex = r.Next(0, _figures.Length);
                    _nextFigure = _figures[figIndex].Copy();
                }

                nextFigureGround.Children.Clear();
                _cellsNextFigure = new Cell[_nextFigure.Height][];
                for (int i = 0; i < _cellsNextFigure.Length; i++)
                {
                    _cellsNextFigure[i] = new Cell[_nextFigure.Width];
                    for (int j = 0; j < _cellsNextFigure[i].Length; j++)
                    {
                        _cellsNextFigure[i][j] = new Cell();
                        _cellsNextFigure[i][j].Grid.Width = w;
                        _cellsNextFigure[i][j].Grid.Height = h;
                        _cellsNextFigure[i][j].Grid.Margin = new Thickness(left, top, right, bottom);

                        left += m;
                        right -= m;

                        if (_nextFigure.HorizontalStruct[i][j] != null)
                            _cellsNextFigure[i][j].Grid.Background = _nextFigure.HorizontalStruct[i][j].Background;
                        else
                            continue;

                        nextFigureGround.Children.Add(_cellsNextFigure[i][j].Grid);
                    }

                    left = 0;
                    right = 259;
                    top += m;
                    bottom -= m;
                }

                while (!IsFalled(_currentFig, _x, _y))
                {
                    _y++;
                    DrawFigure(_currentFig, ref _x, ref _y);

                    await Task.Run(() => Thread.Sleep(delay));
                }

                _score += 30;

                if (_currentFig.IsHorizontal)
                    for (int i = 0; i < _currentFig.Height; i++)
                        for (int j = 0; j < _currentFig.Width; j++)
                        {
                            if (_currentFig.HorizontalStruct[i][j] != null)
                            {
                                _cells[_y + i][_x + j].IsFree = false;
                                _cells[_y + i][_x + j].IsActive = false;
                            }
                        }
                else
                    for (int i = 0; i < _currentFig.Width; i++)
                        for (int j = 0; j < _currentFig.Height; j++)
                            if (_currentFig.VerticalStruct[i][j] != null)
                            {
                                _cells[_y + i][_x + j].IsFree = false;
                                _cells[_y + i][_x + j].IsActive = false;
                            }

                for (int i = 0; i < _horizontalCount; i++)
                    if (!_cells[0][i].IsFree)
                    {
                        _currentFig = null;
                        _nextFigure = null;
                        startGame.Content = $"Счет: {_score}\n\nЕще раз?";
                        startGame.Visibility = Visibility.Visible;

                        await DeathMessage("Сдох :(", 500);

                        return;
                    }

                do
                {
                    busyCellsCount = 0;

                    for (int i = _cells.Length - 1; i >= 0; i--)
                    {
                        for (int j = 0; j < _cells[i].Length; j++)
                            if (!_cells[i][j].IsFree)
                                busyCellsCount++;

                        if (busyCellsCount == _horizontalCount)
                        {
                            if (!commentHasBeenReceived)
                            {
                                commentHasBeenReceived = true;
                                await Task.WhenAny(GetComment());
                            }

                            _score += 1500;

                            for (int n = i; n > 0; n--)
                                for (int j = 0; j < _cells[n].Length; j++)
                                {
                                    _cells[n][j].Grid.Background = new SolidColorBrush((_cells[n - 1][j].Grid.Background as SolidColorBrush).Color);
                                    _cells[n][j].IsFree = _cells[n - 1][j].IsFree;
                                }

                            break;
                        }
                        else
                            busyCellsCount = 0;
                    }
                }
                while (busyCellsCount == _horizontalCount);

                for (int i = _lastConditionIndex; i < _scoreValuesCond.Length && delay >= 100; i++)
                    if (_score > _scoreValuesCond[i])
                    {
                        _lastConditionIndex = i + 1;
                        delay -= 100;
                    }

                score.Content = $"Счет: {_score}";
                commentHasBeenReceived = false;
            }
        }

        private bool IsFalled(Figure fig, int x, int y)
        {
            if (fig.IsHorizontal)
                if (y == _verticalCount - fig.Height)
                    return true;
                else
                    for (int i = x; i < fig.Width + x; i++)
                    {
                        if (!_cells[y + fig.Height][i].IsFree && (_cells[y + fig.Height][i].Grid.Background as SolidColorBrush).Color != _colorGameGround && (_cells[y + fig.Height - 1][i].Grid.Background as SolidColorBrush).Color != _colorGameGround)
                            return true;
                    }
            else
            {
                if (y == _verticalCount - fig.Width)
                    return true;
                else
                {
                    int n, k;

                    for (int i = x; i < fig.Height + x; i++)
                    {
                        n = 1;
                        k = 0;

                        while (!_cells[y + fig.Width - n][i].IsActive)
                        {
                            n++;
                            k++;
                        }

                        if (!_cells[y + fig.Width - k][i].IsFree && _cells[y + fig.Width - n][i].IsFree && (_cells[y + fig.Width - k][i].Grid.Background as SolidColorBrush).Color != _colorGameGround && (_cells[y + fig.Width - n][i].Grid.Background as SolidColorBrush).Color != _colorGameGround)
                            return true;
                    }
                }
            }

            return false;
        }

        private void DrawFigure(Figure fig, ref int x, ref int y, bool rotate = false)
        {
            if (fig.IsHorizontal)
                for (int i = 0; i < fig.Height + 1; i++)
                    for (int j = 0; j < fig.Width + 1; j++)
                    {
                        if (x + j - 1 >= 0)
                            if (_cells[y + i - 1][x + j - 1].IsFree)
                            {
                                _cells[y + i - 1][x + j - 1].Grid.Background = new SolidColorBrush(_colorGameGround);
                                _cells[y + i - 1][x + j - 1].IsActive = false;
                            }
                        if (x + j + 1 < _horizontalCount)
                            if (_cells[y + i - 1][x + j].IsFree)
                            {
                                _cells[y + i - 1][x + j].Grid.Background = new SolidColorBrush(_colorGameGround);
                                _cells[y + i - 1][x + j].IsActive = false;
                            }
                        if (x + fig.Width - 1 < _horizontalCount - 1)
                            if (_cells[y + i - 1][x + fig.Width].IsFree)
                            {
                                _cells[y + i - 1][x + fig.Width].Grid.Background = new SolidColorBrush(_colorGameGround);
                                _cells[y + i - 1][x + fig.Width].IsActive = false;
                            }
                    }
            else
                for (int i = 0; i < fig.Width + 1; i++)
                    for (int j = 0; j < fig.Height + 1; j++)
                    {
                        if (x + j - 1 >= 0)
                            if (_cells[y + i - 1][x + j - 1].IsFree)
                            {
                                _cells[y + i - 1][x + j - 1].Grid.Background = new SolidColorBrush(_colorGameGround);
                                _cells[y + i - 1][x + j - 1].IsActive = false;
                            }
                        if (x + j + 1 < _horizontalCount)
                            if (_cells[y + i - 1][x + j].IsFree)
                            {
                                _cells[y + i - 1][x + j].Grid.Background = new SolidColorBrush(_colorGameGround);
                                _cells[y + i - 1][x + j].IsActive = false;
                            }
                        if (x + fig.Height - 1 < _horizontalCount - 1)
                            if (_cells[y + i - 1][x + fig.Height].IsFree)
                            {
                                _cells[y + i - 1][x + fig.Height].Grid.Background = new SolidColorBrush(_colorGameGround);
                                _cells[y + i - 1][x + fig.Height].IsActive = false;
                            }
                    }

            if (rotate)
                fig.Rotate();

            if (fig.IsHorizontal)
            {
                for (int i = 0; i < fig.Height; i++)
                    for (int j = 0; j < fig.Width; j++)
                    {
                        if (y + i >= _verticalCount)
                        {
                            for (int m = 0; m < i; m++)
                                for (int n = 0; n < fig.Height; n++)
                                {
                                    _cells[y + m][x + n].Grid.Background = new SolidColorBrush(_colorGameGround);
                                    _cells[y + m][x + n].IsActive = false;
                                }

                            y--;
                            i = -1;
                            break;
                        }
                        else if (x + j >= _horizontalCount)
                        {
                            for (int n = 0; n < j; n++)
                            {
                                _cells[y + i][x + n].Grid.Background = new SolidColorBrush(_colorGameGround);
                                _cells[y + i][x + n].IsActive = false;
                            }

                            x--;
                            j = -1;
                        }
                        else
                        {
                            if (fig.HorizontalStruct[i][j] != null)
                            {
                                _cells[y + i][x + j].Grid.Background = fig.HorizontalStruct[i][j].Background == new SolidColorBrush(_colorEmpty) ? new SolidColorBrush(_colorGameGround) : fig.HorizontalStruct[i][j].Background;
                                _cells[y + i][x + j].IsActive = _cells[y + i][x + j].Grid.Background != new SolidColorBrush(_colorGameGround);
                            }
                        }

                    }
            }
            else
            {
                for (int i = 0; i < fig.Width; i++)
                    for (int j = 0; j < fig.Height; j++)
                    {
                        if (y + i >= _verticalCount)
                        {
                            for (int m = 0; m < i; m++)
                                for (int n = 0; n < fig.Height; n++)
                                {
                                    _cells[y + m][x + n].Grid.Background = new SolidColorBrush(_colorGameGround);
                                    _cells[y + m][x + n].IsActive = false;
                                }

                            y--;
                            i = -1;
                            break;
                        }
                        else if (x + j >= _horizontalCount)
                        {
                            for (int n = 0; n < j; n++)
                            {
                                _cells[y + i][x + n].Grid.Background = new SolidColorBrush(_colorGameGround);
                                _cells[y + i][x + n].IsActive = false;
                            }

                            x--;
                            j = -1;
                        }
                        else
                        {
                            if (fig.VerticalStruct[i][j] != null)
                            {
                                _cells[y + i][x + j].Grid.Background = fig.VerticalStruct[i][j].Background == new SolidColorBrush(_colorEmpty) ? new SolidColorBrush(_colorGameGround) : fig.VerticalStruct[i][j].Background;
                                _cells[y + i][x + j].IsActive = _cells[y + i][x + j].Grid.Background != new SolidColorBrush(_colorGameGround);
                            }
                        }
                    }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_currentFig != null)
            {

                switch (e.Key)
                {
                    case Key.Up:
                        {
                            bool isFree = true;
                            int n = _x - 1 < 0 ? 0 : 1;

                            if (_currentFig.IsHorizontal)
                                for (int i = 0; i < _currentFig.Height; i++)
                                {

                                    if (!_cells[_y + i][_x - n].IsFree && (_cells[_y + i][_x].Grid.Background as SolidColorBrush).Color != _colorGameGround)
                                    {
                                        isFree = false;
                                        break;
                                    }
                                }
                            else
                                for (int i = 0; i < _currentFig.Width; i++)
                                {
                                    if (!_cells[_y + i][_x - n].IsFree && (_cells[_y + i][_x].Grid.Background as SolidColorBrush).Color != _colorGameGround)
                                    {
                                        isFree = false;
                                        break;
                                    }
                                }

                            if (isFree)
                                DrawFigure(_currentFig, ref _x, ref _y, true);

                            break;
                        }
                    case Key.Down:
                        {
                            if (!IsFalled(_currentFig, _x, _y))
                            {
                                _y++;
                                DrawFigure(_currentFig, ref _x, ref _y);
                            }
                            break;
                        }
                    case Key.Left:
                        {
                            if (_x - 1 >= 0)
                            {
                                bool isFree = true;

                                if (_currentFig.IsHorizontal)
                                    for (int i = 0; i < _currentFig.Height; i++)
                                    {
                                        if (!_cells[_y + i][_x - 1].IsFree && (_cells[_y + i][_x].Grid.Background as SolidColorBrush).Color != _colorGameGround)
                                        {
                                            isFree = false;
                                            break;
                                        }
                                    }
                                else
                                    for (int i = 0; i < _currentFig.Width; i++)
                                    {
                                        if (!_cells[_y + i][_x - 1].IsFree && (_cells[_y + i][_x].Grid.Background as SolidColorBrush).Color != _colorGameGround)
                                        {
                                            isFree = false;
                                            break;
                                        }
                                    }

                                if (isFree)
                                {
                                    _x--;
                                    DrawFigure(_currentFig, ref _x, ref _y);
                                }
                            }
                            break;
                        }
                    case Key.Right:
                        {
                            if (_currentFig.IsHorizontal)
                            {
                                if (_x + _currentFig.Width < 10)
                                {
                                    int i = 0;
                                    for (; i < _currentFig.Height; i++)
                                        if (!_cells[_y + i][_x + _currentFig.Width].IsFree && (_cells[_y + i][_x + _currentFig.Width - 1].Grid.Background as SolidColorBrush).Color != _colorGameGround)
                                            break;

                                    if (i == _currentFig.Height)
                                    {
                                        _x++;
                                        DrawFigure(_currentFig, ref _x, ref _y);
                                    }
                                }
                            }
                            else if (_x + _currentFig.Height < 10)
                            {
                                int i = 0;
                                for (; i < _currentFig.Width; i++)
                                    if (!_cells[_y + i][_x + _currentFig.Height].IsFree && (_cells[_y + i][_x + _currentFig.Height - 1].Grid.Background as SolidColorBrush).Color != _colorGameGround)
                                        break;

                                if (i == _currentFig.Width)
                                {
                                    _x++;
                                    DrawFigure(_currentFig, ref _x, ref _y);
                                }
                            }

                            break;
                        }
                    case Key.Space:
                        {
                            while (!IsFalled(_currentFig, _x, _y))
                            {
                                _y++;
                                DrawFigure(_currentFig, ref _x, ref _y);
                            }

                            break;
                        }
                }
            }
        }
    }
}