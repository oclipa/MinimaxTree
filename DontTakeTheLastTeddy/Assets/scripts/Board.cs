using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The game board (bins)
/// </summary>
public class Board : MonoBehaviour
{
    [SerializeField]
    GameObject prefabBin;

    List<Bin> bins = new List<Bin>();
    Configuration configuration;

    // saved for efficiency
    float binWidth;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start()
	{
        // bin width may already be set
        if (binWidth == 0)
        {
            SetBinWidth();
        }
	}
	
    #region Properties

    /// <summary>
    /// Gets and sets the board configuration
    /// </summary>
    /// <value>board configuration</value>
    public Configuration Configuration
    {
        get { return configuration; }
        set
        { 
            configuration = value; 
            SetBearCounts(configuration.BearCounts);
        }
    }

    #endregion

    /// <summary>
    /// Creates a new board
    /// </summary>
    public void CreateNewBoard()
    {
        // destroy existing board
        for (int i = bins.Count - 1; i >= 0; i--)
        {
            Destroy(bins[i].gameObject);
        }
        bins.Clear();

        // bin width may not be set yet
        if (binWidth == 0)
        {
            SetBinWidth();
        }

        // Randomly pick the number of bins
        int binCount = Random.Range(GameConstants.MinBins, 
                                    GameConstants.MaxBins + 1);

        // center the bins properly
        float binX = transform.position.x - 
                              ((binCount - 1) * 
                               binWidth / 2); 

        // create the bins
        for (int i = 0; i < binCount; i++)
        {
            GameObject binObject = Instantiate<GameObject>(prefabBin,
                transform.position, Quaternion.identity);
            Bin bin = binObject.GetComponent<Bin>();
            bin.X = binX;
            bins.Add(bin);
            binX += binWidth; 
        }

        // Randomly pick the number of bears in each bin
        List<int> bearCounts = new List<int>();
        for (int i = 1; i <= binCount; i++)
        {
            int bearsPerBin = Random.Range(1, GameConstants.MaxBearsPerBin + 1);
            bearCounts.Add(bearsPerBin);
        }

        // create the new configuration
        configuration = new Configuration(bearCounts);

        // set the number of bears in each bin
        SetBearCounts(bearCounts);
    }

    /// <summary>
    /// Sets the bin width
    /// </summary>
    void SetBinWidth()
    {
        // cache bin width
        GameObject tempBinObject = Instantiate<GameObject>(prefabBin);
        Bin tempBin = tempBinObject.GetComponent<Bin>();
        binWidth = tempBin.Width;
        Destroy(tempBinObject);
    }

    /// <summary>
    /// Sets the bear counts for the board
    /// </summary>
    /// <param name="bearCounts">bear contents</param>
    void SetBearCounts(IList<int> bearCounts)
    {
        for (int i = 0; i < bins.Count; i++)
        {
            // set the number of bears in this bin
            bins[i].Count = bearCounts[i]; 
        }
    }
}
