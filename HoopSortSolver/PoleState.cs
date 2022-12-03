
namespace HoopSortSolver
{

    public class PoleState {
        public int PoleNumber { get; set; }
        public List<int> Hoops = new();
        public bool IsEmpty() 
        {
            return Hoops.Count == 0;
        }

        public bool IsFinished(GameState game)
        {
            if (Hoops.Count != game.MaxPoleHeight) return false;
            var baseColor = Hoops[0];
            for (int i = 1; i < game.MaxPoleHeight; i++)
            {
                if (Hoops[i] != baseColor) return false;
            }
            return true;
        }

        public int? TopColor() 
        {
            if (Hoops.Count > 0) {
                return Hoops[Hoops.Count - 1];
            }
            return null;
        }

        public string ToHashString()
        {
            if (Hoops.Count == 0) return "Empty";
            return String.Join(" > ", Hoops);
        }

        public bool CanMove(GameState game)
        {
            if (Hoops.Count == 0) return false;
            if (IsFinished(game)) return false;
            return true;
        }
    }
}