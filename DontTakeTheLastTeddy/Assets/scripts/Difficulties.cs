using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulties
{
    private Difficulty difficultyP1;
    private Difficulty difficultyP2;

    public Difficulties(Difficulty difficultyP1, Difficulty difficultyP2)
    {
        this.difficultyP1 = difficultyP1;
        this.difficultyP2 = difficultyP2;
    }

    //public int CompareTo(object obj)
    //{
    //    if (obj is Difficulties)
    //    {
    //        if (((Difficulties)obj).DifficultyP1 == difficultyP1
    //            && ((Difficulties)obj).DifficultyP2 == difficultyP2)
    //            return 0;
    //        else
    //            return -1;
    //    }
    //    else
    //    {
    //        return -1;
    //    }
    //}

    public Difficulty DifficultyP1 { get { return this.difficultyP1; } }
    public Difficulty DifficultyP2 { get { return this.difficultyP2; } }

    public override string ToString()
    {
        return difficultyP1 + ", " + difficultyP2;
    }
}

public class DifficultiesUniqueIdEqualityComparer : IEqualityComparer<Difficulties>
{
    public bool Equals(Difficulties a, Difficulties b)
    {
        return a.DifficultyP1 == b.DifficultyP1 && a.DifficultyP2 == b.DifficultyP2;
    }

    public int GetHashCode(Difficulties a)
    {
        int hashCode = 17;

        hashCode = (hashCode * 23) + a.DifficultyP1.GetHashCode() ^ +a.DifficultyP1.GetHashCode();

        return hashCode;
    }
}