using System;
using System.Linq;

namespace OrbItProcs {
  //event, 
  //delegate, 
  //linq, 
  //bitflag, if, for, dictionary, dynamic, generic, inheritance, reflection, property


  //throw an exceptional party
  public class Puzzle<T> where T : Puzzle<T> {
    [Flags]
    public enum pieces {
      Switch = 4,
      Lever = 1,
      Button = 2
    };

    public Puzzle(pieces a) {
      Type t = typeof (pieces);
      var l = ((pieces[]) Enum.GetValues(t))
        .Where(e => ((pieces) a).HasFlag(e))
        .Select(e => (int) e);
      attempt += i => i/2.0;
      dynamic result = 0;
      foreach (int i in l)
        result += attempt.Invoke(i);
      win = result == 3.0 ? true : false;
      if (win) throw new Party();
    }

    public bool win { get; private set; }
    event Func<double, double> attempt;

    private class Party : Exception {}
  }


  class LeetnessTest {
    //Are you leet enough to throw a party?
    //LeetAttempt s =  new LeetAttempt(/*answer goes here*/);
  }

  class LeetAttempt : Puzzle<LeetAttempt> {
    public LeetAttempt(pieces p) : base(p) {}
  }
}