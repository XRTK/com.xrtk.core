﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using XRTK.Extensions;
using XRTK.Utilities;
using Random = System.Random;

namespace XRTK.Definitions.BoundarySystem
{
    /// <summary>
    /// The InscribedRectangle class defines the largest rectangle within an
    /// arbitrary shape.
    /// </summary>
    public class InscribedRectangle
    {
        /// <summary>
        /// Total number of starting points randomly generated within the boundary.
        /// </summary>
        private const int RANDOM_POINT_COUNT = 30;

        /// <summary>
        /// The total amount of height, in meters,  we want to gain with each binary search
        /// change before we decide that it's good enough.
        /// </summary>
        private const float MINIMUM_HEIGHT_GAIN = 0.01f;

        /// <summary>
        /// Angles to use for fitting the rectangle within the boundary.
        /// </summary>
        private static readonly float[] FitAngles = { 0, 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165 };

        /// <summary>
        /// Aspect ratios used when fitting rectangles within the boundary.
        /// </summary>
        private static readonly float[] AspectRatios =
        {
            1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f, 4.5f,
            5.0f, 5.5f, 6, 6.5f, 7, 7.5f, 8.0f, 8.5f, 9.0f,
            9.5f, 10.0f, 10.5f, 11.0f, 11.5f, 12.0f, 12.5f,
            13.0f, 13.5f, 14.0f, 14.5f, 15.0f
        };

        /// <summary>
        /// The center point of the inscribed rectangle.
        /// </summary>
        public Vector2 Center { get; private set; } = EdgeUtilities.InvalidPoint;

        /// <summary>
        /// The width of the inscribed rectangle.
        /// </summary>
        public float Width { get; private set; } = 0f;

        /// <summary>
        /// The height of the inscribed rectangle.
        /// </summary>
        public float Height { get; private set; } = 0f;

        /// <summary>
        /// The rotation angle, in degrees, of the inscribed rectangle.
        /// </summary>
        public float Angle { get; private set; } = 0f;

        /// <summary>
        /// Is the described rectangle valid?
        /// </summary>
        /// <remarks>
        /// A rectangle is considered valid if it's center point is valid.
        /// </remarks>
        public bool IsValid => EdgeUtilities.IsValidPoint(Center);

        /// <summary>
        /// The defined edges of the described rectangle.
        /// </summary>
        public Edge[] Edges { get; }

        /// <summary>
        /// Finds a large inscribed rectangle. Tries to be maximal but this is
        /// best effort. The algorithm used was inspired by the blog post
        /// https://d3plus.org/blog/behind-the-scenes/2014/07/08/largest-rect/
        /// Random points within the polygon are chosen, and then 2 lines are
        /// drawn through those points. The midpoints of those lines are
        /// used as the center of various rectangles, using a binary search to
        /// vary the size, until the largest fit-able rectangle is found.
        /// This is then repeated for predefined angles (0-180 in steps of 15)
        /// and aspect ratios (1 to 15 in steps of 0.5).
        /// </summary>
        /// <param name="geometryEdges">The boundary geometry.</param>
        /// <param name="randomSeed">Random number generator seed.</param>
        /// <remarks>
        /// For the most reproducible results, use the same randomSeed value 
        /// each time this method is called.
        /// </remarks>
        public InscribedRectangle(Edge[] geometryEdges, int randomSeed)
        {
            if (geometryEdges == null || geometryEdges.Length == 0)
            {
                Debug.LogError("InscribedRectangle requires an array of Edges. You passed in a null or empty array.");
                return;
            }

            Edges = geometryEdges;

            // Clear previous rectangle
            Center = EdgeUtilities.InvalidPoint;
            Width = 0;
            Height = 0;
            Angle = 0;

            float minX = EdgeUtilities.MaxWidth;
            float minY = EdgeUtilities.MaxWidth;
            float maxX = -EdgeUtilities.MaxWidth;
            float maxY = -EdgeUtilities.MaxWidth;

            // Find min x, min y, max x, max y 
            for (int i = 0; i < geometryEdges.Length; i++)
            {
                var edge = geometryEdges[i];

                if ((edge.PointA.x < minX) || (edge.PointB.x < minX))
                {
                    minX = Mathf.Min(edge.PointA.x, edge.PointB.x);
                }

                if ((edge.PointA.y < minY) || (edge.PointB.y < minY))
                {
                    minY = Mathf.Min(edge.PointA.y, edge.PointB.y);
                }

                if ((edge.PointA.x > maxX) || (edge.PointB.x > maxX))
                {
                    maxX = Mathf.Max(edge.PointA.x, edge.PointB.x);
                }

                if ((edge.PointA.y > maxY) || (edge.PointB.y > maxY))
                {
                    maxY = Mathf.Min(edge.PointA.y, edge.PointB.y);
                }

            }

            // Generate random points until we have randomPointCount starting points
            var startingPoints = new Vector2[RANDOM_POINT_COUNT];
            var random = new Random(randomSeed);

            for (int i = 0; i < startingPoints.Length; i++)
            {
                Vector2 candidatePoint;

                do
                {
                    candidatePoint.x = ((float)random.NextDouble() * (maxX - minX)) + minX;
                    candidatePoint.y = ((float)random.NextDouble() * (maxY - minY)) + minY;
                }
                while (!EdgeUtilities.IsInsideBoundary(geometryEdges, candidatePoint));

                startingPoints[i] = candidatePoint;
            }

            for (int angleIndex = 0; angleIndex < FitAngles.Length; angleIndex++)
            {
                for (int pointIndex = 0; pointIndex < startingPoints.Length; pointIndex++)
                {
                    float angleRadians = MathUtilities.DegreesToRadians(FitAngles[angleIndex]);

                    // Find the collision point of a cross through the given point at the given angle.
                    // Note, we are ignoring the return value as we are checking each point's validity
                    // individually.
                    FindSurroundingCollisionPoints(
                        geometryEdges,
                        startingPoints[pointIndex],
                        angleRadians,
                        out var topCollisionPoint,
                        out var bottomCollisionPoint,
                        out var leftCollisionPoint,
                        out var rightCollisionPoint);

                    float newWidth;
                    float newHeight;

                    if (EdgeUtilities.IsValidPoint(topCollisionPoint) && EdgeUtilities.IsValidPoint(bottomCollisionPoint))
                    {
                        float aX = topCollisionPoint.x;
                        float aY = topCollisionPoint.y;
                        float bX = bottomCollisionPoint.x;
                        float bY = bottomCollisionPoint.y;

                        // Calculate the midpoint between the top and bottom collision points.
                        Vector2 verticalMidpoint = new Vector2((aX + bX) * 0.5f, (aY + bY) * 0.5f);
                        if (TryFitMaximumRectangle(
                            geometryEdges,
                            verticalMidpoint,
                            angleRadians,
                            Width * Height,
                            out newWidth,
                            out newHeight))
                        {
                            Center = verticalMidpoint;
                            Angle = FitAngles[angleIndex];
                            Width = newWidth;
                            Height = newHeight;
                        }
                    }

                    if (EdgeUtilities.IsValidPoint(leftCollisionPoint) && EdgeUtilities.IsValidPoint(rightCollisionPoint))
                    {
                        float aX = leftCollisionPoint.x;
                        float aY = leftCollisionPoint.y;
                        float bX = rightCollisionPoint.x;
                        float bY = rightCollisionPoint.y;

                        // Calculate the midpoint between the left and right collision points.
                        var horizontalMidpoint = new Vector2((aX + bX) * 0.5f, (aY + bY) * 0.5f);

                        if (TryFitMaximumRectangle(
                            geometryEdges,
                            horizontalMidpoint,
                            angleRadians,
                            Width * Height,
                            out newWidth,
                            out newHeight))
                        {
                            Center = horizontalMidpoint;
                            Angle = FitAngles[angleIndex];
                            Width = newWidth;
                            Height = newHeight;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find points at which there are collisions with the geometry around a given point.
        /// </summary>
        /// <param name="geometryEdges">The boundary geometry.</param>
        /// <param name="point">The point around which collisions will be identified.</param>
        /// <param name="angleRadians">The angle, in radians, at which the collision points will be oriented.</param>
        /// <param name="topCollisionPoint">Receives the coordinates of the upper collision point.</param>
        /// <param name="bottomCollisionPoint">Receives the coordinates of the lower collision point.</param>
        /// <param name="leftCollisionPoint">Receives the coordinates of the left collision point.</param>
        /// <param name="rightCollisionPoint">Receives the coordinates of the right collision point.</param>
        /// <returns>
        /// True if all of the required collision points are located, false otherwise. 
        /// If a point is unable to be found, the appropriate out parameter will be set to <see cref="EdgeUtilities.InvalidPoint"/>.
        /// </returns>
        private static bool FindSurroundingCollisionPoints(
            Edge[] geometryEdges,
            Vector2 point,
            float angleRadians,
            out Vector2 topCollisionPoint,
            out Vector2 bottomCollisionPoint,
            out Vector2 leftCollisionPoint,
            out Vector2 rightCollisionPoint)
        {
            // Initialize out parameters.
            topCollisionPoint = EdgeUtilities.InvalidPoint;
            bottomCollisionPoint = EdgeUtilities.InvalidPoint;
            leftCollisionPoint = EdgeUtilities.InvalidPoint;
            rightCollisionPoint = EdgeUtilities.InvalidPoint;

            // Check to see if the point is inside the geometry.
            if (!EdgeUtilities.IsInsideBoundary(geometryEdges, point))
            {
                return false;
            }

            // Define values that are outside of the maximum boundary size.
            float largeValue = EdgeUtilities.MaxWidth;
            float smallValue = -largeValue;

            // Find the top and bottom collision points by creating a large line segment that goes through the point to MAX and MIN values on Y
            var topEndpoint = new Vector2(point.x, largeValue);
            var bottomEndpoint = new Vector2(point.x, smallValue);
            topEndpoint = topEndpoint.RotateAroundPoint(point, angleRadians);
            bottomEndpoint = bottomEndpoint.RotateAroundPoint(point, angleRadians);
            var verticalLine = new Edge(topEndpoint, bottomEndpoint);

            // Find the left and right collision points by creating a large line segment that goes through the point to MAX and Min values on X
            var rightEndpoint = new Vector2(largeValue, point.y);
            var leftEndpoint = new Vector2(smallValue, point.y);
            rightEndpoint = rightEndpoint.RotateAroundPoint(point, angleRadians);
            leftEndpoint = leftEndpoint.RotateAroundPoint(point, angleRadians);
            var horizontalLine = new Edge(rightEndpoint, leftEndpoint);

            for (int i = 0; i < geometryEdges.Length; i++)
            {
                // Look for a vertical collision
                var verticalIntersectionPoint = EdgeUtilities.GetIntersectionPoint(geometryEdges[i], verticalLine);

                if (EdgeUtilities.IsValidPoint(verticalIntersectionPoint))
                {
                    // Is the intersection above or below the point?
                    if (verticalIntersectionPoint.RotateAroundPoint(point, -angleRadians).y > point.y)
                    {
                        // Update the top collision point
                        if (!EdgeUtilities.IsValidPoint(topCollisionPoint) ||
                            (Vector2.SqrMagnitude(point - verticalIntersectionPoint) < Vector2.SqrMagnitude(point - topCollisionPoint)))
                        {
                            topCollisionPoint = verticalIntersectionPoint;
                        }
                    }
                    else
                    {
                        // Update the bottom collision point
                        if (!EdgeUtilities.IsValidPoint(bottomCollisionPoint) ||
                            Vector2.SqrMagnitude(point - verticalIntersectionPoint) < Vector2.SqrMagnitude(point - bottomCollisionPoint))
                        {
                            bottomCollisionPoint = verticalIntersectionPoint;
                        }
                    }
                }

                // Look for a horizontal collision
                var horizontalIntersection = EdgeUtilities.GetIntersectionPoint(geometryEdges[i], horizontalLine);

                if (!EdgeUtilities.IsValidPoint(horizontalIntersection)) { continue; }

                // Is this intersection to the left or the right of the point?
                if (horizontalIntersection.RotateAroundPoint(point, -angleRadians).x < point.x)
                {
                    // Update the left collision point
                    if (!EdgeUtilities.IsValidPoint(leftCollisionPoint) ||
                        (Vector2.SqrMagnitude(point - horizontalIntersection) < Vector2.SqrMagnitude(point - leftCollisionPoint)))
                    {
                        leftCollisionPoint = horizontalIntersection;
                    }
                }
                else
                {
                    // Update the right collision point
                    if (!EdgeUtilities.IsValidPoint(rightCollisionPoint) ||
                        (Vector2.SqrMagnitude(point - horizontalIntersection) < Vector2.SqrMagnitude(point - rightCollisionPoint)))
                    {
                        rightCollisionPoint = horizontalIntersection;
                    }
                }
            }

            // Each corner of the rectangle must intersect with the geometry.
            return EdgeUtilities.IsValidPoint(topCollisionPoint) &&
                   EdgeUtilities.IsValidPoint(leftCollisionPoint) &&
                   EdgeUtilities.IsValidPoint(rightCollisionPoint) &&
                   EdgeUtilities.IsValidPoint(bottomCollisionPoint);
        }

        /// <summary>
        /// Determine of the provided point lies within the defined rectangle.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>
        /// True if the point is within the rectangle's bounds, false otherwise.
        /// </returns>
        /// <exception cref="InvalidOperationException">The rectangle is not valid.</exception>
        public bool IsInsideBoundary(Vector2 point)
        {
            if (!IsValid)
            {
                throw new InvalidOperationException("A point cannot be within an invalid rectangle.");
            }

            return EdgeUtilities.IsInsideBoundary(Edges, point);
        }

        /// <summary>
        /// Check to see if a rectangle centered at the specified point and oriented at
        /// the specified angle will fit within the geometry.
        /// </summary>
        /// <param name="geometryEdges">The boundary geometry.</param>
        /// <param name="centerPoint">The center point of the rectangle.</param>
        /// <param name="angleRadians">The orientation, in radians, of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <returns></returns>
        private static bool CheckRectangleFit(Edge[] geometryEdges, Vector2 centerPoint, float angleRadians, float width, float height)
        {
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            // Calculate the rectangle corners.
            var topLeft = new Vector2(centerPoint.x - halfWidth, centerPoint.y + halfHeight);
            var topRight = new Vector2(centerPoint.x + halfWidth, centerPoint.y + halfHeight);
            var bottomLeft = new Vector2(centerPoint.x - halfWidth, centerPoint.y - halfHeight);
            var bottomRight = new Vector2(centerPoint.x + halfWidth, centerPoint.y - halfHeight);

            // Rotate the rectangle.
            topLeft = topLeft.RotateAroundPoint(centerPoint, angleRadians);
            topRight = topRight.RotateAroundPoint(centerPoint, angleRadians);
            bottomLeft = bottomLeft.RotateAroundPoint(centerPoint, angleRadians);
            bottomRight = bottomRight.RotateAroundPoint(centerPoint, angleRadians);

            // Get the rectangle edges.
            var topEdge = new Edge(topLeft, topRight);
            var rightEdge = new Edge(topRight, bottomRight);
            var bottomEdge = new Edge(bottomLeft, bottomRight);
            var leftEdge = new Edge(topLeft, bottomLeft);

            // Check for collisions with the boundary geometry. If any of our edges collide, 
            // the rectangle will not fit within the playspace.
            for (int i = 0; i < geometryEdges.Length; i++)
            {
                if (EdgeUtilities.IsValidPoint(EdgeUtilities.GetIntersectionPoint(geometryEdges[i], topEdge)) ||
                    EdgeUtilities.IsValidPoint(EdgeUtilities.GetIntersectionPoint(geometryEdges[i], rightEdge)) ||
                    EdgeUtilities.IsValidPoint(EdgeUtilities.GetIntersectionPoint(geometryEdges[i], bottomEdge)) ||
                    EdgeUtilities.IsValidPoint(EdgeUtilities.GetIntersectionPoint(geometryEdges[i], leftEdge)))
                {
                    return false;
                }
            }

            // No collisions found with the rectangle. Success!
            return true;
        }

        /// <summary>
        /// Attempt to fit the largest rectangle possible within the geometry.
        /// </summary>
        /// <param name="geometryEdges">The boundary geometry.</param>
        /// <param name="centerPoint">The center point for the rectangle.</param>
        /// <param name="angleRadians">The rotation, in radians, of the rectangle.</param>
        /// <param name="minArea">The smallest allowed area.</param>
        /// <param name="width">Returns the width of the rectangle.</param>
        /// <param name="height">Returns the height of the rectangle.</param>
        /// <returns>
        /// True if a rectangle with an area greater than or equal to minArea was able to be fit
        /// within the geometry at centerPoint.
        /// </returns>
        private static bool TryFitMaximumRectangle(
            Edge[] geometryEdges,
            Vector2 centerPoint,
            float angleRadians,
            float minArea,
            out float width,
            out float height)
        {
            width = 0.0f;
            height = 0.0f;

            // Find the collision points with the geometry
            if (!FindSurroundingCollisionPoints(
                geometryEdges,
                centerPoint,
                angleRadians,
                out var topCollisionPoint,
                out var bottomCollisionPoint,
                out var leftCollisionPoint,
                out var rightCollisionPoint))
            {
                return false;
            }

            // Start by calculating max width and height by ray-casting a cross from the point at the given angle
            // and taking the shortest leg of each ray. Width is the longest.
            float verticalMinDistanceToEdge = Mathf.Min(
                Vector2.Distance(centerPoint, topCollisionPoint),
                Vector2.Distance(centerPoint, bottomCollisionPoint));

            float horizontalMinDistanceToEdge = Mathf.Min(
                Vector2.Distance(centerPoint, leftCollisionPoint),
                Vector2.Distance(centerPoint, rightCollisionPoint));

            // Width is the largest of the possible dimensions
            float maxWidth = Math.Max(verticalMinDistanceToEdge, horizontalMinDistanceToEdge) * 2.0f;
            float maxHeight = Math.Min(verticalMinDistanceToEdge, horizontalMinDistanceToEdge) * 2.0f;

            float aspectRatio = 0.0f;

            // For each aspect ratio we do a binary search to find the maximum rectangle that fits, 
            // though once we start increasing our area by minimumHeightGain we call it good enough.
            for (int i = 0; i < AspectRatios.Length; i++)
            {
                // The height is limited by the width. If a height would make our width exceed maxWidth, it can't be used
                float searchHeightUpperBound = Mathf.Max(maxHeight, maxWidth / AspectRatios[i]);

                // Set to the min height that will out perform our previous area at the given aspect ratio. This is 0 the first time.
                // Derived from biggestAreaSoFar=height*(height*aspectRatio)
                float searchHeightLowerBound = Mathf.Sqrt(Mathf.Max((width * height), minArea) / AspectRatios[i]);

                // If the lowest value needed to outperform the previous best is greater than our max, 
                // this aspect ratio can't outperform what we've already calculated.
                if (searchHeightLowerBound > searchHeightUpperBound ||
                    searchHeightLowerBound * AspectRatios[i] > maxWidth)
                {
                    continue;
                }

                float currentTestingHeight = Mathf.Max(searchHeightLowerBound, maxHeight * 0.5f);

                // Perform the binary search until continuing to search will not give us a significant win.
                do
                {
                    if (CheckRectangleFit(geometryEdges,
                        centerPoint,
                        angleRadians,
                        AspectRatios[i] * currentTestingHeight,
                        currentTestingHeight))
                    {
                        // Binary search up-ward
                        // If the rectangle will fit, increase the lower bounds of our binary search
                        searchHeightLowerBound = currentTestingHeight;

                        width = currentTestingHeight * AspectRatios[i];
                        height = currentTestingHeight;
                        aspectRatio = AspectRatios[i];
                        currentTestingHeight = (searchHeightUpperBound + currentTestingHeight) * 0.5f;
                    }
                    else
                    {
                        // If the rectangle won't fit, update our upper bound and lower our binary search
                        searchHeightUpperBound = currentTestingHeight;
                        currentTestingHeight = (currentTestingHeight + searchHeightLowerBound) * 0.5f;
                    }
                }
                while ((searchHeightUpperBound - searchHeightLowerBound) > MINIMUM_HEIGHT_GAIN);
            }

            return (aspectRatio > 0.0f);
        }
    }
}
