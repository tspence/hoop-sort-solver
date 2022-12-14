using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;

namespace HoopSortSolver {

    public class GameState {
        public int MaxPoleHeight { get; set; }
        public List<PoleState> Poles { get; set; } = new List<PoleState>();
        public PotentialMove? LastMove { get; set; } = null;
        public int MovesTaken { get; set; } = 0;
        public bool IsWin() 
        {
            return !((from pole in Poles where !pole.IsEmpty() && !pole.IsFinished(this) select pole).Any());
        }

        public int Score()
        {
            int score = MovesTaken;

            // We count the number of "trapped" hoops as our score
            foreach (var pole in Poles)
            {
                var topColor = pole.TopColor();
                foreach (var color in pole.Hoops) { 
                    if (color != topColor) score += 5;
                }
            }
            return score;
        }

        public List<PotentialMove> ListAvailableMoves()
        {
            var AvailableSpaces = new List<Tuple<int, string>>();
            var Moves = new List<PotentialMove>();

            // Only need to keep track of one empty hoop at most
            int EmptyHoop = -1;

            // Determine which poles can accept what hoops
            for (int i = 0; i < Poles.Count; i++)
            {
                if (Poles[i].IsEmpty())
                {
                    EmptyHoop = i;
                } 
                else if (Poles[i].Hoops.Count != MaxPoleHeight)
                {
                    var color = Poles[i].TopColor();
                    if (color != null) {
                        AvailableSpaces.Add(new Tuple<int, string>(i, color));
                    }
                }
            }

            // Figure out what we can move
            for (int i = 0; i < Poles.Count; i++)
            {
                if (!Poles[i].IsEmpty())
                {
                    var color = Poles[i].TopColor();
                    if (EmptyHoop > -1) { 
                        Moves.Add(GenerateMove(i, EmptyHoop));
                    }
                    for (int j = 0; j < AvailableSpaces.Count; j++)
                    {
                        int PoleNum = AvailableSpaces[j].Item1;
                        if (PoleNum != i && AvailableSpaces[j].Item2 == color)
                        {
                            Moves.Add(GenerateMove(i, PoleNum));
                        }
                    }
                }
            }
            return Moves;
        }

        private PotentialMove GenerateMove(int fromPole, int toPole)
        {
            var color = Poles[fromPole].TopColor();
            if (color == null)
            {
                throw new Exception("Invalid move");
            }

            // Construct a new game state with updated poles
            var NewState = new GameState()
            {
                Poles = new List<PoleState>(),
                MaxPoleHeight = MaxPoleHeight,
                MovesTaken = this.MovesTaken + 1,
            };
            NewState.Poles.AddRange(Poles);
            NewState.Poles[fromPole] = new PoleState();
            NewState.Poles[fromPole].Hoops.AddRange(this.Poles[fromPole].Hoops);
            NewState.Poles[toPole] = new PoleState();
            NewState.Poles[toPole].Hoops.AddRange(this.Poles[toPole].Hoops);

            // Transfer as many items between poles as possible
            int numHoopsMoved = 0;
            while (NewState.Poles[fromPole].TopColor() == color && NewState.Poles[toPole].Hoops.Count < MaxPoleHeight)
            {
                NewState.Poles[fromPole].Hoops.RemoveAt(NewState.Poles[fromPole].Hoops.Count - 1);
                NewState.Poles[toPole].Hoops.Add(color);
                numHoopsMoved++;
            }

            // Construct the move for this change
            var move = new PotentialMove(this, NewState, $"Move {numHoopsMoved} {color} from #{fromPole} to #{toPole}");
            NewState.LastMove = move;
            return move;
        }

        public static GameState Load(string filename)
        {
            int maxHeight = 0;
            int numEmptyPoles = 0;
            var colors = new List<string>();
            var list = new List<PoleState>();
            var text = File.ReadAllText(filename);
            foreach (var line in text.Split(Environment.NewLine))
            {
                if (line.Equals("empty", StringComparison.OrdinalIgnoreCase))
                {
                    var pole = new PoleState();
                    list.Add(pole);
                    numEmptyPoles++;
                }
                else if (!line.StartsWith("#"))
                {
                    var pole = new PoleState();
                    foreach (var color in line.Split(", ")) {
                        if (!colors.Contains(color))
                        {
                            colors.Add(color);
                        }
                        pole.Hoops.Add(color);
                    }
                    if (maxHeight == 0)
                    {
                        maxHeight = pole.Hoops.Count;
                    } 
                    else if (maxHeight != pole.Hoops.Count)
                    {
                        throw new Exception($"Incorrect height {pole.Hoops.Count}, should be {maxHeight}");
                    }
                    list.Add(pole);
                } 
            }

            // Check consistency
            if (numEmptyPoles != 2)
            {
                throw new Exception($"Needs at least two empty poles, found {numEmptyPoles}");
            }
            if (colors.Count + numEmptyPoles != list.Count)
            {
                throw new Exception($"Mismatch in colors: Found {colors.Count} colors and {numEmptyPoles} empty poles but {list.Count} total poles");
            }

            // Here's the game state
            return new GameState()
            {
                MaxPoleHeight = maxHeight,
                Poles = list,
            };
        }

        public string ToGameString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Game: ");
            for (int i = 0; i < Poles.Count; i++)
            {
                sb.AppendLine($"  Pole {i}: {Poles[i].ToGameString()}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Represents a sorted game state for deduplication purposes.
        /// </summary>
        /// <returns></returns>
        public string ToHashString()
        {
            return String.Join(Environment.NewLine, (from pole in Poles select pole.ToGameString()).OrderBy(a => a));
        }
    }
}