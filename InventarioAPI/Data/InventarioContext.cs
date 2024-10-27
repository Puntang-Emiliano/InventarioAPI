using InventarioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Data
{
    public class InventarioContex : DbContext
    {
        public InventarioContex(DbContextOptions<InventarioContex> options) :base(options)
        {

        }

        public DbSet<UsuarioLogingDTO> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedidos { get; set; }
        public DbSet<OrdenCompra> OrdenCompra { get; set; }
        public DbSet<DetalleOrdenCompra> DetalleOrdenCompra { get; set; }
        public DbSet<Rol> Roles { get; set; }


    }
}
