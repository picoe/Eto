using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Input;


namespace ColorPickerSampleApplication
{

    public partial class SampleViewer : Window
    {

        public SampleViewer()
        {
            InitializeComponent();
            
        }



        #region Dependency Property Fields

        public static readonly DependencyProperty SelectedShapeProperty =
            DependencyProperty.Register
            ("SelectedShape", typeof(Shape), typeof(SampleViewer),
            new PropertyMetadata(null, selectedShape_Changed));

        public static readonly DependencyProperty DrawingModeProperty =
           DependencyProperty.Register
           ("DrawingMode", typeof(DrawingMode), typeof(SampleViewer),
           new PropertyMetadata(DrawingMode.Select));

        public static readonly DependencyProperty FillColorProperty =
           DependencyProperty.Register
           ("FillColor", typeof(Color), typeof(SampleViewer),
           new PropertyMetadata(Colors.LightGray));

        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register
            ("StrokeColor", typeof(Color), typeof(SampleViewer),
            new PropertyMetadata(Colors.Black));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register
            ("StrokeThickness", typeof(double), typeof(SampleViewer),
            new PropertyMetadata(1.0, strokeThickness_Changed));

        #endregion


        #region Public Properties

        public DrawingMode DrawingMode
        {
            get
            {
                return (DrawingMode)GetValue(DrawingModeProperty);
            }
            set
            {
                SetValue(DrawingModeProperty, value);
            }
        }


        public Color FillColor
        {
            get
            {
                return (Color)GetValue(FillColorProperty);
            }
            set
            {
                SetValue(FillColorProperty, value);
            }
        }

        public Color StrokeColor
        {
            get
            {
                return (Color)GetValue(StrokeColorProperty);
            }
            set
            {
                SetValue(StrokeColorProperty, value);
            }
        }

        public double StrokeThickness
        {
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }
            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        } 

        #endregion


        

        private static void selectedShape_Changed(object sender,
            DependencyPropertyChangedEventArgs e)
        {

            SampleViewer sViewer = (SampleViewer)sender;
            sViewer.OnSelectedShapeChanged((Shape)e.OldValue, (Shape)e.NewValue);

        }

        protected void OnSelectedShapeChanged(Shape oldShape, Shape newShape)
        {
            if (newShape != null)
            {
                FillColor = ((SolidColorBrush)newShape.Fill).Color;
                StrokeColor = ((SolidColorBrush)newShape.Stroke).Color;
                StrokeThickness = newShape.StrokeThickness;
            }
        }

        private static void strokeThickness_Changed(object sender,
            DependencyPropertyChangedEventArgs e)
        {

            SampleViewer sViewer = (SampleViewer)sender;
            sViewer.OnStrokeThicknessChanged((double)e.OldValue, (double)e.NewValue);

        }

        protected void OnStrokeThicknessChanged(double oldThickness, double newThickness)
        {
            Shape currentShape = (Shape)GetValue(SelectedShapeProperty);
            if (currentShape != null)
            {
                currentShape.StrokeThickness = newThickness;
            }
        }
 
      
        private void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(DrawingCanvas);
            if (DrawingMode == DrawingMode.Select)
            {
                if (e.OriginalSource is Shape)
                {
                    Shape s = (Shape)e.OriginalSource;
                    SetValue(SelectedShapeProperty, s);
                    shapeClickPoint = e.GetPosition(s);
                }
                else
                    SetValue(SelectedShapeProperty, null);
            }
            else if (DrawingMode == DrawingMode.DrawRectangle && e.LeftButton == MouseButtonState.Pressed)
            {
                newRectangle = new Rectangle();
                newRectangle.Stroke = new SolidColorBrush(StrokeColor);
                newRectangle.StrokeThickness = StrokeThickness;
                newRectangle.Fill = new SolidColorBrush(FillColor);
                Canvas.SetLeft(newRectangle, clickPoint.X);
                Canvas.SetTop(newRectangle, clickPoint.Y);
                DrawingCanvas.Children.Add(newRectangle);
                SetValue(SelectedShapeProperty, newRectangle);
            }
            else if (DrawingMode == DrawingMode.DrawEllipse && e.LeftButton == MouseButtonState.Pressed)
            {
                newEllipse = new Ellipse();
                newEllipse.Stroke = new SolidColorBrush(StrokeColor);
                newEllipse.StrokeThickness = StrokeThickness;
                newEllipse.Fill = new SolidColorBrush(FillColor);
                Canvas.SetLeft(newEllipse, clickPoint.X);
                Canvas.SetTop(newEllipse, clickPoint.Y);
                DrawingCanvas.Children.Add(newEllipse);
                SetValue(SelectedShapeProperty, newEllipse);
            }
            else if (DrawingMode == DrawingMode.DrawLine && e.LeftButton == MouseButtonState.Pressed)
            {
                newLine = new Line();
                newLine.Stroke = new SolidColorBrush(StrokeColor);
                newLine.Fill = new SolidColorBrush(FillColor);;
                newLine.X1 = clickPoint.X;
                newLine.Y1 = clickPoint.Y;
                newLine.StrokeThickness = StrokeThickness;
                DrawingCanvas.Children.Add(newLine);
                SetValue(SelectedShapeProperty, newLine);
            }
        }

        private void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (DrawingMode == DrawingMode.DrawRectangle)
            {
                newRectangle = null;

            }
            else if (DrawingMode == DrawingMode.DrawEllipse)
            {
                newEllipse = null;

            }
            else if (DrawingMode == DrawingMode.DrawLine)
            {
                newLine = null;

            }
        }

        private void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
        {
            Point dropPoint = e.GetPosition(DrawingCanvas);
            if (DrawingMode == DrawingMode.Select)
            {
                Shape s = (Shape)GetValue(SelectedShapeProperty);
                if (s != null && e.LeftButton == MouseButtonState.Pressed)
                {
                    
                    Canvas.SetLeft(s, dropPoint.X - shapeClickPoint.X);
                    Canvas.SetTop(s, dropPoint.Y - shapeClickPoint.Y);
                    s.BitmapEffect = null;
                }
            }
            else if (DrawingMode == DrawingMode.DrawRectangle)
            {
                if (newRectangle != null)
                {
                    if (dropPoint.X > Canvas.GetLeft(newRectangle))
                        newRectangle.Width = dropPoint.X - Canvas.GetLeft(newRectangle);
                    if (dropPoint.Y > Canvas.GetTop(newRectangle))
                        newRectangle.Height = dropPoint.Y - Canvas.GetTop(newRectangle);
                }
            }
            else if (DrawingMode == DrawingMode.DrawEllipse)
            {
                if (newEllipse != null)
                {
                    if (dropPoint.X > Canvas.GetLeft(newEllipse))
                        newEllipse.Width = dropPoint.X - Canvas.GetLeft(newEllipse);
                    if (dropPoint.Y > Canvas.GetTop(newEllipse))
                        newEllipse.Height = dropPoint.Y - Canvas.GetTop(newEllipse);
                }
            }
            else if (DrawingMode == DrawingMode.DrawLine)
            {
                if (newLine != null)
                {
                    newLine.X2 = dropPoint.X;
                    newLine.Y2 = dropPoint.Y;
                }
            }
        }

        private void OnDrawingCanvasKeyDown(object sender, KeyEventArgs e)
        {

            Shape s = (Shape)GetValue(SelectedShapeProperty);
            if (e.Key == Key.Delete && s!= null)
            {
                DrawingCanvas.Children.Remove(s);
                SetValue(SelectedShapeProperty, null);
            }
        }
        
        
        private void SetStroke(object sender, RoutedEventArgs e)
        {
            Shape selectedShape = (Shape)GetValue(SelectedShapeProperty);
            
            

                Microsoft.Samples.CustomControls.ColorPickerDialog cPicker 
                    = new Microsoft.Samples.CustomControls.ColorPickerDialog();         
                
                cPicker.StartingColor = StrokeColor;
                cPicker.Owner = this;
                
                bool? dialogResult = cPicker.ShowDialog(); 
                if (dialogResult != null && (bool)dialogResult == true)
                {
                    
                    if (selectedShape != null)
                        selectedShape.Stroke = new SolidColorBrush(cPicker.SelectedColor);
                    StrokeColor = cPicker.SelectedColor;
                    
                }
                
        }
        
        
        private void SetFill(object sender, RoutedEventArgs e)
        {
            Shape selectedShape = (Shape)GetValue(SelectedShapeProperty);

                Microsoft.Samples.CustomControls.ColorPickerDialog cPicker 
                    = new Microsoft.Samples.CustomControls.ColorPickerDialog();                   
                cPicker.StartingColor = FillColor;
                cPicker.Owner = this;
                
                bool? dialogResult = cPicker.ShowDialog(); 
                if (dialogResult != null && (bool)dialogResult == true)
                {

                    if (selectedShape != null)
                        selectedShape.Fill = new SolidColorBrush(cPicker.SelectedColor);
                    FillColor = cPicker.SelectedColor;
   
                }
                
        }

        private void drawingModeChanged(object sender, EventArgs e)
        {
            
            RadioButton r = (RadioButton)sender;
            SetValue(DrawingModeProperty, Enum.Parse(typeof(DrawingMode), r.Name));
             
        }


        private void exitMenuItemClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
         


        #region Private Fields
        private Point shapeClickPoint;
        private Rectangle newRectangle;
        private Line newLine;
        private Ellipse newEllipse;

        #endregion

    }

    public enum DrawingMode
    {
        Select=0, DrawLine=1, DrawRectangle=2, DrawEllipse=3

    }
}