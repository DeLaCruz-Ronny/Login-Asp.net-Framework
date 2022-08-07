using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Login.Models;
using System.Data.SqlClient;
using System.Data;

namespace Login.Controllers
{
    public class AccesoController : Controller
    {

        static string cadena = "Data Source=DESKTOP-SC15GE0;Initial Catalog=Acceso;Integrated Security=true";

        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Usuario ousuario)
        {
            bool registrado;
            string mensaje;

            //Condicion de que si las clave son iguales te permita agregarlas y la encripte de lo contrario mande un mensaje de que las claves no son iguales
            if (ousuario.Clave == ousuario.ConfirmarClave)
            {
                ousuario.Clave = ConverttoSha256(ousuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las Clave no Coinciden";
                return View();
            }

            //Hacemos la coneccion a la db con los respectivos StoreProcedure
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrtarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", ousuario.Correo);
                cmd.Parameters.AddWithValue("Clave", ousuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                cmd.ExecuteNonQuery();
                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();

            }
            ViewData["Mensaje"] = mensaje;
            if (registrado) 
            {
                return RedirectToAction("Login","Acceso");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Login(Usuario ousuario)
        {
            ousuario.Clave = ConverttoSha256(ousuario.Clave);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_validar", cn);
                cmd.Parameters.AddWithValue("Correo", ousuario.Correo);
                cmd.Parameters.AddWithValue("Clave", ousuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                ousuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if (ousuario.IdUsuario != 0)
            {
                Session["usuario"] = ousuario;
                return RedirectToAction("Index","Home");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }

            
        }

        public static string ConverttoSha256(string txt)
        {
            //using System.Text;
            //Usar la referencia de "System.Security.Cryptography"

            StringBuilder sb = new StringBuilder();
            using(SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(txt));
                foreach (byte b in result)
                {   
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }
    }
}