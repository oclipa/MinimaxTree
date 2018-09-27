using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// A configuration of the game "board"
/// </summary>
public class Configuration
{
    #region Fields

    List<int> bearCounts = new List<int>();
    List<int> nonEmptyBins = new List<int>();
    int totalBearCount;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="binContents">contents of each bin</param>
    public Configuration(List<int> binContents)
    {
        // copy bin contents into bins
        bearCounts.AddRange(binContents);

        for (int i = 0; i < bearCounts.Count; i++)
        {
            int bearCount = bearCounts[i];
            if (bearCount > 0)
            {
                nonEmptyBins.Add(bearCount);
                totalBearCount += bearCount;
            }
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a read-only list of the number of bears in each bin
    /// </summary>
    public IList<int> BearCounts
    {
        get { return bearCounts.AsReadOnly(); }
    }

    /// <summary>
    /// Gets whether all the bins in the configuration are empty
    /// </summary>
    public bool Empty
    {
        get
        {
            foreach (int bearCount in bearCounts)
            {
                if (bearCount > 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Gets a read-only list of the bin counts for the bins that aren't empty
    /// </summary>
    public IList<int> NonEmptyBins
    {
        get { return nonEmptyBins.AsReadOnly(); }
    }

    /// <summary>
    /// Gets the total number of bears across all bins
    /// </summary>
    public int TotalBearCount
    {
        get { return totalBearCount; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Converts the configuration to a string
    /// </summary>
    /// <returns>the string</returns>
    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("[Configuration: ");
        for (int i = 0; i < bearCounts.Count; i++)
        {
            builder.Append(bearCounts[i]);
            if (i < bearCounts.Count - 1)
            {
                builder.Append(" ");
            }
        }
        builder.Append("]");
        return builder.ToString();
    }

    #endregion
}
