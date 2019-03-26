using System;

/// This example shows some problems with pattern-matching and deconstruction syntax.
/// Type declarations and var are handled differently in pattern-matching and deconstruction,
/// discard symbols are higly context-dependent.

namespace CSharp8Examples.Example3
{
    class Pair 
    {   
        object X { get; set; }
        object Y { get; set; }
        
        public Pair(object x, object y) => (X, Y) = (x, y);
        public void Deconstruct(out object X, out object Y) => (X, Y) = (this.X, this.Y);
    }
    
    class Example
    {
        static int Add(Pair p)
        {
            // behaviour of this pattern-matching is non-intuitive
            // due to context-dependent discard
            // const int x = 10;
            // int _ = 0;
            // if (p is Pair(x, _)) return x + y;
            
            // deconstructing assignment 
            // will fail at compile-time if underlaying object is not int
            // (int x, int y) = p;
            
            // but:
            //     if (p is Pair(int x, int y)) return x + y;
            // is not deconstruction, but a pattern-match
            // that will fail in run-time
            
            // while
            if (p is Pair(var x, var y)) return x + y;
            // is deconstruction into variables
            // and will generate compile-time errors
            // due to type mismatch for operator +
            
            return 0;
        }
        
        static void Main(string[] args)
        {
            //var x = new Pair(2,3);
            var x = new Pair("A", "B");
            
            var res = Add(x);
            Console.WriteLine(res);
        }
    }
}
