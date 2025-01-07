namespace SysSoniaInventory.ViewModels
{
    public class ProductoViewModel
    {
        public class GananciaMensualViewModel
        {
            public int Año { get; set; }
            public int Mes { get; set; }
            public string Fecha { get; set; }
            public decimal Ganancia { get; set; } //para saber cuanto es la ganancia por mes

            public int Cantidad { get; set; } //para saber cuantos productos se vendieron por mes


            // Propiedad calculada para el nombre del mes
            public string NombreMes
            {
                get
                {
                    return new DateTime(Año, Mes, 1).ToString("MMMM", new System.Globalization.CultureInfo("es-ES"));
                }
            }
        }




    }
}
