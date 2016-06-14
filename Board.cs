using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleApplication2
{
    [Serializable]
    class Board
    {
        public IDictionary<int, IList<Ball>> Rows;
        public IDictionary<int, IList<Ball>> Columns;
        public IList<string> FlingLevels;

        public Board()
        {
            Rows = new Dictionary<int, IList<Ball>>();
            Columns = new Dictionary<int, IList<Ball>>();
            FlingLevels = new List<string>();
        }

        public static T DeepClone<T>(T obj) // to avoid copy by reference
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public void AddInRowColumn(Ball ball, int index, bool isRow)
        {
            var entity = isRow ? Rows : Columns;

            if (!entity.ContainsKey(index))
            {
                entity[index] = new List<Ball>();
            }

            entity[index].Add(ball);

            if (entity[index].Count > 1)
            {
                var prevBall = entity[index][entity[index].Count - 2];
                var condition = isRow
                    ? ball.ColumnIndex - 1 != prevBall.ColumnIndex
                    : ball.RowIndex - 1 != prevBall.RowIndex;

                if (condition)
                {
                    prevBall.CurrentHit.Add(ball);
                    ball.CurrentHit.Add(prevBall);
                }
            }
        }

        private void RemoveBallInRow(Board newBoard, Ball ballT)
        {
            var found = false;
            for (int index = 0; !found && index < newBoard.Rows[ballT.RowIndex].Count; index++) // remove ballToHit from row
            {
                var ball = newBoard.Rows[ballT.RowIndex][index];
                if (ball.Id == ballT.Id)
                {
                    newBoard.Rows[ballT.RowIndex].RemoveAt(index);
                    found = true;
                }

                if (newBoard.Rows[ballT.RowIndex].Count == 0)
                {
                    newBoard.Rows.Remove(ballT.RowIndex);
                }
            }
        }

        private void RemoveBallInCol(Board newBoard, Ball ballT)
        {
            var flag = false;
            for (int index = 0; !flag && index < newBoard.Columns[ballT.ColumnIndex].Count; index++) // remove ballToHit from column
            {
                var ball = newBoard.Columns[ballT.ColumnIndex][index];
                if (ball.Id == ballT.Id)
                {
                    newBoard.Columns[ballT.ColumnIndex].RemoveAt(index);
                    flag = true;
                }

                if (newBoard.Columns[ballT.ColumnIndex].Count == 0)
                {
                    newBoard.Columns.Remove(ballT.ColumnIndex);
                }
            }
        }

        private void RemoveCurrentHitForBallInRow(Board newBoard, Ball ballT, bool updateFlingedBall, Ball ballToFling, Ball flingedBall)
        {
            for (int index = 0; index < newBoard.Rows[ballT.RowIndex].Count; index++) // remove ball from row whose current hit is ballToHit
            {
                var ball = newBoard.Rows[ballT.RowIndex][index];
                if (ball.CurrentHit.Count > 0)
                {
                    for (var idx = 0; idx < ball.CurrentHit.Count; idx++)
                    {
                        if (ball.CurrentHit[idx].Id == ballT.Id)
                        {
                            ball.CurrentHit.RemoveAt(idx);
                        }
                    }
                }
                if (updateFlingedBall && ball.Id == ballToFling.Id)
                {
                    newBoard.Rows[ballT.RowIndex][index] = flingedBall;
                }
            }
        }

        private void RemoveCurrentHitForBallInCol(Board newBoard, Ball ballT, bool updateFlingedBall, Ball ballToFling, Ball flingedBall)
        {
            for (int index = 0; index < newBoard.Columns[ballT.ColumnIndex].Count; index++) // remove currentHit of all balls whose currHit is ballToHit
            {
                var ball = newBoard.Columns[ballT.ColumnIndex][index];
                if (ball.CurrentHit.Count > 0)
                {
                    for (var idx = 0; idx < ball.CurrentHit.Count; idx++)
                    {
                        if (ball.CurrentHit[idx].Id == ballT.Id)
                        {
                            ball.CurrentHit.RemoveAt(idx);
                        }
                    }
                }
                if (updateFlingedBall && ball.Id == ballToFling.Id)
                {
                    newBoard.Columns[ballT.ColumnIndex][index] = flingedBall;
                }
            }
        }

        public Board ReInitializeBoard(Board board, Ball ballToFling, Ball ballToHit)
        {

            var newBoard = DeepClone(board);
            var flingedBall = DeepClone(ballToFling);
            var flingDirection = FlingDirection.Up;
            bool isNeighbour = false;
            var neighbour = DeepClone(ballToHit); ;

            flingedBall.CurrentHit = new List<Ball>();
            if (ballToFling.ColumnIndex == ballToHit.ColumnIndex)
            {
                flingDirection = FlingDirection.Up; // move up.
                if (ballToFling.RowIndex < ballToHit.RowIndex)
                {
                    flingDirection = FlingDirection.Down;//move down
                }

                switch (flingDirection)
                {
                    case FlingDirection.Up: //up
                        flingedBall.RowIndex = ballToHit.RowIndex + 1;
                        while (board.Rows.ContainsKey(ballToHit.RowIndex - 1))
                        {
                            if (board.Rows.ContainsKey(ballToHit.RowIndex - 1))// find if ballToHit has neighbour
                            {
                                var flag = false;
                                foreach (var ball in board.Rows[ballToHit.RowIndex - 1])
                                {
                                    if (ball.ColumnIndex == ballToHit.ColumnIndex)
                                    {
                                        ballToHit = ball;
                                        isNeighbour = true;
                                        flag = true;
                                    }
                                }
                                if (!flag)
                                {
                                    break;
                                }
                            }
                        }
                        break;

                    case FlingDirection.Down: //down
                        flingedBall.RowIndex = ballToHit.RowIndex - 1;
                        while (board.Rows.ContainsKey(ballToHit.RowIndex + 1))
                        {
                            if (board.Rows.ContainsKey(ballToHit.RowIndex + 1))// find if ballToHit has neighbour
                            {
                                var flag = false;
                                foreach (var ball in board.Rows[ballToHit.RowIndex + 1])
                                {
                                    if (ball.ColumnIndex == ballToHit.ColumnIndex)
                                    {
                                        ballToHit = ball;
                                        isNeighbour = true;
                                        flag = true;
                                    }
                                }
                                if (!flag)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                }

                RemoveBallInRow(newBoard, ballToHit);  // remove ballToHit from row

                if (newBoard.Rows.ContainsKey(ballToHit.RowIndex))
                {
                    RemoveCurrentHitForBallInRow(newBoard, ballToHit, false, null, null);
                }

                RemoveBallInCol(newBoard, ballToHit); // remove ballToHit from column

                if (newBoard.Columns.ContainsKey(ballToHit.ColumnIndex))
                {
                    RemoveCurrentHitForBallInCol(newBoard, ballToHit, true, ballToFling, flingedBall);
                }

                RemoveBallInRow(newBoard, ballToFling);// remove ballToFling in row

                if (newBoard.Rows.ContainsKey(ballToFling.RowIndex))
                {
                    RemoveCurrentHitForBallInRow(newBoard, ballToFling, false, null, null);
                }


                if (!newBoard.Rows.ContainsKey(flingedBall.RowIndex))
                    newBoard.Rows[flingedBall.RowIndex] = new List<Ball>();

                newBoard.Rows[flingedBall.RowIndex].Add(flingedBall);
            }

            else if (ballToFling.RowIndex == ballToHit.RowIndex)
            {
                flingDirection = FlingDirection.Left;
                if (ballToFling.ColumnIndex < ballToHit.ColumnIndex)
                {
                    flingDirection = FlingDirection.Right;
                }

                switch (flingDirection)
                {
                    case FlingDirection.Left://LEFT
                        flingedBall.ColumnIndex = ballToHit.ColumnIndex + 1;
                        while (board.Columns.ContainsKey(ballToHit.ColumnIndex - 1))
                        {
                            if (board.Columns.ContainsKey(ballToHit.ColumnIndex - 1))// find if ballToHit has neighbour
                            {
                                var flag = false;
                                foreach (var ball in board.Columns[ballToHit.ColumnIndex - 1])
                                {
                                    if (ball.RowIndex == ballToHit.RowIndex)
                                    {
                                        ballToHit = ball;
                                        isNeighbour = true;
                                        flag = true;
                                    }
                                }
                                if (!flag)
                                {
                                    break;
                                }
                            }
                        }
                        
                        break;

                    case FlingDirection.Right://RIGHT
                        flingedBall.ColumnIndex = ballToHit.ColumnIndex - 1;
                        while (board.Columns.ContainsKey(ballToHit.ColumnIndex + 1))
                        {
                            if (board.Columns.ContainsKey(ballToHit.ColumnIndex + 1))// find if ballToHit has neighbour
                            {
                                var flag = false;
                                foreach (var ball in board.Columns[ballToHit.ColumnIndex + 1])
                                {
                                    if (ball.RowIndex == ballToHit.RowIndex)
                                    {
                                        ballToHit = ball;
                                        isNeighbour = true;
                                        flag = true;
                                    }
                                }
                                if (!flag)
                                {
                                    break;
                                }
                            }
                        }
                        
                        break;
                }

                RemoveBallInCol(newBoard, ballToHit); // remove ballToHit in column

                if (newBoard.Columns.ContainsKey(ballToHit.ColumnIndex))
                {
                    RemoveCurrentHitForBallInCol(newBoard, ballToHit, false, null, null);// remove ball from column whose currentHit is ballToHit
                }

                RemoveBallInRow(newBoard, ballToHit); // remove ballToHit  in row

                if (newBoard.Rows.ContainsKey(ballToHit.RowIndex))
                {
                    RemoveCurrentHitForBallInRow(newBoard, ballToHit, true, ballToFling, flingedBall); //remove ball in row whose current hit is ballToHit and update ballToFling in row
                }

                /***********Remove all ball whose currentHit is ballToFling in column********/
                if (newBoard.Columns[ballToFling.ColumnIndex].Count > 0)
                {
                    RemoveCurrentHitForBallInCol(newBoard, ballToFling, false, null, null);
                }

                RemoveBallInCol(newBoard, ballToFling);

                if (!newBoard.Columns.ContainsKey(flingedBall.ColumnIndex))
                    newBoard.Columns[flingedBall.ColumnIndex] = new List<Ball>();

                newBoard.Columns[flingedBall.ColumnIndex].Add(flingedBall);
            }

            if (isNeighbour)
            {
                for (var i = 0; i < newBoard.Rows[neighbour.RowIndex].Count; i++)
                {
                    if (newBoard.Rows[neighbour.RowIndex][i].Id == neighbour.Id)
                    {
                        if (newBoard.Rows[neighbour.RowIndex][i].CurrentHit.Count > 0)
                        {
                            for (int j = 0; j < newBoard.Rows[neighbour.RowIndex][i].CurrentHit.Count; j++)
                            {
                                if (newBoard.Rows[neighbour.RowIndex][i].CurrentHit[j].Id == ballToFling.Id)
                                {
                                    newBoard.Rows[neighbour.RowIndex][i].CurrentHit.RemoveAt(j);
                                }
                            }
                        }
                    }
                }

                for (var i = 0; i < newBoard.Columns[neighbour.ColumnIndex].Count; i++)
                {
                    if (newBoard.Columns[neighbour.ColumnIndex][i].Id == neighbour.Id)
                    {
                        if (newBoard.Columns[neighbour.ColumnIndex][i].CurrentHit.Count > 0)
                        {
                            for (int j = 0; j < newBoard.Columns[neighbour.ColumnIndex][i].CurrentHit.Count; j++)
                            {
                                if (newBoard.Columns[neighbour.ColumnIndex][i].CurrentHit[j].Id == ballToFling.Id)
                                {
                                    newBoard.Columns[neighbour.ColumnIndex][i].CurrentHit.RemoveAt(j);
                                }
                            }
                        }
                    }
                }
            }

            newBoard.FlingLevels.Add(string.Format("Fling! {0} {1}", ballToFling.Id, Enum.GetName(flingDirection.GetType(), flingDirection)));

            //after fling
            //If same row has other balls in column
            if (newBoard.Rows[flingedBall.RowIndex].Count > 1)
            {
                newBoard.Rows[flingedBall.RowIndex] =
                    newBoard.Rows[flingedBall.RowIndex].OrderBy(x => x.ColumnIndex).ToList();
                var flingIndex = -1;
                for (var i = 0; flingIndex == -1 && i < newBoard.Rows[flingedBall.RowIndex].Count; i++)
                {
                    if (newBoard.Rows[flingedBall.RowIndex][i].Id == flingedBall.Id)
                    {
                        flingIndex = i;
                    }
                }
                if (flingIndex - 1 > -1)
                {
                    var prevBall = newBoard.Rows[flingedBall.RowIndex].ElementAt(flingIndex - 1);
                    if (prevBall != null)
                    {
                        prevBall.CurrentHit = prevBall.CurrentHit.OrderBy(x => x.ColumnIndex).ToList();
                        for (var idx = 0; idx < prevBall.CurrentHit.Count; idx++)
                        {
                            if (prevBall.CurrentHit[idx].ColumnIndex >= flingedBall.ColumnIndex) // if flinged ball lands in between prev ball's currhit and prev ball
                            {
                                prevBall.CurrentHit.RemoveAt(idx);
                            }
                        }

                        if (prevBall.ColumnIndex + 1 != flingedBall.ColumnIndex)
                        {
                            prevBall.CurrentHit.Add(flingedBall);
                            flingedBall.CurrentHit.Add(prevBall);
                        }
                    }
                }

                if (flingIndex + 1 < newBoard.Rows[flingedBall.RowIndex].Count)
                {
                    var nextBall = newBoard.Rows[flingedBall.RowIndex].ElementAt(flingIndex + 1);
                    if (nextBall != null)
                    {
                        nextBall.CurrentHit = nextBall.CurrentHit.OrderBy(x => x.ColumnIndex).ToList();
                        for (var idx = 0; idx < nextBall.CurrentHit.Count; idx++)
                        {
                            if (nextBall.CurrentHit[idx].ColumnIndex <= flingedBall.ColumnIndex)
                            {
                                nextBall.CurrentHit.RemoveAt(idx);
                            }
                        }

                        if (nextBall.ColumnIndex - 1 != flingedBall.ColumnIndex)
                        {
                            nextBall.CurrentHit.Add(flingedBall);
                            flingedBall.CurrentHit.Add(nextBall);
                        }

                    }
                }
            }

            //If same column has other balls in row
            if (newBoard.Columns[flingedBall.ColumnIndex].Count > 1)
            {
                newBoard.Columns[flingedBall.ColumnIndex] =
                    newBoard.Columns[flingedBall.ColumnIndex].OrderBy(x => x.RowIndex).ToList();
                var flingIndex = -1;
                for (var i = 0; flingIndex == -1 && i < newBoard.Columns[flingedBall.ColumnIndex].Count; i++)
                {
                    if (newBoard.Columns[flingedBall.ColumnIndex][i].Id == flingedBall.Id)
                    {
                        flingIndex = i;
                    }
                }

                if (flingIndex - 1 > -1)
                {
                    var prevBall = newBoard.Columns[flingedBall.ColumnIndex].ElementAt(flingIndex - 1);
                    if (prevBall != null)
                    {
                        prevBall.CurrentHit = prevBall.CurrentHit.OrderBy(x => x.RowIndex).ToList();
                        for (var idx = 0; idx < prevBall.CurrentHit.Count; idx++)
                        {
                            if (prevBall.CurrentHit[idx].RowIndex >= flingedBall.RowIndex)
                            {
                                prevBall.CurrentHit.RemoveAt(idx);
                            }
                        }

                        if (prevBall.RowIndex + 1 != flingedBall.RowIndex)
                        {
                            prevBall.CurrentHit.Add(flingedBall);
                            flingedBall.CurrentHit.Add(prevBall);
                        }
                    }
                }


                if (flingIndex + 1 < newBoard.Columns[flingedBall.ColumnIndex].Count)
                {
                    var nextBall = newBoard.Columns[flingedBall.ColumnIndex].ElementAt(flingIndex + 1);
                    if (nextBall != null)
                    {
                        nextBall.CurrentHit = nextBall.CurrentHit.OrderBy(x => x.RowIndex).ToList();
                        for (var idx = 0; idx < nextBall.CurrentHit.Count; idx++)
                        {
                            if (nextBall.CurrentHit[idx].RowIndex <= flingedBall.RowIndex)
                            {
                                nextBall.CurrentHit.RemoveAt(idx);
                            }
                        }

                        if (nextBall.RowIndex - 1 != flingedBall.RowIndex)
                        {
                            nextBall.CurrentHit.Add(flingedBall);
                            flingedBall.CurrentHit.Add(nextBall);
                        }

                    }
                }
            }

            return newBoard;
        }

        public void Fling(Board board, List<IList<string>> solutions)
        {
            if (board.Rows.Count == 1 && board.Columns.Count == 1)
            {
                solutions.Add(board.FlingLevels);
            }

            foreach (var row in board.Rows)
            {
                for (var i = 0; i < row.Value.Count; i++)
                {
                    if (row.Value[i].CurrentHit.Count > 0)
                    {
                        foreach (var ballToHit in row.Value[i].CurrentHit)
                        {
                            var newBoard = ReInitializeBoard(board, row.Value[i], ballToHit);
                            Fling(newBoard, solutions);
                        }
                    }

                }
            }

        }

    }

    enum FlingDirection
    {
        Up,
        Down,
        Right,
        Left
    }
}
