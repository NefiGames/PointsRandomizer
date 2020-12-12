using System;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

public class Teachers
{
    [Name("Преподаватель")]
    public string Teacher { get; set; }

    [Name("Предмет")]
    public string Subject { get; set; }

    [Name("Группа")]
    public string Group { get; set; }

    [Name("Количество")]
    public string Count { get; set; }
}
public class Students
{
    [Name("Группа")]
    public string Group { get; set; }

    [Name("ФИО")]
    public string FullName { get; set; }

    [Name("Номер в списке")]
    public string Number { get; set; }
}
public class Result
{
    [Name("Преподаватель")]
    public string Teacher { get; set; }

    [Name("Предмет")]
    public string Subject { get; set; }

    [Name("Группа")]
    public string Group { get; set; }

    [Name("Имя студента")]
    public string FullName { get; set; }

    [Name("Приз")]
    public string Prize { get; set; }

    public Result(string _teacher, string _subject, string _group)
    {
        Teacher = _teacher;
        Subject = _subject;
        Group = _group;
    }
}