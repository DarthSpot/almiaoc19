﻿using System.IO;
using System.Text;

namespace AoCCore
{
    public class AoCHelper
    {
        private static readonly string _path = @"E:\Projekte\Github\almiaoc19\AoCCore\Input";
        public static string ReadFile(int num)
        {
            return File.ReadAllText(_path + $@"\aoc{num}.txt");
        }

        public static string[] ReadFileArr(int num)
        {
            return File.ReadAllLines(_path + $@"\aoc{num}.txt");
        }
    }
}