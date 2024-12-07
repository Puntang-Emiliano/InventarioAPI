using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.ModelsDTO
{
    public class CrearProductoDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser un valor positivo o cero.")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "El ID de la categoría es obligatorio.")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El ID del proveedor es obligatorio.")]
        public int ProveedorId { get; set; }

        // Modificado para aceptar un archivo de imagen
        [Required(ErrorMessage = "La imagen es obligatoria.")]
        public IFormFile Imagen { get; set; }
    }

}
