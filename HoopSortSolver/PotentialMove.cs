using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoopSortSolver
{
    public class PotentialMove
    {
        public GameState OldState { get; set; }
        public string Move { get; set; }
        public GameState NewState { get; set; }
        public int Score { get; set; }

        public PotentialMove(GameState oldState, GameState newState, string move)
        {
            this.OldState = oldState;
            this.NewState = newState;
            this.Move = move;
            this.Score = NewState.Score();
        }
    }
}
