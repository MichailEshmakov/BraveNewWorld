using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BraveNewWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            int playerJumpHeight = 3;
            int fallingPeriod = 200;
            char playerSymbol = '@';

            string mapName = "map";
            int playerPositionX;
            int playerPositionY;
            char[,] map = LoadMap(mapName, out playerPositionX, out playerPositionY, playerSymbol);
            DrawMap(map);

            bool isPlay = true;
            Console.SetCursorPosition(playerPositionY, playerPositionX);
            Console.CursorVisible = false;

            Task falling = Task.Run(() =>
            {
                while (isPlay)
                {
                    Thread.Sleep(fallingPeriod);
                    Move("down", map, ref playerPositionX, ref playerPositionY, playerSymbol, ref isPlay);
                }
            });

            while (isPlay)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.A:
                        Move("left", map, ref playerPositionX, ref playerPositionY, playerSymbol, ref isPlay);
                        break;
                    case ConsoleKey.D:
                        Move("right", map, ref playerPositionX, ref playerPositionY, playerSymbol, ref isPlay);
                        break;
                    case ConsoleKey.Spacebar:
                        Jump(map, ref playerPositionX, ref playerPositionY, playerSymbol, playerJumpHeight, ref isPlay);
                        break;
                }
            }

            Console.SetCursorPosition(0, map.GetLength(0) + 1);
            Console.WriteLine("Вы победили!");
        }

        static char[,] LoadMap(string mapName, out int playerPositionX, out int playerPositionY, char playerSymbol)
        {
            playerPositionX = 0;
            playerPositionY = 0;
            string[] strings = File.ReadAllLines($"../../../maps/{mapName}.txt");
            char[,] map = new char[strings.Length, strings[0].Length];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = strings[i][j];
                    if (map[i, j] == playerSymbol)
                    {
                        playerPositionX = i;
                        playerPositionY = j;
                    }
                }
            }

            return map;
        }

        static void DrawMap(char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.Write(map[i, j]);
                }

                Console.WriteLine();
            }
        }

        static void Jump(char[,] map, ref int currentPositionX, ref int currentPositionY, char jumperSymbol, int playerJumpHeight, ref bool isPlay)
        {
            if (map[currentPositionX + 1, currentPositionY] == '#')
            {
                for (int i = 0; i < playerJumpHeight; i++)
                {
                    Move("up", map, ref currentPositionX, ref currentPositionY, jumperSymbol, ref isPlay);
                }
            }
        }

        static void Move(string direction, char[,] map, ref int currentPositionX, ref int currentPositionY, char moverSymbol, ref bool isPlay)
        {
            int newPositionX = currentPositionX;
            int newPositionY = currentPositionY;

            switch (direction)
            {
                case "up":
                    newPositionX--;
                    break;
                case "down":
                    newPositionX++;
                    break;
                case "left":
                    newPositionY--;
                    break;
                case "right":
                    newPositionY++;
                    break;
            }

            if (map[newPositionX, newPositionY] != '#')
            {
                if (map[newPositionX, newPositionY] == '!')
                {
                    isPlay = false;
                }

                RewriteSymbol(currentPositionX, currentPositionY, ' ', map);
                currentPositionX = newPositionX;
                currentPositionY = newPositionY;
                RewriteSymbol(currentPositionX, currentPositionY, moverSymbol, map);
            }
        }

        static void RewriteSymbol(int positionX, int positionY, char symbol, char[,] map)
        {
            Console.SetCursorPosition(positionY, positionX);
            Console.Write(symbol);
            map[positionX, positionY] = symbol;
        }
    }
}
