     namespace mi_tension_backend.Services
        /// <summary>
        /// Estructura interna que agrupa los umbrales de clasificación.
        /// </summary>
{
    
         class UmbralesPresion
        {
            public int CrisisSistolica   { get; }
            public int CrisisDiastolica  { get; }
            public int AltaSistolica     { get; }
            public int AltaDiastolica    { get; }
            public int ElevadaSistolica  { get; }
            public int ElevadaDiastolica { get; }

            public UmbralesPresion(
                int crisisSistolica,  int crisisDiastolica,
                int altaSistolica,    int altaDiastolica,
                int elevadaSistolica, int elevadaDiastolica)
            {
                CrisisSistolica   = crisisSistolica;
                CrisisDiastolica  = crisisDiastolica;
                AltaSistolica     = altaSistolica;
                AltaDiastolica    = altaDiastolica;
                ElevadaSistolica  = elevadaSistolica;
                ElevadaDiastolica = elevadaDiastolica;
            }
        }
}
