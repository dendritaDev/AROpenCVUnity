using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Demo;
using OpenCvSharp;

public class CountorFinder : WebCamera
{
    [SerializeField] private FlipMode ImageFlip;
    [SerializeField] private float Threshold = 96.4f;
    [SerializeField] private bool ShowProcessingImage = true;
    [SerializeField] private float CurveAccuracy = 10f;
    [SerializeField] private float MinArea = 5000f;
    [SerializeField] private PolygonCollider2D PolygonCollider;

    private Mat image;
    private Mat processImage = new Mat();
    private Point[][] contours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        //--- Input ---
        image = OpenCvSharp.Unity.TextureToMat(input);

        //--- Process ---

        //Nos pide un input, un output para que le pasemos el resultado (que se lo pasamos al mismo input q hemos usado) y un tipo de ImageFlip
        Cv2.Flip(image, image, ImageFlip);

        //lo mismo de arriba pero en vez de flip un color, en este caso pasamos de color a gray
        Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2GRAY); 

        //Lo mismo que arriba, pero ahora le pasamos un threshold, le decimos que todos los que cumplan ese threshold salgan en color blanco (255) y le damos un tipo de ThresholdType
        Cv2.Threshold(processImage, processImage, Threshold, 255, ThresholdTypes.BinaryInv);

        //Basicamente nos encuentra los puntos de los contornos de los objetos que detecta
        Cv2.FindContours(processImage, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);

        PolygonCollider.pathCount = 0; //le quitamos los puntos que pueda tener/path, etc.

        foreach (Point[] contour in contours)
        {
            Point[] points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true); //Nos genera los contornos de los objetos
            var area = Cv2.ContourArea(contour); //Calcula el area del contorno que le hemos dado

            if(area > MinArea)
            {
                DrawContour(processImage, new Scalar(127, 127, 127), 2, points);

                PolygonCollider.pathCount++;
                PolygonCollider.SetPath(PolygonCollider.pathCount-1, ToVector2(points));
            }
        }

        if (output == null) //Lo creamos
            output = OpenCvSharp.Unity.MatToTexture(ShowProcessingImage ? processImage : image);
        else //Si ya esta creado, simplemente le pasamos el input de nuevo, para no crear mas, sino nos comemos la memoria
            OpenCvSharp.Unity.MatToTexture(ShowProcessingImage ? processImage : image, output);

        return true;
       
    }

    private void DrawContour(Mat Image, Scalar Color, int Thickness, Point[] points)
    {
        for (int i = 1; i < points.Length; i++)
        {
            Cv2.Line(Image, points[i - 1], points[i], Color, Thickness);

        }

        Cv2.Line(Image, points[points.Length - 1], points[0], Color, Thickness); //Linea del ultimo punto al primero
    }

    private Vector2[] ToVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);

        }

        return vectorList;
    }
}
