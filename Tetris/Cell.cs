using System.Windows.Controls;

namespace Tetris
{
    class Cell
    {
        private bool _isFree = true, _isAcrive = false;
        private Grid _grid = new Grid();

        public Grid Grid
        {
            get { return _grid; }
        }
        public bool IsFree
        {
            get { return _isFree; }
            set { _isFree = value; }
        }
        public bool IsActive
        {
            get { return _isAcrive; }
            set { _isAcrive = value; }
        }
    }
}