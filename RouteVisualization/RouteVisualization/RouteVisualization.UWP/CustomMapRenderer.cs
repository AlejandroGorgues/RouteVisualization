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


[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace RouteVisualization.UWP
{
    class CustomMapRenderer : MapRenderer
    {
        MapControl nativeMap;
        CustomMap formsMap;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {

            }

            if (e.NewElement != null)
            {
                formsMap = (CustomMap)e.NewElement;
                nativeMap = Control as MapControl;
                updateRouteAsync();
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
    }
}
