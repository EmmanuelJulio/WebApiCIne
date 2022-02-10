using Microsoft.AspNetCore.Http;

namespace WebApiPeliculas.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env,IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public  Task BorrarArchivo(string ruta, string contenedor)
        {
            if (ruta != null)
            {
                var nombreArchivo= Path.GetFileName(ruta);
                string directorioArchivo = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);
              
                if(File.Exists(directorioArchivo))
                    File.Delete(directorioArchivo);

            }

                return Task.FromResult(0);
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string ruta, string contenedor, string contentType)
        {
            await BorrarArchivo(ruta, contenedor);

            return await GuardarArchivo(contenido, extension, contenedor, contentType);
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string contenedor, string contentType)
        {
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, contenedor);
            if(!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string ruta = Path.Combine(folder, nombreArchivo);
            await File.WriteAllBytesAsync(ruta, contenido);
#pragma warning disable CS8602 // Desreferencia de una referencia posiblemente NULL.
            var urlActual = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
#pragma warning restore CS8602 // Desreferencia de una referencia posiblemente NULL.
            var urlParaLaBaseDeDatos = Path.Combine(urlActual, contenedor, nombreArchivo).Replace("\\", "//");
            return urlParaLaBaseDeDatos;
        }
    }
}
