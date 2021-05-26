using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using saga_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;

namespace saga_app.Controllers
{
    public class TableController : Controller
    {
        private readonly IConfiguration     Configuration;
        private static List<TableModel>     tableList = new List<TableModel>();
        private TableModel                  tableItem;

        public TableController(IConfiguration configuration)
        {
            Configuration = configuration;
            tableList = GrabList();
        }
        
        public IActionResult Index()
        {
            tableList = GrabList();
            return View();
        }

        public IActionResult Upsert(int id = 0)
        {
            tableItem = new TableModel();
            if (id == 0)
                return View(tableItem);

            tableItem = tableList.Find(item => item.Id == id);
            if (tableItem == null)
                return NotFound();

            return View(tableItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(int id, [Bind("Id, Field1, Field2, Field3")] TableModel itemModel)
        {
            if (!ModelState.IsValid)
                return Json(new { isValid = false, html = Utils.RenderRazorViewToString(this, "Upsert", itemModel) });

            itemModel.Id = id;

            bool bHasQueryExecuted = UpsertItem(itemModel);
            if (!bHasQueryExecuted)
                return Json(new { isValid = true, message = "Query failed!", html = Utils.RenderRazorViewToString(this, "Index") });

            tableList = GrabList();

            return Json(new { isValid = true, message = "Query executed successfully!", html = Utils.RenderRazorViewToString(this, "Index") });
            
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = tableList });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            bool bHasQueryExecuted = DeleteItem(id);
            if(!bHasQueryExecuted)
                return Json(new { success = false, message = "Query failed!" });

            if (tableList.RemoveAll(item => item.Id == id) == 0)
                return Json(new { success = false, message = "Query failed!"  });

            return Json(new { success = true, message = "Query executed successfully!" });
        }

        #region Database Calls
        public List<TableModel> GrabList()
        {
            List<TableModel> auxList = new List<TableModel>();
            FbConnection conn = new FbConnection(Configuration.GetConnectionString("LocalConnection"));
            FbDataReader reader = null;

            FbCommand cmd = new FbCommand("SELECT * FROM APPDATA;", conn);
            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    TableModel aux = new TableModel();
                    aux.Id = Int32.Parse(reader[0].ToString());
                    aux.Field1 = reader[1].ToString();
                    aux.Field2 = reader[2].ToString();
                    aux.Field3 = reader[3].ToString();
                    auxList.Add(aux);
                }
                conn.Close();
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                return null;
            }
            return auxList;
        }

        public bool UpsertItem(TableModel itemModel)
        {
            FbConnection conn = new FbConnection(Configuration.GetConnectionString("LocalConnection"));
            int affectedRows;

            String query;

            if (itemModel.Id != 0)
            {
                query = String.Format("UPDATE APPDATA SET FIELD1 = '{0}', FIELD2 = '{1}', FIELD3 = '{2}' WHERE ID = {3};",
                    itemModel.Field1,
                    itemModel.Field2,
                    itemModel.Field3,
                    itemModel.Id);
            }
            else
            {
                query = String.Format("INSERT INTO APPDATA(FIELD1, FIELD2, FIELD3) VALUES('{0}', '{1}', '{2}');",
                    itemModel.Field1,
                    itemModel.Field2,
                    itemModel.Field3);
            }

            FbCommand cmd = new FbCommand(query, conn);
            try
            {
                conn.Open();
                affectedRows = cmd.ExecuteNonQuery();
                conn.Close();

                if (affectedRows == 0)
                    return false;
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                return false;
            }
            return true;
        }

        public bool DeleteItem(int id)
        {
            FbConnection conn = new FbConnection(Configuration.GetConnectionString("LocalConnection"));
            int affectedRows = 0;

            String query = String.Format("DELETE from APPDATA WHERE ID={0};", id);

            FbCommand cmd = new FbCommand(query, conn);
            try
            {
                conn.Open();
                affectedRows = cmd.ExecuteNonQuery();
                conn.Close();

                if (affectedRows == 0)
                    return false;
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
                return false;
            }
            return true;
        }

        #endregion
    }
}
