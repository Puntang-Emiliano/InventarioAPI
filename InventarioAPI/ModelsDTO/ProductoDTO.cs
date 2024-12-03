namespace InventarioAPI.ModelsDTO
{
    public class ProductoDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }
        public int ProveedorId { get; set; }
        public string Imagen { get; set; }
    }
}
