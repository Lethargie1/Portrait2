using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
//using System.Drawing;

namespace EditorInterface.ViewModel
{
    public class FactionGroupFleetCircleViewModel
    {
        public string Color1 { get; set; } = "#FF0000FF";
        public string Color2 { get; set; } = "#FFFF00";

        public int SegmentNumber { get; set; } = 6;
        public DrawingImage GeometryImage 
        {
            get
            {
                GeometryDrawing seg1 = SegmentedCircleDrawer(0, Color1, SegmentNumber, 25);
                GeometryDrawing seg2 = SegmentedCircleDrawer(1, Color2, SegmentNumber, 25);
                DrawingGroup group = new DrawingGroup();
                group.Append();
                group.Children.Add(seg1);
                group.Children.Add(seg2);
                //
                // Use a DrawingImage and an Image control
                // to display the drawing.
                //
                return new DrawingImage(group);
            }
        }

        public FactionGroupFleetCircleViewModel()
        {

        }

        private PathFigure SegmentedCirclePathCreator(int drawOnOdd, int segmentNumber, double radius)
        {
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(radius, 0);
            bool isStroked;
            double segmentAngle = Math.PI * 2 / SegmentNumber;
            
            double x;
            double y;
            double angle;
            for (int segmentNbr = 0; segmentNbr < segmentNumber; segmentNbr++)
            {
                isStroked = segmentNbr % 2 == drawOnOdd;
                angle = (segmentNbr + 1) * segmentAngle;
                x = radius * Math.Cos(angle);
                y = radius * Math.Sin(angle);

                myPathFigure.Segments.Add(
                    new ArcSegment(
                        new Point(x, y),
                        new Size(radius, radius),
                        0,
                        false, /* IsLargeArc */
                        SweepDirection.Clockwise,
                        isStroked /* IsStroked */ ));
            }
            return myPathFigure;
        }

        private GeometryDrawing SegmentedCircleDrawer(int drawOnOdd,string color, int segmentNumber, double radius)
        {
            /// Create a PathGeometry to contain the figure.
            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures.Add(SegmentedCirclePathCreator(drawOnOdd,segmentNumber, radius));


            GeometryDrawing aGeometryDrawing = new GeometryDrawing();
            aGeometryDrawing.Geometry = myPathGeometry;

            // Outline the drawing with a solid color.
            aGeometryDrawing.Pen = new Pen(new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)), 1);

            return aGeometryDrawing;
        }
    }
}
