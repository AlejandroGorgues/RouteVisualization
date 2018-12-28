using System;
using System.Collections.Generic;
using System.Text;

namespace RouteVisualization
{
    class PuntoBing
    {
        
        private double latitude;
        private double longitude;
        private string accion;
        private string tipo;
        private string tipoRoad;
        private double distancia;
        private double tiempo;
        private List<string> nombre;
        private List<Warning> warnings;
        private List<string> signs;

        public PuntoBing(double latitude, double longitude, string accion, string tipo, string tipoRoad, double distancia, double tiempo, List<string> nombre, List<Warning> warnings, List<string> signs)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.accion = accion;
            this.tipo = tipo;
            this.tipoRoad = tipoRoad;
            this.distancia = distancia;
            this.tiempo = tiempo;
            this.nombre = nombre;
            this.warnings = warnings;
            this.signs = signs;
        }

        public double Latitude { get => latitude; set => latitude = value; }
        public double Longitude { get => longitude; set => longitude = value; }
        public string Accion { get => accion; set => accion = value; }
        public string Tipo { get => tipo; set => tipo = value; }
        public string TipoRoad { get => tipoRoad; set => tipoRoad = value; }
        public double Distancia { get => distancia; set => distancia = value; }
        public double Tiempo { get => tiempo; set => tiempo = value; }
        public List<string> Nombre { get => nombre; set => nombre = value; }
        public List<string> Signs { get => signs; set => signs = value; }
        internal List<Warning> Warnings { get => warnings; set => warnings = value; }


        public void probar() {
        }
    }
}
