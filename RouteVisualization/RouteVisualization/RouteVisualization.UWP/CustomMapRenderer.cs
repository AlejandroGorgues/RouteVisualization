using RouteVisualization;
using RouteVisualization.UWP;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;
using System;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;
using System.Linq;
using Windows.Storage.Streams;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace RouteVisualization.UWP
{
    class CustomMapRenderer : MapRenderer
    {
        MapControl nativeMap;
        CustomMap formsMap;
        List<CustomPin> customPins;
        XamarinMapOverlay mapOverlay;
        bool xamarinOverlayShown = false;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                nativeMap.MapElementClick -= OnMapElementClick;
                nativeMap.Children.Clear();
                mapOverlay = null;
                nativeMap = null;
            }

            if (e.NewElement != null)
            {
                formsMap = (CustomMap)e.NewElement;
                nativeMap = Control as MapControl;
                customPins = formsMap.CustomPins;

                
                updateRouteAsync();
                updatePins();
            }
        }

        private void updatePins()
        {
            if(customPins != null)
            {

            
                nativeMap.Children.Clear();
                nativeMap.MapElementClick += OnMapElementClick;
                foreach (var pin in customPins)
                {
                    var snPosition = new BasicGeoposition { Latitude = pin.Position.Latitude, Longitude = pin.Position.Longitude };
                    var snPoint = new Geopoint(snPosition);

                    var mapIcon = new MapIcon();
                    mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/car.png"));
                    mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                    mapIcon.Location = snPoint;
                    mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1.0);

                    nativeMap.MapElements.Add(mapIcon);
                }
            }
        }

        private async void updateRouteAsync()
        {
            var coordinates = new List<Geopoint>();
            foreach (var position in formsMap.RouteCoordinates)
            {
                coordinates.Add(new Geopoint(new BasicGeoposition() { Latitude = position.Latitude, Longitude = position.Longitude }));
            }

            for (int i = 1; i < coordinates.Count; i++)
            {
                MapRouteFinderResult routeResult =
                                    await MapRouteFinder.GetDrivingRouteAsync(
                                    startPoint: coordinates[i - 1],
                                    endPoint: coordinates[i],
                                    optimization: MapRouteOptimization.Time,
                                    restrictions: MapRouteRestrictions.None);

                if (routeResult.Status == MapRouteFinderStatus.Success)
                {

                    // Usa la ruta para inicializar MapRouteView.
                    MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                    viewOfRoute.RouteColor = Colors.Yellow;
                    viewOfRoute.OutlineColor = Colors.Black;

                    // Añade el nuevo MapRouteView al conjunto de rutas
                    // de MapControl.
                    nativeMap.Routes.Add(viewOfRoute);
                }
            }
        }

        private void OnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var mapIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;
            if (mapIcon != null)
            {
                if (!xamarinOverlayShown)
                {
                    var customPin = GetCustomPin(mapIcon.Location.Position);
                    if (customPin == null)
                    {
                        throw new Exception("Custom pin not found");
                    }

                    if (customPin.Id.ToString() == "Ruta")
                    {
                        mapOverlay = new XamarinMapOverlay(customPin);

                        var snPosition = new BasicGeoposition { Latitude = customPin.Position.Latitude, Longitude = customPin.Position.Longitude };
                        var snPoint = new Geopoint(snPosition);
                        
                        nativeMap.Children.Add(mapOverlay);
                        MapControl.SetLocation(mapOverlay, snPoint);
                        MapControl.SetNormalizedAnchorPoint(mapOverlay, new Windows.Foundation.Point(0.5, 1.0));
                        xamarinOverlayShown = true;
                    }
                }
                else
                {
                    nativeMap.Children.Remove(mapOverlay);
                    xamarinOverlayShown = false;
                }
            }
        }

        CustomPin GetCustomPin(BasicGeoposition position)
        {
            var pos = new Position(position.Latitude, position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == pos)
                {
                    return pin;
                }
            }
            return null;
}
    }
}
