using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    public enum Position
    {
        One, Two, Three, Four
    }

    class Figure
    {

        private Position _pos = Position.One;

        private int _w, _h, _rotatesCount = 0;

        private bool _isHorizontal = true;

        private Grid[][] _figHorizontal;
        private Grid[][] _figVertical;

        private int[][] _cellsLineCordinatsHorizontal;
        private int[][] _cellsLineCordinatsVertical;
        private Color[] _colorsException;

        public bool IsHorizontal
        {
            get { return _isHorizontal; }
        }

        public Grid[][] HorizontalStruct
        {
            get { return _figHorizontal; }
        }

        public Grid[][] VerticalStruct
        {
            get { return _figVertical; }
        }

        public int Width
        {
            get { return _w; }
        }

        public int Height
        {
            get { return _h; }
        }

        public Position RotatePosition
        {
            get { return _pos; }
        }

        public Figure(int w, int h, int[][] cellsLineCoordinats, Color[] colorsException)
        {
            _figHorizontal = new Grid[h][];
            _figVertical = new Grid[w][];

            _w = w;
            _h = h;

            _cellsLineCordinatsHorizontal = new int[cellsLineCoordinats.Length][];

            for (int i = 0; i < _cellsLineCordinatsHorizontal.Length; i++)
                _cellsLineCordinatsHorizontal[i] = new int[cellsLineCoordinats[i].Length];

            cellsLineCoordinats.CopyTo(_cellsLineCordinatsHorizontal, 0);

            _cellsLineCordinatsVertical = new int[w][];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (_cellsLineCordinatsVertical[i] == null)
                        _cellsLineCordinatsVertical[i] = new int[h];

                    _cellsLineCordinatsVertical[i][j] = _cellsLineCordinatsHorizontal[j][i];
                }
            }

            _colorsException = new Color[colorsException.Length];
            colorsException.CopyTo(_colorsException, 0);

            int k;
            Random r = new Random();
            Color c;

            for (int i = 0; i < h; i++)
            {
                _figHorizontal[i] = new Grid[w];

                for (int j = 0; j < cellsLineCoordinats[i].Length; j++)
                    if (cellsLineCoordinats[i][j] != -1)
                    {
                        do
                        {
                            c = Color.FromRgb(byte.Parse(r.Next(1, 255).ToString()), byte.Parse(r.Next(1, 150).ToString()), byte.Parse(r.Next(1, 150).ToString()));

                            for (k = 0; k < colorsException.Length; k++)
                                if (c == colorsException[k])
                                    break;
                        }
                        while (k < colorsException.Length);

                        _figHorizontal[i][cellsLineCoordinats[i][j]] = new Grid();
                        _figHorizontal[i][cellsLineCoordinats[i][j]].Background = new SolidColorBrush(c);
                    }
            }

            for (int i = 0; i < w; i++)
            {
                _figVertical[i] = new Grid[h];

                for (int j = 0; j < h; j++)
                    _figVertical[i][j] = _figHorizontal[j][i];
            }
        }

        public Figure Copy()
        {
            Figure fig = new Figure(_w, _h, _cellsLineCordinatsHorizontal, _colorsException);
            fig._isHorizontal = true;

            return fig;
        }

        public void Rotate()
        {
            _isHorizontal = !_isHorizontal;

            switch (_pos)
            {
                case Position.One:
                    {
                        if (_rotatesCount > 0)
                            Array.Reverse(_figVertical);
                        foreach (Grid[] line in _figVertical)
                            Array.Reverse(line);
                        _pos = Position.Two;
                        break;
                    }
                case Position.Two:
                    {
                        Array.Reverse(_figHorizontal);
                        foreach (Grid[] line in _figHorizontal)
                            Array.Reverse(line);
                        _pos = Position.Three;
                        break;
                    }
                case Position.Three:
                    {
                        Array.Reverse(_figVertical);
                        foreach (Grid[] line in _figVertical)
                            Array.Reverse(line);
                        _pos = Position.Four;
                        break;
                    }
                case Position.Four:
                    {
                        Array.Reverse(_figHorizontal);
                        foreach (Grid[] line in _figHorizontal)
                            Array.Reverse(line);
                        _rotatesCount++;
                        _pos = Position.One;
                        break;
                    }
            }
        }
    }
}