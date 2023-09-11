using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Data;
using System.Data.Sql;

using APIWEB2.Models;
using System.Data.SqlClient;

namespace APIWEB2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class inventarioController : ControllerBase
    {
        private readonly string SQLString;

        public inventarioController(IConfiguration config)
        {
            SQLString = config.GetConnectionString("SQLConnection");
        }

        [HttpGet]
        [Route("lista")]

        public IActionResult lista() {
            List<inventario> lista = new List<inventario>();

            try
            {
                using(var conexion = new SqlConnection(SQLString))
                {
                    conexion.Open();

                    var cmd = new SqlCommand("sp_lista_inventario", conexion);

                    cmd.CommandType = CommandType.StoredProcedure;

                    using(var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read()) 
                        {
                            lista.Add(new inventario()
                            {
                                id = Convert.ToInt32(rd["id"]),
                                nombre = (rd["nombre"]).ToString(),
                                distribuidora = rd["distribuidora"].ToString(),
                                cantidad = Convert.ToInt32(rd["cantidad"]),
                                precio = Convert.ToDecimal(rd["precio"])
                            });


                        }
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = lista });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message});
            }

        
        
        
        }

        [HttpGet]
        [Route("buscar/{id:int}")]

        public IActionResult buscar(int id)
        {


            inventario producto = null;
            try
            {
                using (var conexion = new SqlConnection(SQLString))
                {
                    conexion.Open();

                    var cmd = new SqlCommand("sp_buscar_inventario", conexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandType = CommandType.StoredProcedure;


                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            producto = new inventario()
                            {
                                id = Convert.ToInt32(rd["id"]),
                                nombre = rd["nombre"].ToString(),
                                distribuidora = rd["distribuidora"].ToString(),
                                cantidad = Convert.ToInt32(rd["cantidad"]),
                                precio = Convert.ToInt32(rd["precio"])
                            };

                            return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = producto });

                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status200OK, new { message = "No se ha encontrado el producto en el inventario"});
                        }
                    }
                }
            }

            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message });
            }




        }

        [HttpPost]
        [Route("agregar")]

        public IActionResult agregar([FromBody] inventario objeto)
        {

            try
            {
                using (var conexion = new SqlConnection(SQLString))
                {
                    conexion.Open();

                    var cmd = new SqlCommand("sp_añadir_inventario", conexion);
                    cmd.Parameters.AddWithValue("@nombre", objeto.nombre);
                    cmd.Parameters.AddWithValue("@distribuidora", objeto.distribuidora);
                    cmd.Parameters.AddWithValue("@cantidad", objeto.cantidad);
                    cmd.Parameters.AddWithValue("@precio", objeto.precio);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();


                }

                return StatusCode(StatusCodes.Status200OK, new { message = "Se ha agregado el producto al inventario exitosamente!" });
            }

            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message});
            }




        }




        [HttpPut]
        [Route("Editar")]

        public IActionResult Editar([FromBody] inventario objeto)
        {

            try
            {
                using (var conexion = new SqlConnection(SQLString))
                {
                    conexion.Open();

                    var cmd = new SqlCommand("sp_editar_inventario", conexion);

                    cmd.Parameters.AddWithValue("@id", objeto.id == 0 ? DBNull.Value : objeto.id);
                    cmd.Parameters.AddWithValue("@nombre", objeto.nombre is null ? DBNull.Value : objeto.nombre);
                    cmd.Parameters.AddWithValue("@distribuidora", objeto.distribuidora is null ? DBNull.Value : objeto.distribuidora);
                    cmd.Parameters.AddWithValue("@cantidad", objeto.cantidad == 0 ? DBNull.Value : objeto.cantidad);
                    cmd.Parameters.AddWithValue("@precio", objeto.precio == 0 ? DBNull.Value : objeto.precio);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();


                }

                return StatusCode(StatusCodes.Status200OK, new { message = "Se ha actualizado el producto con éxito!" });
            }

            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message });
            }




        }




        [HttpDelete]
        [Route("Eliminar/{id}")]

        public IActionResult Eliminar(int id)
        {

            try
            {
                using (var conexion = new SqlConnection(SQLString))
                {
                    conexion.Open();

                    var cmd = new SqlCommand("sp_eliminar_inventario", conexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandType = CommandType.StoredProcedure;
                    int filesaffected = cmd.ExecuteNonQuery();

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (filesaffected == 1)
                        {         
                            return StatusCode(StatusCodes.Status200OK, new { message = "Se ha eliminado el producto con éxito!" });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status200OK, new { message = "No se ha encontrado ningun producto con el ID seleccionado" });
                        }

                    }

                }

                
            }

            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = error.Message });
            }




        }
    }
}
