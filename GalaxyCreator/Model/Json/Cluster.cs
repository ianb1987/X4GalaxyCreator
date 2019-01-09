﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GalaxyCreator.Model.Json
{
    public class Cluster
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Music { get; set; }
        public String Sunlight { get; set; }
        public String Economy { get; set; }
        public String Security { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public String Backdrop { get; set; }
        public bool? NoBelts { get; set; }
        public Faction? FactionHq { get; set; }
        public FactionStart FactionStart { get; set; }
        public ObservableCollection<Connection> Connections { get; set; }
        public ObservableCollection<Belt> Belts { get; set; }
        public ObservableCollection<Station> Stations { get; set; }
        public ObservableCollection<SpaceObject> SpaceObjects { get; set; }

        /* VARIBLES/FUNCTIONS USED MY THE EDITOR */
        [JsonIgnore]
        public Guid UId { get; set; }
        [JsonIgnore]
        public Polygon Polygon { get; set; }
        //public Hex Hex { get; set; }
        [JsonIgnore]
        public bool IsEnabled { get; set; }

        [JsonIgnore]
        public bool GameStart { get; set; }


        [JsonIgnore]
        public CanvasBaseObject CanvasObject { get; set; }


        public Cluster(int x, int y, double hexSize, bool newCluster, int clusterIdCounter)
        {
            FactionStart = new FactionStart();
            FactionStart.ClusterId = Id;

            UId = Guid.NewGuid();
            X = x;
            Y = y;

            Connections = new ObservableCollection<Connection>();
            Belts = new ObservableCollection<Belt>();
            Stations = new ObservableCollection<Station>();
            SpaceObjects = new ObservableCollection<SpaceObject>();

            if (newCluster == true)
            {
                IsEnabled = false;

                /* SET THE DEFAULTS FOR A NEW CLUSTER */

                Name = "Sector";
                Description = "";
                Music = "music_cluster_02";
                Sunlight = "1";
                Economy = "0.75";
                Security = "0.75";
                Backdrop = "cluster_01";

                Id = clusterIdCounter.ToString().PadLeft(3, '0');

            }
            else
                IsEnabled = true;

            double OuterRadius = 75;
            double InnerRadius = OuterRadius * 0.866025404f;

            var corners = new PointCollection()
            {
                new Point(OuterRadius *0.5f,InnerRadius),
                new Point(OuterRadius *-0.5f,InnerRadius),
                new Point(-OuterRadius, 0F),
                new Point(OuterRadius *-0.5f,-InnerRadius),
                new Point(OuterRadius *0.5f,-InnerRadius),
                new Point(OuterRadius, 0F),
            };

            int shiftedY = (-Y) + 10;
            int shiftedX = X + 10;

            Console.WriteLine($"Creating Hex X:{x}/{shiftedX} Y:{y}/{shiftedY}");

            Polygon = new Polygon();
            Polygon.Tag = UId;
            Polygon.Stroke = Brushes.Black;

            if ( IsEnabled)
                Polygon.Fill = Brushes.Pink;
            else
                Polygon.Fill = Brushes.LightGray;

            Polygon.StrokeThickness = 1;
            Polygon.HorizontalAlignment = HorizontalAlignment.Center;
            Polygon.VerticalAlignment = VerticalAlignment.Center;
            Polygon.Points = corners;


            double a = (shiftedY + (shiftedX / 2) - (shiftedX * 0.5f));
            double b = (InnerRadius * 2f);

            double aa = (shiftedX / 2);
            double aaa = (shiftedX * 0.5f);


            double yPos = a * b + OuterRadius;

            Console.WriteLine($"a: {a}, b: {b}, yPos: {yPos} - aa: {aa}, aaa: {aaa}");

            Polygon.RenderTransform = new TranslateTransform((shiftedX * (OuterRadius * 1.5f) + OuterRadius), yPos);

            Polygon.MouseEnter += Polygon_MouseEnter;
            Polygon.MouseLeave += Polygon_MouseLeave;

        }

        private void Polygon_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /* THIS DRAWS A TOOL TIP */
            Point pos = Polygon.TransformToAncestor(MainData.Canvas).Transform(new Point(0, 0));

            Rectangle rectangle = new Rectangle();
            Canvas.SetLeft(rectangle, pos.X);
            Canvas.SetTop(rectangle, pos.Y);

            rectangle.Width = 400;
            rectangle.Height = 300;
            rectangle.Stroke = Brushes.Blue;
            rectangle.Fill = Brushes.Black;
            rectangle.IsHitTestVisible = false;
            rectangle.RadiusX = 10;
            rectangle.RadiusY = 10;

            CanvasObject = new CanvasBaseObject(rectangle, 20);


            TextBox label = new TextBox();
            label.Height = 70;
            label.Width = 390;
            label.Text = "Name: " + Name;
            label.Foreground = Brushes.White;
            label.IsHitTestVisible = false;
            label.FontSize = 35;
            CanvasObject.AddChild(label, 21, pos.X + 15, pos.Y + 5);

            label = new TextBox();
            label.Height = 70;
            label.Width = 390;
            label.Text = "ID: " + Id;
            label.Foreground = Brushes.White;
            label.IsHitTestVisible = false;
            label.FontSize = 35;
            CanvasObject.AddChild(label, 21, pos.X + 15, pos.Y + 80);

            label = new TextBox();
            label.Height = 70;
            label.Width = 390;
            label.Text = "Location: " + X + "," + Y;
            label.Foreground = Brushes.White;
            label.IsHitTestVisible = false;
            label.FontSize = 35;
            CanvasObject.AddChild(label, 21, pos.X + 15, pos.Y + 155);


            CanvasObject.DrawObject(MainData.Canvas);
        }

        private void Polygon_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /* THIS CLEARS A TOOL TIP */
            CanvasObject.ClearObject(MainData.Canvas);
        }
    }

    public class CanvasConnection
    {
        public Guid UId { get; set; }
        public string ConnectionId1 { get; set; }
        public ConnectionType Connection1Type { get; set; }

        public string ConnectionId2 { get; set; }
        public ConnectionType Connection2Type { get; set; }

        public Line Line { get; set; }

        public CanvasConnection(string connectionId1, string connectionId2, ConnectionType connection1Type)
        {
            ConnectionId1 = connectionId1;
            ConnectionId2 = connectionId2;
            Connection1Type = connection1Type;

            switch (connection1Type)
            {
                case ConnectionType.N:
                    {
                        Connection2Type = ConnectionType.S;
                        break;
                    }
                case ConnectionType.NE:
                    {
                        Connection2Type = ConnectionType.SW;
                        break;
                    }
                case ConnectionType.NW:
                    {
                        Connection2Type = ConnectionType.SE;
                        break;
                    }
                case ConnectionType.S:
                    {
                        Connection2Type = ConnectionType.N;
                        break;
                    }
                case ConnectionType.SE:
                    {
                        Connection2Type = ConnectionType.NW;
                        break;
                    }
                case ConnectionType.SW:
                    {
                        Connection2Type = ConnectionType.NE;
                        break;
                    }
            }

            UId = Guid.NewGuid();


        }
    
        public void GenerateLine()
        {
            Point cluster1Pos = MainData.GetGalaxyMap().Clusters.First(c => c.Id == ConnectionId1).Polygon.TransformToAncestor(MainData.Canvas).Transform(new Point(0, 0));
            Point cluster2Pos = MainData.GetGalaxyMap().Clusters.First(c => c.Id == ConnectionId2).Polygon.TransformToAncestor(MainData.Canvas).Transform(new Point(0, 0));

            Console.WriteLine("1: " + cluster1Pos.ToString());

            cluster1Pos = GetPoint(cluster1Pos, 55, Connection1Type);
            cluster2Pos = GetPoint(cluster2Pos, 55, Connection2Type);

            Line = new Line();
            Line.X1 = cluster1Pos.X;
            Line.Y1 = cluster1Pos.Y;
            Line.X2 = cluster2Pos.X;
            Line.Y2 = cluster2Pos.Y;


            Line.Stroke = Brushes.Orange;
            Line.StrokeThickness = 5;
            Line.Tag = UId;

            MainData.AddChildToCanvas(CanvasElementType.CONNECTION, Line, UId.ToString());
        }

        private Point GetPoint(Point center, float radius, ConnectionType connectionType)
        {
            double angle = 0.0;

            switch (connectionType)
            {
                case ConnectionType.N:
                    angle = 90;
                    break;
                case ConnectionType.NW:
                    angle = 30;
                    break;
                case ConnectionType.NE:
                    angle = 150;
                    break;
                case ConnectionType.S:
                    angle = 270;
                    break;
                case ConnectionType.SW:
                    angle = 330;
                    break;
                case ConnectionType.SE:
                    angle = 210;
                    break;
            }

            double rad =  angle * Math.PI / 180.0;

            return new Point((center.X + radius * Math.Cos(rad)), (center.Y + radius * Math.Sin(rad)));
        }
    }
}
