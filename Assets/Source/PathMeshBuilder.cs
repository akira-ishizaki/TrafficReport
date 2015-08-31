﻿using System;
using UnityEngine;
using System.Collections.Generic;

namespace TrafficReport
{
	public class PathMeshBuilder {


		public float width = 4f;
		public float laneOffset = -2.0f;

		public float curveRetractionFactor = 1.5f;
		public float lineScale = 0.5f;


		List<Vector3> verts = new List<Vector3> ();
		List<int> triangles = new List<int> ();
		List<Vector2> uvs = new List<Vector2> ();
		Vector3 lastPoint;
		float textureOffset = 0.0f;


		public void AddPoints(Vector3[] points){

            lastPoint = points[0];

            for (int i = 0; i < points.Length - 1; i++)
            {
                
              

                Vector3 start = points[i];
                Vector3 end = points[i + 1];

                if ((end - start).magnitude < width * 3)
                {
                    //continue;
                }

                if (i != 0)
                {
                    start += (end - start).normalized * (width * curveRetractionFactor);
                }

                if (i != points.Length - 2)
                {
                    end += (start - end).normalized * (width * curveRetractionFactor);
                }


                AddSegment(start, end);


                if (i < points.Length - 2)
                {
                  


                    Vector3 cornerPoint = points[i + 1];

                    Vector3 nextStart = cornerPoint;
                    Vector3 nextEnd = points[i + 2];
                    nextStart -= (nextStart - nextEnd).normalized * (width * curveRetractionFactor);
                    
                    Debug.DrawLine(end, end + Vector3.up, Color.blue, 20000);
                    Debug.DrawLine(nextStart, nextStart + Vector3.up, Color.blue, 20000);


                    Vector3 p0 = end;
                    Vector3 p1 = Vector3.Lerp(end, cornerPoint, 0.5f);
                    Vector3 p2 = Vector3.Lerp(nextStart, cornerPoint, 0.5f);
                    Vector3 p3 = nextStart;

                    Debug.DrawLine(p0,p1, Color.yellow,2000);
                    Debug.DrawLine(p1, p2, Color.blue, 2000);
                    Debug.DrawLine(p2, p3, Color.yellow, 2000);

                    Vector3 startDir = (end - start).normalized;
                    Vector3 endDir = (nextEnd - nextStart).normalized;

                    float step = 0.2f;
                    for (float a = 0; a <= 1.0f; a += step)
                    {
                        Vector3 point = Beizer.CalculateBezierPoint(a, p0, p1, p2, p3);
                        Vector3 fwd = Vector3.Lerp(startDir, endDir, a).normalized;

                        textureOffset = (a * width * curveRetractionFactor * lineScale) / 2.0f;
                        AddVertexPair(point, fwd);
                    }
                }
            }


            //Vector3 direction = points[1] - points[0];

            //for (int i = 0; i < points.Length - 1; i++)
            //{
            //    //Vector3 p0 = points[i];
            //    //Vector3 p1 = Vector3.Lerp (points[i],cornerPoint, 0.5f);
            //    //Vector3 p2 = Vector3.Lerp (nextStart,cornerPoint, 0.5f);
            //    //Vector3 p3 = nextStart;

            //    //float step = 0.2f;
            //    //for(float a = 0 ; a <= 1.0f; a += step) {
            //    //    Vector3 point = Beizer.CalculateBezierPoint(a, p0,p1,p2,p3);
            //    //    Vector3 fwd = Vector3.Lerp(startDir,endDir, a);

            //    //    textureOffset = (a * width * curveRetractionFactor * lineScale)/ 2.0f;
            //    //    AddVertexPair(point,fwd);
            //    //}

            //    //direction = points[i]-points

            //    AddSegment(points[i], points[i + 1]);
            //}


			GenerateIndiciesAsLineStrip ();

            Vector3 startFwd = (points[0] - points[1]);
            //			if (startFwd.magnitude < width * 3.0f) {
            //				startFwd = (points[0] - points [2]);
            //			}
            AddEndStop(points[0], startFwd.normalized, false);

            Vector3 endFwd = (points[points.Length - 1] - points[points.Length - 2]);
            if (endFwd.magnitude < width * 3.0f)
            {
                endFwd = (points[points.Length - 1] - points[points.Length - 3]);
            }
            AddEndStop(points[points.Length - 1], endFwd.normalized, true);
		}

		void AddEndStop(Vector3 point, Vector3 fwd, bool isEnd) {

			float v1 = 0.5f;
			float v2 = 0.26f;

			if(isEnd){
				v1 = 0.25f;
				v2 = 0.0f;
			}

			Vector3 offset = Vector3.Cross(fwd, Vector3.up);

			int baseVert = verts.Count;

			verts.Add (point);
			uvs.Add (new Vector2 (0.5f, v1));

			int steps = 32;
			float anglePerStep = ((float)Math.PI * 2) / steps;
			for (int step = 0; step <= steps; step++) {

				float angle = (step * anglePerStep);

				Vector3 p = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
				verts.Add(point + (p*width*2));
				uvs.Add (new Vector2 (0.5f, v2));
			}

			
			for (int i = baseVert; i < baseVert + steps; i++) {
				triangles.Add (baseVert);
				triangles.Add (i + 2);
				triangles.Add (i + 1);

				triangles.Add (baseVert);
				triangles.Add (i + 1);
				triangles.Add (i + 2);
			}
		}

		void GenerateIndiciesAsLineStrip(){

			for(int i = 0;  i < verts.Count-2; i +=2) {
				
				triangles.Add (i);
				triangles.Add (i + 2);
				triangles.Add (i + 1);
				
				triangles.Add (i + 1);
				triangles.Add (i + 2);
				triangles.Add (i);
				
				triangles.Add (i + 1);
				triangles.Add (i + 2);
				triangles.Add (i + 3);
				
				triangles.Add (i + 3);
				triangles.Add (i + 2);
				triangles.Add (i + 1);
			}

		}

		void AddSegment(Vector3 start, Vector3 end) {

			Vector3 fwd = (end - start).normalized;

			textureOffset = 0;
			AddVertexPair (start, fwd);
			textureOffset = Mathf.Floor ((end - start).magnitude * lineScale / width);
			AddVertexPair (end, fwd);

		}

		void AddVertexPair(Vector3 point, Vector3 fwd) {
			Vector3 offset = Vector3.Cross(fwd, Vector3.up).normalized ;

			verts.Add (point - offset * (width /2));              
			verts.Add (point + offset * (width /2));
			
			uvs.Add (new Vector2 (textureOffset, 0.6f));
			uvs.Add (new Vector2 (textureOffset, 0.9f));

			lastPoint = point;

            //Debug.DrawLine(point, point + fwd,Color.green,20000);
		}

		public Mesh GetMesh() {
			Mesh m = new Mesh();
			m.vertices = verts.ToArray();
			m.triangles = triangles.ToArray();
			m.uv = uvs.ToArray();
			m.RecalculateNormals();
			return m;
		}
	}
}

