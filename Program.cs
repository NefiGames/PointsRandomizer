using System;
using CsvHelper;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PointsRandomizer
{
    class Program
    {
        public static void WriteConsole(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color; // устанавливаем цвет
            Console.WriteLine(text);
            Console.ResetColor(); // сбрасываем в стандартный
        }
        public static List<T> GetTable<T>(string path)
        {
            List<T> result = new List<T>();
            using (StreamReader sr = new StreamReader(path))
            {
                using (CsvReader csvReader = new CsvReader(sr, System.Globalization.CultureInfo.CurrentCulture))
                {
                    csvReader.Configuration.Delimiter = ",";
                    IEnumerable<T> records = csvReader.GetRecords<T>();
                    foreach (var r in records)
                    {
                        result.Add(r);
                    }
                    return result;
                }
            }
        }
        public static void OutputTable(string path, List<Result> results)
        {
            using (StreamWriter streamReader = new StreamWriter(path))
            {
                using (CsvWriter csvReader = new CsvWriter(streamReader, System.Globalization.CultureInfo.CurrentCulture))
                {
                    csvReader.Configuration.Delimiter = ",";
                    csvReader.WriteRecords(results);
                }
            }
        }
        static void Main()
        {
            WriteConsole("          ВЫДАЧА ПРИЗОВ", ConsoleColor.Cyan);
            List<Result> results = new List<Result>();
            string inputFilesDirectory = Directory.GetCurrentDirectory() + "\\" + "Input Files Here";
            Directory.CreateDirectory(inputFilesDirectory);
            string pathOutput = Directory.GetCurrentDirectory() + "\\" + "output\\";
            WriteConsole("\n Вы хотите изменить директорию таблиц? (" + inputFilesDirectory + ")");
            WriteConsole("Y - YES                   N - NO");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                WriteConsole("\nВведите новую директорию: ");
                inputFilesDirectory = Console.ReadLine();
            }
            DirectoryInfo inputDyrectoryInfo = new DirectoryInfo(inputFilesDirectory);
            FileInfo[] files = inputDyrectoryInfo.GetFiles("*.csv");
            List<Teachers> teachers = new List<Teachers>();
            List<Students> students = new List<Students>();
            foreach (var file in files)
            {
                using StreamReader sr = new StreamReader(file.FullName);
                string columnsLine = sr.ReadLine();
                sr.Close();
                if (columnsLine.Contains("Преподаватель") && columnsLine.Contains("Предмет") && columnsLine.Contains("Группа") && columnsLine.Contains("Количество"))
                {
                    teachers = GetTable<Teachers>(file.FullName);
                }
                else if (columnsLine.Contains("Группа") && columnsLine.Contains("ФИО") && columnsLine.Contains("Номер в списке"))
                {
                    var currentStudents = GetTable<Students>(file.FullName);
                    foreach (var student in currentStudents)
                    {
                        students.Add(student);
                    }
                }
            }
            foreach (var teacher in teachers)
            {
                List<Students> neededStudents = new List<Students>();
                foreach (var student in students.Where(student => student.Group == teacher.Group))
                {
                    neededStudents.Add(student);
                }
                if (neededStudents.Count > 0)
                {
                    if (teacher.Count.Contains("по"))
                    {
                        string[] words = teacher.Count.Split(new char[] { ' ' }); // countPeople набора по countPoints балов
                        short countPeople = short.Parse(words[0]);
                        short countPoints = short.Parse(words[3]);
                        for (int i = 0; i < countPeople; i++)
                        {
                            if (neededStudents.Count > 0)
                            {
                                var result = new Result(teacher.Teacher, teacher.Subject, teacher.Group);
                                result.Prize = countPoints + " " + words[4];
                                int index = new Random().Next(0, neededStudents.Count - 1);
                                result.FullName = neededStudents[index].FullName;
                                var newStudents = students.Where(student => student.FullName != neededStudents[index].FullName);
                                results.Add(result);
                                neededStudents.Remove(neededStudents[index]);
                            }
                            else WriteConsole("Не хватает студентов. Не выдан набор по " + countPoints + " баллов у " + teacher.Teacher, ConsoleColor.Yellow);
                        }
                    }
                    else if (teacher.Count.Contains("зачёт") || teacher.Count.Contains("зачет"))
                    {
                        string[] words = teacher.Count.Split(new char[] { ' ' }); // countZachot зачет
                        short countZachot = short.Parse(words[0]);
                        for (int i = 0; i < countZachot; i++)
                        {
                            if (neededStudents.Count > 0)
                            {
                                var result = new Result(teacher.Teacher, teacher.Subject, teacher.Group);
                                result.Prize = "зачёт";
                                int index = new Random().Next(0, neededStudents.Count - 1);
                                result.FullName = neededStudents[index].FullName;
                                var newStudents = students.Where(student => student.FullName != neededStudents[index].FullName);
                                results.Add(result);
                                neededStudents.Remove(neededStudents[index]);
                            }
                            else WriteConsole("Не хватает студентов. Не выдан зачёт у " + teacher.Teacher, ConsoleColor.Yellow);
                        }
                    }
                }
            }
            var _onGroupSortedResults = from result in results orderby result.Group select result;
            var _onTeachersSortedResults = from result in results orderby result.Teacher select result;
            List<Result> onGroupSortedResults = new List<Result>();
            List<Result> onTeachersSortedResults = new List<Result>();
            foreach (var result in _onGroupSortedResults)
            {
                onGroupSortedResults.Add(result);
            }
            foreach (var result in _onTeachersSortedResults)
            {
                onTeachersSortedResults.Add(result);
            }
            if (onGroupSortedResults.Count > 0)
            {
                try
                {
                    Directory.Delete(pathOutput, true);
                    WriteConsole("Файлы перезаписаны в папке \"output\"", ConsoleColor.Green);
                }
                catch (Exception ex) { }
                Directory.CreateDirectory(pathOutput);
                OutputTable(pathOutput + "OnGroup.csv", onGroupSortedResults);
                OutputTable(pathOutput + "OnTeachers.csv", onTeachersSortedResults);
            }
            else
            {
                WriteConsole("Файлы не найдены", ConsoleColor.Red);
            }
            Console.Write("\nНажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
