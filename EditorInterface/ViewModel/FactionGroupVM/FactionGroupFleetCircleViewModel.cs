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
using SSEditor.MonitoringField;
using SSEditor.FileHandling;
using System.Windows.Data;
using Stylet;
//using System.Drawing;

namespace EditorInterface.ViewModel
{
    public class FactionGroupFleetCircleViewModel : Screen
    {
        public string Color1 { get; set; } = "#FF0000FF";
        public string Color2 { get; set; } = "#FFFF00";



        public int SegmentNumber 
        {
            get
            {
                if (SecondarySegment == null)
                    return 1;
                try
                {
                    return Convert.ToInt32(SecondarySegment.Value);
                }catch(Exception e)
                {
                    return 1;
                }
            }
        } 

        public MonitoredColorViewModel Color { get; set; }

        public MonitoredColorViewModel SecondaryColor { get; set; }

        public MonitoredValueViewModel SecondarySegment { get; set; }

        private List<IEventBinding> binding = new List<IEventBinding>();
        public FactionGroupFleetCircleViewModel(MonitoredColorViewModel color, MonitoredColorViewModel secondaryColor, MonitoredValueViewModel secondarySegment)
        {
            this.Color = color;
            this.SecondaryColor = secondaryColor;
            this.SecondarySegment = secondarySegment;
            binding.Add(SecondarySegment.Bind(x => x.Value, (sender, eventarg) => { NotifyOfPropertyChange(nameof(Segments1)); NotifyOfPropertyChange(nameof(Segments2)); }));
        }

        protected override void OnClose()
        {
            foreach (IEventBinding b in binding)
                b.Unbind();
            base.OnClose();
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

        public Geometry Segments1
        {
            get
            {
                if (SegmentNumber <= 1)
                {
                    EllipseGeometry myEllipseGeometry = new EllipseGeometry();
                    myEllipseGeometry.Center = new Point(25, 25);
                    myEllipseGeometry.RadiusX = 25;
                    myEllipseGeometry.RadiusY = 25;
                    return myEllipseGeometry;
                }
                else
                {
                    PathGeometry myPathGeometry = new PathGeometry();
                    myPathGeometry.Figures.Add(SegmentedCirclePathCreator(0, this.SegmentNumber, 25));
                    return myPathGeometry;
                }
            }
        }

        public Geometry Segments2
        {
            get
            {
                if (SegmentNumber < 1)
                {
                    return null;
                }
                else
                {
                    PathGeometry myPathGeometry = new PathGeometry();
                    myPathGeometry.Figures.Add(SegmentedCirclePathCreator(1, this.SegmentNumber, 25));
                    return myPathGeometry;
                }
            }
        }

    }
}
