namespace WebApiPeliculas.DTOS
{
    public class FiltroPeliculaDTO
    {
        public int Pagina { get; set; } = 1;
        public int CantidadDeRegistrosPorPagina { get; set; } = 10;
        public PaginacionDTO Paginacion 
            {
                get{ return new PaginacionDTO() { Pagina = Pagina, CantidadRegistrosPorPagina = CantidadDeRegistrosPorPagina }; }
            }

        public string Titulo { get; set; }
        public int GeneroID { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosExtrenoss { get; set; }

        public string CampoOrdenar { get; set; }
        public bool OrdenAsendente { get; set; } = true;
    }
}
