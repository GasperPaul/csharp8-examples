using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

/// This example shows improvements over Example 1
/// Features not yet available in .NET Core 3 Preview 3 commented out

namespace CSharp8Examples.Example2
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
            => (Name?.GetHashCode()*17 + Age.GetHashCode()).GetValueOrDefault();
        
        #endregion
        
        // this is a proposed addition in C# 8 for 
        //     var s = s1 with { Faculty = "CS" }
        //public Student With(string Name = this.Name, int Age = this.Age, string Faculty = this.Faculty)
        //    => new Student(Name, Age, Faculty);
            
        public void Deconstruct(out string Name, out int Age, out string Faculty)
            => (Name, Age, Faculty) = (this.Name, this.Age, this.Faculty);
    };
    
    // this will replace the above class definition
    // class Student(string Name, int Age, string Faculty); 
    
    interface IStudentSource : IDisposable {
        IEnumerable<Student> GetStudents();
        IAsyncEnumerable<Student> GetStudentsAsync();
        
        // this is allowed and will be called on 
        // variables implicitly typed as this interface
        // if actual realization doesn't have appropriate members
        // public int Length() { return Count; } 
        // protected int Count { get; }
    }
    
    class Faculty : IStudentSource
    {
        // type of new expression can be inferred
        List<Student> Students { get; set; } = new /**/List<Student>/**/()
        {
            new Student("Adam", 17, "Math"),
            new Student("Beth", 18, "Math"),
            new Student("Carl", 19, "CS"),
            new Student("Dora", 20, "CS")
        };
        
        public void Dispose() { /*this is here as an example*/ }
        
        // IAsyncEnumerable allows combining enumeration and async
        public async IAsyncEnumerable<Student> GetStudentsAsync() {
            foreach (var student in Students) {
                await Task.Delay(200); // let's pretend we're doing work
                yield return student;
            }
        }
        
        public IEnumerable<Student> GetStudents() => Students;
        
        // new switch expression syntax with less clutter
        static string Display(Student student)
            => student switch
            {
                // new pattern-matching options
                Student { Name: var name, Faculty: "Math" } => $"Math student: {name}",
                Student { Name: var name } => $"Non-math student: {name}",
                _ => "Is this even a student?",
            };
        
        static async Task Main(string[] args)
        {
            using IStudentSource source = new Faculty();
            
            // new foreach statement for async enumerables
            await foreach (var student in source.GetStudentsAsync())
                Console.WriteLine(Display(student));
                
            Console.WriteLine();
            
            // new indexing, currently supported only on Arrays
            var lastStudent = source.GetStudents()?.OrderBy(x => x.Age).ToArray()[^1];
            Console.WriteLine($"Last student is {lastStudent.Name}");
        }
    }
}
