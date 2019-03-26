using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

/// This example shows a C#7 compliant code 
/// that can be improved in C#8

namespace CSharp8Examples.Example1
{
    class Student : IEquatable<Student>
    { 
        public string Name { get; set; } 
        public int Age { get; set; }
        public string Faculty { get; set; }
        
        public Student(string name, int age, string faculty) 
            => (Name, Age, Faculty) = (name, age, faculty);
        
        #region Default implementation
        
        public bool Equals(Student other) 
            => other != null && Equals(other.Name, Name) 
                && Equals(other.Age, Age) && Equals(other.Faculty, Faculty);
        
        public override bool Equals(object other)
            => this.Equals(other as Student);
        
        public override int GetHashCode()
            => (Name?.GetHashCode()*17 + Age.GetHashCode()).GetValueOrDefault(); // something like that
            
        #endregion
            
        public void Deconstruct(out string Name, out int Age, out string Faculty)
            => (Name, Age, Faculty) = (this.Name, this.Age, this.Faculty);
    };
    
    interface IStudentSource : IDisposable {
        IEnumerable<Student> GetStudents();
        Task<IEnumerable<Student>> GetStudentsAsync();
    }
    
    class Faculty : IStudentSource
    {
        List<Student> Students { get; set; } = new List<Student>()
        {
            new Student("Adam", 17, "Math"),
            new Student("Beth", 18, "Math"),
            new Student("Carl", 19, "CS"),
            new Student("Dora", 20, "CS")
        };
        
        public IEnumerable<Student> GetStudents() {
            foreach (var student in Students) {
                Thread.Sleep(200); // let's pretend we're doing work
                yield return student;
            }
        }
        
        public async Task<IEnumerable<Student>> GetStudentsAsync()
            => await Task.Run(GetStudents);
            
        public void Dispose() { /* this is here for demonstration */ }
        
        static string Display(Student student)
        {
            switch (student)
            {
                case Student { Name: var name, Faculty: "Math" }:
                    return $"Math student: {name}";
                case Student { Name: var name }:
                    return $"Non-math student: {name}";
                default:
                    return "Is this even a student?";
            }
        }
        
        static async Task Main(string[] args)
        {
            using (IStudentSource source = new Faculty())
            {
                foreach (var student in await source.GetStudentsAsync())
                    Console.WriteLine(Display(student));
                    
                Console.WriteLine();
                
                var lastStudent = source.GetStudents().OrderBy(x => x.Age).Last();
                Console.WriteLine($"Last student is {lastStudent.Name}");
            }
        }
    }
}
