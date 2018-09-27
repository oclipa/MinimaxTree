using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A player
/// </summary>
public class Player : MonoBehaviour
{
    // the players name
    PlayerName myName;

    // controls the time available for thinking about the next move
    Timer thinkingTimer;

    // minimax search support
    Difficulty difficulty;
    int searchDepth = 0;
    MinimaxTree<Configuration> tree;

    // events invoked by class
    TurnOver turnOverEvent = new TurnOver();

    // saved for efficiency
    LinkedList<MinimaxTreeNode<Configuration>> nodeList =
        new LinkedList<MinimaxTreeNode<Configuration>>();
    List<int> currentBearCounts = new List<int>();
    List<Configuration> newConfigurations =
        new List<Configuration>();

    /// <summary>
    /// Awake is called before Start
    /// </summary>
    void Awake()
	{
        // set name
		if (CompareTag("Player1"))
        {
            myName = PlayerName.Player1;
        }
        else
        {
            myName = PlayerName.Player2;
        }

        // add timer component
        thinkingTimer = gameObject.AddComponent<Timer>();
        thinkingTimer.Duration = GameConstants.AiThinkSeconds;
        thinkingTimer.AddTimerFinishedListener(HandleThinkingTimerFinished);

        // register as invoker and listener
        EventManager.AddTurnOverInvoker(this);
        EventManager.AddTakeTurnListener(HandleTakeTurnEvent);
	}

    /// <summary>
    /// Gets and sets the difficulty for the player
    /// </summary>
    public Difficulty Difficulty
    {
        get { return difficulty; }
        set
        {
            difficulty = value;
            switch (difficulty)
            {
                case Difficulty.Easy:
                    searchDepth = GameConstants.EasyMinimaxDepth;
                    break;
                case Difficulty.Medium:
                    searchDepth = GameConstants.MediumMinimaxDepth;
                    break;
                case Difficulty.Hard:
                    searchDepth = GameConstants.HardMinimaxDepth;
                    break;
                default:
                    searchDepth = GameConstants.EasyMinimaxDepth;
                    break;
            }
        }
    }

    /// <summary>
    /// Adds the given listener for the TurnOver event
    /// </summary>
    /// <param name="listener">listener</param>
    public void AddTurnOverListener(
        UnityAction<PlayerName, Configuration> listener)
    {
        turnOverEvent.AddListener(listener);
    }

    /// <summary>
    /// Handles the TakeTurn event
    /// </summary>
    /// <param name="player">whose turn it is</param>
    /// <param name="boardConfiguration">current board configuration</param>
    void HandleTakeTurnEvent(PlayerName player,
        Configuration boardConfiguration)
    {
        // only take turn if it's our turn
        if (player == myName)
        {
            tree = BuildTree(boardConfiguration);
            thinkingTimer.Run();
        }
    }

    /// <summary>
    /// Builds the tree
    /// </summary>
    /// <param name="boardConfiguration">current board configuration</param>
    /// <returns>tree</returns>
    MinimaxTree<Configuration> BuildTree(
        Configuration boardConfiguration)
    {
        // build tree to appropriate depth
        MinimaxTree<Configuration> tree =
            new MinimaxTree<Configuration>(boardConfiguration);

        // clear the list of nodes
        nodeList.Clear();

        // add the root as the initial node
        nodeList.AddLast(tree.Root);

        // iterate over all nodes while there are nodes to process
        while (nodeList.Count > 0)
        {
            // get the currentNode
            MinimaxTreeNode<Configuration> currentNode =
                nodeList.First.Value;

            // remove it from the list of nodes (to avoid processing it again)
            nodeList.RemoveFirst();

            // Get all possible configurations that might result from
            // from the one for the current node (as long as they 
            // contain bears).
            // These configurations represent possible child nodes of this node.
            List<Configuration> possibleConfigurations =
                GetNextConfigurations(currentNode.Value);

            // get the depth of this node
            int childDepth = getChildDepth(currentNode);

            // if we are still within the search depth
            if (childDepth <= searchDepth)
            {
                // iterate over all possible configurations where 
                // bins contain bears
                foreach (Configuration configuration in possibleConfigurations)
                {
                    // create a new child node with this configuration
                    MinimaxTreeNode<Configuration> childNode =
                        new MinimaxTreeNode<Configuration>(
                            configuration, currentNode);

                    // add the child node to the tree
                    tree.AddNode(childNode);

                    // and add it to the node list
                    nodeList.AddLast(childNode);
                }
            }
        }
        return tree;
    }

    /// <summary>
    /// Gets the child depth.
    /// </summary>
    /// <returns>The child depth.</returns>
    /// <param name="currentNode">Current node.</param>
    private int getChildDepth(MinimaxTreeNode<Configuration> currentNode)
    {
        // get child depth by counting number of ancestors (0 = root)
        int childDepth = 0;
        MinimaxTreeNode<Configuration> parentNode = currentNode.Parent;
        while(parentNode != null)
        {
            parentNode = parentNode.Parent;
            childDepth++;
        }
        return childDepth;
    }

    /// <summary>
    /// Handles the thinking timer finishing
    /// </summary>
    void HandleThinkingTimerFinished()
    {
        // Timer has finished so now the player needs to pick the
        // best child node

        // do the search and pick the move
        Minimax(tree.Root, true);

        // now we are looking for the configuration
        // that has the maximum score

        // find child node with maximum score
        // get all immediate children of root
        IList<MinimaxTreeNode<Configuration>> children =
            tree.Root.Children;
        // pick a random child (e.g. first)
        MinimaxTreeNode<Configuration> maxChildNode = children[0];
        // iterate over children to find one with largest minimax score
        for (int i = 1; i < children.Count; i++)
        {
            if (children[i].MinimaxScore > maxChildNode.MinimaxScore)
            {
                maxChildNode = children[i];
            }
        }

        // chosen child is probably the best node to pick

        // provide new configuration (obtained from child node)
        // as second argument
        turnOverEvent.Invoke(myName, maxChildNode.Value);
    }

    /// <summary>
    /// Gets a list of the possible next configurations
    /// given the current configuration
    /// </summary>
    /// <param name="currentConfiguration">current configuration</param>
    /// <returns>list of next configurations</returns>
    List<Configuration> GetNextConfigurations(
        Configuration currentConfiguration)
    {
        // currentConfiguration is the configuration for the child 
        // that was chosen during the previous turn.
        // Based on that choice, we now want to know all possible 
        // configurations that might result from this configuration so we 
        // can present these to the player for this turn.

        // remove all existing configurations
        newConfigurations.Clear();

        // get the bear counts for the current configuration
        IList<int> bearCounts = currentConfiguration.BearCounts;

        // iterate over the bearCounts for every bin
        for (int i = 0; i < bearCounts.Count; i++)
        {
            // for this bin...

            // get the number of bears in this bin
            int currentBearCount = bearCounts[i];

            // If there is 1 or more bears, create a configuration for each
            // possible number of bears.
            // If there are no bears in this bin, do not create a configuration.
            while (currentBearCount > 0)
            {
                // remove one teddy from this bin
                currentBearCount--;

                // clear the current list of bear counts
                currentBearCounts.Clear();
                // now add all the bearCounts into the current list
                currentBearCounts.AddRange(bearCounts);
                // Now set the new bear count for this bin
                currentBearCounts[i] = currentBearCount;

                // add new next configuration to list
                newConfigurations.Add(
                    new Configuration(currentBearCounts));
            }
        }

        // this list contains every possible configuration for
        // every bin in the board that contains bears
        return newConfigurations;
    }

    /// <summary>
    /// Assigns minimax scores to the tree nodes
    /// </summary>
    /// <param name="tree">tree to mark with scores</param>
    /// <param name="maximizing">whether or not we're maximizing</param>
    void Minimax(MinimaxTreeNode<Configuration> currentNode,
        bool maximizing)
    {
        // get all immediate children for this node
        IList<MinimaxTreeNode<Configuration>> children = currentNode.Children;
        if (children.Count > 0) // all child nodes have bears.
        {
            // do minimax for children of this node
            foreach (MinimaxTreeNode<Configuration> child in children)
            {
                // toggle maximizing as we move down
                Minimax(child, !maximizing);
            }

            // set default node minimax score
            if (maximizing)
            {
                currentNode.MinimaxScore = int.MinValue;
            }
            else
            {
                currentNode.MinimaxScore = int.MaxValue;
            }

            // find maximum or minimum value in children
            foreach (MinimaxTreeNode<Configuration> child in children)
            {
                if (maximizing)
                {
                    // check for higher minimax score
                    // if child has higher minimax score than this node
                    if (child.MinimaxScore > currentNode.MinimaxScore) 
                    {
                        // set minimax score for this node to match child 
                        // minimax score
                        currentNode.MinimaxScore = child.MinimaxScore;
                    }
                }
                else
                {
                    // minimizing, check for lower minimax score
                    // if child has lower minimax score than this node
                    if (child.MinimaxScore < currentNode.MinimaxScore) 
                    {
                        // set minimax score for this node to match child 
                        // minimax score
                        currentNode.MinimaxScore = child.MinimaxScore; 
                    }
                }
            }
        }
        else 
        {
            // Reaching this point means that we have encountered 
            // either an end game config (no bears) or the maximum
            // search depth.

            AssignHeuristicMinimaxScore(currentNode, maximizing);
        }
    }
        
    /// <summary>
    /// Assigns a heuristic minimax score to the given node
    /// </summary>
    /// <param name="node">node to mark with score</param>
    /// <param name="maximizing">whether or not we're maximizing</param>
    void AssignHeuristicMinimaxScore(
        MinimaxTreeNode<Configuration> node,
        bool maximizing)
    {
        // At this point, the current node has no children that 
        // contain bears, meaning that, whatever the next move is,
        // the player of that move will lose.

        // might have reached an end-of-game configuration
        if (node.Value.Empty)
        {
            AssignEndOfGameMinimaxScore(node, maximizing);
        }
        else // this was a node at the max search depth
        {
            // If we have reached this point, we have reached a node
            // where we don't know what comes next (we didn't search
            // that far when building the tree), so we have to assign
            // a score based on the current situation (as it appears
            // to this node).  This is termed a "heuristic evaluation".

            // If there are only two bears left across all bins, this
            // is good for the current player since the next player 
            // must lose on the next turn.
            if (node.Value.TotalBearCount == 2)
            {
                // score depends on whether player is minimizing or maximising
                node.MinimaxScore = maximizing ? 1 : 0;
            }
            else 
            {
                // we split the difference, since we don't know if this config
                // is good or bad for either player
                node.MinimaxScore = 0.5f;
            }
        }
    }

    /// <summary>
    /// Assigns a heuristic minimax score to the given node
    /// </summary>
    /// <param name="node">node to mark with score</param>
    /// <param name="maximizing">whether or not we're maximizing</param>
    void AssignHeuristicMinimaxScore2(
        MinimaxTreeNode<Configuration> node,
        bool maximizing)
    {
        // might have reached an end-of-game configuration
        if (node.Value.Empty)
        {
            AssignEndOfGameMinimaxScore(node, maximizing);
        }
        else
        {
            // use a heuristic evaluation function to score the node

            int fullBins = node.Value.NonEmptyBins.Count;
            List<int> teddysPerBinFull;

            switch (fullBins)
            {
                case 1:
                    // if we only have 1 bin with bears
                    // check if the total number of bears in greater than 1
                    if (node.Value.TotalBearCount > 1)
                        node.MinimaxScore = 1f;
                    else
                    {
                        node.MinimaxScore = 0;
                    }
                    break;

                case 2:
                    // if we have 2 bins with bears
                    // get a list of how many bears are in each bin.
                    // The list is sorted from smallest number of bears
                    // to larget.
                    teddysPerBinFull = CountTeddysPerBinFull(node);

                    // check if smallest number of bears in a bin is 1
                    if (teddysPerBinFull[0] == 1)
                        node.MinimaxScore = 1;
                    else
                    {
                        node.MinimaxScore = 0.5f;
                    }
                    break;

                case 3:
                    // if we have 3 bins with bears
                    // get a list of how many bears are in each bin.
                    // The list is sorted from smallest number of bears
                    // to larget.                    
                    teddysPerBinFull = CountTeddysPerBinFull(node);

                    // check if all 3 bins only have 1 bear
                    if (teddysPerBinFull[0] == 1 && teddysPerBinFull[1] == 1 && teddysPerBinFull[2] == 1)
                        node.MinimaxScore = 1;
                    else
                    {
                        node.MinimaxScore = 0.5f;
                    }
                    break;

                default:
                    // get a list of how many bears are in each bin.
                    // The list is sorted from smallest number of bears
                    // to larget.                    
                    teddysPerBinFull = CountTeddysPerBinFull(node);

                    // check if the 3 least populated bins all have 1 bear
                    if (teddysPerBinFull[0] == 1 && teddysPerBinFull[1] == 1 && teddysPerBinFull[2] == 1 && teddysPerBinFull[3] != 1)
                        node.MinimaxScore = 1;
                    else
                    {
                        node.MinimaxScore = 0.5f;
                    }
                    break;


            }

            //node.MinimaxScore = 0.5f;
            if (!maximizing)
                node.MinimaxScore = 1 - node.MinimaxScore;
        }
    }

    private List<int> CountTeddysPerBinFull(MinimaxTreeNode<Configuration> node)
    {
        List<int> teddysPerBin = new List<int>();

        foreach (int bin in node.Value.NonEmptyBins)
            teddysPerBin.Add(bin);

        teddysPerBin.Sort();

        return teddysPerBin;
    }



    /// <summary>
    /// Assigns a heuristic minimax score to the given node
    /// </summary>
    /// <param name="node">node to mark with score</param>
    /// <param name="maximizing">whether or not we're maximizing</param>
    void AssignHeuristicMinimaxScore3(
        MinimaxTreeNode<Configuration> node,
        bool maximizing)
    {
        // might have reached an end-of-game configuration
        if (node.Value.Empty)
        {
            AssignEndOfGameMinimaxScore(node, maximizing);
        }
        else
        {
            // use a heuristic evaluation function to score the node


            // check for the bin counts that aren't empty
            // A higher score is assigned the greater the amount
            // of teddys on the number of non-empty bins,
            // since there are more possibilities for play.

            // returns a total count of the bins thar are no empty
            int binsCount = node.Value.NonEmptyBins.Count;

            // returns a total number of teddys bears
            int teddysAmount = node.Value.TotalBearCount;

            // low score case
            if (teddysAmount < binsCount) { node.MinimaxScore = 5; }

            // middle-low score case
            else if (teddysAmount == binsCount) { node.MinimaxScore = 10; }

            // middle score case
            else if (teddysAmount > binsCount && teddysAmount <= (binsCount * 2)) { node.MinimaxScore = 20; }

            // middle-max score case
            else if (teddysAmount > (binsCount * 2) && teddysAmount < (binsCount * 4)) { node.MinimaxScore = 30; }

            // max score case
            else if (teddysAmount == (binsCount * 4)) { node.MinimaxScore = 40; }

            else node.MinimaxScore = 0.5f;
        }
    }

    /// <summary>
    /// Assigns a heuristic minimax score to the given node
    /// </summary>
    /// <param name="node">node to mark with score</param>
    /// <param name="maximizing">whether or not we're maximizing</param>
    void AssignHeuristicMinimaxScore4(
    MinimaxTreeNode<Configuration> node,
    bool maximizing)
    {
        // might have reached an end-of-game configuration
        if (node.Value.Empty)
        {
            AssignEndOfGameMinimaxScore(node, maximizing);
        }
        else
        {
            // Default
            node.MinimaxScore = 0.5f;

            // use a heuristic evaluation function to score the node
            // Find cases where the player should win
            // Player should win if there's only two bears left (if maximizing)
            if (node.Value.TotalBearCount % 2 == 1)
            {
                if (maximizing)
                {
                    node.MinimaxScore = 0.75f;
                }
                else
                {
                    node.MinimaxScore = 0.25f;
                }
            }
            // Avoid loosing... perhaps
            // Give this a very low score, to avoid losing
            else if (node.Value.TotalBearCount == 1)
            {
                if (maximizing)
                {
                    node.MinimaxScore = 0;
                }
                else
                {
                    node.MinimaxScore = 1;
                }
            }
        }
    }

    /// <summary>
    /// Assigns the end of game minimax score
    /// </summary>
    /// <param name="node">node to mark with score</param> 
    /// <param name="maximizing">whether or not we're maximizing</param>
    void AssignEndOfGameMinimaxScore(MinimaxTreeNode<Configuration> node,
        bool maximizing)
    {
        if (maximizing)
        {
            // other player took the last teddy
            node.MinimaxScore = 1;
        }
        else
        {
            // we took the last teddy
            node.MinimaxScore = 0;
        }
    }
}
