using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp.Demo;
using OpenCvSharp;

public class CountorFinder : WebCamera
{
    private Mat image;
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        //Input
        image = OpenCvSharp.Unity.TextureToMat(input);

        //Process
        if (output == null) //Lo creamos
            output = OpenCvSharp.Unity.MatToTexture(image);
        else //Si ya esta creado, simplemente le pasamos el input de nuevo, para no crear mas, sino nos comemos la memoria
            OpenCvSharp.Unity.MatToTexture(image, output);

        return true;
       
    }
}
