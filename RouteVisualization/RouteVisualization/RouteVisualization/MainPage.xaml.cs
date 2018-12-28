using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Diagnostics;
#if WINDOWS_APP || WINDOWS_PHONE_APP || WINDOWS_UWP
using Windows.Data.Json;
#endif
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace RouteVisualization
{
    public sealed partial class MainPage : ContentPage
    {

        Pin mapIconStart = new Pin();
        Pin mapIconEnd = new Pin();
        CustomMap mapView;
        Pin mapIconRuta;
        String urlInicial = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=";
        String urlMedio = "&wp.1=";
        String urlFinal = "&avoid=minimizeTolls&output=json&key=88yPY0kZcOGX7RaKeHM8~ogAnZjvVpFAWmF1mTZUDZQ~AnnZzfuaCDOzl0HmlTs8aFZ9zjIgW8JFlm69BS6UUPsppWQgusCRU1C0VRk0wVHR";
#if WINDOWS_APP || WINDOWS_PHONE_APP || WINDOWS_UWP
        JsonArray arrayRuta;
#endif
        ArrayList puntos = new ArrayList();
        ArrayList warningItems = new ArrayList();
        //List<CustomPin> customPins = new List<CustomPin>();
        int iteracionEscritura = 1;



        public MainPage()
        {
            this.InitializeComponent();
            mapView = new CustomMap
            {
                MapType = MapType.Street,
                WidthRequest = 908,
                HeightRequest = 990,
                HorizontalOptions = LayoutOptions.End,
                Margin = new Thickness(582, 0, 0, 0)
            };

        }

        private void Button_Click(object sender, EventArgs e)
        {

            var mapViewAUX = new CustomMap
            {
                MapType = MapType.Street,
                WidthRequest = 908,
                HeightRequest = 990,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            
            rutaE.Text = "";
            clearContainers();
            Position lastPoint = new Position(), currentPoint;

            Device.BeginInvokeOnMainThread(async () =>
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage stream = await client.GetAsync(urlInicial + lugarIncialTextBox.Text + urlMedio + lugarFinalTextBox.Text + urlFinal);
                //Si ha obtenido una ruta como respuesta, escribe en el mapa
                if (stream.IsSuccessStatusCode)
                {
                    string str = await stream.Content.ReadAsStringAsync();
#if WINDOWS_APP || WINDOWS_PHONE_APP || WINDOWS_UWP
                    JsonValue jsonValue = JsonValue.Parse(str);
                    arrayRuta = jsonValue.GetObject().GetNamedArray("resourceSets").GetObjectAt(0).GetNamedArray("resources").GetObjectAt(0).GetNamedArray("routeLegs").GetObjectAt(0).GetNamedArray("itineraryItems");

                    //Bucle que dibuja cada punto de la ruta y la ruta entre el punto actual y el anterior en el mapa
                    var first = arrayRuta.First();
                    foreach (var puntoRuta in arrayRuta)
                    {
                        PuntoBing puntoBing = obtenerPunto(puntoRuta.GetObject());

                        currentPoint = new Position(puntoBing.Latitude, puntoBing.Longitude);
                        string nombres = "";
                        foreach (string nombre in puntoBing.Nombre)
                        {
                            nombres = nombres + Environment.NewLine + nombre;
                        }

                        mapIconRuta = new Pin
                        {
                            Type = PinType.Place,
                            Position = currentPoint,
                            Label = "custom pin",
                            Address = "asdfsfdsdf"
                        };

                        mapIconRuta.Clicked += (object senderPin, EventArgs ePin) => {
                            pinClicked(senderPin, ePin);
                        };

                        mapViewAUX.Pins.Add(mapIconRuta);

                        puntos.Add(puntoBing);
                        escribePunto(puntoBing);
                        

                        //Si el punto actual es el último, pasa a ser el actual en el código

                        if (!puntoRuta.Equals(first))
                        {

                            // Obtiene la ruta entre el punto anterior y el actual.
                            mapViewAUX.RouteCoordinates.Add(new Position(puntoBing.Latitude, puntoBing.Longitude));

                            //El punto actual se convierte en el anterior
                            lastPoint = currentPoint;
                        }
                        else
                        {
                            lastPoint = currentPoint;
                        }
                    }
                    iteracionEscritura = 1;
                    JsonArray firstCoordinates = first.GetObject().GetNamedObject("maneuverPoint").GetNamedArray("coordinates");
                 mapViewAUX.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(firstCoordinates.GetNumberAt(0), firstCoordinates.GetNumberAt(1)), Distance.FromMiles(100)));
                    mainStack.Children[1] = mapViewAUX;

#endif
                }
                else
                {
                    await DisplayAlert("Alert", "Error al obtener los datos", "OK");
                }
            });
        }

#if WINDOWS_APP || WINDOWS_PHONE_APP || WINDOWS_UWP
        //Devuelve el contenido de la clave warning como un objeto de tipo Warning
        private Warning obtenerWarning(JsonObject warning)
        {
            if (warning.ContainsKey("origin"))
            {
                return new Warning(warning.GetNamedString("severity"), warning.GetNamedString("text"), warning.GetNamedString("warningType"), warning.GetNamedString("origin"), warning.GetNamedString("to"));
            }
            else
            {
                return new Warning(warning.GetNamedString("severity"), warning.GetNamedString("text"), warning.GetNamedString("warningType"), "0", "0");
            }

        }

        //Devuelve el contenido de un punto de tipo ruta como un objeto de tipo PuntoBing 
        private PuntoBing obtenerPunto(JsonObject puntoJson)
        {
            List<string> names = new List<string>();
            List<Warning> warnings = new List<Warning>();
            List<string> signs = new List<string>();

            JsonObject datosManiobrabilidad = puntoJson.GetNamedObject("maneuverPoint");
            JsonArray coordenadas = datosManiobrabilidad.GetNamedArray("coordinates");
            JsonObject instruccion = puntoJson.GetNamedObject("instruction");
            JsonObject detalles = puntoJson.GetNamedArray("details").GetObjectAt(0);

            if (detalles.ContainsKey("names"))
            {
                foreach (var namesAux in detalles.GetNamedArray("names"))
                {
                    names.Add(namesAux.GetString());
                }
            }

            if (puntoJson.ContainsKey("warnings"))
            {
                foreach (var warningAux in puntoJson.GetNamedArray("warnings"))
                {
                    warnings.Add(obtenerWarning(warningAux.GetObject()));
                }
            }


            if (puntoJson.ContainsKey("signs"))
            {
                foreach (var sign in puntoJson.GetNamedArray("signs"))
                {
                    signs.Add(sign.GetString());
                }
            }

            return new PuntoBing(coordenadas.GetNumberAt(0), coordenadas.GetNumberAt(1), instruccion.GetNamedString("text"), instruccion.GetNamedString("maneuverType"), detalles.GetNamedString("roadType"), puntoJson.GetNamedNumber("travelDistance"), puntoJson.GetNamedNumber("travelDuration"), names, warnings, signs);
        }
#endif

        private void escribePunto(PuntoBing punto)
        {

            rutaE.Text = rutaE.Text + Environment.NewLine + "Acción " + iteracionEscritura + " : " + Environment.NewLine + " A " + punto.Distancia + "Km " + punto.Accion;
            iteracionEscritura += 1;
        }

        private void pinClicked(object sender, EventArgs args)
        {
            var pinMap = sender as Pin;
            Position locationIcon = pinMap.Position;
            ArrayList listNumberWarning = new ArrayList();
            var warningI = 1;
            warningsE.Text = "";
            signalsE.Text = "";

            foreach (PuntoBing puntoBing in puntos)
            {
                if (puntoBing.Latitude == Math.Round(locationIcon.Latitude, 5) && puntoBing.Longitude == Math.Round(locationIcon.Longitude, 5))
                {
                    latitudE.Text = puntoBing.Latitude.ToString();
                    longitudE.Text = puntoBing.Longitude.ToString();
                    distanciaE.Text = puntoBing.Distancia.ToString();
                    tiempoE.Text = puntoBing.Tiempo.ToString();
                    foreach (var signal in puntoBing.Signs)
                    {
                        signalsE.Text =  signal.ToString() + Environment.NewLine;
                    }

                    foreach (var warning in puntoBing.Warnings)
                    {
                        warningItems.Add(warning as Warning);
                        listNumberWarning.Add("Advertencia: " + warningI);
                        warningI++;
                    }
                    warningsPicker.ItemsSource = listNumberWarning;


                }
            }

        }


        


        private void pickerClicked(object sender, EventArgs e)
        {
            var comboB = sender as Picker;
            var index = comboB.SelectedIndex;
            if (index > -1)
            {
                Warning wSelected = warningItems[index] as Warning;
                warningsE.Text = "Grado: " + wSelected.Grado;
                warningsE.Text = warningsE.Text + Environment.NewLine + "Descripción: " + wSelected.Descripcion;
                warningsE.Text = warningsE.Text + Environment.NewLine + "Inicio: " + wSelected.CoordinateStart;
                warningsE.Text = warningsE.Text + Environment.NewLine + "Final: " + wSelected.CoordinateEnd;

            }

        }

        //Método que elimina el texto que describe el Pin seleccionado
        private void clearContainers()
        {
            warningsPicker.ItemsSource = null;
            warningsE.Text = "";
            latitudE.Text = "";
            longitudE.Text = "";
            distanciaE.Text = "";
            tiempoE.Text = "";
            signalsE.Text = "";

        }
    }
}
