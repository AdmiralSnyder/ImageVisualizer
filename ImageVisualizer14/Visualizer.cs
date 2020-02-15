﻿using System;
using ImageVisualizer;
using Microsoft.VisualStudio.DebuggerVisualizers;

//System.Drawing.Bitmap
[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(Visualizer),
#if VS16
    typeof(BitmapVisualizerObjectSource),
#else
    typeof(VisualizerObjectSource),
#endif
    Target = typeof(System.Drawing.Bitmap), 
    Description = "Image Visualizer")]

//System.Windows.Media.ImageSource, System.Windows.Media.Imaging.BitmapImage, System.Windows.Media.Imaging.BitmapSource
[assembly: System.Diagnostics.DebuggerVisualizer(
    typeof(Visualizer), 
    typeof(ImageVisualizerObjectSource), 
    Target = typeof(System.Windows.Media.ImageSource), 
    Description = "Image Visualizer")]

namespace ImageVisualizer
{
    /// <summary>
    /// A Visualizer for <see cref="System.Windows.Media.Imaging.BitmapImage"/> and <see cref="System.Drawing.Bitmap"/>.
    /// </summary>
    public class Visualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException(nameof(windowService), "This debugger does not support modal visualizers.");
            if (objectProvider == null)
                throw new ArgumentNullException(nameof(objectProvider));

            using (var imageForm = new ImageForm(objectProvider))
            {
                windowService.ShowDialog(imageForm);
            }
        }

#if DEBUG
        /// <summary>
        /// Tests the visualizer by hosting it outside of the debugger.
        /// </summary>
        /// <param name="objectToVisualize">The object to display in the visualizer.</param>
        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(Visualizer), typeof(ImageVisualizerObjectSource));
            visualizerHost.ShowVisualizer();
        }
#endif
    }
}
