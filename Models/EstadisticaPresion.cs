    public class EstadisticasPresion
    {
        public int TotalRegistros     { get; set; }
        public int PromedioSistolica  { get; set; }
        public int PromedioDiastolica { get; set; }
        public int PromedioPulso      { get; set; }

        public int RegistrosNormales  { get; set; }
        public int RegistrosBien      { get; set; }
        public int RegistrosAltos     { get; set; }
        public int RegistrosMuyAltos  { get; set; }

        public ClasificacionPresion? UltimaClasificacion { get; set; }

        public double PorcentajeNormales => TotalRegistros > 0
            ? Math.Round((double)RegistrosNormales / TotalRegistros * 100, 1)
            : 0;

        public double PorcentajePreocupantes => TotalRegistros > 0
            ? Math.Round((double)(RegistrosAltos + RegistrosMuyAltos) / TotalRegistros * 100, 1)
            : 0;
    }