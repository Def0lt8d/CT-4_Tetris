namespace Tetris
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Run();
        }
    }

    class Piece
    {
        public int[][] Shape { get; private set; }
        public int Row { get; set; }
        public int Col { get; set; }

        private static readonly List<int[][][]> Shapes = new List<int[][][]>
        {
            // I
            new int[][][] {
                new int[][] { new int[] {1,1,1,1} },
                new int[][] { new int[] {1}, new int[] {1}, new int[] {1}, new int[] {1} }
            },
            // O
            new int[][][] {
                new int[][] { new int[] {1,1}, new int[] {1,1} }
            },
            // T
            new int[][][] {
                new int[][] { new int[] {0,1,0}, new int[] {1,1,1} },
                new int[][] { new int[] {1,0}, new int[] {1,1}, new int[] {1,0} },
                new int[][] { new int[] {1,1,1}, new int[] {0,1,0} },
                new int[][] { new int[] {0,1}, new int[] {1,1}, new int[] {0,1} }
            },
            // L
            new int[][][] {
                new int[][] {
                    new int[] {1,0},
                    new int[] {1,0},
                    new int[] {1,1}
                },
                new int[][] {
                    new int[] {1,1,1},
                    new int[] {1,0,0}
                },
                new int[][] {
                    new int[] {1,1},
                    new int[] {0,1},
                    new int[] {0,1}
                },
                new int[][] {
                    new int[] {0,0,1},
                    new int[] {1,1,1}
                }
            },
            // J
            new int[][][] {
                new int[][] {
                    new int[] {0,1},
                    new int[] {0,1},
                    new int[] {1,1}
                },
                new int[][] {
                    new int[] {1,0,0},
                    new int[] {1,1,1}
                },
                new int[][] {
                    new int[] {1,1},
                    new int[] {1,0},
                    new int[] {1,0}
                },
                new int[][] {
                    new int[] {1,1,1},
                    new int[] {0,0,1}
                }
            },
            // S
            new int[][][] {
                new int[][] {
                    new int[] {0,1,1},
                    new int[] {1,1,0}
                },
                new int[][] {
                    new int[] {1,0},
                    new int[] {1,1},
                    new int[] {0,1}
                }
            },
            // Z
            new int[][][] {
                new int[][] {
                    new int[] {1,1,0},
                    new int[] {0,1,1}
                },
                new int[][] {
                    new int[] {0,1},
                    new int[] {1,1},
                    new int[] {1,0}
                }
            }
        };

        public Piece(int row, int col)
        {
            Random rand = new Random();
            int type = rand.Next(Shapes.Count);
            int rotation = rand.Next(Shapes[type].Length);

            Shape = CloneShape(Shapes[type][rotation]);
            Row = row;
            Col = col;
        }

        public Piece(Piece other)
        {
            Shape = CloneShape(other.Shape);
            Row = other.Row;
            Col = other.Col;
        }

        private int[][] CloneShape(int[][] source)
        {
            int[][] newShape = new int[source.Length][];
            for (int i = 0; i < source.Length; i++)
            {
                newShape[i] = new int[source[i].Length];
                Array.Copy(source[i], newShape[i], source[i].Length);
            }
            return newShape;
        }

        public void Rotate()
        {
            int[][] newShape = new int[Shape[0].Length][];
            for (int i = 0; i < newShape.Length; i++)
            {
                newShape[i] = new int[Shape.Length];
                for (int j = 0; j < Shape.Length; j++)
                {
                    newShape[i][j] = Shape[Shape.Length - j - 1][i];
                }
            }
            Shape = newShape;
        }
    }

    class Game
    {
        private const int Width = 10;
        private const int Height = 20;
        private int[,] field = new int[Height, Width];
        private Piece currentPiece;
        private bool gameOver = false;

        public Game()
        {
            currentPiece = new Piece(0, Width / 2 - 1);
        }

        private bool CanPlacePiece(Piece piece, int newRow, int newCol)
        {
            for (int i = 0; i < piece.Shape.Length; i++)
            {
                for (int j = 0; j < piece.Shape[i].Length; j++)
                {
                    if (piece.Shape[i][j] == 1)
                    {
                        int row = newRow + i;
                        int col = newCol + j;

                        if (col < 0 || col >= Width || row >= Height)
                            return false;

                        if (row >= 0 && field[row, col] == 1)
                            return false;
                    }
                }
            }
            return true;
        }

        private void MergePiece()
        {
            for (int i = 0; i < currentPiece.Shape.Length; i++)
            {
                for (int j = 0; j < currentPiece.Shape[i].Length; j++)
                {
                    if (currentPiece.Shape[i][j] == 1)
                    {
                        int row = currentPiece.Row + i;
                        int col = currentPiece.Col + j;
                        if (row >= 0) field[row, col] = 1;
                    }
                }
            }
        }

        private void CheckLines()
        {
            for (int row = Height - 1; row >= 0; row--)
            {
                bool full = true;
                for (int col = 0; col < Width; col++)
                {
                    if (field[row, col] == 0)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    for (int r = row; r > 0; r--)
                    {
                        for (int c = 0; c < Width; c++)
                        {
                            field[r, c] = field[r - 1, c];
                        }
                    }
                    row++;
                }
            }
        }

        private void DrawField()
        {
            Console.SetCursorPosition(0, 0);
            for (int row = 0; row < Height; row++)
            {
                Console.Write("│");
                for (int col = 0; col < Width; col++)
                {
                    bool isPiece = false;
                    if (currentPiece != null)
                    {
                        int pr = row - currentPiece.Row;
                        int pc = col - currentPiece.Col;
                        if (pr >= 0 && pr < currentPiece.Shape.Length &&
                            pc >= 0 && pc < currentPiece.Shape[pr].Length)
                        {
                            if (currentPiece.Shape[pr][pc] == 1)
                            {
                                Console.Write("██");
                                isPiece = true;
                            }
                        }
                    }

                    if (!isPiece)
                    {
                        Console.Write(field[row, col] == 1 ? "██" : "  ");
                    }
                }
                Console.WriteLine("│");
            }
            Console.WriteLine(new string('═', Width * 2 + 2));
        }

        public void Run()
        {
            Console.CursorVisible = false;
            DateTime lastFall = DateTime.Now;

            while (!gameOver)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.LeftArrow:
                            if (CanPlacePiece(currentPiece, currentPiece.Row, currentPiece.Col - 1))
                                currentPiece.Col--;
                            break;
                        case ConsoleKey.RightArrow:
                            if (CanPlacePiece(currentPiece, currentPiece.Row, currentPiece.Col + 1))
                                currentPiece.Col++;
                            break;
                        case ConsoleKey.DownArrow:
                            if (CanPlacePiece(currentPiece, currentPiece.Row + 1, currentPiece.Col))
                                currentPiece.Row++;
                            break;
                        case ConsoleKey.UpArrow:
                            Piece rotated = new Piece(currentPiece);
                            rotated.Rotate();
                            if (CanPlacePiece(rotated, rotated.Row, rotated.Col))
                                currentPiece.Rotate();
                            break;
                    }
                }

                if ((DateTime.Now - lastFall).TotalMilliseconds > 1000)
                {
                    if (CanPlacePiece(currentPiece, currentPiece.Row + 1, currentPiece.Col))
                    {
                        currentPiece.Row++;
                    }
                    else
                    {
                        MergePiece();
                        CheckLines();

                        currentPiece = new Piece(0, Width / 2 - 1);
                        if (!CanPlacePiece(currentPiece, currentPiece.Row, currentPiece.Col))
                        {
                            gameOver = true;
                        }
                    }
                    lastFall = DateTime.Now;
                }

                DrawField();
                Thread.Sleep(50);
            }

            Console.Clear();
            Console.WriteLine("Конец игры! Нажмите любую кнопку, чтобы начать заново...");
            Console.ReadKey();
            Console.Clear();
            field = new int[Height, Width];
            gameOver = false;
            Run();
        }
    }
}
