using CommandLine;
using System.Diagnostics;

namespace HoopSortSolver
{
    internal class Program
    {
        public class Options
        {
            [Option(Default = true, HelpText = "The game file to parse.")]
            public string GameFile { get; set; } = "";
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions);
        }
        static void RunOptions(Options options)
        {
            try
            {
                var game = GameState.Load(options.GameFile);
                Console.WriteLine("Loaded game state");
                Console.WriteLine(game.ToGameString());
                Console.WriteLine($"Current score is {game.Score()}");

                // Solve the puzzle
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var steps = Solver(game);
                sw.Stop();
                if (steps.Count == 0)
                {
                    Console.WriteLine("This game cannot be solved.");
                }
                else
                {
                    Console.WriteLine($"Solved in {sw.ElapsedMilliseconds}ms.");
                    Console.WriteLine($"It takes {steps.Count} moves to win the game:");
                    foreach (var step in steps)
                    {
                        Console.WriteLine($" - {step.Move}");
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine($"Failure: {e.Message}");
            }
        }

        /// <summary>
        /// This is an implementation of A* that uses a priority queue and a distance function for game states.
        /// 
        /// Potential improvements to the solver:
        ///  * Detect when one game state gets to the same position as another in fewer moves and reset its children
        ///  * Reduce the number of unnecessary allocations, maybe convert to structs
        ///  * Instead of using strings for colors use integers
        ///  * Figure out a better score calculation algorithm
        ///  * Avoid making a possible move that doesn't lift the entire vertical stack
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        static List<PotentialMove> Solver(GameState game)
        {
            var queue = new PriorityQueue<PotentialMove, int>();
            var pastGameStates = new HashSet<string>
            {
                game.ToGameString()
            };
            int numberOfMovesConsidered = 0;
            int lastScoreMessagePrinted = 0;

            // Set up initial conditions
            foreach (var move in game.ListAvailableMoves())
            {
                numberOfMovesConsidered++;
                queue.Enqueue(move, move.Score);
            }

            // Go through the queue
            while (queue.Count > 0)
            {
                // Remove top item from queue
                var current = queue.Dequeue();
                if (numberOfMovesConsidered - lastScoreMessagePrinted >= 1000)
                {
                    Console.WriteLine($"Best score is {current.Score} in {current.NewState.MovesTaken} moves (examined {numberOfMovesConsidered}, {queue.Count} in queue)");
                    lastScoreMessagePrinted += 1000;
                }

                // Examine all child moves only if we haven't seen this state before
                var serializedString = current.NewState.ToHashString();
                if (!pastGameStates.Contains(serializedString))
                {
                    pastGameStates.Add(serializedString);
                    var newMoves = current.NewState.ListAvailableMoves();
                    foreach (var move in newMoves)
                    {
                        numberOfMovesConsidered++;
                        if (move.NewState.IsWin())
                        {
                            Console.WriteLine($"Found a solution after examining {numberOfMovesConsidered} possible moves.");

                            // Compute the list of moves to get to the win
                            var movesToWin = new List<PotentialMove>();
                            var thisMove = move;
                            while (thisMove != null)
                            {
                                movesToWin.Insert(0, thisMove);
                                thisMove = thisMove.OldState.LastMove;
                            }
                            return movesToWin;
                        }
                        else
                        {
                            queue.Enqueue(move, move.Score);
                        }
                    }
                }
            }

            Console.WriteLine("Didn't find any wins");
            return new List<PotentialMove>();
        }
    }
}