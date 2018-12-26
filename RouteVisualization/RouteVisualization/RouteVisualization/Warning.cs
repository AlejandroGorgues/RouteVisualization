using System;
using System.Collections.Generic;
using System.Text;

namespace RouteVisualization
{
    class Warning
    {
        private string grado;
        private string descripcion;
        private string tipo;
        private string coordinateStart;
        private string coordinateEnd;

        public Warning(string grado, string descripcion, string tipo, string coordinateStart, string coordinateEnd)
        {
            this.grado = grado;
            this.descripcion = descripcion;
            this.tipo = tipo;
            this.coordinateStart = coordinateStart;
            this.coordinateEnd = coordinateEnd;
        }

        public string Grado { get => grado; set => grado = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public string Tipo { get => tipo; set => tipo = value; }
        public string CoordinateStart { get => coordinateStart; set => coordinateStart = value; }
        public string CoordinateEnd { get => coordinateEnd; set => coordinateEnd = value; }
    }
}
