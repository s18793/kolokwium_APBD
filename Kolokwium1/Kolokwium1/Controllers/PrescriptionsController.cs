using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Kolokwium1.DTOs;
using Kolokwium1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1.Controllers
{
    [Route("api/prescriptions")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetPerscription(int id)
        {
            

                using (SqlConnection con = new SqlConnection("Data Source = db-mssql; Initial Catalog=s18793; Integrated Security=True"))
                using (SqlCommand com = new SqlCommand())
                {
                try
                {
                    
                    var list = new List<Prescriptions>();
                    var lista = new List<PrescriptionsMedicament>();
                    com.Connection = con;

                    com.Parameters.AddWithValue("IdPrescription", id);
                    com.CommandText = "Select * from Prescription where IdPrescription =@id";
                    
                    con.Open();



                    SqlDataReader dr = com.ExecuteReader();

                    while (dr.Read())
                    {
                        var per = new Prescriptions();

                        per.IdPerscription = (int)dr["IdPrescription"];
                        per.Date = DateTime.Parse(dr["Date"].ToString());
                        per.DueTime = DateTime.Parse(dr["DueDate"].ToString());
                        per.IdPatient = (int)dr["IdPatient"];
                        per.IdDoctor = (int)dr["IdDoctor"];
                        list.Add(per);
                    }
                   
                    com.ExecuteNonQuery();
                    com.CommandText = "Select * from Prescription_Medicament where IdPrescription= @id";

                    com.Parameters.AddWithValue("idPerc", id);

                    while (dr.Read())
                    {
                        var permed = new PrescriptionsMedicament();
                        var listaMed = new List<Medicament>();

                        permed.IdPerscription = (int)dr["IdPrescription"]; ;
                        permed.IdMedicament = (int)dr["IdMedicament"];
                       
                        permed.Dose = (int)dr["Dose"];
                        permed.Details = (dr["Details"].ToString());

                        lista.Add(permed);
                    }

    
                    return Ok(list + "\n" + lista);



                }
                catch (SqlException sql)
                {
                    return BadRequest("Nie prawidłowy parametr/nie podano parametru");
                }
            }
            

        }

        [HttpPost]
        public IActionResult addPresctription(PrescriptionRequest request)
        {

            try
            {
                using (SqlConnection con = new SqlConnection("Data Source = db-mssql; Initial Catalog=s18793; Integrated Security=True"))
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;

                    con.Open();

                    com.CommandText = "Insert Into Prescription (Date, DueDate, IdPatient, IdDoctor) Values (@data, @ddata, @patient, @doctor)";


                    //Id Autonumerowane także nie dodaje
                    com.Parameters.AddWithValue("data", request.Date);
                    com.Parameters.AddWithValue("ddata", request.DueTime);
                    com.Parameters.AddWithValue("patient", request.IdPatient);
                    com.Parameters.AddWithValue("doctor", request.IdDoctor);

                    if (DateTime.Compare(request.DueTime, request.Date) < 0) return BadRequest("Data ważności/format jest błędna/y");
                    

                    com.ExecuteNonQuery();
                    return Ok();
                }
            }catch(SqlException ex)
            {
                return BadRequest("Brak danych lub dane nie poprawnego typu");
            }

        }



    }
}