using System;
using System.Collections.Generic;

namespace ConsoleApplication2
{
    [Serializable]
    class Ball
    {
        public string Id;
        public IList<Ball> CurrentHit;
        public int RowIndex;
        public int ColumnIndex;

        public Ball()
        {
            CurrentHit = new List<Ball>();
        }
        public Ball(string id, int row, int column)
        {
            Id = id;
            RowIndex = row;
            ColumnIndex = column;
            CurrentHit = new List<Ball>();
        }
    }

}
