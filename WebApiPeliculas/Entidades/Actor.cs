﻿using System.ComponentModel.DataAnnotations;

namespace WebApiPeliculas.Entidades
{
    public class Actor
    {
        public int id { get; set; }

        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
        [Required]
        
        public DateTime FechaNacimiento { get; set; }
        public string? Foto { get; set; }

        public List<PeliculasActores> PeliculasActores { get; set; }
    }
}
