﻿namespace InventarioAPI.ModelsDTO
{
    public class DetalleOrdenCompraDTO
    {
        public int IdDetalleOrdenCompra { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int OrdenCompraId { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
    }
}
