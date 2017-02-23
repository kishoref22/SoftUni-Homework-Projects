﻿namespace SharpStore.Views.Home
{
    using System.IO;
    using MVCFramework.MVC.Interfaces;

    public class Index : IRenderable
    {
        public string Render()
        {
            return File.ReadAllText("../../content/home.html");
        }
    }
}