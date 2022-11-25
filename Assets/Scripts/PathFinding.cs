using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// A* Pathfinding
public static class PathFinding {

    //Recursiva?
    //https://forum.unity.com/threads/a-pathfinding-movement-range.696911/
   

    public static List<TileNode> FindPath(TileNode origin, TileNode target){
       
        List<TileNode> toSearch = new List<TileNode>() {origin};
        List<TileNode> processed = new List<TileNode>();

        while(toSearch.Any()){
            TileNode current = toSearch[0];

            //Find the best F cost node
            //TODO Usar heap com binary search para melhorar desempenho
            foreach(var t in toSearch){
                if(t.F < current.F || t.F == current.F && t.H < current.H) current = t; 
            }

            processed.Add(current);
            toSearch.Remove(current);

            //if Found the target node
            if (current == target){
                TileNode currentPathTile = target;
                List<TileNode> path = new List<TileNode>();

                //Debug.Log(target.Coordinates.Position);
                var count = 100;
                while( currentPathTile != origin){
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.Connection;
                    
                    count--;
                    if (count < 0) throw new Exception();
                }
                return path;
            }

            //Search in the neighbors aqui a gente checa a altura tbm
            foreach(var neighbor in current.Neighbors.Where(t => t.isWalkable && !processed.Contains(t))){
                var inSearch = toSearch.Contains(neighbor);

                var costToNeighbor = current.G + current.Coordinates.GetDistance(neighbor.Coordinates);
                if(!inSearch || costToNeighbor < neighbor.G){
                    neighbor.setG(costToNeighbor);
                    neighbor.SetConnection(current);

                    if(!inSearch){
                        neighbor.setH(neighbor.Coordinates.GetDistance(target.Coordinates));
                        toSearch.Add(neighbor);
                    }
                }
            }

        } 
        return null;
    }
}