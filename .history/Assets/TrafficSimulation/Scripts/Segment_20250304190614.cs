// Traffic Simulation
// https://github.com/mchrbn/unity-traffic-simulation

using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
namespace TrafficSimulation {
    public class Segment : MonoBehaviour {
        public List<Segment> nextSegments;
        public int cost;
        // My personal inclusion
        public int dynamicCost;
        public int carDensity=UnityEngine.Random.Range(0, 100);
        
        // dynamic cost
        public int CalculateDynamicCost()
        {
            return dynamicCost = cost + carDensity;// find better car cost calculation
        }

        [HideInInspector] public int id;
        [HideInInspector] public List<Waypoint> waypoints;

        private TrafficSystem ts;

        private void Awake()
        {
            ts = GetComponentInParent<TrafficSystem>();
            if (ts == null)
                Debug.LogError("TrafficSystem not found in parent.");
        }

        public bool IsOnSegment(Vector3 _p)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Waypoints are not properly initialized.");
                return false;
            }

            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                float d1 = Vector3.Distance(waypoints[i].transform.position, _p);
                float d2 = Vector3.Distance(waypoints[i + 1].transform.position, _p);
                float d3 = Vector3.Distance(waypoints[i].transform.position, waypoints[i + 1].transform.position);

                if (Mathf.Abs((d1 + d2) - d3) < ts.segDetectThresh)
                    return true;
            }

            // Compute and assign cost (one-time computation)
            cost = (int)Vector3.Distance(waypoints[0].transform.position, waypoints[waypoints.Count - 1].transform.position);
            return false;
        }

        public bool IsPointOnSegmentOptimized(Vector3 _p)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Waypoints are not properly initialized.");
                return false;
            }

            Vector3 start = waypoints[0].transform.position;
            Vector3 end = waypoints[waypoints.Count - 1].transform.position;

            Vector3 v = end - start;
            Vector3 w = _p - start;

            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
                return false;

            float c2 = Vector3.Dot(v, v);
            if (c2 <= c1)
                return false;

            float b = c1 / c2;
            Vector3 Pb = start + b * v;

            return Vector3.Distance(_p, Pb) < ts.segDetectThresh;
        }

        public float DistanceFromSegment(Vector3 _p)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Waypoints are not properly initialized.");
                return float.MaxValue;
            }

            Vector3 start = waypoints[0].transform.position;
            Vector3 end = waypoints[waypoints.Count - 1].transform.position;

            Vector3 v = end - start;
            Vector3 w = _p - start;

            float c1 = Vector3.Dot(w, v);
            if (c1 <= 0)
                return Vector3.Distance(_p, start);

            float c2 = Vector3.Dot(v, v);
            if (c2 <= c1)
                return Vector3.Distance(_p, end);

            float b = c1 / c2;
            Vector3 Pb = start + b * v;
            return Vector3.Distance(_p, Pb);
        }
    }
}
