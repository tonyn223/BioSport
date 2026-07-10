namespace BioSport.ViewModels
{
    public class ReporteViewModel
    {
        public int TotalClientes { get; set; }
        public int TotalEntrenadores { get; set; }
        public int TotalRecepcionistas { get; set; }
        public int TotalAdministradores { get; set; }

        public int MembresiasActivas { get; set; }
        public int MembresiasVencidas { get; set; }

        public decimal IngresosTotales { get; set; }

        public int AsistenciasHoy { get; set; }

        public string PlanMasVendido { get; set; } = string.Empty;
    }
}
