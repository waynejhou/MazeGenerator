using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MazeGenerator
{
    partial class Maze
    {
        MyGridSize mazeSize;
        MazeBlocks mazeBlocks;
        int Rows { get { return mazeSize.Rows; } }
        int Cols { get { return mazeSize.Cols; } }
        MyBorder StartBlock;
        List<MyBorder> ListFrontier;
        Grid mazeGrid;
        Random random = new Random();
        bool isMaked = false;
        private Brush EnterColor = Brushes.Red;
        private Brush AnsColor = Brushes.Aqua;
        private Brush EndColor = Brushes.Orange;
        private Brush FrontColor = Brushes.LightGray;
        public Maze(int rows, int cols, Grid grid)
        {
            mazeSize.Rows = rows;
            mazeSize.Cols = cols;
            mazeGrid = grid;
            Reset();
        }
        public void Reset()
        {
            isMaked = false;
            mazeBlocks = new MazeBlocks(mazeSize);
            StartBlock = mazeBlocks.GetBorder(RandomPick<MyGridPoint>.Pick(new List<MyGridPoint>
                {
                    new MyGridPoint(0,(new Random().Next()%(mazeSize.Cols/2))),
                    new MyGridPoint(0,(new Random().Next()%(mazeSize.Rows/2)))
                }));
            StartBlock.mazeMark = MazeMark.Maze;
            ListFrontier = new List<MyBorder>();
            ListFrontier.AddRange(mazeBlocks.GetMarkedNeightborPoint(StartBlock.point, MazeMark.Map));
        }
        public void SetSize(int num)
        {
            mazeSize.Rows = num;
            mazeSize.Cols = num;
        }
        public void StepProcess()
        {
            if (ListFrontier.Count != 0)
            {
                isMaked = false;
                var picked_Frontier = RandomPick<MyBorder>.Pick(ListFrontier);
                picked_Frontier.mazeMark = MazeMark.Maze;
                var next_Block = RandomPick<MyBorder>.Pick(mazeBlocks.GetMarkedNeightborPoint(picked_Frontier.point, MazeMark.Maze));
                mazeBlocks.SetWallThinkness(picked_Frontier, next_Block);
                ListFrontier.Remove(picked_Frontier);
                ListFrontier.AddRange(mazeBlocks.GetMarkedNeightborPoint(picked_Frontier.point, MazeMark.Map).FindAll(x => !ListFrontier.Contains(x)));
            }
            else
            {
                isMaked = true;
            }
        }
        public void MakeMaze()
        {
            mazeGrid.Children.Clear();
            mazeGrid.RowDefinitions.Clear();
            mazeGrid.ColumnDefinitions.Clear();
            mazeGrid.UpdateLayout();
            var minLength = Math.Min(mazeGrid.ActualHeight, mazeGrid.ActualWidth);
            for (int i = 0; i < mazeSize.Rows; i++)
                mazeGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(minLength / mazeSize.Rows, GridUnitType.Pixel) });
            for (int j = 0; j < mazeSize.Cols; j++)
                mazeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(minLength / mazeSize.Cols, GridUnitType.Pixel) });
            for (int i = 0; i < mazeSize.Rows; i++)
                for (int j = 0; j < mazeSize.Cols; j++)
                {
                    mazeBlocks.GetBorder(i, j).BeginInit();
                    mazeBlocks.GetBorder(i, j).BorderThickness = mazeBlocks.GetBorder(i, j).WallThickness;
                    mazeBlocks.GetBorder(i, j).EndInit();
                    Grid.SetRow(mazeBlocks.GetBorder(i, j), i);
                    Grid.SetColumn(mazeBlocks.GetBorder(i, j), j);
                    mazeGrid.Children.Add(mazeBlocks.GetBorder(i, j));
                }
            mazeGrid.UpdateLayout();
        }
        public void Process()
        {
            while (ListFrontier.Count != 0)
                StepProcess();
        }
        public void FindPath()
        {
            if (isMaked)
            {
                var EntryPoint = RandomPick<MyGridPoint>.Pick(new List<MyGridPoint>{
                        new MyGridPoint(random.Next() % (Rows / 2), 0),
                        new MyGridPoint(0, random.Next() % (Cols / 2))
                    });
                PathBlock RootBlock = new PathBlock { Data = mazeBlocks.GetBorder(EntryPoint) };


            }
        }
    }
    partial class Maze
    {
        struct MyGridPoint
        {
            public int Row;
            public int Col;
            public MyGridPoint(int row, int col)
            {
                Row = row;
                Col = col;
            }
        }
        struct MyGridSize
        {
            public int Rows;
            public int Cols;
        }
        public enum MazeMark { None = 0, Map, Maze };
        class MyBorder : Border
        {
            public MyGridPoint point;
            static Brush MapColor = Brushes.Gray;
            static Brush MazeColor = Brushes.White;
            public MazeMark _mazeMark = MazeMark.None;
            public MazeMark mazeMark
            {
                get { return _mazeMark; }
                set
                {
                    _mazeMark = value;
                    switch (_mazeMark)
                    {
                        case MazeMark.None:
                            break;
                        case MazeMark.Map:
                            this.Background = MapColor;
                            break;
                        case MazeMark.Maze:
                            this.Background = MazeColor;
                            break;
                        default:
                            break;
                    }
                }
            }
            public Thickness WallThickness = new Thickness(1, 1, 1, 1);
            public MyBorder(MyGridPoint myGridPoint)
            {
                point = myGridPoint;
                BorderThickness = WallThickness;
                mazeMark = MazeMark.Map;
            }
        }
        class MazeBlocks
        {
            MyBorder[,] MyBorder;
            MyGridSize size;
            int WallThick = 1;
            public MazeBlocks(MyGridSize myGridSize)
            {
                this.MyBorder = new MyBorder[myGridSize.Rows, myGridSize.Cols];
                this.size = myGridSize;
                for (int i = 0; i < size.Rows; i++)
                    for (int j = 0; j < size.Cols; j++)
                    {
                        MyBorder[i, j] = new MyBorder(new MyGridPoint(i, j));
                        MyBorder[i, j].BeginInit();
                        MyBorder[i, j].BorderBrush = Brushes.Black;
                        MyBorder[i, j].BorderThickness = new Thickness(WallThick, WallThick, WallThick, WallThick);
                        MyBorder[i, j].EndInit();
                    }

            }
            public MyBorder GetBorder(MyGridPoint myGridPoint) => MyBorder[myGridPoint.Row, myGridPoint.Col];
            public MyBorder GetBorder(int row, int col) => MyBorder[row, col];
            public List<MyBorder> GetMarkedNeightborPoint(MyGridPoint myGridPoint, MazeMark mazeMark)
            {
                List<MyBorder> list = new List<MyBorder>();
                if (myGridPoint.Col - 1 >= 0)
                    if (MyBorder[myGridPoint.Row, myGridPoint.Col - 1].mazeMark == mazeMark)
                        list.Add(MyBorder[myGridPoint.Row, myGridPoint.Col - 1]);
                if (myGridPoint.Row - 1 >= 0)
                    if (MyBorder[myGridPoint.Row - 1, myGridPoint.Col].mazeMark == mazeMark)
                        list.Add(MyBorder[myGridPoint.Row - 1, myGridPoint.Col]);
                if (myGridPoint.Col + 1 < size.Cols)
                    if (MyBorder[myGridPoint.Row, myGridPoint.Col + 1].mazeMark == mazeMark)
                        list.Add(MyBorder[myGridPoint.Row, myGridPoint.Col + 1]);
                if (myGridPoint.Row + 1 < size.Rows)
                    if (MyBorder[myGridPoint.Row + 1, myGridPoint.Col].mazeMark == mazeMark)
                        list.Add(MyBorder[myGridPoint.Row + 1, myGridPoint.Col]);
                return list;
            }
            public void SetWallThinkness(MyBorder A, MyBorder B)
            {
                int r = A.point.Row - B.point.Row;
                int c = A.point.Col - B.point.Col;
                if (r == 0 && c > 0)
                { A.WallThickness.Left = 0; B.WallThickness.Right = 0; }
                else if (r > 0 && c == 0)
                { A.WallThickness.Top = 0; B.WallThickness.Bottom = 0; }
                else if (r == 0 && c < 0)
                { A.WallThickness.Right = 0; B.WallThickness.Left = 0; }
                else if (r < 0 && c == 0)
                { A.WallThickness.Bottom = 0; B.WallThickness.Top = 0; }
            }
        }
        class PathBlock
        {
            public MyBorder Data;
            public List<PathBlock> Children;
        }
        class RandomPick<T>
        {
            private List<T> Objs;
            public RandomPick(T[] items)
            {
                Objs = new List<T>(items);
            }
            public T Pick(bool del_picked = false)
            {
                if (Objs.Count > 1)
                {
                    var picked = Objs[(new Random()).Next() % Objs.Count];
                    if (del_picked)
                        Objs.Remove(picked);
                    return picked;
                }
                else if (Objs.Count == 1)
                    return Objs[0];
                else
                    return default(T);
            }
            public T Pick(int seed, bool del_picked = false)
            {
                if (Objs.Count > 1)
                {
                    var picked = Objs[(new Random(seed)).Next() % Objs.Count];
                    if (del_picked)
                        Objs.Remove(picked);
                    return picked;
                }
                else if (Objs.Count == 1)
                    return Objs[0];
                else
                    return default(T);
            }
            public void Shuffle()
            {
                List<T> newone = new List<T>();
                while (Objs.Count != 0)
                    newone.Add(this.Pick(true));
                Objs = newone;
            }
            public static T Pick(List<T> items, bool del_picked = false)
            {
                if (items.Count > 1)
                {
                    var picked = items[(new Random()).Next() % items.Count];
                    if (del_picked)
                        items.Remove(picked);
                    return picked;
                }
                else if (items.Count == 1)
                    return items[0];
                else
                    return default(T);
            }
            public static T Pick(List<T> items, int seed, bool del_picked = false)
            {
                if (items.Count > 1)
                {
                    var picked = items[(new Random(seed)).Next() % items.Count];
                    if (del_picked)
                        items.Remove(picked);
                    return picked;
                }
                else if (items.Count == 1)
                    return items[0];
                else
                    return default(T);
            }
            public static void Shuffle(ref List<T> items)
            {
                List<T> oldone = new List<T>(items);
                List<T> newone = new List<T>();
                while (oldone.Count != 0)
                    newone.Add(Pick(oldone, true));
                items = newone;
            }
        }
        class PathNode
    }
}