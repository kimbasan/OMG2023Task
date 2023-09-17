using System;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    
    public class ChessGridNavigator : IChessGridNavigator
    {
        private static readonly Vector2Int UP_LEFT = new Vector2Int(-1, 1);
        private static readonly Vector2Int UP_RIGHT = new Vector2Int(1, 1);
        private static readonly Vector2Int DOWN_RIGHT = new Vector2Int(1, -1);
        private static readonly Vector2Int DOWN_LEFT = new Vector2Int(-1, -1);

        private static readonly UnitMoveSettings PAWN = new UnitMoveSettings(new Vector2Int[] { Vector2Int.up }, false, false);
        private static readonly UnitMoveSettings KING = new UnitMoveSettings(new Vector2Int[] {
                UP_LEFT, Vector2Int.up,  UP_RIGHT,
                Vector2Int.left, Vector2Int.right,
                DOWN_LEFT, Vector2Int.down, DOWN_RIGHT }, false, true);
        private static readonly UnitMoveSettings QUEEN = new UnitMoveSettings(new Vector2Int[] {
                UP_LEFT, Vector2Int.up,  UP_RIGHT,
                Vector2Int.left, Vector2Int.right,
                DOWN_LEFT, Vector2Int.down, DOWN_RIGHT }, true, true);
        private static readonly UnitMoveSettings ROOK = new UnitMoveSettings(new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down }, true, false);
        private static readonly UnitMoveSettings KNIGHT = new UnitMoveSettings(new Vector2Int[] { new Vector2Int(-2,1), new Vector2Int(-1, 2), new Vector2Int(1, 2), new Vector2Int(2, 1),
                new Vector2Int(2, -1), new Vector2Int(1, -2), new Vector2Int(-2, -1), new Vector2Int(-1, -2) }, false, false);
        private static readonly UnitMoveSettings BISHOP = new UnitMoveSettings(new Vector2Int[] { UP_RIGHT, DOWN_RIGHT, UP_LEFT, DOWN_LEFT }, true, true);
        
        private readonly static UnitMoveSettings[] _unitMoveSettings =
        {
            PAWN,
            KING,
            QUEEN,
            ROOK,
            KNIGHT,
            BISHOP
        };

        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            UnitMoveSettings moveSettings = GetUnitMoveSet(unit);
            List<UnitMove> reachable = new();
            UnitMove startMove = new UnitMove(from, null, Vector2Int.zero, to, moveSettings);
            startMove.Cost = 0;
            reachable.Add(startMove);

            List<UnitMove> explored = new();
            
            while (reachable.Count > 0)
            {
                var move = reachable.OrderBy(m => m.Cost + m.DistanceToTarget).First();
                explored.Add(move);
                reachable.Remove(move);

                if (move.Position == to)
                {
                    return BuildPath(move, moveSettings);
                }

                foreach (var step in moveSettings.PossibleMoves)
                {
                    Vector2Int nextPosition = move.Position + step;
                    
                    if (IsPossible(grid, nextPosition) && IsNotOccupied(grid, nextPosition))
                    {
                        UnitMove nextMove = new UnitMove(nextPosition, move, step, to, moveSettings);
                        
                        if (!explored.Contains(nextMove))
                        {
                            int nextMoveCost = GetMoveCost(move, step, moveSettings);
                            if (reachable.Contains(nextMove))
                            {
                                if (nextMoveCost < nextMove.Cost)
                                {
                                    nextMove.Previous = move;
                                    nextMove.Cost = nextMoveCost;
                                }
                            } else
                            {
                                nextMove.Cost = nextMoveCost;
                                reachable.Add(nextMove);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private UnitMoveSettings GetUnitMoveSet(ChessUnitType unit)
        {
            return _unitMoveSettings[(int)unit];
        }

        private bool IsNotOccupied(ChessGrid grid, Vector2Int position)
        {
            return grid.Get(position) == null;
        }

        private bool IsPossible(ChessGrid grid, Vector2Int position)
        {
            return (grid.Size.x > position.x && grid.Size.y > position.y) && (position.x >= 0 && position.y >= 0);
        }

        private int GetMoveCost(UnitMove previousMove, Vector2Int stepDirection, UnitMoveSettings settings)
        {
            int result;
            if (settings.MoveMultipleTiles && stepDirection == previousMove.Direction)
            {
                result = previousMove.Cost;
            } else
            {
                result = previousMove.Cost + 1;
            }

            return result;
        }

        private List<Vector2Int> BuildPath(UnitMove end, UnitMoveSettings settings)
        {
            List<Vector2Int> result = new()
            {
                end.Position
            };

            UnitMove move = end;
            while (move.Previous != null)
            {
                var direction = move.Direction;
                move = move.Previous;
                if (settings.MoveMultipleTiles && move.Direction == direction)
                {
                    continue;
                } else
                {
                    result.Insert(0, move.Position);
                }
            }

            return result;
        }
    }

    internal class UnitMoveSettings
    {
        public Vector2Int[] PossibleMoves { get; private set; }
        
        public bool MoveMultipleTiles { get; private set; }

        public bool DiagonalMove { get; private set; }

        public UnitMoveSettings(Vector2Int[] possibleMoves, bool moveMultipleTiles, bool diagonalMove)
        {
            PossibleMoves = possibleMoves;
            MoveMultipleTiles = moveMultipleTiles;
            DiagonalMove = diagonalMove;
        }
    }

    internal class UnitMove
    {
        public Vector2Int Position { get; private set; }
        public Vector2Int Direction { get; private set; }
        public int Cost { get; set; }
        public double DistanceToTarget { get; private set; }
        public UnitMove Previous { get; set; }
        
        public UnitMove(Vector2Int position, UnitMove previous, Vector2Int direction, Vector2Int end, UnitMoveSettings settings)
        {
            Position = position;
            Previous = previous;
            Direction = direction;
            DistanceToTarget = DistanceTo(end, settings.DiagonalMove);
        }

        private double DistanceTo(Vector2Int end, bool diagonalMove)
        {
            if (diagonalMove)
                return Math.Sqrt(Math.Pow(this.Position.x - end.x, 2) + Math.Pow(this.Position.y - end.y, 2));
            else
                return Math.Abs(this.Position.x - end.x) + Math.Abs(this.Position.y - end.y);
        }

        public override bool Equals(object obj)
        {
            return obj is UnitMove move &&
                   Position.Equals(move.Position);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position);
        }
    }
}